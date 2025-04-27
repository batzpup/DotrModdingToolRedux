using System.Drawing;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class RandomiserWindow : IImGuiWindow
{
    ImFontPtr monoSpaceFont = Fonts.MonoSpace;

    bool shouldRandomise;
    bool randomiseStartingDecks;
    bool randomiseOpponentDecks;
    bool randomiseCardAcquisition;
    bool randomiseMaps;
    bool randomiseLeaderRanks;
    bool randomiseLeaderAbilities;
    bool randomiseCardATKDEF;
    bool balancedRandom;
    bool lowPowerLevelRandom;
    bool randomiseMonsterEffects;
    bool randomiseMagicEffects;
    bool randomisePowerUpValues;
    bool randomiseMapsTiles;
    bool randomiseOpponentLeaderRanks;
    bool randomiseSummoningPower;
    bool randomiseAttributes;
    bool randomiseKinds;
    bool randomiseRareDrop;
    bool randomiseReincarnation;
    bool randomiseSlots;
    bool balancedDeckCorrectAmount = true;
    bool randomMagicUseMonsterEff;
    bool randomMonsterFlipUseMagic;
    bool swapTerrainTypesRandomTiles;
    bool swapTerrainTypesTrueRandom;
    bool banSpecificCards;

    int monsterCount = 25;
    int spellCount = 5;
    int trapCount = 5;
    int equipCount = 3;
    int ritualCount = 2;
    int randomEffectChange = 5;
    int maxRandomRank = 12;
    int maxLeaderAbilities = 10;
    int leaderAbilityChance = 5;
    int maxEquipDelta;
    int maxStartingDc;
    int maxStartingSP;
    int maxStartingDef;
    int maxStartingAtk;
    int maxMonsterDefenseDelta;
    int maxMonsterAttackDelta;
    int maxSummoningDelta;

    HashSet<int> bannedCards = new HashSet<int>();
    List<string> bannedCardFilteredList = new List<string>();
    HashSet<string> selectedCards = new HashSet<string>();
    int currentBannedCardIndex = 0;
    string cardSearch = "";
    int? bannedCardToRemove = null;
    string SearchSortField = "ID";
    bool SearchAscending = true;

    public RandomiserWindow()
    {
        EditorWindow.OnIsoLoaded += FilterAndSort;
    }

    public void Render()
    {
        ImGui.PushFont(monoSpaceFont);
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text($"Please load ISO file");
            ImGui.PopFont();
            return;
        }

        ImGui.TextColored(new GuiColour(Color.Red).value,
            @"Using the randomiser makes all AI's DMK as this is the most vertsatile and least likely to brick
Secondly Deck Cost will be meaningless when randomiser, this will make all battle avaiable regardless of DC
Not recommended to edit card effects as it will not update the text, but im not the law");
        ImGui.Checkbox("Enable Randomiser", ref shouldRandomise);
        ImGui.Separator();

        ImGui.Checkbox("Randomise Starting Decks", ref randomiseStartingDecks);


        ImGui.Checkbox("Randomise Enemy Decks", ref randomiseOpponentDecks);


        ImGui.Checkbox("Randomise Card Acquisition", ref randomiseCardAcquisition);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises how cards can be acquired");

        if (randomiseCardAcquisition)
        {
            ImGui.Indent();
            if (ImGui.Checkbox("Randomise Slots", ref randomiseSlots))
            {
            }

            if (ImGui.Checkbox("Randomise Rare Drop", ref randomiseRareDrop))
            {
            }

            if (ImGui.Checkbox("Randomise Reincarnation", ref randomiseReincarnation))
            {
            }
            ImGui.Unindent();

        }
        ImGui.Checkbox("Randomise Maps", ref randomiseMaps);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises which map is assigned to which duelist");

        ImGui.Checkbox("Randomise Map Tiles", ref randomiseMapsTiles);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises tiles on every existing map");
        if (randomiseMapsTiles)
        {
            ImGui.Indent();
            if (ImGui.Checkbox("Swap Terrain Types", ref swapTerrainTypesRandomTiles))
            {
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("All tiles of a certain type are replaced with another type");


            if (ImGui.Checkbox("True random", ref swapTerrainTypesTrueRandom))
            {
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("All tiles are completely random");
            ImGui.Unindent();
        }
        ImGui.Checkbox("Enemy Leader Ranks", ref randomiseOpponentLeaderRanks);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises the leader ranks of your opponents");

        ImGui.Checkbox("Leader Starting Leader Ranks", ref randomiseLeaderRanks);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises the leader ranks of your starter options");
        if (randomiseLeaderRanks)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Max Possible Rank", ref maxRandomRank, 0, 12))
            {
            }
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise Leader Abilities", ref randomiseLeaderAbilities);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises the leader abilities a card can have");
        if (randomiseLeaderRanks)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Max number of abilities ", ref maxLeaderAbilities, 0, 10))
            {
            }

            if (ImGui.SliderInt("Leader Ability % Chance", ref leaderAbilityChance, 0, 100))
            {
            }
            ImGui.Unindent();
        }
        ImGui.Checkbox("Randomise Card ATK-DEF", ref randomiseCardATKDEF);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomise creates attack and defense within a range, 0 = no limits");
        if (randomiseCardATKDEF)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Max Attack Difference ", ref maxMonsterAttackDelta, 0, 9999))
            {
            }

            if (ImGui.SliderInt("Max Defence Difference", ref maxMonsterDefenseDelta, 0, 9999))
            {
            }
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise Card Summoning Power", ref randomiseSummoningPower);
        if (randomiseSummoningPower)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Max Summoning Power Difference", ref maxSummoningDelta, 0, 12))
            {
            }
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise Card Attribute", ref randomiseAttributes);

        ImGui.Checkbox("Randomise Card Kind", ref randomiseKinds);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises monsters to other monster kinds");

        ImGui.Checkbox("Balanced Deck", ref balancedRandom);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("you will always have X monsters X equips X spells X traps and X rituals\n Must add up to 40 cards");
        if (balancedRandom)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Monsters", ref monsterCount, 0, 40))
            {
            }
            if (ImGui.SliderInt("Spells", ref spellCount, 0, 40))
            {
            }
            if (ImGui.SliderInt("Traps", ref trapCount, 0, 40))
            {
            }
            if (ImGui.SliderInt("PowerUps", ref equipCount, 0, 12))
            {
            }
            if (ImGui.SliderInt("Rituals", ref ritualCount, 0, 12))
            {
            }
            int totalCount = monsterCount + spellCount + trapCount + equipCount + ritualCount;
            if (totalCount != 40)
            {
                balancedDeckCorrectAmount = false;
            }
            else
            {
                balancedDeckCorrectAmount = true;
            }
            ImGui.Unindent();
        }

        ImGui.Checkbox("No OP creatures in starter decks", ref lowPowerLevelRandom);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Starter decks can no start with cards with a variety of options");
        if (lowPowerLevelRandom)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Max Attack", ref maxStartingAtk, 0, 9999))
            {
            }
            if (ImGui.SliderInt("Max Defense", ref maxStartingDef, 0, 9999))
            {
            }

            if (ImGui.SliderInt("Max SP", ref maxStartingSP, 0, 12))
            {
            }

            if (ImGui.SliderInt("Max DC", ref maxStartingDc, 0, 99))
            {
            }
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise Monster Effects", ref randomiseMonsterEffects);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(@"Attack|Movement|Flip|Nature|Destruction 
Will give monsters a 1/X chance in each effect slot to get a random monster effect. 
Flip Effect may get spells effects");
        if (randomiseMonsterEffects)
        {
            ImGui.Indent();
            if (ImGui.Checkbox("Allow magic effects for monster flip effects", ref randomMonsterFlipUseMagic)) ;
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise magic effects", ref randomiseMagicEffects);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises the effect of a spell with another spell or monster flip effect");
        if (randomiseMagicEffects)
        {
            ImGui.Indent();
            if (ImGui.Checkbox("Allow monster flip effects", ref randomMagicUseMonsterEff)) ;
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise power-up values", ref randomisePowerUpValues);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Gives power increase power ups a random value within a range, 0 = unlimited range");
        if (randomisePowerUpValues)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Max equip difference", ref maxEquipDelta, 0, 9999))
            {
            }
            ImGui.Unindent();

        }

        ImGui.Checkbox("Ban specific cards", ref banSpecificCards);

        if (banSpecificCards)
        {
            ImGui.Indent();
            RenderCardSearch();
            ImGui.Text("Banned Cards");
            ImGui.SetNextItemWidth(ImGui.CalcTextSize("Winged Dragon, Guardian of the Fortress #1 Remove").X);
            if (ImGui.BeginListBox("##BannedCards"))
            {
                foreach (var cardID in bannedCards)
                {
                    ImGui.PushID(cardID);
                    ImGui.Text(Card.GetNameByIndex(cardID));
                    ImGui.SameLine();
                    if (ImGui.Button("X"))
                    {
                        bannedCardToRemove = cardID;
                    }
                    ImGui.PopID();
                }
                ImGui.EndListBox();
            }
            if (bannedCardToRemove.HasValue)
            {
                bannedCards.Remove(bannedCardToRemove.Value);
            }
            bannedCardToRemove = null;
            ImGui.Unindent();
        }
        ImGui.PopFont();
    }

    void RenderCardSearch()
    {
        ImGui.Text("Sort by");
        if (ImGui.Button("ID"))
        {
            if (SearchSortField == "ID")
            {
                SearchAscending = !SearchAscending;
            }
            else
            {
                SearchSortField = "ID";
                SearchAscending = true;
            }
            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Button("Name"))
        {
            if (SearchSortField == "Name")
            {
                SearchAscending = !SearchAscending;
            }
            else
            {
                SearchSortField = "Name";
                SearchAscending = true;
            }
            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Button("ATK"))
        {
            if (SearchSortField == "ATK")
            {
                SearchAscending = !SearchAscending;
            }
            else
            {
                SearchSortField = "ATK";
                SearchAscending = true;
            }
            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Button("DEF"))
        {
            if (SearchSortField == "DEF")
            {
                SearchAscending = !SearchAscending;
            }
            else
            {
                SearchSortField = "DEF";
                SearchAscending = true;
            }

            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Button("Level"))
        {
            if (SearchSortField == "Level")
            {
                SearchAscending = !SearchAscending;
            }
            else
            {
                SearchSortField = "Level";
                SearchAscending = true;
            }

            FilterAndSort();
        }
        ImGui.SameLine();

        ImGui.SameLine();

        if (ImGui.Button("Attribute"))
        {
            if (SearchSortField == "Attribute")
            {
                SearchAscending = !SearchAscending;
            }
            else
            {
                SearchSortField = "Attribute";
                SearchAscending = true;
            }

            FilterAndSort();
        }

        ImGui.SameLine();
        if (ImGui.Button("Kind"))
        {
            if (SearchSortField == "Kind")
            {
                SearchAscending = !SearchAscending;
            }
            else
            {
                SearchSortField = "Kind";
                SearchAscending = true;

            }
            FilterAndSort();
        }

        ImGui.SameLine();
        if (ImGui.Button("DC"))
        {
            if (SearchSortField == "DC")
            {
                SearchAscending = !SearchAscending;
            }
            else
            {
                SearchSortField = "DC";
                SearchAscending = true;
            }

            FilterAndSort();
        }
        ImGui.SameLine();
        if (ImGui.Checkbox("Ascending", ref SearchAscending))
        {
            FilterAndSort();
        }
        ImGui.SetNextItemWidth(ImGui.CalcTextSize("Winged Dragon, Guardian of the Fortress #1 Remove").X);
        if (ImGui.BeginListBox("##Cards"))
        {
            ImGui.Text("Card Search");
            if (ImGui.InputText("##CardSearch", ref cardSearch, 32))
            {
                FilterAndSort();
            }

            foreach (string filteredName in bannedCardFilteredList)
            {
                ImGui.PushID(filteredName);
                bool isSelected = selectedCards.Contains(filteredName);

                ImGui.SetNextItemWidth(ImGui.CalcTextSize("Winged Dragon, Guardian of the Fortress #1").X);
                if (ImGui.Selectable($"{filteredName}", isSelected, ImGuiSelectableFlags.None,
                        ImGui.CalcTextSize("Winged Dragon, Guardian of the Fortress #1")))
                {
                    if (ImGui.GetIO().KeyShift)
                    {
                        int startIndex = bannedCardFilteredList.IndexOf(Card.cardNameList[currentBannedCardIndex]);
                        int endIndex = bannedCardFilteredList.IndexOf(filteredName);
                        if (startIndex != -1 && endIndex != -1)
                        {
                            for (int i = Math.Min(startIndex, endIndex); i <= Math.Max(startIndex, endIndex); i++)
                            {
                                selectedCards.Add(bannedCardFilteredList[i]);
                            }
                            currentBannedCardIndex = Array.IndexOf(Card.cardNameList, filteredName);
                        }
                    }
                    else if (ImGui.GetIO().KeyCtrl)
                    {
                        if (selectedCards.Add(filteredName))
                        {
                            currentBannedCardIndex = Array.IndexOf(Card.cardNameList, filteredName);
                        }
                        else
                        {
                            selectedCards.Remove(filteredName);
                            currentBannedCardIndex = Array.IndexOf(Card.cardNameList, selectedCards.Last());
                        }
                    }
                    else
                    {
                        selectedCards.Clear();
                        selectedCards.Add(filteredName);
                        currentBannedCardIndex = Array.IndexOf(Card.cardNameList, filteredName);
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(filteredName);
                }

                ImGui.SameLine();
                if (ImGui.Button($"Add"))
                {
                    if (selectedCards.Contains(filteredName))
                    {
                        foreach (var card in selectedCards)
                        {
                            bannedCards.Add(Array.IndexOf(Card.cardNameList, card));
                        }
                    }
                    else
                    {
                        bannedCards.Add(Array.IndexOf(Card.cardNameList, filteredName));
                    }


                }
                ImGui.PopID();
            }
            ImGui.EndListBox();
        }
    }


    void FilterAndSort()
    {
        bannedCardFilteredList = Card.cardNameList
            .Where(cardName => cardName.Contains(cardSearch, StringComparison.OrdinalIgnoreCase))
            .ToList();

        bannedCardFilteredList.Sort((a, b) =>
        {
            if (!CardConstant.CardLookup.TryGetValue(a, out var cardA) || !CardConstant.CardLookup.TryGetValue(b, out var cardB))
                return 0;

            int result = 0;
            switch (SearchSortField)
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
            return SearchAscending ? result : -result;
        });
    }

    public void Free()
    {
        EditorWindow.OnIsoLoaded -= FilterAndSort;
    }
}