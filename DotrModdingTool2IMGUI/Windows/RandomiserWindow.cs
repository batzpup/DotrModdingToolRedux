using System.Drawing;
using System.Security.Cryptography;
using GameplayPatches;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class RandomiserWindow : IImGuiWindow
{
    ImFontPtr monoSpaceFont = Fonts.MonoSpace;

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
    bool randomiseDecks;
    bool swapTiles = true;
    bool hasRandomised = false;
    bool banSpecificCards;

    int maxCrushTiles = 16;
    int maxLabTiles = 16;

    int monsterCount = 25;
    int spellCount = 5;
    int trapCount = 5;
    int equipCount = 3;
    int ritualCount = 2;
    int randomEffectChance = 15;
    int maxRandomRank = 12;
    int maxLeaderAbilities = 16;
    int leaderAbilityChance = 15;
    int bannedDcLimit = 99;
    int bannedLevelLimit = 12;
    int bannedDefLimit = 8000;
    int bannedAtkLimit = 8000;

    //Default delta Random Values
    bool randomSPRange;
    bool randomAtkDefRange;
    int powerUpDelta = 1000;
    int defenseDelta = 1000;
    int attackDelta = 1000;
    int levelDelta = 3;

    //Cap style random
    bool randomSPCap;
    bool randomAtkDefCap;
    int levelCap = 12;
    int attackCap = 8000;
    int defenseCap = 8000;


    HashSet<int> bannedCards = new HashSet<int>();
    List<string> bannedCardFilteredList = new List<string>();
    HashSet<string> selectedCards = new HashSet<string>();
    int currentBannedCardIndex = 0;
    string cardSearch = "";
    int? bannedCardToRemove = null;
    string SearchSortField = "ID";
    bool SearchAscending = true;
    EnemyEditorWindow _enemyEditorWindow;

    Dictionary<int, DeckLeaderRank> leaderRanksOriginal = new Dictionary<int, DeckLeaderRank>();

    public RandomiserWindow(EnemyEditorWindow enemyEditorWindow)
    {
        EditorWindow.OnIsoLoaded += FilterAndSort;
        EditorWindow.OnIsoLoaded += LoadLeaderRanks;
        _enemyEditorWindow = enemyEditorWindow;

    }

    void LoadLeaderRanks()
    {
        for (var index = 0; index < Deck.DeckList.Count; index++)
        {
            leaderRanksOriginal[index] = Deck.DeckList[index].DeckLeader.Rank;
        }
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
        if (ImGui.Button("Randomise"))
        {

            
            RandomiseCardConstData();
            RandomiseLeaderData();
            RandomiseDecks();
            RandomiseLeaderRanks();
            RandomiseMaps();
            ChangeAllAi();
            _enemyEditorWindow.DeckEditorWindow.UpdateDeckData();
            hasRandomised = true;


        }
        ImGui.Separator();

        ImGui.Checkbox("Randomise Decks", ref randomiseDecks);
        if (randomiseDecks)
        {
            ImGui.Indent();
            ImGui.Checkbox("Randomise Starting Decks", ref randomiseStartingDecks);
            ImGui.Checkbox("Randomise Enemy Decks", ref randomiseOpponentDecks);
            ImGui.Checkbox("Balanced Deck", ref balancedRandom);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("you will always have X monsters X equips X spells X traps and X rituals\n Must add up to 40 cards");
            if (balancedRandom)
            {
                if (!balancedDeckCorrectAmount)
                {
                    ImGui.TextColored(new GuiColour(Color.Red).value, "Error not 40 cards in deck");
                }
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
                if (ImGui.InputInt("Max Attack", ref bannedAtkLimit, 100, 1000))
                {
                    bannedAtkLimit = Math.Clamp(bannedAtkLimit, 0, 9999);
                }
                if (ImGui.InputInt("Max Defense", ref bannedDefLimit, 100, 1000))
                {
                    bannedDefLimit = Math.Clamp(bannedDefLimit, 0, 9999);
                }
                if (ImGui.SliderInt("Max SP", ref bannedLevelLimit, 0, 12))
                {
                }

                if (ImGui.SliderInt("Max DC", ref bannedDcLimit, 0, 99))
                {
                }
                ImGui.Unindent();
            }
            ImGui.Unindent();
        }

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
            if (ImGui.SliderInt("Max Crush tiles", ref maxCrushTiles, 0, 49))
            {
            }
            if (ImGui.SliderInt("Max Labyrinth tiles", ref maxLabTiles, 0, 49))
            {
            }
            if (ImGui.Checkbox("Swap Terrain Types", ref swapTiles))
            {
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("If enabled all tiles of a certain type are replaced with another type rather than being true random");
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
        if (randomiseLeaderAbilities)
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
            ImGui.Text("You can only pick one");
            if (ImGui.Checkbox("Randomise ATK-DEF in Range", ref randomAtkDefRange))
            {
                randomAtkDefCap = false;
            }
            if (randomAtkDefRange)
            {
                ImGui.Indent();
                if (ImGui.InputInt("Max Attack Range ", ref attackDelta, 100, 1000))
                {
                    attackDelta = Math.Clamp(attackDelta, 0, 9999);
                }
                if (ImGui.InputInt("Max Defence Range", ref defenseDelta, 100, 1000))
                {
                    defenseDelta = Math.Clamp(defenseDelta, 0, 9999);
                }
                ImGui.Unindent();
            }
            if (ImGui.Checkbox("Randomise ATK-DEF with Cap", ref randomAtkDefCap))
            {
                randomAtkDefRange = false;
            }
            else if (randomAtkDefCap)
            {
                ImGui.Indent();
                if (ImGui.InputInt("Max Attack Cap ", ref attackCap, 50, 100))
                {
                    attackCap = Math.Clamp(attackCap, 0, 8000);
                }
                if (ImGui.InputInt("Max Defence Cap", ref defenseCap, 50, 100))
                {
                    defenseCap = Math.Clamp(defenseCap, 0, 8000);
                }
                ImGui.Unindent();
            }
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise Card Summoning Power", ref randomiseSummoningPower);
        if (randomiseSummoningPower)
        {

            ImGui.Indent();
            if (ImGui.Checkbox("Randomise SP in range", ref randomSPRange))
            {
                randomSPCap = false;
            }
            if (randomSPRange)
            {
                ImGui.Indent();
                if (ImGui.SliderInt("Max Summoning Power Difference", ref levelDelta, 0, 12))
                {
                }
                ImGui.Unindent();
            }
            if (ImGui.Checkbox("Randomise SP with cap", ref randomSPCap))
            {
                randomSPRange = false;
            }
            else if (randomSPCap)
            {
                ImGui.Indent();
                if (ImGui.SliderInt("Max Summoning Power Difference", ref levelCap, 0, 12))
                {
                }
                ImGui.Unindent();
            }

            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise Card Attribute", ref randomiseAttributes);
        ImGui.Checkbox("Randomise Card Kind", ref randomiseKinds);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises monsters to other monster kinds");

        ImGui.Checkbox("Randomise power-up values", ref randomisePowerUpValues);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Gives power increase power ups a random value within a range, 0 = unlimited range");
        if (randomisePowerUpValues)
        {
            ImGui.Indent();
            if (ImGui.InputInt("Max equip difference", ref powerUpDelta, 100, 1000))
            {
                powerUpDelta = Math.Clamp(powerUpDelta, 0, 9999);
            }
            ImGui.Unindent();

        }

        ImGui.Checkbox("Ban specific cards", ref banSpecificCards);

        if (banSpecificCards)
        {
            ImGui.Indent();
            RenderCardSearch();
            ImGui.Text("Banned Cards");
            ImGui.SameLine();
            if (ImGui.Button("Clear"))
            {
                bannedCards.Clear();
            }
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
        ImGui.Separator();
        ImGui.TextColored(new GuiColour(Color.Red).value, "Not recommended");
        ImGui.Separator();
        ImGui.Checkbox("Randomise Monster Effects", ref randomiseMonsterEffects);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(@"Will give monsters a X/100 chance top be assigned a random monster effect. ");
        if (randomiseMonsterEffects)
        {
            ImGui.SliderInt("Effect Chance", ref randomEffectChance, 1, 100);
        }

        ImGui.Checkbox("Randomise Magic Effects", ref randomiseMagicEffects);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises the effect of a spell with another spell or monster flip effect");

        ImGui.PopFont();
    }


    void ChangeAllAi()
    {
        foreach (var enemy in Enemies.EnemyList)
        {
            enemy.AiId = Enemies.EnemyList[22].AiId;
        }
    }

    void RandomiseLeaderData()
    {
        if (!randomiseLeaderAbilities)
            return;

        var abilityValidations = new Dictionary<DeckLeaderAbilityType, Func<StartingDeckData.StarterDeckDataEnums.Kind, bool>> {
            { DeckLeaderAbilityType.TerrainChange, ValidForTerrainChange },
            { DeckLeaderAbilityType.LevelCostReduction, ValidForCostReduction },
            { DeckLeaderAbilityType.SpellbindSpecificEnemyType, ValidForSpellbindEnemy },
            { DeckLeaderAbilityType.WeakenSpecificEnemyType, ValidForWeakenEnemy },
            { DeckLeaderAbilityType.DestroySpecificEnemyType, ValidForDestroyEnemy },

        };

        foreach (var deckLeaderAbilityInstance in CardDeckLeaderAbilities.MonsterAbilities)
        {
            int numberOfPossibleAbilities = RandomNumberGenerator.GetInt32(3, maxLeaderAbilities + 1);
            int attemptedAbilities = 0;
            StartingDeckData.StarterDeckDataEnums.Kind cardKind =
                (StartingDeckData.StarterDeckDataEnums.Kind)CardConstant.Monsters[deckLeaderAbilityInstance.CardId].CardKind.Id + 1;
            for (int i = 0; i < deckLeaderAbilityInstance.Abilities.Length; i++)
            {
                deckLeaderAbilityInstance.Abilities[i].SetEnabled(false);
            }

            for (int i = 0; i < deckLeaderAbilityInstance.Abilities.Length && attemptedAbilities < numberOfPossibleAbilities; i++)
            {
                DeckLeaderAbilityType currentAbility = (DeckLeaderAbilityType)i;
                bool isValid = true;
                if (abilityValidations.TryGetValue(currentAbility, out var validationFunc))
                {
                    isValid = validationFunc(cardKind);
                }

                if (isValid)
                {

                    attemptedAbilities++;
                    if (RandomBoolIn100(leaderAbilityChance))
                    {
                        deckLeaderAbilityInstance.Abilities[i].SetEnabled(true);
                        deckLeaderAbilityInstance.Abilities[i].RankRequired = RandomNumberGenerator.GetInt32(1, 13);
                    }
                }
            }
        }
    }

    bool ValidForCostReduction(StartingDeckData.StarterDeckDataEnums.Kind kind)
    {
        switch (kind)
        {
            case StartingDeckData.StarterDeckDataEnums.Kind.Dragon:
            case StartingDeckData.StarterDeckDataEnums.Kind.Beast_Warrior:
            case StartingDeckData.StarterDeckDataEnums.Kind.Winged_Beast:
            case StartingDeckData.StarterDeckDataEnums.Kind.Fish:
            case StartingDeckData.StarterDeckDataEnums.Kind.Sea_Serpent:
            case StartingDeckData.StarterDeckDataEnums.Kind.Machine:
            case StartingDeckData.StarterDeckDataEnums.Kind.Immortal:
                return true;
            default:
                return false;
        }
    }

    bool ValidForTerrainChange(StartingDeckData.StarterDeckDataEnums.Kind kind)
    {
        switch (kind)
        {

            case StartingDeckData.StarterDeckDataEnums.Kind.Spellcaster:
            case StartingDeckData.StarterDeckDataEnums.Kind.Fiend:
            case StartingDeckData.StarterDeckDataEnums.Kind.Insect:
            case StartingDeckData.StarterDeckDataEnums.Kind.Dinosaur:
            case StartingDeckData.StarterDeckDataEnums.Kind.Aqua:
            case StartingDeckData.StarterDeckDataEnums.Kind.Immortal:
                return true;
            default:
                return false;
        }
    }

    bool ValidForSpellbindEnemy(StartingDeckData.StarterDeckDataEnums.Kind kind)
    {
        switch (kind)
        {
            case StartingDeckData.StarterDeckDataEnums.Kind.Plant:
            case StartingDeckData.StarterDeckDataEnums.Kind.Pyro:
            case StartingDeckData.StarterDeckDataEnums.Kind.Reptile:
            case StartingDeckData.StarterDeckDataEnums.Kind.Fairy:
                return true;
            default:
                return false;
        }
    }

    bool ValidForWeakenEnemy(StartingDeckData.StarterDeckDataEnums.Kind kind)
    {
        switch (kind)
        {
            case StartingDeckData.StarterDeckDataEnums.Kind.Spellcaster:
            case StartingDeckData.StarterDeckDataEnums.Kind.Zombie:
            case StartingDeckData.StarterDeckDataEnums.Kind.Warrior:
            case StartingDeckData.StarterDeckDataEnums.Kind.Beast:
            case StartingDeckData.StarterDeckDataEnums.Kind.Winged_Beast:
            case StartingDeckData.StarterDeckDataEnums.Kind.Fiend:
            case StartingDeckData.StarterDeckDataEnums.Kind.Insect:
            case StartingDeckData.StarterDeckDataEnums.Kind.Reptile:
            case StartingDeckData.StarterDeckDataEnums.Kind.Fish:
            case StartingDeckData.StarterDeckDataEnums.Kind.Thunder:
            case StartingDeckData.StarterDeckDataEnums.Kind.Aqua:
            case StartingDeckData.StarterDeckDataEnums.Kind.Pyro:
                return true;
            default:
                return false;
        }
    }

    bool ValidForDestroyEnemy(StartingDeckData.StarterDeckDataEnums.Kind kind)
    {
        switch (kind)
        {
            case StartingDeckData.StarterDeckDataEnums.Kind.Fairy:
            case StartingDeckData.StarterDeckDataEnums.Kind.Reptile:
            case StartingDeckData.StarterDeckDataEnums.Kind.Pyro:
            case StartingDeckData.StarterDeckDataEnums.Kind.Rock:
            case StartingDeckData.StarterDeckDataEnums.Kind.Plant:
                return true;
            default:
                return false;
        }
    }

    bool RandomBoolIn100(int percentChanceTrue)
    {
        return RandomNumberGenerator.GetInt32(1, 101) <= percentChanceTrue;
    }

    void RandomiseLeaderRanks()
    {
        if (!randomiseLeaderRanks && !randomiseOpponentLeaderRanks)
            return;
        for (var index = 0; index < Deck.DeckList.Count; index++)
        {
            if (randomiseLeaderRanks && index < 17)
            {
                Deck.DeckList[index].DeckLeader.Rank = (DeckLeaderRank)RandomNumberGenerator.GetInt32(1, 13);
            }
            else if (randomiseOpponentLeaderRanks && index > 27)
            {
                Deck.DeckList[index].DeckLeader.Rank = (DeckLeaderRank)RandomNumberGenerator.GetInt32(1, 13);
            }
            else
            {
                Deck.DeckList[index].DeckLeader.Rank = leaderRanksOriginal[index];
            }
        }
    }

    void RandomiseDecks()
    {
        if (randomiseDecks)
        {
            if (balancedRandom)
            {

                for (var deckIndex = 0; deckIndex < Deck.DeckList.Count; deckIndex++)
                {
                    int currentMonsterCount = 0;
                    int currentSpellCount = 0;
                    int currentEquipCount = 0;
                    int currentTrapCount = 0;
                    int currentRitualCount = 0;
                    Deck? deck = Deck.DeckList[deckIndex];

                    //Still need to check for my than triples or disable it.
                    for (var deckSlot = 0; deckSlot < deck.CardList.Count; deckSlot++)
                    {

                        Dictionary<int, int> CurrentDeckCardCount = new Dictionary<int, int>();
                        if (randomiseStartingDecks)
                        {
                            if (deckIndex < 17)
                            {
                                deck.DeckLeader = CreateRandomCard("monster", true, DeckLeaderRank.LT2);
                                if (currentMonsterCount < monsterCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "monster", true);
                                    currentMonsterCount++;
                                }
                                else if (currentSpellCount < spellCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "spell", true);
                                    currentSpellCount++;
                                }
                                else if (currentEquipCount < equipCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "equip", true);
                                    currentEquipCount++;
                                }
                                else if (currentTrapCount < trapCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "trap", true);
                                    currentTrapCount++;
                                }
                                else if (currentRitualCount < ritualCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "ritual", true);
                                    currentRitualCount++;
                                }
                            }
                        }
                        if (randomiseOpponentDecks)
                        {
                            if (deckIndex > 27)
                            {
                                deck.DeckLeader = CreateRandomCard("monster", false, leaderRanksOriginal[deckIndex]);
                                if (currentMonsterCount < monsterCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "monster", false);
                                    currentMonsterCount++;
                                }
                                else if (currentSpellCount < spellCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "spell", false);
                                    currentSpellCount++;
                                }
                                else if (currentEquipCount < equipCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "equip", false);
                                    currentEquipCount++;
                                }
                                else if (currentTrapCount < trapCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "trap", false);
                                    currentTrapCount++;
                                }
                                else if (currentRitualCount < ritualCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "ritual", false);
                                    currentRitualCount++;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (var deckIndex = 0; deckIndex < Deck.DeckList.Count; deckIndex++)
                {
                    Dictionary<int, int> CurrentDeckCardCount = new Dictionary<int, int>();
                    Deck? deck = Deck.DeckList[deckIndex];
                    for (var deckSlot = 0; deckSlot < deck.CardList.Count; deckSlot++)
                    {
                        if (randomiseStartingDecks)
                        {
                            if (deckIndex < 17)
                            {
                                deck.DeckLeader = CreateRandomCard("monster");
                                FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "", false);
                            }
                        }
                        if (randomiseOpponentDecks)
                        {
                            if (deckIndex > 27)
                            {
                                deck.DeckLeader = CreateRandomCard("monster");
                                FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "", false);
                            }
                        }
                    }
                }
            }
        }
    }

    void FillDeckWithType(Dictionary<int, int> CurrentDeckCardCount, Deck deck, int deckSlot, string type, bool isStarterDeck)
    {
        bool cardAdded = false;
        while (!cardAdded)
        {
            DeckCard card = CreateRandomCard(type, isStarterDeck);
            if (CurrentDeckCardCount.TryGetValue(card.CardConstant.Index, out int count))
            {
                if (count >= 3)
                {
                    continue;
                }
                CurrentDeckCardCount[card.CardConstant.Index] = count + 1;
            }
            else
            {
                CurrentDeckCardCount.Add(card.CardConstant.Index, 1);
            }
            deck.CardList[deckSlot] = card;
            cardAdded = true;
        }
    }

    DeckCard CreateRandomCard(string type, bool isStarterDeck = false, DeckLeaderRank rank = DeckLeaderRank.NCO)
    {
        while (true)
        {
            int cardIndex;
            switch (type)
            {
                case "monster":
                    cardIndex = RandomNumberGenerator.GetInt32(0, 683);
                    break;
                case "spell":
                    cardIndex = RandomNumberGenerator.GetInt32(683, 752);
                    break;
                case "equip":
                    cardIndex = RandomNumberGenerator.GetInt32(752, 801);
                    break;
                case "trap":
                    cardIndex = RandomNumberGenerator.GetInt32(801, 830);
                    break;
                case "ritual":
                    cardIndex = RandomNumberGenerator.GetInt32(830, 854);
                    break;
                default:
                    cardIndex = RandomNumberGenerator.GetInt32(0, 854);
                    break;
            }
            if (banSpecificCards)
            {
                if (bannedCards.Contains(cardIndex))
                    continue;
            }
            DeckCard card = new DeckCard(CardConstant.List[cardIndex], rank);
            if (lowPowerLevelRandom && isStarterDeck)
            {
                bool invalidAtk = card.Attack > bannedAtkLimit;
                bool invalidDef = card.Defense > bannedDefLimit;
                bool invalidLevel = card.Level > bannedLevelLimit;
                bool invalidDeckCost = card.DeckCost > bannedDcLimit;
                if (invalidAtk || invalidDef || invalidLevel || invalidDeckCost)
                    continue;
            }
            return card;
        }
    }

    void RandomiseMaps()
    {
        if (randomiseMapsTiles)
        {
            foreach (var map in DataAccess.Instance.maps)
            {
                int currentCrushAmount = 0;
                int currentLabAmount = 0;
                if (!swapTiles)
                {
                    for (var index0 = 0; index0 < map.tiles.GetLength(0); index0++)
                    for (var index1 = 0; index1 < map.tiles.GetLength(1); index1++)
                    {
                        map.tiles[index0, index1] = GetRandomTerrain();
                    }
                }
                else
                {
                    //tile replacement map
                    Dictionary<Terrain, Terrain> terrainSwapMap = new Dictionary<Terrain, Terrain>();
                    for (int i = 0; i < 10; i++)
                    {
                        Terrain originalTerrain = (Terrain)i;
                        Terrain newTerrain = GetRandomTerrain();
                        terrainSwapMap[originalTerrain] = newTerrain;
                    }

                    // Then apply this mapping to all tiles
                    for (var index0 = 0; index0 < map.tiles.GetLength(0); index0++)
                    for (var index1 = 0; index1 < map.tiles.GetLength(1); index1++)
                    {

                        Terrain swapResult = terrainSwapMap[map.tiles[index0, index1]];
                        if (swapResult == Terrain.Crush)
                        {
                            if (currentCrushAmount < maxCrushTiles)
                            {
                                currentCrushAmount++;
                            }
                            else
                            {
                                foreach (var kvp in terrainSwapMap)
                                {
                                    if (kvp.Value == Terrain.Crush)
                                    {
                                        Terrain newTerrain = GetRandomTerrain();
                                        while (newTerrain == Terrain.Crush)
                                        {
                                            newTerrain = GetRandomTerrain();
                                        }
                                        terrainSwapMap[kvp.Key] = newTerrain;

                                    }
                                }
                                swapResult = terrainSwapMap[map.tiles[index0, index1]];
                            }

                        }
                        else if (swapResult == Terrain.Labyrinth)
                        {
                            if (currentLabAmount < maxLabTiles)
                            {
                                currentLabAmount++;
                            }
                            else
                            {
                                foreach (var kvp in terrainSwapMap)
                                {
                                    if (kvp.Value == Terrain.Labyrinth)
                                    {
                                        Terrain newTerrain = GetRandomTerrain();
                                        while (newTerrain == Terrain.Labyrinth)
                                        {
                                            newTerrain = GetRandomTerrain();
                                        }
                                        terrainSwapMap[kvp.Key] = newTerrain;

                                    }
                                }
                                swapResult = terrainSwapMap[map.tiles[index0, index1]];
                            }
                        }
                        map.tiles[index0, index1] = swapResult;
                    }
                }
            }
        }
        if (randomiseMaps)
        {
            DotrMap[] maps = DataAccess.Instance.maps;
            DotrMap[] newMapOrder = new DotrMap[maps.Length];
            Array.Copy(maps, newMapOrder, maps.Length);

            // Fisher-Yates shuffle
            for (int i = newMapOrder.Length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(0, i + 1);
                // could use xor swap instead, eh
                // ReSharper disable once SwapViaDeconstruction
                DotrMap temp = newMapOrder[i];
                newMapOrder[i] = newMapOrder[j];
                newMapOrder[j] = temp;
            }
            Array.Copy(newMapOrder, DataAccess.Instance.maps, newMapOrder.Length);
        }
    }

    Terrain GetRandomTerrain()
    {
        return (Terrain)RandomNumberGenerator.GetInt32(0, 10);
    }

    void RandomiseCardConstData()
    {
        foreach (var cardConstant in CardConstant.List)
        {
            if (randomiseSlots)
            {
                cardConstant.AppearsInSlotReels = GetRandomBool();
            }
            if (randomiseRareDrop)
            {
                cardConstant.IsSlotRare = GetRandomBool();
            }
            if (randomiseReincarnation)
            {
                cardConstant.AppearsInReincarnation = GetRandomBool();
            }
            if (!cardConstant.AppearsInSlotReels && !cardConstant.AppearsInReincarnation && !cardConstant.IsSlotRare)
            {
                cardConstant.IsSlotRare = true;
            }

            if (cardConstant.CardKind.isMonster())
            {
                if (randomiseCardATKDEF)
                {

                    if (randomAtkDefRange)
                    {
                        cardConstant.Attack = (ushort)Math.Clamp(
                            Math.Round(GetRandomAroundTarget(cardConstant.Attack, attackDelta) / 50.0) * 50,
                            0, 8000);
                        cardConstant.Defense = (ushort)Math.Clamp(
                            Math.Round(GetRandomAroundTarget(cardConstant.Defense, defenseDelta) / 50.0) * 50,
                            0, 8000);
                    }
                    else
                    {

                        if (randomAtkDefCap)
                        {
                            cardConstant.Attack = (ushort)(Math.Round(RandomNumberGenerator.GetInt32(0, attackCap + 1) / 50.0) * 50);
                            cardConstant.Defense = (ushort)(Math.Round(RandomNumberGenerator.GetInt32(0, defenseCap + 1) / 50.0) * 50);
                        }
                    }

                }
                if (randomiseSummoningPower)
                {

                    if (randomSPCap)
                    {

                        cardConstant.Level = (byte)RandomNumberGenerator.GetInt32(0, levelCap);
                    }
                    else
                    {

                        if (randomSPRange)
                        {
                            cardConstant.Level = (byte)GetRandomAroundTarget(cardConstant.Level, levelDelta);
                        }
                    }

                }
                if (randomiseKinds)
                {
                    cardConstant.Kind = (byte)RandomNumberGenerator.GetInt32(0, 21);
                }
                if (randomiseAttributes)
                {
                    cardConstant.Attribute = (byte)RandomNumberGenerator.GetInt32(0, 6);
                }
                if (randomiseMonsterEffects)
                {
                    cardConstant.EffectId = UInt16.MaxValue;
                    if (RandomBoolIn100(randomEffectChance))
                    {
                        cardConstant.EffectId = (ushort)RandomNumberGenerator.GetInt32(0, Effects.MonsterEffectsList.Count);
                    }
                    cardConstant.setCardColor();
                }
            }
            else if (cardConstant.CardKind.isMagic() && randomiseMagicEffects)
            {
                cardConstant.EffectId = (ushort)RandomNumberGenerator.GetInt32(0, Effects.MagicEffectsList.Count);
                cardConstant.setCardColor();
            }
        }

        if (randomisePowerUpValues)
        {
            for (int i = 0; i < EnchantData.EnchantIds.Count; i++)
            {
                if (EnchantData.EnchantIds[i] == 0)
                {
                    EnchantData.EnchantScores[i] = (ushort)Math.Clamp(GetRandomAroundTarget(EnchantData.EnchantScores[i], powerUpDelta), 0, 8000);
                }
            }
        }
    }

    int GetRandomAroundTarget(int targetNumber, int Delta, bool updown = true)
    {
        if (updown)
        {
            int lowerBound = targetNumber - Delta;
            return RandomNumberGenerator.GetInt32(Math.Max(lowerBound, 0), targetNumber + Delta + 1);
        }
        else
        {
            return RandomNumberGenerator.GetInt32(targetNumber, targetNumber + Delta + 1);
        }
    }

    static bool GetRandomBool()
    {

        return RandomNumberGenerator.GetInt32(0, 2) == 1;
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
        EditorWindow.OnIsoLoaded -= LoadLeaderRanks;
    }

    public void DoRandomiserPatches()
    {
        if (hasRandomised)
        {
            new ExtendedCardCopyLimitPatch().ApplyOrRemove(true);
            //No DC all Game
            var dcPatch = new RemoveDCRequirementsPostGame();
            if (dcPatch.IsApplied())
            {
                dcPatch.ApplyOrRemove(false);
            }
            new RemoveDCRequirementsGeneral().ApplyOrRemove(true);
            new AllKindsExtraCardLeaderAbility().ApplyOrRemove(true);

        }
    }
}