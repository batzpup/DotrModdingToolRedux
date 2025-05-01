using System.Numerics;
using System.Text;
using ImGuiNET;
using Color = System.Drawing.Color;
using ImGui = ImGuiNET.ImGui;

namespace DotrModdingTool2IMGUI;

class CardEditorWindow : IImGuiWindow
{
    ImFontPtr font;
    ImFontPtr smallerFont;
    Vector2 cardImageSize;
    Vector2 frameImageSize;
    float imageScale = 2.0f;

    string cardSearch = "";
    int currentCardIndex;
    int currentMonsterAttack;
    int currentMonsterDefense;
    int currentCardSummonLevel;
    int currentCardAttribute;
    int currentCardDeckCost;
    int currentCardKind;
    string currentMonsterStatString;
    string currentCardPassword;
    CardConstant currentCardConst;
    string cardEditorSearchSortField = "ID";
    bool cardEditorSearchAscending = true;
    HashSet<string> selectedCards = new HashSet<string>();
    bool showHelpText = true;
    
    List<string> filteredList = new List<string>();
    CardColourType currentCardType = CardColourType.NormalMonster;
    IntPtr cardImage;
    IntPtr cardFrame;   
    
    //16x16 size
    IntPtr starImagePtr;
    string cardName;

    Vector2 textBoxTopLeftInnerOffsetInPixelsScaled;
    Vector2 innerTextBoxSizeInPixelsScaled;
    float xTextPadding = 5f;
    float yTextPadding = 5f;


    DeckLeaderAbilityInstance abilityInstance;
    string[] unlockRanksNames = Enum.GetNames(typeof(DeckLeaderRank));
    int[] currentAbilityRankIndex = new int[20];
    MonsterEffects? currentEffects = null;
    string[] effectsTableVerticalHeaders = Enum.GetNames(typeof(MonsterEffects.MonsterEffectType));
    string[] effectsTableHorizontalHeaders = { "Effect Id", "Effect Target", "Extra Data" };
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
        smallerFont = Fonts.LoadCustomFont(pixelSize: 26);
        cardImageSize = new Vector2(192 * imageScale, 192 * imageScale);
        frameImageSize = new Vector2(256 * imageScale, 368 * imageScale);
        textBoxTopLeftInnerOffsetInPixelsScaled = new Vector2(19, 22) * imageScale;
        innerTextBoxSizeInPixelsScaled = new Vector2(218, 26) * imageScale;
        EditorWindow.OnIsoLoaded += updateCardChanges;
        EditorWindow.OnIsoLoaded += FilterAndSort;
    }

    public void SetCurrentCard(string name)
    {

        selectedCards.Clear();
        currentCardIndex = currentCardIndex = Array.IndexOf(Card.cardNameList, name);
        updateCardChanges();
        FilterAndSort();
        selectedCards.Add(name);
    }

    void updateCardChanges()
    {

        currentCardConst = CardConstant.List[currentCardIndex];
        cardImage = GlobalImages.Instance.Cards[Card.cardNameList[currentCardIndex]];
        cardFrame = GlobalImages.Instance.CardFrames[currentCardConst.CardColor];
        cardName = currentCardConst.Name;
        currentMonsterAttack = currentCardConst.Attack;
        currentMonsterDefense = currentCardConst.Defense;
        currentMonsterStatString = currentMonsterAttack.ToString();
        currentCardSummonLevel = currentCardConst.Level;
        currentCardAttribute = CardAttribute.GetAttributeVisual(currentCardConst);
        currentCardDeckCost = currentCardConst.DeckCost;
        currentCardKind = CardKind.Kinds.Keys.ToList().IndexOf(currentCardConst.CardKind.Id);
        currentCardPassword = currentCardConst.Password;
        if (currentCardConst.EffectId != 0xffff)
        {
            currentEffects = Effects.MonsterEffectsList[currentCardConst.EffectId];
        }
        else
        {
            currentEffects = null;
        }

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
        ImGui.Text("Cards:");
        ImGui.SameLine();
        ImGui.Checkbox("Show help", ref showHelpText);
        ImGui.SameLine();
        
        ImGui.ColorEdit4("Difference highlight colour", ref UserSettings.CardEditorDifferenceHighlightColour ,ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs);
        if (showHelpText)
        {
            ImGui.PushFont(smallerFont);
            ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.CornflowerBlue).value);
            ImGui.TextWrapped(
                "Multi select works the same as the deck editor but you cannot select cards of different types (only monsters, only power up, etc..");

            ImGui.PushStyleColor(ImGuiCol.Text,UserSettings.CardEditorDifferenceHighlightColour );
            ImGui.TextWrapped(
                "If a field is this colour, it means at least one of the values across all the cards are different for the given field");
            ImGui.PopStyleColor();
            ImGui.TextWrapped(
                "Example: if you select all the light dragons, their attribute and Kind would be normal but their level and DC would be orange because they are different values");
            ImGui.PopStyleColor();
            ImGui.PopFont();
        }


        ImGui.Separator();
        ImGui.Text("Sort by");
        if (ImGui.Button("ID"))
        {
            if (cardEditorSearchSortField == "ID")
            {
                cardEditorSearchAscending = !cardEditorSearchAscending;
            }
            else
            {
                cardEditorSearchSortField = "ID";
                cardEditorSearchAscending = true;
            }
            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Button("Name"))
        {
            if (cardEditorSearchSortField == "Name")
            {
                cardEditorSearchAscending = !cardEditorSearchAscending;
            }
            else
            {
                cardEditorSearchSortField = "Name";
                cardEditorSearchAscending = true;
            }
            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Button("ATK"))
        {
            if (cardEditorSearchSortField == "ATK")
            {
                cardEditorSearchAscending = !cardEditorSearchAscending;
            }
            else
            {
                cardEditorSearchSortField = "ATK";
                cardEditorSearchAscending = true;
            }
            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Button("DEF"))
        {
            if (cardEditorSearchSortField == "DEF")
            {
                cardEditorSearchAscending = !cardEditorSearchAscending;
            }
            else
            {
                cardEditorSearchSortField = "DEF";
                cardEditorSearchAscending = true;
            }

            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Button("Level"))
        {
            if (cardEditorSearchSortField == "Level")
            {
                cardEditorSearchAscending = !cardEditorSearchAscending;
            }
            else
            {
                cardEditorSearchSortField = "Level";
                cardEditorSearchAscending = true;
            }

            FilterAndSort();
        }
        ImGui.SameLine();

        ImGui.SameLine();

        if (ImGui.Button("Attribute"))
        {
            if (cardEditorSearchSortField == "Attribute")
            {
                cardEditorSearchAscending = !cardEditorSearchAscending;
            }
            else
            {
                cardEditorSearchSortField = "Attribute";
                cardEditorSearchAscending = true;
            }

            FilterAndSort();
        }

        ImGui.SameLine();
        if (ImGui.Button("Kind"))
        {
            if (cardEditorSearchSortField == "Kind")
            {
                cardEditorSearchAscending = !cardEditorSearchAscending;
            }
            else
            {
                cardEditorSearchSortField = "Kind";
                cardEditorSearchAscending = true;

            }
            FilterAndSort();
        }

        ImGui.SameLine();
        if (ImGui.Button("DC"))
        {
            if (cardEditorSearchSortField == "DC")
            {
                cardEditorSearchAscending = !cardEditorSearchAscending;
            }
            else
            {
                cardEditorSearchSortField = "DC";
                cardEditorSearchAscending = true;
            }

            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Checkbox("Ascending", ref cardEditorSearchAscending))
        {
            FilterAndSort();
        }



        float availableHeight = windowBottom - ImGui.GetCursorPosY();
        ImGui.PushItemWidth(windowSize.X / 3f);

        if (ImGui.BeginListBox("##Cards", new Vector2(0, availableHeight)))
        {
            ImGui.Text("Card Search");
            ImGui.SetNextItemWidth(windowSize.X / 3f);
            if (ImGui.InputText("##CardSearch", ref cardSearch, 32))
            {
                FilterAndSort();
            }

            foreach (string filteredName in filteredList)
            {
                bool isSelected = selectedCards.Contains(filteredName);

                if (ImGui.Selectable($"{filteredName}", isSelected, ImGuiSelectableFlags.AllowDoubleClick))
                {

                    if (ImGui.GetIO().KeyShift)
                    {
                        int startIndex = filteredList.IndexOf(Card.cardNameList[currentCardIndex]);
                        int endIndex = filteredList.IndexOf(filteredName);
                        if (startIndex != -1 && endIndex != -1)
                        {
                            for (int i = Math.Min(startIndex, endIndex); i <= Math.Max(startIndex, endIndex); i++)
                            {
                                if (CanSelectCard(CardConstant.CardLookup[filteredList[i]]))
                                {
                                    selectedCards.Add(filteredList[i]);

                                }
                            }
                            if (CanSelectCard(CardConstant.CardLookup[filteredName]))
                            {
                                currentCardIndex = Array.IndexOf(Card.cardNameList, filteredName);
                            }
                        }
                    }
                    else if (ImGui.GetIO().KeyCtrl)
                    {
                        if (CanSelectCard(CardConstant.CardLookup[filteredName]))
                        {
                            if (selectedCards.Add(filteredName))
                            {
                                currentCardIndex = Array.IndexOf(Card.cardNameList, filteredName);
                            }
                            else
                            {
                                selectedCards.Remove(filteredName);
                                currentCardIndex = Array.IndexOf(Card.cardNameList, selectedCards.Last());
                            }

                        }
                    }
                    else
                    {
                        selectedCards.Clear();
                        selectedCards.Add(filteredName);
                        currentCardIndex = Array.IndexOf(Card.cardNameList, filteredName);
                    }

                }

                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(filteredName);
                }
            }


            ImGui.EndListBox();
        }
        updateCardChanges();
        ImGui.PopFont();
        ImGui.EndChild();


        ImGui.SameLine();


        ImGui.BeginChild("MiddlePanel", new Vector2(windowSize.X / 3, windowSize.Y), ImGuiChildFlags.Border | ImGuiChildFlags.AlwaysAutoResize);

        //Draw card frame
        Vector2 middleWindowSize = ImGui.GetContentRegionAvail();
        Vector2 pos = new Vector2((middleWindowSize.X / 2f) - frameImageSize.X / 2f, (middleWindowSize.Y / 2f) - frameImageSize.Y / 2f);
        ImGui.SetCursorPos(pos);
        ImGui.Image(cardFrame, frameImageSize);

        Vector2 textSize = AutoSizeTextToRect(cardName, innerTextBoxSizeInPixelsScaled - new Vector2(22 * imageScale, 0));
        Vector2 InnerTextBoxPos = pos + textBoxTopLeftInnerOffsetInPixelsScaled +
                                  new Vector2(0, innerTextBoxSizeInPixelsScaled.Y / 2f - textSize.Y / 2f);

        ImGui.SetCursorPos(pos + textBoxTopLeftInnerOffsetInPixelsScaled +
                           new Vector2(innerTextBoxSizeInPixelsScaled.X - 22 * imageScale, 4 * imageScale));

        ImGui.Image(GlobalImages.Instance.CardElements[(AttributeVisual)currentCardAttribute], new Vector2(22 * imageScale, 22 * imageScale));
        //DRAW TEXT
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));
        ImGui.SetCursorPos(InnerTextBoxPos);
        ImGui.PushFont(font);
        ImGui.Text(cardName);
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
                currentMonsterAttack = Math.Clamp(currentMonsterAttack, 0, 8191);
                currentCardConst.Attack = (ushort)currentMonsterAttack;
            }
            ImGui.SetCursorPos(pos + new Vector2(100 * imageScale + xTextPadding, 301 * imageScale + 46 * imageScale / 2f));

            if (ImGui.InputInt("##defenseValue", ref currentMonsterDefense, 100))
            {
                currentMonsterDefense = Math.Clamp(currentMonsterDefense, 0, 8191);
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
                RenderMonsterEquips();
                ImGui.EndTabItem();
            }
            if (currentCardConst.CardKind.Id == 64 && currentCardConst.Index >= 752 && currentCardConst.Index <= 800)
            {
                if (ImGui.BeginTabItem("Enchant Data"))
                {
                    RenderEnchantData();
                    ImGui.EndTabItem();
                }
            }
            else
            {
                if (ImGui.BeginTabItem("Effect"))
                {
                    RenderEffects();
                    ImGui.EndTabItem();
                }
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

    void FilterAndSort()
    {

        filteredList = Card.cardNameList
            .Where(cardName => cardName.Contains(cardSearch, StringComparison.OrdinalIgnoreCase))
            .ToList();

        filteredList.Sort((a, b) =>
        {
            if (!CardConstant.CardLookup.TryGetValue(a, out var cardA) || !CardConstant.CardLookup.TryGetValue(b, out var cardB))
                return 0;

            int result = 0;
            switch (cardEditorSearchSortField)
            {
                case "Name":
                    result = string.Compare(cardA.Name, cardB.Name, StringComparison.OrdinalIgnoreCase);
                    break;
                case "ID":
                    result = cardA.Index.CompareTo(cardB.Index);
                    break;
                case "Kind":
                    result = cardA.Kind.CompareTo(cardB.Kind);
                    break;
                case "Attribute":
                    int comparison = CardAttribute.GetAttributeVisual(cardA).CompareTo(CardAttribute.GetAttributeVisual(cardB));
                    if (comparison == 0)
                    {
                        result = cardA.Index.CompareTo(cardB.Index);
                    }
                    else
                    {
                        result = comparison;
                    }
                    break;
                case "DC":
                    result = cardA.DeckCost.CompareTo(cardB.DeckCost);
                    break;
                case "ATK":
                    result = cardA.Attack.CompareTo(cardB.Attack);
                    break;
                case "DEF":
                    result = cardA.Defense.CompareTo(cardB.Defense);
                    break;
                case "Level":
                    result = cardA.Level.CompareTo(cardB.Level);
                    break;
            }
            return cardEditorSearchAscending ? result : -result;
        });
    }


    void RenderEnchantData()
    {
        ImGuiTableColumnFlags columnFlags = ImGuiTableColumnFlags.WidthStretch;
        if (ImGui.BeginTable("##EnchantIdTable", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupColumn("Enchant Id Name", columnFlags, 1);
            ImGui.TableSetupColumn("Enchant Id", columnFlags, 1);

            ImGui.TableHeadersRow();
            ImGui.TableNextRow();
            int currentEnchantId = EnchantData.EnchantIds[currentCardIndex - Card.EquipCardStartIndex];

            ImGui.TableSetColumnIndex(0);
            ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X / 2f);
            ImGui.Text(EnchantData.GetEnchantIdName(currentEnchantId));
            ImGui.TableNextColumn();

            ColourMatchFrame(() => !AllSelectedEnchantDataHaveSame(index => EnchantData.EnchantIds[index]), () =>
            {
                if (ImGui.InputInt("##EnchantId", ref currentEnchantId, 0))
                {
                    currentEnchantId = Math.Clamp(currentEnchantId, 0, 50);
                    foreach (var selectedCard in selectedCards)
                    {
                        EnchantData.EnchantIds[CardConstant.CardLookup[selectedCard].Index - Card.EquipCardStartIndex] = (byte)currentEnchantId;
                    }


                }
            });
            ImGui.EndTable();
        }
        if (ImGui.BeginTable("##EnchantScoreTable", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupColumn("Enchant Score Name", columnFlags, 1);
            ImGui.TableSetupColumn("Enchant Score", columnFlags, 1);
            ImGui.TableHeadersRow();
            ImGui.TableNextRow();

            int currentEnchantScore = EnchantData.EnchantScores[currentCardIndex - Card.EquipCardStartIndex];
            ImGui.TableSetColumnIndex(0);
            ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X / 2f);
            ImGui.Text(EnchantData.GetEnchantScoreName(currentEnchantScore));

            ImGui.TableNextColumn();

           ColourMatchFrame(() => !AllSelectedEnchantDataHaveSame(index => EnchantData.EnchantScores[index]), () =>
            {
                if (ImGui.InputInt("##EnchantScore", ref currentEnchantScore, 0))
                {
                    currentEnchantScore = Math.Clamp(currentEnchantScore, 0, 8191);
                    foreach (var selectedCard in selectedCards)
                    {
                        EnchantData.EnchantScores[CardConstant.CardLookup[selectedCard].Index - Card.EquipCardStartIndex] = (ushort)currentEnchantScore;
                    }


                }
            });
            ImGui.EndTable();
        }

    }

    void RenderMonsterEquips()
    {
        if (currentCardIndex < 0 || currentCardIndex > 682)
        {
            ImGui.Text("Enchantments only work for monster cards");
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
            ColourMatchFrame(() => !AllSelectedMonsterEnchantFlagsHaveSame(i), () =>
            {

                if (ImGui.RadioButton($"##equip{i}", MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[i]))
                {
                    MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[i] =
                        !MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[i];
                    foreach (var selectedCard in selectedCards)
                    {
                        int cardIndex = CardConstant.CardLookup[selectedCard].Index;
                        MonsterEnchantData.MonsterEnchantDataList[cardIndex].Flags[i] =
                            MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[i];
                    }

                }
            });
        }
    }

    void RenderCardText()
    {
        ImGui.Text("Card Name: ");
        ImGui.Text("(Read only,highlightable for copy paste purposes)");
        string name = StringDecoder.StringTable[StringDecoder.CardNamesOffset + currentCardIndex];
        ImGui.InputText("##NameText", ref name, 32, ImGuiInputTextFlags.ReadOnly | ImGuiInputTextFlags.AutoSelectAll);
        ImGui.Spacing();
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
                    ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.EffectId), () =>
                    {

                        if (ImGui.InputInt("##EffectId", ref effectId))
                        {
                            foreach (var selectedCard in selectedCards)
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

                                CardConstant.CardLookup[selectedCard].EffectId = (ushort)effectId;
                                CardConstant.CardLookup[selectedCard].setCardColor();
                            }
                        }
                    });

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
                ImGui.Text($"Original");
                ImGui.SetNextItemWidth(200);

                if (ImGui.InputInt("Current monster effect index", ref monsterEffectTableEditorIndex))
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
                if (ImGui.InputInt("Current Magic effect index", ref magicEffectTableEditorIndex))
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
                                ImGui.Text(
                                    $"{Effects.MagicEffectsList[magicEffectTableEditorIndex].effectName} : {(int)Effects.MagicEffectsList[magicEffectTableEditorIndex].EffectId}");
                                break;
                            case 1:
                                ImGui.Text(
                                    $"{Effects.MagicEffectsList[magicEffectTableEditorIndex].searchModeName} : {(int)Effects.MagicEffectsList[magicEffectTableEditorIndex].SearchMode}");
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
                                    Effects.MagicEffectsList[magicEffectTableEditorIndex].EffectDataLower = (ushort)currentMagicEffectDataLower;
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

            ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.Attribute), () =>
            {
                GlobalImgui.CardEditorCombo("##Attribute", ref currentCardAttribute, CardAttribute.AttributeNames, (newValue) =>
                {
                    foreach (var selectedCardName in selectedCards)
                    {
                        if (Array.IndexOf(Card.cardNameList, selectedCardName) < 683 &&
                            CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                        {
                            card.Attribute = (byte)newValue;
                        }
                    }
                });
            });

        }
        var kindPos = new Vector2(ImGui.CalcTextSize("Attribute").X + 30, topRow.Y);
        ImGui.SetCursorPos(kindPos);
        ImGui.Text("Kind");

        float textWidth = ImGui.CalcTextSize("Trap (Limited Range)").X;
        ImGui.SetNextItemWidth(textWidth);
        ImGui.SetCursorPosX(kindPos.X);

        String[] kindsToShow;



        if (currentCardConst.Index < 683)
        {
            kindsToShow = CardKind.Kinds.Where(k => k.Key <= 20).Select(k => k.Value).ToArray();

        }
        else if (currentCardConst.Index >= 752 && currentCardConst.Index <= 800)
        {
            kindsToShow = new[] { "Power Up", "Magic" };
            //kindsToShow = CardKind.Kinds.Where(k => k.Key == 64).Select(k => k.Value).ToArray();
        }
        else
        {
            kindsToShow = CardKind.Kinds.Where(k => k.Key > 20 && k.Key != 64).Select(k => k.Value).ToArray();
        }

        byte selectedKey = CardKind.Kinds.ElementAt(currentCardKind).Key;
        CardKind.Kinds.TryGetValue(selectedKey, out string selectedValue);
        currentCardKind = Array.IndexOf(kindsToShow, selectedValue);

        if (currentCardKind < 0 || currentCardKind >= kindsToShow.Length)
        {
            currentCardKind = 0;
        }
        bool allKindSame = AllSelectedCardConstHaveSame(c => c.Kind);
        if (!allKindSame)
        {
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new GuiColour(Color.Orange).value);
        }
        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.Kind), () =>
        {
            GlobalImgui.CardEditorCombo("##Kind", ref currentCardKind, kindsToShow, (newValue) =>
            {
                foreach (var selectedCardName in selectedCards)
                {
                    if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                    {
                        card.Kind = CardKind.Kinds
                            .FirstOrDefault(k => k.Value == kindsToShow[currentCardKind])
                            .Key;
                        currentCardConst.setCardColor();
                    }
                }
            });
        });
        if (!allKindSame)
        {
            ImGui.PopStyleColor();
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

        ImGui.Text("Level");

        ImGui.SetCursorPos(kindPos + new Vector2(textWidth + 100 + ImGui.GetStyle().ItemSpacing.X * 2, 0));
        ImGui.Text("DC");

        ImGui.SetCursorPosX(kindPos.X + textWidth + ImGui.GetStyle().ItemSpacing.X);

        ImGui.SetNextItemWidth(100);

        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.Level), () =>
        {
            if (ImGui.SliderInt("##Select Value", ref currentCardSummonLevel, 0, 12))
            {
                foreach (var selectedCardName in selectedCards)
                {
                    if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                    {
                        card.Level = (byte)currentCardSummonLevel;
                    }
                }
            }
        });

        ImGui.SameLine();
        ImGui.SetNextItemWidth(80);
        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.DeckCost), () =>
        {
            if (ImGui.InputInt("##DC", ref currentCardDeckCost, 0))
            {
                foreach (var selectedCardName in selectedCards)
                {
                    if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                    {
                        currentCardDeckCost = Math.Clamp(currentCardDeckCost, 0, 99);
                        card.DeckCost = (byte)currentCardDeckCost;
                    }
                }

            }
        });
        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.AppearsInReincarnation), () =>
        {
            if (ImGui.RadioButton("Appears in reincarnation", currentCardConst.AppearsInReincarnation))
            {
                currentCardConst.AppearsInReincarnation = !currentCardConst.AppearsInReincarnation;
                foreach (var selectedCardName in selectedCards)
                {
                    if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                    {
                        card.AppearsInReincarnation = currentCardConst.AppearsInReincarnation;
                    }
                }
            }
        });
        ImGui.SameLine();
        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.PasswordWorks), () =>
        {
            if (ImGui.RadioButton("Enable Password", currentCardConst.PasswordWorks))
            {
                currentCardConst.PasswordWorks = !currentCardConst.PasswordWorks;
                foreach (var selectedCardName in selectedCards)
                {
                    if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                    {
                        card.PasswordWorks = currentCardConst.PasswordWorks;
                    }
                }
            }
        });
        if (currentCardConst.PasswordWorks)
        {
            unsafe
            {
                ImGui.Text("Password:");
                ImGui.SameLine();

                if (ImGui.InputText($"##password{currentCardConst.Index}", ref currentCardPassword, 8, ImGuiInputTextFlags.CallbackCharFilter,
                        FilterPasswordInput))
                {
                    if (currentCardPassword.Length != 8)
                    {
                        StringBuilder stringBuilder = new StringBuilder(currentCardPassword);
                        for (int i = currentCardPassword.Length; i < 8; i++)
                        {
                            stringBuilder.Append('0');
                        }
                        currentCardPassword = stringBuilder.ToString();
                    }
                    currentCardConst.Password = currentCardPassword;
                }
            }
        }


        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.IsSlotRare), () =>
        {
            if (ImGui.RadioButton("Is Slot Rare", currentCardConst.IsSlotRare))
            {
                CardConstant.List[currentCardIndex].IsSlotRare = !CardConstant.List[currentCardIndex].IsSlotRare;
                foreach (var selectedCardName in selectedCards)
                {
                    if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                    {
                        if (card.Index != currentCardIndex)
                        {
                            card.IsSlotRare = CardConstant.List[currentCardIndex].IsSlotRare;
                        }
                    }
                }
            }
        });
        ImGui.SameLine();
        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.AppearsInSlotReels), () =>
        {
            if (ImGui.RadioButton("In slots", currentCardConst.AppearsInSlotReels))
            {
                CardConstant.List[currentCardIndex].AppearsInSlotReels = !CardConstant.List[currentCardIndex].AppearsInSlotReels;
                foreach (var selectedCardName in selectedCards)
                {
                    if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                    {
                        if (card.Index != currentCardIndex)
                        {
                            card.AppearsInSlotReels = CardConstant.List[currentCardIndex].AppearsInSlotReels;
                        }
                    }
                }
            }
        });
        if (currentCardConst.CardKind.isMonster())
        {
            ImGui.SameLine();

            ColourMatchFrame(() => !AllSelectedMonsterEnchantFlagsHaveSame(49), () =>
            {
                if (ImGui.RadioButton($"Strong on toon terrain", MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[49]))
                {
                    MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[49] =
                        !MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[49];
                    foreach (var selectedCardName in selectedCards)
                    {
                        if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                        {
                            if (card.Index != currentCardIndex)
                            {
                                MonsterEnchantData.MonsterEnchantDataList[card.Index].Flags[49] =
                                    MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[49];
                            }
                        }
                    }
                }
            });
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
                ColourMatchFrame(() => !AllSelectedMonsterLeaderAbilityHaveSame(i, c => c.IsEnabled), () =>
                {
                    if (ImGui.RadioButton($"##{abilityInstance.Abilities[i].Name}", abilityInstance.Abilities[i].IsEnabled))
                    {
                        abilityInstance.Abilities[i].ToggleEnabled();
                        foreach (var selectedCardName in selectedCards)
                        {
                            if (currentCardIndex != CardConstant.CardLookup[selectedCardName].Index)
                            {
                                CardDeckLeaderAbilities.MonsterAbilities[CardConstant.CardLookup[selectedCardName].Index].Abilities[i]
                                    .SetEnabled(abilityInstance.Abilities[i].IsEnabled);
                            }

                        }
                    }
                });
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(DeckLeaderAbilityInfo.NameAndDescriptions[abilityInstance.Abilities[i].AbilityIndex][1]);
                }
                if (abilityInstance.Abilities[i].IsEnabled && i >= 2)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.CornflowerBlue).value);
                    ImGui.Text("Rank Requirement");
                    ImGui.SameLine();

                    ColourMatchFrame(() => !AllSelectedMonsterLeaderAbilityHaveSame(i, c => c.RankRequired), () =>
                    {
                        GlobalImgui.CardEditorCombo($"##RR{i}", ref currentAbilityRankIndex[i], unlockRanksNames, (newValue) =>
                        {
                            foreach (var selectedCardName in selectedCards)
                            {
                                DeckLeaderAbilityInstance instance =
                                    CardDeckLeaderAbilities.MonsterAbilities[CardConstant.CardLookup[selectedCardName].Index];
                                instance.Abilities[i].SetEnabled(true);
                                instance.Abilities[i].RankRequired = currentAbilityRankIndex[i];
                            }
                        });
                    });
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

    unsafe int FilterPasswordInput(ImGuiInputTextCallbackData* data)
    {
        char typedChar = (char)data->EventChar;
        if (!(char.IsLetterOrDigit(typedChar) && ((typedChar >= 'a' && typedChar <= 'z') || (typedChar >= 'A' && typedChar <= 'Z') ||
                                                  (typedChar >= '0' && typedChar <= '9'))))
        {
            return 1;
        }

        return 0;
    }

    bool CanSelectCard(CardConstant card)
    {
        if (selectedCards.Count == 0)
            return true; // No restriction if nothing is selected

        CardConstant firstSelected = CardConstant.CardLookup[selectedCards.First()];

        if (firstSelected.CardKind.isMonster()) return card.CardKind.isMonster();
        if (firstSelected.CardKind.isMagic()) return card.CardKind.isMagic();
        if (firstSelected.CardKind.isPowerUp()) return card.CardKind.isPowerUp();
        if (firstSelected.CardKind.isTrap()) return card.CardKind.isTrap();
        if (firstSelected.CardKind.isRitual()) return card.CardKind.isRitual();

        return false;
    }

    bool AllSelectedCardConstHaveSame<T>(Func<CardConstant, T> propertySelector)
    {
        if (selectedCards.Count == 0) return true;

        T firstValue = propertySelector(CardConstant.List.Find(c => c.Name == selectedCards.First()));
        return selectedCards.All(cardName =>
            EqualityComparer<T>.Default.Equals(propertySelector(CardConstant.List.Find(c => c.Name == cardName)), firstValue));
    }
bool AllSelectedEnchantDataHaveSame<T>(Func<int, T> propertySelector)
{
    if (selectedCards.Count == 0) return true;

    int firstEnchantIndex = CardConstant.CardLookup[selectedCards.First()].Index - Card.EquipCardStartIndex;
    
    if (firstEnchantIndex < 0 || firstEnchantIndex >= EnchantData.EnchantIds.Count)
        return false;

    T firstValue = propertySelector(firstEnchantIndex);

    return selectedCards.All(cardName =>
    {
        if (!CardConstant.CardLookup.TryGetValue(cardName, out var card)) 
            return false; 

        int enchantIndex = card.Index - Card.EquipCardStartIndex;
        
        if (enchantIndex < 0 || enchantIndex >= EnchantData.EnchantIds.Count)
            return false;

        return EqualityComparer<T>.Default.Equals(propertySelector(enchantIndex), firstValue);
    });
}



    bool AllSelectedMonsterEnchantFlagsHaveSame(int flagNumber)
    {
        if (selectedCards.Count == 0) return true;
        bool firstFlagValue = MonsterEnchantData.MonsterEnchantDataList[CardConstant.CardLookup[selectedCards.First()].Index].Flags[flagNumber];
        return selectedCards.All(cardName =>
        {
            var cardIndex = CardConstant.CardLookup[cardName].Index;
            return MonsterEnchantData.MonsterEnchantDataList[cardIndex].Flags[flagNumber] == firstFlagValue;
        });
    }


    bool AllSelectedMonsterLeaderAbilityHaveSame<T>(int index, Func<DeckLeaderAbility, T> propertySelector)
    {
        if (selectedCards.Count == 0) return true;


        T firstPropertyValue =
            propertySelector(CardDeckLeaderAbilities.MonsterAbilities[CardConstant.CardLookup[selectedCards.First()].Index].Abilities[index]);

        return selectedCards.All(cardName =>
        {
            var cardIndex = CardConstant.CardLookup[cardName].Index;
            T currentPropertyValue = propertySelector(CardDeckLeaderAbilities.MonsterAbilities[cardIndex].Abilities[index]);


            return EqualityComparer<T>.Default.Equals(currentPropertyValue, firstPropertyValue);
        });
    }


    public void ColourMatchFrame(Func<bool> checkCondition, Action functionBody)
    {
        if (checkCondition())
        {
            ImGui.PushStyleColor(ImGuiCol.FrameBg, UserSettings.CardEditorDifferenceHighlightColour);
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, UserSettings.CardEditorDifferenceHighlightColour);
        }
        functionBody();
        if (checkCondition())
        {
            ImGui.PopStyleColor(2);
        }
    }


    public void Free()
    {
        EditorWindow.OnIsoLoaded -= updateCardChanges;
    }

    public void SaveCardChanges()
    {
        DataAccess.Instance.WriteCardConstantData(CardConstant.AllBytes);
    }
}