using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using Color = System.Drawing.Color;
using ImGui = ImGuiNET.ImGui;
namespace DotrModdingTool2IMGUI;

class CardEditorWindow : IImGuiWindow
{
    ImFontPtr font;
    Vector2 cardImageSize;
    Vector2 frameImageSize;
    float imageScale = 2.0f;
    string[] cardNames = Card.cardNameList;
    string cardSearch = "";
    int currentCardIndex;
    string currentMonsterName;
    int currentMonsterAttack;
    int currentMonsterDefense;
    int currentCardSummonCost;
    int currentCardAttribute;
    int currentCardDeckCost;
    int currentCardKind;
    string currentMonsterStatString;
    CardConstant currentCardConst;
    CardColourType currentCardType = CardColourType.NormalMonster;
    IntPtr cardImage;
    IntPtr cardFrame;

    //16x16 size
    IntPtr starImagePtr;
    string monsterName;

    Vector2 textBoxTopLeftInnerOffsetInPixelsScaled;
    Vector2 innerTextBoxSizeInPixelsScaled;
    float xTextPadding = 5f;
    float yTextPadding = 5f;


    DeckLeaderAbilityInstance abilityInstance;
    string[] unlockRanksNames = Enum.GetNames(typeof(DeckLeaderRank));
    int[] currentAbilityRankIndex = new int[20];
    MonsterEffects? currentEffects = null;
    string[] effectsTableVerticalHeaders = Enum.GetNames(typeof(MonsterEffects.MonsterEffectType));
    string[] effectsTableHorizontalHeaders = { "Effect Id", "Search Type", "Extra Data" };
    int[] currentMonsterEffectDataUpper = new int[5];
    int[] currentMonsterEffectDataLower = new int[5];
    int monsterEffectTableEditorIndex = 0;

    int currentMagicEffectDataUpper;
    int currentMagicEffectDataLower;
    int magicEffectTableEditorIndex;

    public CardEditorWindow()
    {
        starImagePtr = ImageHelper.LoadImageImgui($"Images.cardExtras.star.png");
        font = Fonts.MonoSpace;
        cardImageSize = new Vector2(192 * imageScale, 192 * imageScale);
        frameImageSize = new Vector2(256 * imageScale, 368 * imageScale);
        textBoxTopLeftInnerOffsetInPixelsScaled = new Vector2(19, 22) * imageScale;
        innerTextBoxSizeInPixelsScaled = new Vector2(218, 26) * imageScale;
        EditorWindow.OnIsoLoaded += onCardChanged;
    }

    public void SetCurrentCardIndex(int index)
    {

        currentCardIndex = index;
    }

    void onCardChanged()
    {
        currentCardConst = CardConstant.List[currentCardIndex];
        cardImage = GlobalImages.Instance.Cards[Card.cardNameList[currentCardIndex]];
        cardFrame = GlobalImages.Instance.CardFrames[currentCardConst.CardColor];
        monsterName = currentCardConst.Name;
        currentMonsterAttack = currentCardConst.Attack;
        currentMonsterDefense = currentCardConst.Defense;
        currentMonsterStatString = currentMonsterAttack.ToString();
        currentCardSummonCost = currentCardConst.Level;
        currentCardAttribute = GetAttributeVisual(currentCardConst);
        currentCardDeckCost = currentCardConst.DeckCost;
        currentCardKind = CardKind.Kinds.Keys.ToList().IndexOf(currentCardConst.CardKind.Id);

        if (currentCardConst.EffectId != 0xffff)
        {
            currentEffects = Effects.MonsterEffectsList[currentCardConst.EffectId];
        }
        else
        {
            currentEffects = null;
        }

    }

    public int GetAttributeVisual(CardConstant cardConstant)
    {

        if (cardConstant.CardKind.isMonster())
        {
            return currentCardConst.Attribute;
        }
        if (cardConstant.CardKind.isMagic())
        {
            return (int)AttributeVisual.Magic;
        }
        return (int)AttributeVisual.Trap;
    }


    public void Render()
    {
        ImGui.PushFont(font);
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text($"Please load ISO file");
            ImGui.PopFont();
            return;
        }

        Vector2 windowPos = ImGui.GetWindowPos();
        Vector2 windowSize = ImGui.GetWindowSize();
        float windowBottom = windowPos.Y + windowSize.Y - 90f * EditorWindow.AspectRatio.Y;


        ImGui.BeginChild("LeftThirdPanel", new Vector2(windowSize.X / 3f, windowSize.Y),
            ImGuiChildFlags.Border | ImGuiChildFlags.NavFlattened | ImGuiChildFlags.AlwaysAutoResize);
        ImGui.Text("Cards");
        float availableHeight = windowBottom - ImGui.GetCursorPosY();
        ImGui.PushItemWidth(windowSize.X / 3f);

        if (ImGui.BeginListBox("##Cards", new Vector2(0, availableHeight)))
        {
            ImGui.Text("Card Search");
            ImGui.SetNextItemWidth(windowSize.X / 3f);
            ImGui.InputText("##CardSearch", ref cardSearch, 32);
            List<string> filteredStrings = cardNames.Where(cardName => cardName.Contains(cardSearch, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (string cardName in filteredStrings)
            {
                bool isSelected = currentCardIndex == filteredStrings.IndexOf(cardName);

                if (ImGui.Selectable($"{cardName}", isSelected))
                {
                    currentCardIndex = Array.IndexOf(cardNames, cardName);
                }
                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(cardName);
                }
            }
            onCardChanged();
            ImGui.EndListBox();
        }
        ImGui.PopFont();
        ImGui.EndChild();


        ImGui.SameLine();


        ImGui.BeginChild("MiddlePanel", new Vector2(windowSize.X / 3, windowSize.Y), ImGuiChildFlags.Border | ImGuiChildFlags.AlwaysAutoResize);

        //Draw card frame
        Vector2 middleWindowSize = ImGui.GetContentRegionAvail();
        Vector2 pos = new Vector2((middleWindowSize.X / 2f) - frameImageSize.X / 2f, (middleWindowSize.Y / 2f) - frameImageSize.Y / 2f);
        ImGui.SetCursorPos(pos);
        ImGui.Image(cardFrame, frameImageSize);

        Vector2 textSize = AutoSizeTextToRect(monsterName, innerTextBoxSizeInPixelsScaled - new Vector2(22 * imageScale, 0));
        Vector2 InnerTextBoxPos = pos + textBoxTopLeftInnerOffsetInPixelsScaled +
                                  new Vector2(0, innerTextBoxSizeInPixelsScaled.Y / 2f - textSize.Y / 2f);

        ImGui.SetCursorPos(pos + textBoxTopLeftInnerOffsetInPixelsScaled +
                           new Vector2(innerTextBoxSizeInPixelsScaled.X - 22 * imageScale, 4 * imageScale));

        ImGui.Image(GlobalImages.Instance.CardElements[(AttributeVisual)currentCardAttribute], new Vector2(22 * imageScale, 22 * imageScale));
        //DRAW TEXT
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));
        ImGui.SetCursorPos(InnerTextBoxPos);
        ImGui.PushFont(font);
        ImGui.Text(monsterName);
        ImGui.PopFont();

        if (currentCardConst.CardKind.isMonster())
        {
            ImGui.PushFont(font);
            AutoSizeTextToRect(currentMonsterStatString, new Vector2(137 * imageScale - (16 * imageScale), 46 * imageScale / 2f));

            ImGui.SetCursorPos(pos + new Vector2(100 * imageScale + xTextPadding, 301 * imageScale));

            ImGui.PushItemWidth(100 * imageScale - xTextPadding);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0)); // Transparent button background
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.2f, 0.2f, 0.2f, 0.5f)); // Slight hover effect
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.1f, 0.1f, 0.1f, 0.5f)); // Active effect



            if (ImGui.InputInt("##attackValue", ref currentMonsterAttack, 100))
            {
                currentMonsterAttack = Math.Clamp(currentMonsterAttack, 0, 9999);
                currentCardConst.Attack = (ushort)currentMonsterAttack;
            }
            ImGui.SetCursorPos(pos + new Vector2(100 * imageScale + xTextPadding, 301 * imageScale + 46 * imageScale / 2f));

            if (ImGui.InputInt("##defenseValue", ref currentMonsterDefense, 100))
            {
                currentMonsterDefense = Math.Clamp(currentMonsterDefense, 0, 9999);
                currentCardConst.Defense = (ushort)currentMonsterDefense;
            }
            ImGui.PopStyleColor(4);
            ImGui.PopItemWidth();
            ImGui.PopFont();
        }
        ImGui.PopStyleColor();

        //draw card image
        ImGui.SetCursorPos(pos + new Vector2(32 * imageScale, 86 * imageScale));
        ImGui.Image(cardImage, cardImageSize);

        for (int i = currentCardConst.Level; i > 0; i--)
        {
            ImGui.SetCursorPos(InnerTextBoxPos + innerTextBoxSizeInPixelsScaled +
                               new Vector2(-(i * 16 * imageScale), 11 * imageScale));
            ImGui.Image(starImagePtr, new Vector2(16 * imageScale, 16 * imageScale));
        }
        /*
        ImGui.SetCursorPos(new Vector2(ImGui.GetWindowSize().X / 2, 75));
        Vector2 mousePos = ImGui.GetMousePos() - ImGui.GetWindowPos();
        ImGui.Text($"Mouse coordinates \n X: {mousePos.X}, Y: {mousePos.Y}");
        */

        ImGui.SetCursorPosX(pos.X + frameImageSize.X + 25);
        ImGui.SetCursorPosY(0);
        ImGui.SetCursorPosX(pos.X + frameImageSize.X + 25);

        ImGui.PushStyleColor(ImGuiCol.TabSelected, new GuiColour(0, 189, 0).value);
        ImGui.PushStyleColor(ImGuiCol.TabHovered, new GuiColour(128, 128, 0).value);

        ImGui.EndChild();
        ImGui.SameLine();
        font.Scale = 1f;
        ImGui.PushFont(font);
        ImGui.BeginChild("RightSidePanel", Vector2.Zero, ImGuiChildFlags.Border | ImGuiChildFlags.NavFlattened | ImGuiChildFlags.AlwaysAutoResize);

        if (ImGui.BeginTabBar("CardEditorMode"))
        {
            if (ImGui.BeginTabItem("Properties"))
            {
                RenderProperties();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Equips"))
            {
                RenderEquips();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Effect"))
            {
                RenderEffects();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Text"))
            {
                RenderCardText();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }

        ImGui.PopFont();
        ImGui.PopStyleColor(2);
        //ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        //drawList.AddRect(windowPos + pos + textBoxTopLeftInnerOffsetInPixelsScaled,
        //    windowPos + pos + textBoxTopLeftInnerOffsetInPixelsScaled + innerTextBoxSizeInPixelsScaled,
        //    ImGui.GetColorU32(new Vector4(1f, 0f, 0f, 1f)), 0,
        //    ImDrawFlags.Closed, 3f);


    }

    void RenderEquips()
    {

        if (currentCardIndex < 0 || currentCardIndex > 682)
        {
            ImGui.Text("Equips only work for monster cards");

            return;
        }

        for (int i = 0; i < 50; i++)
        {
            if (i == 47 || i == 48 || i == 49)
            {
                //Dont show Insect Imitation and Metalmorph due to hard coded equip logic
                //Dont show strong on toon as its in properties
                continue;
            }
            
            ImGui.Text($"{MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].GetEquipName(i)}");
            ImGui.SameLine();
            if (ImGui.RadioButton($"##equip{i}", MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[i]))
            {
                MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[i] =
                    !MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[i];
            }
        }
    }

    void RenderCardText()
    {
        ImGui.Text("Card Name: ");
        ImGui.Text(StringDecoder.StringTable[StringDecoder.CardNamesOffset + currentCardIndex]);

        ImGui.Text("Card Text: ");
        ImGui.Text(StringDecoder.StringTable[StringDecoder.CardEffectTextOffset + currentCardIndex]);

    }

    void RenderEffects()
    {
        ImGui.TextColored(new GuiColour(Color.Orange).value, "Works but is a WIP");
        if (ImGui.BeginTabBar("EffectTabBar", ImGuiTabBarFlags.None))
        {
            if (ImGui.BeginTabItem("View Card Effects"))
            {
                if (currentCardIndex >= 683)
                {
                    ImGui.Text($"Magic Effect Id: {currentCardConst.EffectId}");
                    for (int j = 0; j < effectsTableHorizontalHeaders.Length; j++)
                    {
                        if (ImGui.BeginTable(effectsTableHorizontalHeaders[j], 1, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                        {
                            ImGui.TableSetupColumn(effectsTableHorizontalHeaders[j],
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableHeadersRow();
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (currentCardConst.EffectId != 0xffff)
                            {
                                ImGui.TableSetColumnIndex(0);
                                switch (j)
                                {
                                    case 0:
                                        ImGui.Text(Effects.MagicEffectsList[currentCardConst.EffectId].effectName);
                                        break;
                                    case 1:
                                        ImGui.Text(Effects.MagicEffectsList[currentCardConst.EffectId].searchModeName);
                                        break;
                                    case 2:
                                        ImGui.Text(Effects.MagicEffectsList[currentCardConst.EffectId].EffectDataUpper.ToString());
                                        ImGui.Separator();
                                        ImGui.Text(Effects.MagicEffectsList[currentCardConst.EffectId].EffectDataLower.ToString());
                                        break;
                                }
                            }
                        }
                        ImGui.EndTable();
                    }
                }
                else
                {
                    ImGui.Text($"Monster Effect Id:");
                    ImGui.SameLine();
                    int effectId = currentCardConst.EffectId;
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 3);
                    if (ImGui.InputInt("##EffectId", ref effectId))
                    {
                        if (effectId == 65536)
                        {
                            effectId = 0;
                        }

                        if (effectId == -1)
                        {
                            effectId = 65535;
                        }
                        else if (effectId == 65534)
                        {
                            effectId = 255;
                        }
                        else if (effectId >= Effects.MonsterEffectsList.Count)
                        {
                            effectId = 65535;
                        }
                        currentCardConst.EffectId = (ushort)effectId;
                        currentCardConst.setCardColor();
                    }

                    for (int j = 0; j < effectsTableHorizontalHeaders.Length; j++)
                    {
                        if (ImGui.BeginTable(effectsTableHorizontalHeaders[j], 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                        {
                            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed);
                            ImGui.TableSetupColumn(effectsTableHorizontalHeaders[j],
                                ImGuiTableColumnFlags.WidthStretch);

                            ImGui.TableHeadersRow();
                            for (int i = 0; i < effectsTableVerticalHeaders.Length; i++)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                ImGui.Text(effectsTableVerticalHeaders[i]);
                                if (currentEffects != null)
                                {
                                    ImGui.TableSetColumnIndex(1);
                                    switch (j)
                                    {
                                        case 0:
                                            ImGui.Text(currentEffects.Effects[i].effectName);
                                            break;
                                        case 1:
                                            ImGui.Text(currentEffects.Effects[i].searchModeName);
                                            break;
                                        case 2:
                                            ImGui.Text(currentEffects.Effects[i].EffectDataUpper.ToString());
                                            ImGui.Separator();
                                            ImGui.Text(currentEffects.Effects[i].EffectDataLower.ToString());
                                            break;
                                    }
                                }
                            }
                            ImGui.EndTable();
                        }
                    }
                }
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Edit Monster Effect Table"))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.Orange).value);
                ImGui.TextWrapped(
                    "Note:\nThis changes all monster cards that reference this id, you cannot change an individual monster effect you must change the effect in this table and then change the effect id on the monster to match the effect you want");
                ImGui.PopStyleColor();
                ImGui.Separator();
                ImGui.SetNextItemWidth(200);

                if (ImGui.InputInt("Current monster effect id", ref monsterEffectTableEditorIndex))
                {
                    if (monsterEffectTableEditorIndex < 0)
                    {
                        monsterEffectTableEditorIndex = Effects.MonsterEffectsList.Count - 1;
                    }
                    else if (monsterEffectTableEditorIndex >= Effects.MonsterEffectsList.Count)
                    {
                        monsterEffectTableEditorIndex = 0;
                    }
                }
                for (int monsterEffectIndex = 0; monsterEffectIndex < 5; monsterEffectIndex++)
                {
                    currentMonsterEffectDataUpper[monsterEffectIndex] = (int)Effects.MonsterEffectsList[monsterEffectTableEditorIndex]
                        .Effects[monsterEffectIndex].EffectDataUpper;
                    currentMonsterEffectDataLower[monsterEffectIndex] = (int)Effects.MonsterEffectsList[monsterEffectTableEditorIndex]
                        .Effects[monsterEffectIndex].EffectDataLower;
                }
                for (int horizontalHeaderIndex = 0; horizontalHeaderIndex < effectsTableHorizontalHeaders.Length; horizontalHeaderIndex++)
                {
                    if (ImGui.BeginTable(effectsTableHorizontalHeaders[horizontalHeaderIndex], 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                    {
                        ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed);
                        ImGui.TableSetupColumn(effectsTableHorizontalHeaders[horizontalHeaderIndex],
                            ImGuiTableColumnFlags.WidthStretch);

                        ImGui.TableHeadersRow();
                        for (int verticalHeaderIndex = 0; verticalHeaderIndex < effectsTableVerticalHeaders.Length; verticalHeaderIndex++)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.Text(effectsTableVerticalHeaders[verticalHeaderIndex]);

                            ImGui.TableSetColumnIndex(1);
                            switch (horizontalHeaderIndex)
                            {
                                case 0:
                                    ImGui.Text(
                                        $"{Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].effectName} : {(int)(Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].EffectId)}");
                                    ImGui.SameLine();

                                    break;
                                case 1:
                                    ImGui.Text(
                                        $"{Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].searchModeName} : {(int)(Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].SearchMode)}");
                                    break;
                                case 2:
                                    ImGui.SetNextItemWidth(100);
                                    if (ImGui.InputInt($"##Monster Effect Data Upper ({effectsTableVerticalHeaders[verticalHeaderIndex]})",
                                            ref currentMonsterEffectDataUpper[verticalHeaderIndex], 0))
                                    {
                                        currentMonsterEffectDataUpper[verticalHeaderIndex] =
                                            Math.Clamp(currentMonsterEffectDataUpper[verticalHeaderIndex], 0, 65535);
                                        Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].EffectDataUpper =
                                            (ushort)currentMonsterEffectDataUpper[verticalHeaderIndex];
                                    }
                                    ImGui.SetNextItemWidth(100);
                                    if (ImGui.InputInt($"##Monster Effect Data Lower ({effectsTableVerticalHeaders[verticalHeaderIndex]})",
                                            ref currentMonsterEffectDataLower[verticalHeaderIndex],
                                            0))
                                    {
                                        currentMonsterEffectDataLower[verticalHeaderIndex] =
                                            Math.Clamp(currentMonsterEffectDataLower[verticalHeaderIndex], 0, 65535);
                                        Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].EffectDataLower =
                                            (ushort)currentMonsterEffectDataLower[verticalHeaderIndex];
                                    }
                                    break;
                            }
                        }
                        ImGui.EndTable();
                    }
                }
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Edit Magic Effect Table"))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.Orange).value);
                ImGui.TextWrapped(
                    "Note:\nThis changes all magic cards that reference this id, you cannot change an individual magic effect you must change the effect in this table and then change the effect id on the card to match the effect you want");
                ImGui.PopStyleColor();
                ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.CornflowerBlue).value);
                ImGui.TextWrapped(
                    "Second Note:\nRitual card materials are hardcoded there is no support for changing them at the moment just the resulting monster");
                ImGui.PopStyleColor();
                ImGui.Separator();

                ImGui.SetNextItemWidth(200);
                if (ImGui.InputInt("Current Magic effect id", ref magicEffectTableEditorIndex))
                {
                    if (magicEffectTableEditorIndex < 0)
                    {
                        magicEffectTableEditorIndex = Effects.MagicEffectsList.Count - 1;
                    }
                    else if (magicEffectTableEditorIndex >= Effects.MagicEffectsList.Count)
                    {
                        magicEffectTableEditorIndex = 0;
                    }
                }
                currentMagicEffectDataUpper = (int)Effects.MagicEffectsList[magicEffectTableEditorIndex].EffectDataUpper;
                currentMagicEffectDataLower = (int)Effects.MagicEffectsList[magicEffectTableEditorIndex].EffectDataLower;
                for (int i = 0; i < effectsTableHorizontalHeaders.Length; i++)
                {
                    if (ImGui.BeginTable(effectsTableHorizontalHeaders[i], 1, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                    {
                        ImGui.TableSetupColumn(effectsTableHorizontalHeaders[i],
                            ImGuiTableColumnFlags.WidthStretch);
                        ImGui.TableHeadersRow();
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);

                        ImGui.TableSetColumnIndex(0);
                        switch (i)
                        {
                            case 0:
                                ImGui.Text(Effects.MagicEffectsList[magicEffectTableEditorIndex].effectName);
                                break;
                            case 1:
                                ImGui.Text(Effects.MagicEffectsList[magicEffectTableEditorIndex].searchModeName);
                                break;
                            case 2:
                                ImGui.SetNextItemWidth(100);
                                if (ImGui.InputInt($"##Magic Effect Data Upper ({effectsTableVerticalHeaders})", ref currentMagicEffectDataUpper, 0))
                                {
                                    currentMagicEffectDataUpper = Math.Clamp(currentMagicEffectDataUpper, 0, 65535);
                                    Effects.MagicEffectsList[magicEffectTableEditorIndex].EffectDataUpper = (ushort)currentMagicEffectDataUpper;
                                }
                                ImGui.SetNextItemWidth(100);
                                if (ImGui.InputInt($"##Magic Effect Data Lower ({effectsTableVerticalHeaders})", ref currentMagicEffectDataLower, 0))
                                {
                                    currentMagicEffectDataLower = Math.Clamp(currentMagicEffectDataLower, 0, 65535);
                                    Effects.MagicEffectsList[currentMagicEffectDataLower].EffectDataLower = (ushort)currentMagicEffectDataLower;
                                }
                                break;
                        }
                    }
                    ImGui.EndTable();
                }
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }

    }

    void RenderProperties()
    {
        Vector2 topRow = ImGui.GetCursorPos();
        ImGui.Text("Attribute");
        ImGui.SetNextItemWidth(125);
        if (currentCardIndex < 683)
        {
            if (ImGui.Combo("##Attribute", ref currentCardAttribute, CardAttribute.AttributeNames, CardAttribute.AttributeNames.Length))
            {
                currentCardConst.Attribute = (byte)currentCardAttribute;
            }
        }
        var kindPos = new Vector2(ImGui.CalcTextSize("Attribute").X + 30, topRow.Y);
        ImGui.SetCursorPos(kindPos);
        ImGui.Text("Kind");

        float textWidth = ImGui.CalcTextSize("Trap (Limited Range)").X;
        ImGui.SetNextItemWidth(textWidth);
        ImGui.SetCursorPosX(kindPos.X);

        String[] kindsToShow;

        byte selectedKey = CardKind.Kinds.ElementAt(currentCardKind).Key;
        CardKind.Kinds.TryGetValue(selectedKey, out string selectedValue);

        if (currentCardIndex < 683)
        {
            kindsToShow = CardKind.Kinds.Where(k => k.Key <= 20).Select(k => k.Value).ToArray();
        }
        else if (currentCardConst.Index >= 752 && currentCardConst.Index <= 800)
        {
            kindsToShow = new[] { "Power Up" };
            //kindsToShow = CardKind.Kinds.Where(k => k.Key == 64).Select(k => k.Value).ToArray();
        }
        else
        {
            kindsToShow = CardKind.Kinds.Where(k => k.Key > 20 && k.Key != 64).Select(k => k.Value).ToArray();
        }

        currentCardKind = Array.IndexOf(kindsToShow, selectedValue);

        if (currentCardKind < 0 || currentCardKind >= kindsToShow.Length)
        {
            currentCardKind = 0;
        }
        if (ImGui.Combo("##Kind", ref currentCardKind, kindsToShow, kindsToShow.Length))
        {
            var result = CardKind.Kinds
                .FirstOrDefault(k => k.Value == kindsToShow[currentCardKind])
                .Key;
            currentCardConst.Kind = result;
            currentCardConst.setCardColor();

        }
        if (ImGui.IsItemHovered())
        {

            ImGui.BeginTooltip();
            if (currentCardConst.CardKind.isMonster())
            {
                ImGui.TextColored(new GuiColour(Color.Green).value, "Free to change for monsters");
            }
            else
            {
                ImGui.TextColored(new GuiColour(Color.Red).value, "Do not change for non monsters unless you know what you are doing");
            }
            ImGui.EndTooltip();

        }
        ImGui.SetCursorPos(kindPos + new Vector2(textWidth + ImGui.GetStyle().ItemSpacing.X, 0));

        ImGui.Text("Cost");

        ImGui.SetCursorPos(kindPos + new Vector2(textWidth + 100 + ImGui.GetStyle().ItemSpacing.X * 2, 0));
        ImGui.Text("DC");

        ImGui.SetCursorPosX(kindPos.X + textWidth + ImGui.GetStyle().ItemSpacing.X);

        ImGui.SetNextItemWidth(100);

        if (ImGui.SliderInt("##Select Value", ref currentCardSummonCost, 0, 12))
        {
            currentCardConst.Level = (byte)currentCardSummonCost;
        }

        ImGui.SameLine();
        ImGui.SetNextItemWidth(80);
        if (ImGui.InputInt("##DC", ref currentCardDeckCost, 0))
        {
            currentCardDeckCost = Math.Clamp(currentCardDeckCost, 0, 99);
            currentCardConst.DeckCost = (byte)currentCardDeckCost;
        }
        if (ImGui.RadioButton("Appears in reincarnation", currentCardConst.AppearsInReincarnation))
        {
            currentCardConst.AppearsInReincarnation = !currentCardConst.AppearsInReincarnation;
        }

        ImGui.SameLine();
        if (ImGui.RadioButton("Enable Password", currentCardConst.PasswordWorks))
        {
            currentCardConst.PasswordWorks = !currentCardConst.PasswordWorks;
        }


        if (ImGui.RadioButton("Is Slot Rare", currentCardConst.IsSlotRare))
        {
            currentCardConst.IsSlotRare = !currentCardConst.IsSlotRare;
        }
        ImGui.SameLine();
        if (ImGui.RadioButton("In slots", currentCardConst.AppearsInSlotReels))
        {
            currentCardConst.AppearsInSlotReels = !currentCardConst.AppearsInSlotReels;
        }
        ImGui.SameLine();
        if (ImGui.RadioButton($"Strong on toon terrain", MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[49]))
        {
            MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[49] =
                !MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[49];
        }
        ImGui.Separator();

        ImGui.Text("Leader Abilities");
        ImGui.Dummy(new Vector2(0, 20));
        if (currentCardIndex < 683)
        {
            abilityInstance = CardDeckLeaderAbilities.MonsterAbilities[currentCardIndex];

            //Do deck leader Abilities
            for (int i = 0; i < DataAccess.CardLeaderAbilityTypeCount; i++)
            {
                if (abilityInstance.Abilities[i].Name == "???")
                {
                    continue;
                }
                currentAbilityRankIndex[i] = abilityInstance.Abilities[i].RankRequired;
                ImGui.Text(abilityInstance.Abilities[i].Name);
                ImGui.SameLine();
                if (ImGui.RadioButton($"##{abilityInstance.Abilities[i].Name}", abilityInstance.Abilities[i].IsEnabled))
                {
                    abilityInstance.Abilities[i].ToggleEnabled();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(DeckLeaderAbilityInfo.NameAndDescriptions[abilityInstance.Abilities[i].AbilityIndex][1]);
                }
                if (abilityInstance.Abilities[i].IsEnabled && i >= 2)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.CornflowerBlue).value);
                    ImGui.Text("Rank Requirement");
                    ImGui.SameLine();
                    if (ImGui.Combo($"##RR{i}", ref currentAbilityRankIndex[i], unlockRanksNames, 13))
                    {
                        abilityInstance.Abilities[i].RankRequired = currentAbilityRankIndex[i];
                    }
                    ImGui.PopStyleColor();
                }
            }
        }
    }

    Vector2 AutoSizeTextToRect(string @string, Vector2 size)
    {

        ImGui.PushFont(font);
        Vector2 textSize = ImGui.CalcTextSize(@string);
        ImGui.PopFont();
        float scaleX = 1;
        float scaleY = 1;

        scaleX = size.X / textSize.X;
        scaleY = size.Y / textSize.Y;
        float finalScale = Math.Min(scaleX, scaleY);

        font.Scale *= finalScale;
        ImGui.PushFont(font);
        textSize = ImGui.CalcTextSize(@string);
        ImGui.PopFont();
        return textSize;
    }

    public void Free()
    {
        EditorWindow.OnIsoLoaded -= onCardChanged;
    }

    public void SaveCardChanges()
    {
        DataAccess.Instance.WriteCardConstantData(CardConstant.AllBytes);
    }
}