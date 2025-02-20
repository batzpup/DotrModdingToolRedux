using System.Numerics;
using System.Text;
using ImGuiNET;
using Raylib_cs;
namespace DotrModdingTool2IMGUI;

public class DeckEditorWindow : IImGuiWindow
{
    ImGuiTableFlags tableFlags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable | ImGuiTableFlags.Sortable |
                                 ImGuiTableFlags.SortMulti | ImGuiTableFlags.BordersV | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders |
                                 ImGuiTableFlags.ScrollX |
                                 ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerH;

    List<Deck> deckLists = new List<Deck>();
    List<CardConstant> sortedTrunkList = new List<CardConstant>();
    List<DeckCard> sortedDeckList = new List<DeckCard>();
    HashSet<int> trunkSelection = new HashSet<int>();

    List<string>
        failedToSaveDecks = new List<string>();

    bool openDeckErrors;
    StringBuilder deckErrorText = new StringBuilder();
    ImFontPtr fontToUse;
    Deck currentDeck;
    int currentDeckListIndex = 0;
    int lastDeckListIndex = 0;
    int enemyImageIndex;
    int deckLeaderImage;
    public ImGuiModalPopup modalPopup = new ImGuiModalPopup();
    string trunkSearchFilter = "";

    Vector4 highlightColour = new GuiColour(8, 153, 154, 155).value;

    public Action<string> ViewCardInEditor;

    public DeckEditorWindow(ImFontPtr fontPtr)
    {
        fontToUse = fontPtr;

    }

    public void Render()
    {
        float availableHeight = ImGui.GetContentRegionAvail().Y;
        ImGui.BeginChild("Trunk", new Vector2(ImGui.GetContentRegionAvail().X / 2f, availableHeight),
            ImGuiChildFlags.Border | ImGuiChildFlags.AlwaysAutoResize);
        DrawTrunkTable();
        ImGui.EndChild();
        ImGui.SameLine();
        ImGui.BeginChild("DeckList", new Vector2(ImGui.GetContentRegionAvail().X, availableHeight),
            ImGuiChildFlags.Border | ImGuiChildFlags.AlwaysAutoResize);
        DrawDeckListTable();
        ImGui.EndChild();
        ImGui.PushFont(Fonts.MonoSpace);
        modalPopup.Draw();
        ImGui.PopFont();


    }

    public void Free()
    {

    }

    public void LoadDeckLists()
    {
        deckLists.Clear();
        deckLists = Deck.LoadDeckListFromBytes(DataAccess.Instance.LoadDecks());
        currentDeck = deckLists[currentDeckListIndex];
        sortedDeckList = new List<DeckCard>(currentDeck.CardList);
    }

    void DrawDeckListTable()
    {
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text("No cards available.");
            return;
        }
        if (currentDeckListIndex != lastDeckListIndex)
        {
            currentDeck = deckLists[currentDeckListIndex];
            sortedDeckList = new List<DeckCard>(currentDeck.CardList);
            lastDeckListIndex = currentDeckListIndex;
        }

        ImGui.PushFont(fontToUse);
        if (ImGui.BeginCombo("Decks", $"{Deck.NamePrefix(currentDeckListIndex)} - {currentDeck.DeckLeader.Name}", ImGuiComboFlags.HeightLarge))
        {
            for (var index = 0; index < deckLists.Count; index++)
            {
                bool isSelected = deckLists[index] == currentDeck;
                if (ImGui.Selectable($"{Deck.NamePrefix(index)} - {deckLists[index].DeckLeader.Name}", isSelected))
                {
                    currentDeckListIndex = index;
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
                if (ImGui.IsItemHovered())
                {
                    if (index >= 27 && index <= 47)
                    {
                        GlobalImgui.RenderTooltipOpponentImage((EEnemyImages)index - 26);
                    }
                    GlobalImgui.RenderTooltipCardImage(deckLists[index].DeckLeader.Name);
                }
            }
            ImGui.EndCombo();
        }
        if (ImGui.BeginCombo("DeckLeader", currentDeck.DeckLeader.ToString(), ImGuiComboFlags.HeightLarge))
        {
            foreach (var monster in CardConstant.Monsters)
            {
                bool isSelected = currentDeck.DeckLeader.CardConstant.Index == monster.Index;
                if (ImGui.Selectable(monster.Name, isSelected))
                {
                    currentDeck.DeckLeader = new DeckCard(monster, currentDeck.DeckLeader.Rank);
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(monster.Name);
                }
            }
            ImGui.EndCombo();
        }

        if (ImGui.BeginCombo("Leader Rank", currentDeck.DeckLeader.Rank.ToString(), ImGuiComboFlags.HeightLarge))
        {
            foreach (var deckLeaderRank in Enum.GetNames(typeof(DeckLeaderRank)))
            {
                DeckLeaderRank leaderRank = Enum.Parse<DeckLeaderRank>(deckLeaderRank);
                bool isSelected = leaderRank == currentDeck.DeckLeader.Rank;
                if (ImGui.Selectable(deckLeaderRank, isSelected))
                {
                    Console.WriteLine($"{currentDeck.DeckLeader} is now rank {deckLeaderRank}");
                    currentDeck.DeckLeader.Rank = leaderRank;
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipRankImage(leaderRank);
                }
            }
            ImGui.EndCombo();
        }

        if (currentDeckListIndex >= 17 && currentDeckListIndex <= 47)
        {
            if (currentDeckListIndex < 27)
            {
                enemyImageIndex = 0;
            }
            else
            {
                enemyImageIndex = currentDeckListIndex - 26;

                int currentAiIndex = Enemies.EnemyList[currentDeckListIndex - 26].AiId;

                if (ImGui.Combo("Enemy AI", ref currentAiIndex, Ai.All.Select(x => x.Name).ToArray(), Ai.All.Count))
                {
                    Enemy enemy = Enemies.EnemyList[currentDeckListIndex - 26];
                    enemy.AiId = currentAiIndex;
                }
            }
            if (Enum.IsDefined(typeof(EEnemyImages), enemyImageIndex))
            {
                ImGui.Image(GlobalImages.Instance.Enemies[(EEnemyImages)enemyImageIndex], new Vector2(128, 128));
                ImGui.SameLine();
            }

        }
        ImGui.Image(GlobalImages.Instance.Cards[currentDeck.DeckLeader.Name], new Vector2(128, 128));

        if (sortedDeckList.Count == 40)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ImageHelper.ColorToVec4Normalised(Color.Green));
            ImGui.Text($"Cards: {sortedDeckList.Count} / 40");
            ImGui.PopStyleColor();
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ImageHelper.ColorToVec4Normalised(Color.Red));
            ImGui.Text($"Cards: {sortedDeckList.Count} / 40");
            ImGui.PopStyleColor();
        }
        ImGui.SameLine();

        ImGui.Text($"Total DC {sortedDeckList.Sum(deckCard => deckCard.CardConstant.DeckCost)}");

        if (ImGui.BeginTable("CurrentDeck", 9, tableFlags))
        {
            unsafe
            {
                float idWidth = ImGui.CalcTextSize("999").X;
                float nameWidth = ImGui.CalcTextSize("Winged Dragon, Guardian of the Fortress #1").X;
                float attackWidth = ImGui.CalcTextSize("9999").X;
                float levelWidth = ImGui.CalcTextSize("LVL").X;
                float attributeWidth = ImGui.CalcTextSize("Attribute").X;
                float typeWidth = ImGui.CalcTextSize("Trap (Full Ranged)").X;
                float actionWidth = ImGui.CalcTextSize("Remove").X;
                float dcWidth = ImGui.CalcTextSize("99").X;

                ImGuiTableColumnFlags columnFlags = ImGuiTableColumnFlags.None;
                ImGui.TableSetupColumn("ID", columnFlags | ImGuiTableColumnFlags.WidthFixed, idWidth + 10);
                ImGui.TableSetupColumn("Name", columnFlags | ImGuiTableColumnFlags.WidthStretch, nameWidth);
                ImGui.TableSetupColumn("ATK", columnFlags | ImGuiTableColumnFlags.WidthFixed, attackWidth + 20);
                ImGui.TableSetupColumn("DEF", columnFlags | ImGuiTableColumnFlags.WidthFixed, attackWidth + 20);
                ImGui.TableSetupColumn("LVL", columnFlags | ImGuiTableColumnFlags.WidthFixed, levelWidth + 20);
                ImGui.TableSetupColumn("Attribute", columnFlags | ImGuiTableColumnFlags.WidthFixed, attributeWidth + 10);
                ImGui.TableSetupColumn("Type", columnFlags | ImGuiTableColumnFlags.WidthFixed, typeWidth + 20);
                ImGui.TableSetupColumn("DC", columnFlags | ImGuiTableColumnFlags.WidthFixed, dcWidth + 20);
                ImGui.TableSetupColumn("Action", columnFlags | ImGuiTableColumnFlags.WidthFixed, actionWidth + 20);
                ImGui.TableHeadersRow();


                ImGuiTableSortSpecsPtr sortSpecifications = ImGui.TableGetSortSpecs();
                bool ascending = sortSpecifications.Specs.SortDirection == ImGuiSortDirection.Ascending;
                if (sortSpecifications.SpecsDirty)
                {
                    sortedDeckList.Sort((a, b) =>
                    {
                        CardConstant cardA = a.CardConstant;
                        CardConstant cardB = b.CardConstant;

                        switch (sortSpecifications.Specs.ColumnIndex)
                        {
                            case 0: return CompareWithFallback(cardA.Index, cardB.Index, cardA.Name, cardB.Name, ascending);
                            case 1: return CompareWithFallback(cardA.Name, cardB.Name, cardA.Index, cardB.Index, ascending);
                            case 2: return CompareWithFallback(cardA.Attack, cardB.Attack, cardA.Index, cardB.Index, ascending);
                            case 3: return CompareWithFallback(cardA.Defense, cardB.Defense, cardA.Index, cardB.Index, ascending);
                            case 4: return CompareWithFallback(cardA.Level, cardB.Level, cardA.Index, cardB.Index, ascending);
                            case 5: return CompareWithFallback( CardAttribute.GetAttributeVisual(cardA), CardAttribute.GetAttributeVisual(cardB), cardA.Index, cardB.Index, ascending);
                            case 6: return CompareWithFallback(cardA.Type, cardB.Type, cardA.Index, cardB.Index, ascending);
                            case 7: return CompareWithFallback(cardA.DeckCost, cardB.DeckCost, cardA.Index, cardB.Index, ascending);
                            default: return 0;
                        }
                    });
                    sortSpecifications.SpecsDirty = false;
                }

                for (var index = 0; index < sortedDeckList.Count; index++)
                {
                    CardConstant cardConstant = sortedDeckList[index].CardConstant;
                    var colour = CardConstantRowColor(cardConstant).value;
                    ImGui.TableNextRow();

                    uint rowColor = (uint)((int)(colour.W * 255) << 24 | (int)(colour.Z * 255) << 16 | (int)(colour.Y * 255) << 8 |
                                           (int)(colour.X * 255));

                    ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, rowColor);


                    ImGui.PushID(index);



                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(cardConstant.Index.ToString());

                    ImGui.TableSetColumnIndex(1);
                    ImGui.Text(cardConstant.Name);

                    ImGui.TableSetColumnIndex(2);
                    ImGui.Text(cardConstant.Attack.ToString());

                    ImGui.TableSetColumnIndex(3);
                    ImGui.Text(cardConstant.Defense.ToString());

                    ImGui.TableSetColumnIndex(4);
                    ImGui.Text(cardConstant.Level.ToString());

                    ImGui.TableSetColumnIndex(5);
                    ImGui.Text(cardConstant.AttributeName);

                    ImGui.TableSetColumnIndex(6);
                    ImGui.Text(cardConstant.Type);

                    ImGui.TableSetColumnIndex(7);
                    ImGui.Text(cardConstant.DeckCost.ToString());

                    ImGui.TableSetColumnIndex(8);
                    if (ImGui.Button("Remove"))
                    {
                        currentDeck.CardList.Remove(currentDeck.CardList.Find(card => card.Name == cardConstant.Name));
                        sortedDeckList.RemoveAt(index);
                        Console.WriteLine($"Removing {cardConstant.Name} from deck");
                    }

                    ImGui.TableSetColumnIndex(0);
                    ImGui.PopID();
                    if (ImGui.Selectable($"##index", false, ImGuiSelectableFlags.SpanAllColumns))
                    {
                    }
                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(cardConstant.Name);
                    }
                }
                ImGui.EndTable();
            }
        }
        ImGui.PopFont();
    }

    void DrawTrunkTable()
    {

        ImGui.PushFont(fontToUse);
        if (sortedTrunkList.Count == 0 || sortedTrunkList.Count != CardConstant.List.Count)
        {
            sortedTrunkList = new List<CardConstant>(CardConstant.List);
        }
        ImGui.TextColored(new GuiColour(System.Drawing.Color.SkyBlue).value,
            "Instructions: Left click = select card, \nCtrl + Left Click = add to selection or remove a card already selected\nShift + left click to add everything between your last clicked card and the this card, Ctrl + Right click to clear all\nShift + Right click = view hovered card in editor");
        ImGui.Text("Search Bar");
        ImGui.SameLine();
        ImGui.InputText("##SearchBar", ref trunkSearchFilter, 32);
        ImGui.SameLine();
        if (ImGui.RadioButton("Use colour", UserSettings.deckEditorUseColours))
        {
            UserSettings.deckEditorUseColours = !UserSettings.deckEditorUseColours;
        }
        ImGui.SameLine();
        if (ImGui.ColorEdit4("Highlight colour", ref highlightColour, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs))
        {
            UserSettings.DeckEditorHighlightcolour = highlightColour;
        }
        if (ImGui.BeginTable("Trunk", 9, tableFlags))
        {
            float idWidth = ImGui.CalcTextSize("999").X;
            float nameWidth = ImGui.CalcTextSize("Winged Dragon, Guardian of the Fortress #1").X;
            float attackWidth = ImGui.CalcTextSize("Defense").X;
            float levelWidth = ImGui.CalcTextSize("Level").X;
            float attributeWidth = ImGui.CalcTextSize("Attribute").X;
            float typeWidth = ImGui.CalcTextSize("Trap (Full Ranged)").X;
            float actionWidth = ImGui.CalcTextSize("Remove").X;
            float dcWidth = ImGui.CalcTextSize("99").X;

            ImGuiTableColumnFlags columnFlags = ImGuiTableColumnFlags.None;
            ImGui.TableSetupColumn("ID", columnFlags | ImGuiTableColumnFlags.WidthFixed, idWidth + 10);
            ImGui.TableSetupColumn("Name", columnFlags | ImGuiTableColumnFlags.WidthStretch, nameWidth + 10);
            ImGui.TableSetupColumn("ATK", columnFlags | ImGuiTableColumnFlags.WidthFixed, attackWidth);
            ImGui.TableSetupColumn("DEF", columnFlags | ImGuiTableColumnFlags.WidthFixed, attackWidth);
            ImGui.TableSetupColumn("LVL", columnFlags | ImGuiTableColumnFlags.WidthFixed, levelWidth + 10);
            ImGui.TableSetupColumn("Attribute", columnFlags | ImGuiTableColumnFlags.WidthFixed, attributeWidth + 20);
            ImGui.TableSetupColumn("Type", columnFlags | ImGuiTableColumnFlags.WidthFixed, typeWidth + 20);
            ImGui.TableSetupColumn("DC", columnFlags | ImGuiTableColumnFlags.WidthFixed, dcWidth + 20);
            ImGui.TableSetupColumn("Action", columnFlags | ImGuiTableColumnFlags.WidthFixed, actionWidth);
            ImGui.TableHeadersRow();

            ImGuiTableSortSpecsPtr sortSpecifications = ImGui.TableGetSortSpecs();
            bool ascending = sortSpecifications.Specs.SortDirection == ImGuiSortDirection.Ascending;
            if (sortSpecifications.SpecsDirty)
            {
                sortedTrunkList.Sort((cardA, cardB) =>
                {
                    
                              switch (sortSpecifications.Specs.ColumnIndex)
                        {
                            case 0: return CompareWithFallback(cardA.Index, cardB.Index, cardA.Name, cardB.Name, ascending);
                            case 1: return CompareWithFallback(cardA.Name, cardB.Name, cardA.Index, cardB.Index, ascending);
                            case 2: return CompareWithFallback(cardA.Attack, cardB.Attack, cardA.Index, cardB.Index, ascending);
                            case 3: return CompareWithFallback(cardA.Defense, cardB.Defense, cardA.Index, cardB.Index, ascending);
                            case 4: return CompareWithFallback(cardA.Level, cardB.Level, cardA.Index, cardB.Index, ascending);
                            case 5: return CompareWithFallback( CardAttribute.GetAttributeVisual(cardA), CardAttribute.GetAttributeVisual(cardB), cardA.Index, cardB.Index, ascending);
                            case 6: return CompareWithFallback(cardA.Type, cardB.Type, cardA.Index, cardB.Index, ascending);
                            case 7: return CompareWithFallback(cardA.DeckCost, cardB.DeckCost, cardA.Index, cardB.Index, ascending);
                            default: return 0;
                        }
                });
                sortSpecifications.SpecsDirty = false;
            }

            List<CardConstant> filteredList =
                sortedTrunkList.Where(card => card.Name.Contains(trunkSearchFilter, StringComparison.OrdinalIgnoreCase)).ToList();

            ImGui.PushStyleColor(ImGuiCol.Header, highlightColour);

            for (var index = 0; index < filteredList.Count; index++)
            {
                CardConstant cardConstant = filteredList[index];
                var colour = CardConstantRowColor(cardConstant).value;
                if (cardConstant.Index == 671)
                {
                    continue;
                }
                ImGui.TableNextRow();


                uint rowColor = (uint)((int)(colour.W * 255) << 24 | (int)(colour.Z * 255) << 16 | (int)(colour.Y * 255) << 8 |
                                       (int)(colour.X * 255));

                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, rowColor);



                ImGui.PushID(cardConstant.Index.ToString());


                bool item_is_selected = trunkSelection.Contains(cardConstant.Index);
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(cardConstant.Index.ToString());

                ImGui.TableSetColumnIndex(1);
                ImGui.Text(cardConstant.Name);

                ImGui.TableSetColumnIndex(2);
                ImGui.Text(cardConstant.Attack.ToString());

                ImGui.TableSetColumnIndex(3);
                ImGui.Text(cardConstant.Defense.ToString());

                ImGui.TableSetColumnIndex(4);
                ImGui.Text(cardConstant.Level.ToString());

                ImGui.TableSetColumnIndex(5);
                ImGui.Text(cardConstant.AttributeName);

                ImGui.TableSetColumnIndex(6);
                ImGui.Text(cardConstant.Type);

                ImGui.TableSetColumnIndex(7);
                ImGui.Text(cardConstant.DeckCost.ToString());

                ImGui.TableSetColumnIndex(8);
                if (ImGui.Button("Add", new Vector2(ImGui.GetContentRegionAvail().X, 0)))
                {
                    if (sortedDeckList.Count(constant => constant.CardConstant.Index == filteredList[index].Index) < 3)
                    {
                        currentDeck.CardList.Add(new DeckCard(filteredList[index], DeckLeaderRank.NCO));
                        sortedDeckList.Add(new DeckCard(filteredList[index], DeckLeaderRank.NCO));
                        Console.WriteLine($"Adding {filteredList[index].Name} to deck");

                    }
                    else
                    {
                        Console.WriteLine($"You already have 3 copies {filteredList[index].Name} in the deck (Single)");
                    }

                    foreach (var i in trunkSelection)
                    {
                        if (i != cardConstant.Index)
                        {
                            CardConstant newCardConst = CardConstant.List[i];
                            if (sortedDeckList.Count(card => card.CardConstant.Index == i) < 3)
                            {
                                sortedDeckList.Add(new DeckCard(newCardConst, DeckLeaderRank.NCO));
                                currentDeck.CardList.Add(new DeckCard(newCardConst, DeckLeaderRank.NCO));
                                Console.WriteLine($"Adding {newCardConst.Name} to deck");
                            }
                            else
                            {
                                Console.WriteLine($"You already have 3 copies {newCardConst.Name} in the deck (Loop)");
                            }
                        }
                    }

                }
                ImGui.TableSetColumnIndex(0);

                if (ImGui.GetIO().KeyCtrl && ImGui.GetIO().MouseClicked[1])
                {
                    trunkSelection.Clear();
                }

                if (ImGui.Selectable("##Selectable", item_is_selected, ImGuiSelectableFlags.SpanAllColumns, new Vector2(0, 0)))
                {
                    if (ImGui.GetIO().KeyCtrl)
                    {
                        if (!trunkSelection.Add(cardConstant.Index))
                        {
                            trunkSelection.Remove(cardConstant.Index);
                        }

                    }
                    else if (!item_is_selected && ImGui.GetIO().KeyShift)
                    {
                        if (trunkSelection.Count > 0)
                        {
                            int originalIndex = filteredList.FindIndex(i => i.Index == trunkSelection.Last());
                            int newIndex = filteredList.FindIndex(i => i.Index == cardConstant.Index);

                            if (originalIndex != -1 && newIndex != -1)
                            {
                                int minIndex = Math.Min(originalIndex, newIndex);
                                int maxIndex = Math.Max(originalIndex, newIndex);

                                for (int i = minIndex; i <= maxIndex; i++)
                                {
                                    trunkSelection.Add(filteredList[i].Index);
                                }
                            }
                        }
                    }
                    else
                    {
                        trunkSelection.Clear();
                        trunkSelection.Add(cardConstant.Index);
                    }


                }
                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(cardConstant.Name);
                    if (ImGui.GetIO().KeyShift && ImGui.GetIO().MouseClicked[1])
                    {
                        ViewCardInEditor?.Invoke(cardConstant.Name);
                    }

                }
                ImGui.PopID();

            }

            ImGui.PopStyleColor();

            ImGui.EndTable();
        }
        ImGui.PopFont();

    }

    GuiColour CardConstantRowColor(CardConstant cardConstant)
    {
        if (UserSettings.deckEditorUseColours)
        {
            switch (cardConstant.CardColor)
            {
                case CardColourType.NormalMonster:
                    return new GuiColour(160, 128, 0);
                case CardColourType.EffectMonster:
                    return new GuiColour(160, 80, 0);
                case CardColourType.Ritual:
                    return new GuiColour(81, 102, 141);
                case CardColourType.Trap:
                    return new GuiColour(160, 16, 64);
                case CardColourType.Magic:
                    return new GuiColour(0, 96, 48);
                default:
                    return new GuiColour(160, 128, 0);
            }
        }

        return new GuiColour(ImGui.GetStyle().Colors[(int)ImGuiCol.TableRowBg]);
    }


    public void SaveAllDecks()
    {
        failedToSaveDecks.Clear();
        for (var index = 0; index < deckLists.Count; index++)
        {
            Deck deck = deckLists[index];
            if (deck.CardList.Count == 40)
            {
                DataAccess.Instance.SaveDeck(index, deck.Bytes);
            }
            else
            {
                failedToSaveDecks.Add($"Deck: {Deck.NamePrefix(index)} - {deck.DeckLeader.Name}");
            }
        }
        LoadDeckLists();
        UpdateStartingDeck.CreateNewStartingDeckData(deckLists);
        if (failedToSaveDecks.Count > 0)
        {
            deckErrorText.Clear();
            deckErrorText.AppendLine("Failed to add:");
            foreach (var deckError in failedToSaveDecks)
            {
                deckErrorText.AppendLine("  " + deckError);
            }
            deckErrorText.AppendLine("Decks must have exactly 40 cards");
            deckErrorText.AppendLine("Your other changes have been saved!");
            modalPopup.Show(deckErrorText.ToString());
        }

    }

    public static int CompareWithFallback<TPrimary, TSecondary>(TPrimary primaryA, TPrimary primaryB, TSecondary secondaryA, TSecondary secondaryB,
        bool ascending) where TPrimary : IComparable<TPrimary>
        where TSecondary : IComparable<TSecondary>
    {
        int result = primaryA.CompareTo(primaryB);
        if (result == 0) // If primary comparison is equal, use secondary
        {
            return ascending ? secondaryA.CompareTo(secondaryB) : secondaryB.CompareTo(secondaryA);
        }
        return ascending ? result : -result;
    }
}