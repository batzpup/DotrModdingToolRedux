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
    HashSet<ModdedStringName> selectedCards = new();
    bool showHelpText = true;

    List<ModdedStringName> filteredList = new();
    CardColourType currentCardType = CardColourType.NormalMonster;
    IntPtr cardImage;
    IntPtr cardFrame;

    IntPtr starImagePtr; //16x16 size
    string cardName;
    // xTextPadding only was used in Stats/Buttons (and y was unused) ; moving them there

    DeckLeaderAbilityInstance abilityInstance;
    string[] unlockRanksNames = Enum.GetNames(typeof(DeckLeaderRank));
    int[] currentAbilityRankIndex = new int[20];
    MonsterEffects? currentEffects = null;
    string[] effectsTableVerticalHeaders = Enum.GetNames(typeof(MonsterEffects.MonsterEffectType));
    string[] effectsTableHorizontalHeaders = { "Effect Id", "Effect Target", "Effect Target Scope", "Extra Data" };
    int[] currentMonsterEffectDataUpper = new int[5];
    int[] currentMonsterEffectDataLower = new int[5];
    int[] currentMonsterEffectId = new int[5];
    int[] currentMonsterEffectSearchMode = new int[5];
    int[] currentMonsterEffectSearchModeTargeting = new int[5];

    int monsterEffectTableEditorIndex = 0;
    int currentMagicEffectDataUpper;
    int currentMagicEffectDataLower;
    int magicEffectTableEditorIndex;
    int currentMagicEffectId;
    int currentMagicEffectSearchMode;
    int currentMagicEffectSearchModeTargeting;

    ImGuiModalPopup _modalPopup = new ImGuiModalPopup();
    bool allowEffectEditing = false;
    bool showEditEffectWarning = true;

    string currentMonsterEffectText = String.Empty;
    bool showMonsterEditNote = true;

    private int? scrollToIndex;

    // Image Scaling
    float imageScale = 2.4f ; // Window can afford a 20% zoom
    Vector2 cardImageSize;
    Vector2 frameImageSize;
    Vector2 textBoxTopLeftInnerOffsetInPixelsScaled;
    Vector2 innerTextBoxSizeInPixelsScaled;
    public float getImageScale() { return this.imageScale; }
    public void setImageScale(float isc) { this.imageScale = isc; }
    public void imageRescale()
    {
        this.cardImageSize.X = 192 * getImageScale();
        this.cardImageSize.Y = 192 * getImageScale();
        this.frameImageSize.X = 256 * getImageScale();
        this.frameImageSize.Y = 368 * getImageScale();
        this.textBoxTopLeftInnerOffsetInPixelsScaled.X = 19 * getImageScale();
        this.textBoxTopLeftInnerOffsetInPixelsScaled.Y = 22 * getImageScale();
        this.innerTextBoxSizeInPixelsScaled.X = 218 * getImageScale();
        this.innerTextBoxSizeInPixelsScaled.Y = 26 * getImageScale();
    }
    
    
    public CardEditorWindow()
    {
        starImagePtr = ImageHelper.LoadImageImgui($"Images.cardExtras.star.png");
        font = FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 32);
        smallerFont = FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 26);

        EditorWindow.OnIsoLoaded += updateCardChanges;
        EditorWindow.OnIsoLoaded += FilterAndSort;
        selectedCards.Add(Card.cardNameList[currentCardIndex]);
        currentCardIndex = 0;

        // Initial (imageScale = 2.4 with 20% zoom) ; actual scaling requires Window Data -> Check Render()
        cardImageSize = new Vector2(192 * imageScale, 192 * imageScale);
        frameImageSize = new Vector2(256 * imageScale, 368 * imageScale);
        textBoxTopLeftInnerOffsetInPixelsScaled = new Vector2(19, 22) * imageScale;
        innerTextBoxSizeInPixelsScaled = new Vector2(218, 26) * imageScale;
        
    }


    public void SetCurrentCard(ModdedStringName name)
    {
        selectedCards.Clear();
        currentCardIndex = Array.IndexOf(Card.cardNameList, name);
        updateCardChanges();
        FilterAndSort();
        selectedCards.Add(name);
        scrollToIndex = currentCardIndex;
    }

    void updateCardChanges()
    {

        currentCardConst = CardConstant.List[currentCardIndex];
        cardImage = GlobalImages.Instance.OriginalCards[Card.cardNameList[currentCardIndex].Default];
        cardFrame = GlobalImages.Instance.CardFrames[currentCardConst.CardColor];
        cardName = StringEditor.StringTable[currentCardIndex + StringEditor.CardNamesOffsetStart];
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
        
        // Image Rescale : Linear Min, Base 2.0f on 1920x1080, 20% zoom ; non-zero with WindowMinSize from program.cs
        setImageScale(1.2f * Math.Min(windowSize.X / 960, windowSize.Y / 540));
        imageRescale();
        
        
        _modalPopup.Draw();
        ImGui.BeginChild("LeftThirdPanel", new Vector2(windowSize.X / 3f, windowSize.Y),
            ImGuiChildFlags.Border);
            
        ImGui.Checkbox("Show help", ref showHelpText);
        ImGui.SameLine();
        ImGui.ColorEdit4("Difference highlight colour", ref UserSettings.CardEditorDifferenceHighlightColour,
            ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs);

        
        if (showHelpText)
        {
            ImGui.PushFont(smallerFont);
            ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.CornflowerBlue).value);
            ImGui.TextWrapped(
                "You can Select multiple cards like in Deck Editor, but only one category (Monster, Spell, Powerup, Trap, Ritual) at a time.");
            //ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.Text, UserSettings.CardEditorDifferenceHighlightColour);
            ImGui.TextWrapped(
                "If a field shows this colour, then some selected cards don't share the same value there.");
            ImGui.PopStyleColor();
            //ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.CornflowerBlue).value);            
            ImGui.TextWrapped(
                "Example : if you select all Light Dragons, they would share Attribute and Type, but not Level and DC.");
            ImGui.PopStyleColor();
            ImGui.PopFont();
        }


        ImGui.PushFont(FontManager.GetBestFitFont("Attribute", true));
        ImGui.Separator();
        ImGui.Text("Sort by");
        ImGui.BeginGroup();

        string[] sortButtons = { "ID", "Name", "ATK", "DEF", "Level", "Attribute", "Type", "DC" };
        float lineWidth = 0;
        float maxWidth = ImGui.GetContentRegionAvail().X;

        for (int i = 0; i < sortButtons.Length; i++)
        {
            string field = sortButtons[i];
            Vector2 buttonSize = ImGui.CalcTextSize(field);
            buttonSize.X += ImGui.GetStyle().FramePadding.X * 2 + ImGui.GetStyle().ItemSpacing.X;


            if (lineWidth + buttonSize.X > maxWidth && lineWidth > 0)
            {
                lineWidth = 0;
            }
            else if (i > 0 && lineWidth > 0)
            {
                ImGui.SameLine();
            }

            if (ImGui.Button(field))
            {
                if (cardEditorSearchSortField == field)
                {
                    cardEditorSearchAscending = !cardEditorSearchAscending;
                }
                else
                {
                    cardEditorSearchSortField = field;
                    cardEditorSearchAscending = true;
                }
                FilterAndSort();
            }

            lineWidth += buttonSize.X;
        }

        ImGui.EndGroup();

        if (ImGui.Checkbox("Ascending", ref cardEditorSearchAscending))
        {
            FilterAndSort();
        }

        ImGui.PopFont();

        ImGui.PushItemWidth(windowSize.X / 3f);
        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 18f);

        ImGui.Text("Card Search");
        Vector2 availArea = ImGui.GetContentRegionAvail();
        ImGui.SetNextItemWidth(availArea.X);
        if (ImGui.InputText("##CardSearch", ref cardSearch, 32))
        {
            FilterAndSort();
        }
        
        Vector4 listBoxBg = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
        ImGui.PushStyleColor(ImGuiCol.TableRowBg, listBoxBg);
        ImGui.PushStyleColor(ImGuiCol.TableRowBgAlt, listBoxBg);

        // Guaranteeing minimum vertical size for the Card List Table. Base row height is 36.
        availArea = ImGui.GetContentRegionAvail();
        if (ImGui.BeginTable("##CardListTable", 2, ImGuiTableFlags.ScrollY | ImGuiTableFlags.RowBg, new Vector2(availArea.X, Math.Max(availArea.Y, 6 * 36 + 9)) ))
        {
            ImGui.TableSetupColumn("##ID", ImGuiTableColumnFlags.WidthFixed, 45);
            ImGui.TableSetupColumn("##Name", ImGuiTableColumnFlags.WidthStretch);

            int count = 0;
            foreach (ModdedStringName filteredName in filteredList)
            {
                bool isSelected = selectedCards.Contains(filteredName);
                int cardId = CardConstant.CardLookup[filteredName].Index;

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);

                //ImGui.PushFont(FontManager.GetBestFitFont(filteredName.Current, availArea.X, availArea.Y, FontManager.FontFamily.NotoSansJP));
                if (scrollToIndex.HasValue && scrollToIndex.Value == count)
                {
                    ImGui.SetScrollHereY(0.5f);
                    scrollToIndex = null;
                }
                if (ImGui.Selectable($"##{filteredName}", isSelected, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                {

                    if (ImGui.GetIO().KeyShift)
                    {
                        int startIndex = filteredList.IndexOf(Card.GetNameByIndex(currentCardIndex));
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
                bool hovered = ImGui.IsItemHovered();
                ImGui.SameLine();
                ImGui.Text($"{cardId}");

                ImGui.TableSetColumnIndex(1);
                ImGui.Text($"{filteredName}");
                //ImGui.PopFont();

                if (hovered)
                {
                    GlobalImgui.RenderTooltipCardImage(filteredName.Default);
                }
                count++;
            }
            ImGui.EndTable();

        }
        ImGui.PopItemWidth();
        ImGui.PopStyleVar(1);
        ImGui.PopStyleColor(2);

        updateCardChanges();
        ImGui.PopFont();
        ImGui.EndChild();


        ImGui.SameLine();


        // ImGuiChildFlags.Border is looking useless - see quick-fix below if using.
        ImGui.BeginChild("MiddlePanel", new Vector2(windowSize.X / 3, windowSize.Y), ImGuiChildFlags.None /* | ImGuiChildFlags.Border */ );

        // Card Frame, centered in MiddlePanel
        Vector2 middleWindowSize = ImGui.GetContentRegionAvail();
        Vector2 pos = new Vector2((middleWindowSize.X / 2f) - frameImageSize.X / 2f, (middleWindowSize.Y / 2f) - frameImageSize.Y / 2f);
        //pos.X = pos.X + middleWindowSize.X * 0.02f; // 2% ImGui offset quick-fix ; useful if Border is on.
        ImGui.SetCursorPos(pos);
        ImGui.Image(cardFrame, frameImageSize);

        // Card Attribute
        ImGui.SetCursorPos(pos + textBoxTopLeftInnerOffsetInPixelsScaled +
                                  new Vector2(innerTextBoxSizeInPixelsScaled.X - (28 - 3) * imageScale, 3 * imageScale));
        ImGui.Image(GlobalImages.Instance.CardElements[(AttributeVisual)currentCardAttribute], new Vector2(22 * imageScale, 22 * imageScale));
        
        // Card Name
        Vector2 textSize = AutoSizeTextToRect(cardName, innerTextBoxSizeInPixelsScaled - new Vector2(30 * imageScale, 0));
        Vector2 fixedTextBoxPos = pos + textBoxTopLeftInnerOffsetInPixelsScaled + 
                                  new Vector2(4 * imageScale, 0);
        Vector2 innerTextBoxPos = fixedTextBoxPos + 
                                  new Vector2(0, innerTextBoxSizeInPixelsScaled.Y / 2f - textSize.Y / 2f);
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));
        ImGui.SetCursorPos(innerTextBoxPos);
        ImGui.PushFont(font);
        ImGui.Text(Card.cardNameList[currentCardIndex].Edited);
        ImGui.PopFont();

        // Level ; using fixedTextBoxPos to keep Y Position constant
        for (int i = currentCardConst.Level; i > 0; i--)
        {
            ImGui.SetCursorPos(fixedTextBoxPos + innerTextBoxSizeInPixelsScaled +
                                  new Vector2((-17 + i * -16) * imageScale, 11 * imageScale));
            ImGui.Image(starImagePtr, new Vector2(16 * imageScale, 16 * imageScale));
        }
        
        // Stats and Buttons
        float xTextPadding = imageScale * (25f/12f); // Linear on imageScale, 5.0f at 2.4f
        float yTextPadding = imageScale * (4f/12f); // Linear on imageScale, 0.8f at 2.4f
        if (currentCardConst.CardKind.isMonster())
        {
            ImGui.PushFont(font);

            // Stat and Button size ; yTextPadding 3 times (before ATK, between ATK and DEF, after DEF)
            Vector2 textStatSize = AutoSizeTextToRect(currentMonsterStatString, new Vector2( 120 * imageScale, ( (46 / 2f) * imageScale - 3f * yTextPadding ) ) );

            ImGui.PushItemWidth(108 * imageScale - xTextPadding);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.2f, 0.2f, 0.2f, 0.5f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.1f, 0.1f, 0.1f, 0.5f));

            // ATK
            ImGui.SetCursorPos(pos + new Vector2( 100 * imageScale + xTextPadding, 301 * imageScale + yTextPadding ));
            if (ImGui.InputInt("##attackValue", ref currentMonsterAttack, 100))
            {
                currentMonsterAttack = Math.Clamp(currentMonsterAttack, 0, 8191);
                currentCardConst.Attack = (ushort)currentMonsterAttack;
            }
            
            // DEF
            ImGui.SetCursorPos(pos + new Vector2( 100 * imageScale + xTextPadding, 301 * imageScale + yTextPadding + (textStatSize.Y + yTextPadding) ) );
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
        cardImage = GlobalImages.Instance.OriginalCards[Card.cardNameList[currentCardIndex].Default];
        ImGui.Image(cardImage, cardImageSize);

        /*
        ImGui.SetCursorPos(new Vector2(ImGui.GetWindowSize().X / 2, 75));
        Vector2 mousePos = ImGui.GetMousePos() - ImGui.GetWindowPos();
        ImGui.Text($"Mouse coordinates \n X: {mousePos.X}, Y: {mousePos.Y}");
        */

        /*
        ImGui.SetCursorPosX(pos.X + frameImageSize.X + 25);
        ImGui.SetCursorPosY(0);
        ImGui.SetCursorPosX(pos.X + frameImageSize.X + 25);
		*/
        
        ImGui.EndChild();
        
        
        ImGui.SameLine();
        
        
        font.Scale = 1f;
        ImGui.PushFont(FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 30));
        ImGui.PushStyleColor(ImGuiCol.TabSelected, new GuiColour(0, 189, 0).value);
        ImGui.PushStyleColor(ImGuiCol.TabHovered, new GuiColour(128, 128, 0).value);
        
        ImGui.BeginChild("RightSidePanel", Vector2.Zero, ImGuiChildFlags.Border /*| ImGuiChildFlags.NavFlattened | ImGuiChildFlags.AlwaysAutoResize */ );

        if (ImGui.BeginTabBar("CardEditorMode"))
        {
            if (ImGui.BeginTabItem("Properties"))
            {
                RenderProperties();
                ImGui.EndTabItem();
            }
            
            // Monster stuff. Moving Leader Skills to their own Tab.
            // Also, "Enchantments only works for Monsters" -> Restricting tab to Monsters.
            // Separate checks if something ever applies to Spells.
            if (currentCardConst.CardKind.Id / 32 == 0) // Card Pool modding and Boost Glitch Fix aware
            {
                if (ImGui.BeginTabItem("Leader"))
                {
                    RenderLeaderAbilities();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Equips"))
                {
                    RenderMonsterEquips();
                    ImGui.EndTabItem();
                }
            }
            
            // Powerup stuff
            if (currentCardConst.CardKind.Id / 32 == 2) // Card Pool modding and Boost Glitch Fix aware
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

        ImGui.EndChild();
    }

    void FilterAndSort()
    {
        if (UserSettings.UseDefaultNames)
        {
            filteredList = Card.cardNameList
                .Where(cardName => cardName.Default.Contains(cardSearch, StringComparison.OrdinalIgnoreCase)
                                   || CardConstant.CardLookup[cardName].Index.ToString().Contains(cardSearch))
                .ToList();
        }
        else
        {
            filteredList = Card.cardNameList
                .Where(cardName => cardName.Current.Contains(cardSearch, StringComparison.OrdinalIgnoreCase)
                                   || CardConstant.CardLookup[cardName].Index.ToString().Contains(cardSearch))
                .ToList();
        }

        filteredList.Sort((a, b) =>
        {
            if (!CardConstant.CardLookup.TryGetValue(a, out var cardA)
                || !CardConstant.CardLookup.TryGetValue(b, out var cardB))
            {
                return 0;
            }


            int result = 0;
            switch (cardEditorSearchSortField)
            {
                case "Name":
                    result = string.Compare(cardA.Name.Current, cardB.Name.Current, StringComparison.OrdinalIgnoreCase);
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

    void RenderLeaderAbilities()
    {
        /* Now unused ; now also Card pool modding and Boost Glitch Fix aware, in case
        if (currentCardConst.CardKind.Id / 32 != 0)
        {
            ImGui.Text("Leader Abilities only work for monster cards");
            return;
        }
        */

        ImFontPtr LSHeadFont = FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 32);
        ImFontPtr LSMainFont = FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 28);
        Vector2 LSYPad = new Vector2(0, 10);
        Dictionary<int, string> LSCat = new Dictionary<int, string>()
        {
            // LS ID and Category
            { 0, "Special Abilities" },
            { 4, "Full Range Abilities" },
            { 10, "Support Range Abilities" },
        };
        ImGui.PushFont(LSMainFont);
        
        abilityInstance = CardDeckLeaderAbilities.MonsterAbilities[currentCardIndex];
        
        for (int i = 0; i < DataAccess.CardLeaderAbilityTypeCount; i++)
        {
            // Skip unknown LS - Release
            if (abilityInstance.Abilities[i].Name == "???")
            {
                continue;
            }
            //
            
            // Check for new Category
            foreach (var cat in LSCat)
            {
                if (i == cat.Key)
                {
                    // New Category : Push Head Font, Write Title, Pop to Main, Add Padding
                    if (i > 0)
                    {
                        ImGui.Dummy(LSYPad);
                    };
                    ImGui.PushFont(LSHeadFont);
                    ImGui.Text(cat.Value);
                    ImGui.PopFont();
                    ImGui.Dummy(LSYPad);
                }
            }
            
            /* Print LS and Skip unknown - Debug
            Console.WriteLine("<LS> " + i + "/" + DataAccess.CardLeaderAbilityTypeCount + ":" + abilityInstance.Abilities[i].Name);
            if (abilityInstance.Abilities[i].Name == "???")
            {
                continue;
            }
            */
            
            currentAbilityRankIndex[i] = abilityInstance.Abilities[i].RankRequired;
            
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
                ImGui.BeginTooltip();
                float wrapWidth = ImGui.GetMainViewport().Size.X
                                  - ImGui.GetStyle().WindowPadding.X * 2;

                ImGui.PushTextWrapPos(wrapWidth);
                ImGui.TextUnformatted(DeckLeaderAbilityInfo.NameAndDescriptions[abilityInstance.Abilities[i].AbilityIndex][1]);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
            
            ImGui.SameLine();
            ImGui.TextWrapped(abilityInstance.Abilities[i].Name);
            
            if (abilityInstance.Abilities[i].IsEnabled && i >= 2)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.CornflowerBlue).value);
                
                // Guarantee some space for the Rank box and avoid stretching it too far
                float minItemWidth = 60f;
                float maxItemWidth = 120f;
                if (ImGui.CalcTextSize("Rank Requirement").X + minItemWidth > ImGui.GetContentRegionAvail().X)
                {
                    ImGui.Text("Rank");
                } else {
                    ImGui.Text("Rank Requirement");
                }

                ImGui.SameLine();

                ColourMatchFrame(() => !AllSelectedMonsterLeaderAbilityHaveSame(i, c => c.RankRequired), () =>
                {
                    ImGui.SetNextItemWidth( Math.Min( Math.Max(ImGui.GetContentRegionAvail().X, minItemWidth), maxItemWidth) );
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
        
        ImGui.PopFont(); // Pop LSMainFont away
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
            ImGui.PopItemWidth();
            
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
            ImGui.PopItemWidth();
            
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
        /* Now unused ; now also Card pool modding and Boost Glitch Fix aware, in case
        if (currentCardConst.CardKind.Id / 32 != 0)
        {
            ImGui.Text("Enchantments only work for monster cards");
            return;
        }
        */

        for (int i = 0; i < 50; i++)
        {
            if (i == 47 || i == 48 || i == 49)
            {
                //Don't show Insect Imitation and Metalmorph due to hard coded equip logic
                //Don't show strong on toon as its in properties
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
        string name = Card.cardNameList[currentCardIndex].Edited;
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputText("##NameText", ref name, 32))
        {
            Card.cardNameList[currentCardIndex].Edited = name;
        }
        ImGui.Spacing();

        ImGui.Text("Card Text: ");
        string text = StringEditor.StringTable[StringEditor.CardEffectTextOffsetStart + currentCardIndex];
        ImGui.InputTextMultiline("##EffectText", ref text, 200, new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeight() * 15));
        if (ImGui.Button("Reset"))
        {
            text = "~";
        }
        StringEditor.StringTable[StringEditor.CardEffectTextOffsetStart + currentCardIndex] = text;
    }

    void RenderEffects()
    {
        ImGui.TextColored(new GuiColour(Color.Orange).value, "Works but is a WIP");
        if (ImGui.Checkbox("Allow Effect editing", ref allowEffectEditing))
        {
            if (showEditEffectWarning && allowEffectEditing)
            {
                _modalPopup.Show(
                    "WARNING:\nNot all effect combinations work, what works and what doesn't isn't fully known.\nIf an effect doesnt work it could be because its not supported by the game to exist.\nHide this prompt in the future?",
                    "Warning", HideEffectWarning, ImGuiModalPopup.ShowType.YesNo);

            }
        }
        if (ImGui.BeginTabBar("EffectTabBar", ImGuiTabBarFlags.None))
        {
            if (ImGui.BeginTabItem("View Card Effects"))
            {
                if (currentCardConst.CardKind.Id / 32 != 0) // Card pool modding and Boost Glitch Fix aware
                {
                    ImGui.Text($"Original effect ({Effects.NonMonsterOwners.ElementAt(magicEffectTableEditorIndex)})");
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
                                        ImGui.Text(
                                            $"{Effects.NonMonsterEffectsList[currentCardConst.EffectId].effectName} : {(int)Effects.NonMonsterEffectsList[currentCardConst.EffectId].EffectId}");
                                        break;
                                    case 1:
                                        ImGui.Text(
                                            $"{Effects.NonMonsterEffectsList[currentCardConst.EffectId].SearchModeName} : {(int)Effects.NonMonsterEffectsList[currentCardConst.EffectId].SearchMode}");
                                        break;
                                    case 2:
                                        ImGui.Text(
                                            $"{Effects.NonMonsterEffectsList[currentCardConst.EffectId].SearchModeTargetingName} : {(int)Effects.NonMonsterEffectsList[currentCardConst.EffectId].SearchModeTargeting}");
                                        break;
                                    case 3:
                                        ImGui.Text(Effects.NonMonsterEffectsList[currentCardConst.EffectId].EffectDataUpper.ToString());
                                        ImGui.Separator();
                                        ImGui.Text(Effects.NonMonsterEffectsList[currentCardConst.EffectId].EffectDataLower.ToString());
                                        break;
                                }
                            }
                        }
                        ImGui.EndTable();
                    }
                }
                else
                {
                    if (currentCardConst.EffectId != 65535)
                    {
                        ImGui.Text($"Original effect ({Effects.MonsterEffectOwners.ElementAt(currentCardConst.EffectId).Value.Current})");
                    }
                    else
                    {
                        ImGui.Text($"Original effect (no effect)");
                    }

                    ImGui.Text($"Monster Effect Id:");
                    ImGui.SameLine();
                    int effectId = currentCardConst.EffectId;

                    ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.EffectId), () =>
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
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
                                            ImGui.Text(currentEffects.Effects[i].SearchModeName);
                                            break;
                                        case 2:
                                            ImGui.Text(currentEffects.Effects[i].SearchModeTargetingName);
                                            break;
                                        case 3:
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
            if (!allowEffectEditing)
            {
                ImGui.BeginDisabled();
            }



            if (ImGui.BeginTabItem("Edit Monster Effect Table"))
            {
                if (currentMonsterEffectText == String.Empty)
                {
                    foreach (var monster in CardConstant.Monsters)
                    {
                        if (monster.EffectId == monsterEffectTableEditorIndex)
                        {
                            currentMonsterEffectText = StringEditor.StringTable[StringEditor.CardEffectTextOffsetStart + monster.Index];
                            break;
                        }

                    }
                }




                ImGui.Separator();

                ImGui.Checkbox("Show note", ref showMonsterEditNote);

                if (showMonsterEditNote)
                {

                    ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.Orange).value);
                    ImGui.TextWrapped(
                        "Note:\nThis changes all monster cards that reference this id, you cannot change an individual monster effect you must change the effect in this table and then change the effect id on the monster to match the effect you want");
                    ImGui.PopStyleColor();
                }
                ImGui.Text($"Original: ({Effects.MonsterEffectOwners.ElementAt(monsterEffectTableEditorIndex).Value.Current})");
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
                    foreach (var monster in CardConstant.Monsters)
                    {
                        if (monster.EffectId == monsterEffectTableEditorIndex)
                        {
                            currentMonsterEffectText = StringEditor.StringTable[StringEditor.CardEffectTextOffsetStart + monster.Index];
                            break;
                        }

                    }
                }
                ImGui.Text("Current Text:");
                ImGui.Text(currentMonsterEffectText);
                for (int monsterEffectIndex = 0; monsterEffectIndex < 5; monsterEffectIndex++)
                {
                    currentMonsterEffectDataUpper[monsterEffectIndex] = (int)Effects.MonsterEffectsList[monsterEffectTableEditorIndex]
                        .Effects[monsterEffectIndex].EffectDataUpper;
                    currentMonsterEffectDataLower[monsterEffectIndex] = (int)Effects.MonsterEffectsList[monsterEffectTableEditorIndex]
                        .Effects[monsterEffectIndex].EffectDataLower;

                    currentMonsterEffectId[monsterEffectIndex] =
                        (int)Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[monsterEffectIndex].EffectId;
                    currentMonsterEffectSearchMode[monsterEffectIndex] =
                        (int)Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[monsterEffectIndex].SearchMode;
                    currentMonsterEffectSearchModeTargeting[monsterEffectIndex] = (int)Effects.MonsterEffectsList[monsterEffectTableEditorIndex]
                        .Effects[monsterEffectIndex].SearchModeTargeting;

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
                            if (horizontalHeaderIndex == 0)
                            {
                                if (ImGui.Button($"Disable##{verticalHeaderIndex}"))
                                {
                                    Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].DisableEffect();
                                }
                            }
                            ImGui.TableSetColumnIndex(1);
                            switch (horizontalHeaderIndex)
                            {
                                case 0:
                                    ImGui.Text($"{Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].effectName}");
                                    if (ImGui.InputInt($"##EffectId{verticalHeaderIndex}", ref currentMonsterEffectId[verticalHeaderIndex]))
                                    {
                                        int value = currentMonsterEffectId[verticalHeaderIndex];
                                        int wrappedValue = ((value - 1) % 88 + 88) % 88 + 1;
                                        Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].EffectId =
                                            (EffectId)wrappedValue;
                                    }
                                    break;
                                case 1:
                                    ImGui.Text(
                                        $"{Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].SearchModeName}");
                                    if (ImGui.InputInt($"##EffectTarget{verticalHeaderIndex}", ref currentMonsterEffectSearchMode[verticalHeaderIndex]))
                                    {
                                        int value = currentMonsterEffectSearchMode[verticalHeaderIndex];
                                        int wrappedValue = (value % 62 + 62) % 62;

                                        Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].SearchMode =
                                            (SearchMode)wrappedValue;
                                    }
                                    break;
                                case 2:
                                    ImGui.Text(
                                        $"{Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].SearchModeTargetingName}");
                                    if (ImGui.InputInt($"##EffectTargetType{verticalHeaderIndex}",
                                            ref currentMonsterEffectSearchModeTargeting[verticalHeaderIndex], 0x40))
                                    {
                                        int value = currentMonsterEffectSearchModeTargeting[verticalHeaderIndex];
                                        int wrappedValue;

                                        if (value > 255)
                                        {
                                            wrappedValue = 0x00;
                                        }
                                        else if (value < 0xc0 && value > 0x80)
                                        {
                                            wrappedValue = 0xc0;
                                        }
                                        else
                                        {
                                            wrappedValue = (value % 0x100 + 0x100) % 0x100;
                                        }
                                        Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[verticalHeaderIndex].SearchModeTargeting =
                                            (SearchModeTargeting)wrappedValue;
                                    }
                                    break;
                                case 3:
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

                ImGui.Text($"Original: ({Effects.NonMonsterOwners.ElementAt(magicEffectTableEditorIndex)})");
                ImGui.SetNextItemWidth(200);
                if (ImGui.InputInt("Current Magic effect index", ref magicEffectTableEditorIndex))
                {
                    if (magicEffectTableEditorIndex < 0)
                    {
                        magicEffectTableEditorIndex = Effects.NonMonsterEffectsList.Count - 1;
                    }
                    else if (magicEffectTableEditorIndex >= Effects.NonMonsterEffectsList.Count)
                    {
                        magicEffectTableEditorIndex = 0;
                    }
                }
                currentMagicEffectId = (int)Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].EffectId;
                currentMagicEffectSearchMode = (int)Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].SearchMode;
                currentMagicEffectSearchModeTargeting = (int)Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].SearchModeTargeting;
                currentMagicEffectDataUpper = (int)Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].EffectDataUpper;
                currentMagicEffectDataLower = (int)Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].EffectDataLower;

                if (ImGui.Button($"Disable##Magic"))
                {
                    Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].DisableEffect();
                }
                for (int i = 0; i < effectsTableHorizontalHeaders.Length; i++)
                {
                    if (ImGui.BeginTable(effectsTableHorizontalHeaders[i], 1, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                    {
                        ImGui.TableSetupColumn(effectsTableHorizontalHeaders[i],
                            ImGuiTableColumnFlags.WidthStretch);
                        ImGui.TableHeadersRow();
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        switch (i)
                        {
                            case 0:
                                ImGui.Text($"{Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].effectName}");
                                if (ImGui.InputInt($"##EffectId{i}", ref currentMagicEffectId))
                                {
                                    int value = currentMagicEffectId;
                                    int wrappedValue = ((value - 1) % 88 + 88) % 88 + 1;
                                    Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].EffectId =
                                        (EffectId)wrappedValue;
                                }
                                break;
                            case 1:
                                ImGui.Text(
                                    $"{Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].SearchModeName}");
                                if (ImGui.InputInt($"##EffectTarget{i}", ref currentMagicEffectSearchMode))
                                {
                                    int value = currentMagicEffectSearchMode;
                                    int wrappedValue = (value % 62 + 62) % 62;

                                    Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].SearchMode =
                                        (SearchMode)wrappedValue;
                                }
                                break;
                            case 2:
                                ImGui.Text(
                                    $"{Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].SearchModeTargetingName}");
                                if (ImGui.InputInt($"##EffectTarget{i}",
                                        ref currentMagicEffectSearchModeTargeting, 0x40))
                                {
                                    int value = currentMagicEffectSearchModeTargeting;
                                    int wrappedValue;

                                    if (value > 255)
                                    {
                                        wrappedValue = 0x00;
                                    }
                                    else if (value < 0xc0 && value > 0x80)
                                    {
                                        wrappedValue = 0xc0;
                                    }
                                    else
                                    {
                                        wrappedValue = (value % 0x100 + 0x100) % 0x100;
                                    }
                                    Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].SearchModeTargeting =
                                        (SearchModeTargeting)wrappedValue;
                                }
                                break;
                            case 3:
                                ImGui.SetNextItemWidth(100);
                                if (ImGui.InputInt($"##Magic Effect Data Upper ({effectsTableVerticalHeaders[i]})",
                                        ref currentMagicEffectDataUpper, 0))
                                {
                                    currentMagicEffectDataUpper =
                                        Math.Clamp(currentMonsterEffectDataUpper[i], 0, 65535);
                                    Effects.MonsterEffectsList[monsterEffectTableEditorIndex].Effects[i].EffectDataUpper =
                                        (ushort)currentMagicEffectDataUpper;
                                }
                                ImGui.SetNextItemWidth(100);
                                if (ImGui.InputInt($"##Magic Effect Data Lower ({effectsTableVerticalHeaders[i]})",
                                        ref currentMagicEffectDataLower,
                                        0))
                                {
                                    currentMagicEffectDataLower =
                                        Math.Clamp(currentMagicEffectDataLower, 0, 65535);
                                    Effects.NonMonsterEffectsList[magicEffectTableEditorIndex].EffectDataLower =
                                        (ushort)currentMagicEffectDataLower;
                                }
                                break;
                        }
                    }

                    ImGui.EndTable();
                }
                ImGui.EndTabItem();
            }
            if (!allowEffectEditing)
            {
                ImGui.EndDisabled();
            }

            ImGui.EndTabBar();
        }

    }

    void HideEffectWarning()
    {
        showEditEffectWarning = false;
    }

    void RenderProperties()
    {
        Dictionary<string, string> SectionTextMaxWidth = new Dictionary<string, string>() {
            { "Attribute", "Water        " }, // 8x Whitespace for arrow stuff
            { "Kind", "Winged-Beast        " },
            { "Level", "12" },
            { "DC", "255" }, // Support for DC beyond 99
        };

        // Leader Abilities now have their own tab ; we got LOADS of room.
        // Newlining most stuff. In fact, Type fits on 360p ; only Level and DC should SameLine.
              
        /*
        float lineWidth = 0;
        float maxWidth = ImGui.GetContentRegionAvail().X;
        */

        foreach (var kvp in SectionTextMaxWidth)
        {
            Vector2 textSize = ImGui.CalcTextSize(kvp.Value);
            Vector2 labelSize = ImGui.CalcTextSize(kvp.Key);
            float groupWidth = Math.Max(textSize.X, labelSize.X) + ImGui.GetStyle().FramePadding.X * 2;
            float totalWidth = groupWidth;

            /* Keeping in case we add more stuff and lineWidth becomes useful again
            if (lineWidth + groupWidth > maxWidth && lineWidth > 0)
            {
                lineWidth = 0;
            }
            else if (kvp.Key != "Attribute" && lineWidth > 0)
            {
                ImGui.SameLine();
            }
            */
            
            if (kvp.Key == "DC")
            {
                ImGui.SameLine();
            }
            else
            {
                //lineWidth = 0;
            }

            ImGui.BeginGroup();
            switch (kvp.Key)
            {
                case "Attribute":
                    if (currentCardConst.CardKind.Id / 32 == 0) // Card pool modding and Boost Glitch Fix aware
                    {
                        // Skip on non-Monsters
                        ImGui.Text("Attribute");
                        ImGui.SetNextItemWidth(groupWidth);
                        
                        int count = selectedCards.Count;
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
                    break;

                case "Kind":
                    ImGui.Text("Type");
                    ImGui.SetNextItemWidth(groupWidth);

                    String[] kindsToShow;
                    if (currentCardConst.CardKind.Id / 32 == 0) // Card Pool modding and Boost Glitch Fix aware
                    {
                        kindsToShow = CardKind.Kinds.Where(k => k.Key <= 31).Select(k => k.Value).ToArray();
                    }
                    else if (currentCardConst.CardKind.Id / 32 == 2) // Powerup ; Card Pool modding and Boost Glitch Fix aware
                    {
                        kindsToShow = new[] { "Power Up", "Magic" };
                    }
                    else
                    {
                        kindsToShow = CardKind.Kinds.Where(k => k.Key / 32 != 0 && k.Key / 32 != 2).Select(k => k.Value).ToArray();
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
                    break;

                case "Level":
                    ImGui.Text("Level");
                    ImGui.SetNextItemWidth(groupWidth * 1.2f); // Ease sliding a bit
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
                    ImGui.Dummy(new Vector2(6,-0.0f)); // Padding X, realigning Y (ImGui hotfix)
                    break;

                case "DC":
                    ImGui.Text("DC");
                    ImGui.SetNextItemWidth(groupWidth);
                    ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.DeckCost), () =>
                    {
                        if (ImGui.InputInt("##DC", ref currentCardDeckCost, 0))
                        {
                            foreach (var selectedCardName in selectedCards)
                            {
                                if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                                {
                                    currentCardDeckCost = Math.Clamp(currentCardDeckCost, 0, 255); // Support for DC beyond 99
                                    card.DeckCost = (byte)currentCardDeckCost;
                                }
                            }
                        }
                    });
                    break;
            }

            ImGui.EndGroup();
            //lineWidth += totalWidth; // Totally bypassing that on Properties
        }

        
        // Reusing fonts and Padding from LS ; not Categories
        ImFontPtr LSHeadFont = FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 32);
        ImFontPtr LSMainFont = FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 28);
        Vector2 LSYPad = new Vector2(0, 10);
        ImGui.Dummy(LSYPad);
        ImGui.PushFont(LSMainFont);
        //float radioButtonSize = ImGui.GetFrameHeight(); // Interesting, but now unused
        
        
        // Special Properties
        /* Future-proofing ; not useful right now, there only is "Strong on TOON". Uncomment when relevant.
        ImGui.PushFont(LSHeadFont);
        ImGui.Text("Special Properties");
        ImGui.PopFont();
        */
        if (currentCardConst.CardKind.isMonster())
        {
            float strongOnToonSize = ImGui.CalcTextSize("Strong on TOON").X + ImGui.GetFrameHeight() + ImGui.GetStyle().ItemSpacing.X;
            
            ColourMatchFrame(() => !AllSelectedMonsterEnchantFlagsHaveSame(49), () =>
            {
                if (ImGui.RadioButton($"Strong on TOON", MonsterEnchantData.MonsterEnchantDataList[currentCardIndex].Flags[49]))
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

        ImGui.Dummy(LSYPad);
        
        
        // Card Availability
        ImGui.PushFont(LSHeadFont);
        ImGui.Text("Card Availability");
        ImGui.PopFont();
        ImGui.Dummy(LSYPad);
        
        // Order : Slots, Slot Rares, Reincarnation, Password Option, Password Field ; Newlining everything.
        
        // SLOTS
        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.AppearsInSlotReels), () =>
        {
            if (ImGui.RadioButton("Graveyard Slots", currentCardConst.AppearsInSlotReels))
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
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Appears in the Graveyard slot machine");
            }
        });
        
        // RARE
        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.IsRareDrop), () =>
        {
            if (ImGui.RadioButton("Slot Rare", currentCardConst.IsRareDrop))
            {
                CardConstant.List[currentCardIndex].IsRareDrop = !CardConstant.List[currentCardIndex].IsRareDrop;
                foreach (var selectedCardName in selectedCards)
                {
                    if (CardConstant.CardLookup.TryGetValue(selectedCardName, out var card))
                    {
                        if (card.Index != currentCardIndex)
                        {
                            card.IsRareDrop = CardConstant.List[currentCardIndex].IsRareDrop;
                        }
                    }
                }
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("May be obtained when aligning three-in-a-row in Slots");
            }
        });
        
        // REINC
        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.AppearsInReincarnation), () =>
        {
            if (ImGui.RadioButton("Reincarnation", currentCardConst.AppearsInReincarnation))
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
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Card can be acquired from Reincarnation");
            }
        });

        // PASS
        ColourMatchFrame(() => !AllSelectedCardConstHaveSame(c => c.PasswordWorks), () =>
        {
            if (ImGui.RadioButton("Password", currentCardConst.PasswordWorks))
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
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Enable or disable use of Passwords");
            }
        });
        if (currentCardConst.PasswordWorks)
        {
            unsafe
            {
                ImGui.Text("Password:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
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
        else
        {
            ImGui.Dummy(new Vector2(0,34)); // May avoid UI issues on very specific setups
        }

        ImGui.PopFont();
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

        var firstCard = CardConstant.List.Find(c => c.Name == selectedCards.First());
        if (firstCard == null)
            throw new InvalidOperationException($"No CardConstant found for name '{selectedCards.First()}'");

        T firstValue = propertySelector(firstCard);

        return selectedCards.All(cardName =>
        {
            var card = CardConstant.List.Find(c => c.Name == cardName);
            if (card == null)
                throw new InvalidOperationException($"No CardConstant found for name '{cardName}'");

            return EqualityComparer<T>.Default.Equals(propertySelector(card), firstValue);
        });
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

    public static void ExportMonstersToCSV(string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            "Name,Type,Level,Deck Cost,Password,Password Works,in Reincarnation,in Slots,effectID,text");

        foreach (var card in CardConstant.List)
        {
            var row = new List<string>();
            row.Add(card.Name.Current);
            row.Add(card.Type.ToString());
            row.Add(card.Level.ToString());
            row.Add(card.DeckCost.ToString());
            row.Add(card.Password.ToString());
            row.Add(card.PasswordWorks.ToString());
            row.Add(card.AppearsInReincarnation.ToString());
            row.Add(card.AppearsInSlotReels.ToString());
            row.Add(card.EffectId.ToString());
            row.Add(StringEditor.StringTable[StringEditor.CardEffectTextOffsetStart + card.Index].Replace("\n", "\\n"));
            sb.AppendLine(string.Join(",", row.Select(v => $"\"{v.Replace("\"", "\"\"")}\"")));
            //sb.AppendLine(string.Join(",", row));
        }
        File.WriteAllText(filePath, sb.ToString());

    }

    public static void ImportMonstersFromCSV(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] csvLine = ParseCsvLine(line);
            if (csvLine.Length < 9)
            {
                throw new Exception($"Malformed CSV line {i}: {line}");
                continue;
            }
            CardConstant card = CardConstant.List[i - 1];
            card.Name.Default = Card.cardNameList[i - 1].Default;
            card.Name.Edited = csvLine[0];
            card.CardKind = new CardKind(CardKind.Kinds.FirstOrDefault(x => x.Value == csvLine[1]).Key);
            card.Level = byte.Parse(csvLine[2]);
            card.DeckCost = byte.Parse(csvLine[3]);
            card.Password = csvLine[4];
            card.PasswordWorks = bool.Parse(csvLine[5]);
            card.AppearsInReincarnation = bool.Parse(csvLine[6]);
            card.AppearsInSlotReels = bool.Parse(csvLine[7]);
            card.EffectId = ushort.Parse(csvLine[8]);

            string effectText = csvLine[9].Replace("\\n", "\n");
            StringEditor.StringTable[StringEditor.CardEffectTextOffsetStart + card.Index] = effectText;
            CardConstant.List[i - 1] = card;

        }
        StringEditor.ReloadStrings();
        CardConstant.RebuildLookup();
    }


    public void Free()
    {
        EditorWindow.OnIsoLoaded -= updateCardChanges;
    }

    public void SaveCardChanges()
    {
        DataAccess.Instance.WriteCardConstantData(CardConstant.AllBytes);
    }

    static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // If inside quotes and next char is another quote → it's an escaped quote
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++; // skip next char
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        result.Add(sb.ToString());
        return result.ToArray();
    }
}