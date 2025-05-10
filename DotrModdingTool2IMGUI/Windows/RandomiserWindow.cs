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
    bool randomiseHiddenCardLocation;
    bool randomiseHiddenCardValue;
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
    bool strongBossDeck;
    bool swapTiles = true;
    bool hasRandomised = false;
    bool banSpecificCards;
    bool banForPlayerOnly = false;
    bool banForEnemyOnly = false;
    bool bossesBypassBan;
    bool randomiseAI = false;
    bool randomiseEquip = false;
    bool randomiseStrongOnToon;
    bool randomiseMusic = false;
    bool SpBasedOnPower;

    int strongOnToonChance = 20;
    int maxCrushTiles = 16;
    int maxLabTiles = 16;
    int maxToonTiles = 49;

    int monsterCount = 25;
    int spellCount = 5;
    int trapCount = 5;
    int equipCount = 3;
    int ritualCount = 2;
    int randomEffectChance = 15;
    int maxRandomRank = 12;
    int maxLeaderAbilities = 16;
    int weakLeaderAbilityChance = 30;
    int strongLeaderAbilityChance = 30;
    int OPLeaderAbilityChance = 30;
    int bannedDcLimit = 99;
    int bannedLevelLimit = 12;
    int bannedDefLimit = 8000;
    int bannedAtkLimit = 8000;

    int EquipPercentChance = 10;
    int slotChance = 50;

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

    List<int> bossDecks = new List<int>() { 27, 45, 46, 47 };
    List<(int index, float strength)> strongestMonsters = new List<(int index, float strength)>();
    List<(int index, byte deckCost)> strongestSpells = new List<(int index, byte deckCost)>();
    List<(int index, byte deckCost)> strongestTraps = new List<(int index, byte deckCost)>();

    List<DeckLeaderAbilityType> weakLeaderAbilities = new List<DeckLeaderAbilityType>();
    List<DeckLeaderAbilityType> strongLeaderAbilities = new List<DeckLeaderAbilityType>();
    List<DeckLeaderAbilityType> OPLeaderAbilities = new List<DeckLeaderAbilityType>();

    HashSet<int> bannedCards = new HashSet<int>();
    List<string> bannedCardFilteredList = new List<string>();
    HashSet<string> selectedCards = new HashSet<string>();
    int currentBannedCardIndex = 0;
    string cardSearch = "";
    int? bannedCardToRemove = null;
    string SearchSortField = "ID";
    bool SearchAscending = true;
    EnemyEditorWindow _enemyEditorWindow;
    MusicEditorWindow _musicEditorWindow;
    Dictionary<int, DeckLeaderRank> leaderRanksOriginal = new Dictionary<int, DeckLeaderRank>();


    public RandomiserWindow(EnemyEditorWindow enemyEditorWindow, MusicEditorWindow musicEditorWindow)
    {
        EditorWindow.OnIsoLoaded += FilterAndSort;
        EditorWindow.OnIsoLoaded += LoadLeaderRanks;
        _enemyEditorWindow = enemyEditorWindow;
        _musicEditorWindow = musicEditorWindow;
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
            @"Using the randomiser makes all AI's DMK if not you choose not randomise the AI, as this is the most versatile and least likely to brick
Secondly Deck Cost will be meaningless when randomiser, this will make all battle available regardless of DC
Not recommended to edit card effects as it will not update the text, but im not the law");
        if (ImGui.Button("Randomise"))
        {
            RandomiseCardConstData();
            FixEquipTargets();
            RandomiseLeaderData();
            RandomiseDecks();
            RandomiseLeaderRanks();
            RandomiseMaps();
            ChangeAllAi();
            RandomiseMusic();
            _enemyEditorWindow.DeckEditorWindow.UpdateDeckData();
            hasRandomised = true;
            if (hasRandomised)
            {
                GameplayPatchesWindow.Instance.bNineCardLimit = true;
                if (GameplayPatchesWindow.Instance.bNoDcPostGame)
                {
                    GameplayPatchesWindow.Instance.bNoDcPostGame = false;
                }
                GameplayPatchesWindow.Instance.bNoDcAllGame = true;
                GameplayPatchesWindow.Instance.bAllKindsExtraSlots = true;
                new ExtendedCardCopyLimitPatch().ApplyOrRemove(true);
            }
        }
        ImGui.SameLine();
        ImGui.Text("Press this button after selecting your settings");
        ImGui.Separator();

        ImGui.Checkbox("Randomise decks", ref randomiseDecks);
        if (randomiseDecks)
        {
            ImGui.Indent();
            ImGui.Checkbox("Randomise starting decks", ref randomiseStartingDecks);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises the leaders and cards in the starter decks");
            ImGui.Checkbox("Randomise enemy decks", ref randomiseOpponentDecks);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomise the leaders and cards in enemy decks");
            if (randomiseOpponentDecks)
            {

                ImGui.Indent();
                ImGui.Checkbox("Strong boss enemies", ref strongBossDeck);
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Boss enemies pull from a selected pool for stronger cards\nBosses are Yugi,Seto,MFL both sides ");
                ImGui.Unindent();
            }
            ImGui.Checkbox("Balanced deck", ref balancedRandom);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("you will always have X monsters X power ups X spells X traps and X rituals\n Must add up to 40 cards");
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
                if (ImGui.InputInt("Max attack", ref bannedAtkLimit, 100, 1000))
                {
                    bannedAtkLimit = Math.Clamp(bannedAtkLimit, 0, 9999);
                }
                if (ImGui.InputInt("Max defense", ref bannedDefLimit, 100, 1000))
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

        ImGui.Checkbox("Randomise card acquisition", ref randomiseCardAcquisition);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("Randomises how cards can be acquired\nIf it has no valid way to get the card it will default to Rare");

        if (randomiseCardAcquisition)
        {
            ImGui.Indent();
            if (ImGui.Checkbox("Randomise slots", ref randomiseSlots))
            {
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises whether the card will appear in the slots");
            if (randomiseSlots)
            {
                ImGui.SliderInt("Chance", ref slotChance, 0, 100);
            }
            if (ImGui.Checkbox("Randomise rare drop", ref randomiseRareDrop))
            {
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises whether the card as 3 in row");
            if (ImGui.Checkbox("Randomise reincarnation", ref randomiseReincarnation))
            {
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises whether the card will appear as a reincarnation reward");
            ImGui.Unindent();
        }
        //Add boss random good cards only

        ImGui.Checkbox("Randomise AI", ref randomiseAI);

        ImGui.Checkbox("Randomise maps", ref randomiseMaps);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises which map is assigned to which duelist");
        if (randomiseMaps)
        {
        }
        ImGui.Checkbox("Random hidden card location", ref randomiseHiddenCardLocation);
        ImGui.Checkbox("Random hidden card", ref randomiseHiddenCardValue);
        ImGui.Checkbox("Randomise map tiles", ref randomiseMapsTiles);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises tiles on existing maps");
        if (randomiseMapsTiles)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Max crush tiles", ref maxCrushTiles, 0, 49))
            {
            }
            if (ImGui.SliderInt("Max labyrinth tiles", ref maxLabTiles, 0, 49))
            {
            }
            if (ImGui.SliderInt("Max toon tiles", ref maxToonTiles, 0, 49))
            {
            }
            if (ImGui.Checkbox("Swap terrain types", ref swapTiles))
            {
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    "If enabled all tiles of a certain type are replaced with another type rather than being true random\nE.g all sea is replace with mountains");
            ImGui.Unindent();
        }
        ImGui.Checkbox("Randomise enemy leader ranks", ref randomiseOpponentLeaderRanks);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises the leader ranks of your opponents");

        ImGui.Checkbox("Randomise starting leader ranks", ref randomiseLeaderRanks);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises the leader ranks of your starter options");
        if (randomiseLeaderRanks)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Max possible rank", ref maxRandomRank, 0, 12))
            {
            }
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise leader abilities", ref randomiseLeaderAbilities);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(
                "Randomises the leader abilities a card can have.\nEach monster will roll once for each valid leader ability it can get\nThe level at which they get the ability is also randomised");
        if (randomiseLeaderAbilities)
        {
            ImGui.Indent();
            if (ImGui.SliderInt("Max number of abilities ", ref maxLeaderAbilities, 0, 10))
            {
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("The max amount of leader abilities a unit get it");

            if (ImGui.SliderInt("Weak leader ability % chance", ref weakLeaderAbilityChance, 0, 100))
            {
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("");

            if (ImGui.SliderInt("Strong leader ability % chance", ref strongLeaderAbilityChance, 0, 100))
            {
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("The % chance of success of a given roll when attempting to get a leader ability");

            if (ImGui.SliderInt("OP leader ability % chance", ref OPLeaderAbilityChance, 0, 100))
            {
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("The % chance of success of a given roll when attempting to get a leader ability");
            ImGui.Unindent();
        }
        ImGui.Checkbox("Randomise card ATK-DEF", ref randomiseCardATKDEF);
        if (randomiseCardATKDEF)
        {
            ImGui.Indent();
            ImGui.Text("You can only pick one");
            if (ImGui.Checkbox("Randomise ATK-DEF in range", ref randomAtkDefRange))
            {
                randomAtkDefCap = false;
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("+ or - X value on the cards original ATK/DEF values");
            if (randomAtkDefRange)
            {
                ImGui.Indent();
                if (ImGui.InputInt("Max Attack Range ", ref attackDelta, 100, 1000))
                {
                    attackDelta = Math.Clamp(attackDelta, 0, 9999);
                }
                if (ImGui.InputInt("Max defence range", ref defenseDelta, 100, 1000))
                {
                    defenseDelta = Math.Clamp(defenseDelta, 0, 9999);
                }
                ImGui.Unindent();
            }
            if (ImGui.Checkbox("Randomise ATK-DEF with cap", ref randomAtkDefCap))
            {
                randomAtkDefRange = false;
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("completely random values that cant go past X value");
            if (randomAtkDefCap)
            {
                ImGui.Indent();
                if (ImGui.InputInt("Max attack cap ", ref attackCap, 50, 100))
                {
                    attackCap = Math.Clamp(attackCap, 0, 8000);
                }
                if (ImGui.InputInt("Max defence cap", ref defenseCap, 50, 100))
                {
                    defenseCap = Math.Clamp(defenseCap, 0, 8000);
                }
                ImGui.Unindent();
            }
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise card summoning power", ref randomiseSummoningPower);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("You can only pick one");
        if (randomiseSummoningPower)
        {

            ImGui.Indent();
            if (ImGui.Checkbox("Randomise SP in range", ref randomSPRange))
            {
                randomSPCap = false;
                SpBasedOnPower = false;
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("+ or - X value on the cards original Cost/Summoning Power");

            if (randomSPRange)
            {
                ImGui.Indent();
                if (ImGui.SliderInt("Max summoning power difference", ref levelDelta, 0, 12))
                {
                }
                ImGui.Unindent();
            }
            if (ImGui.Checkbox("Randomise SP with cap", ref randomSPCap))
            {
                randomSPRange = false;
                SpBasedOnPower = false;
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("completely random values that cant go past X Cost/Summoning Power");
            if (randomSPCap)
            {
                ImGui.Indent();
                if (ImGui.SliderInt("Max summoning power difference", ref levelCap, 0, 12))
                {
                }
                ImGui.Unindent();
            }
            if (ImGui.Checkbox("Normalise SP / Assign SP based on power", ref SpBasedOnPower))
            {
                randomSPRange = false;
                randomSPCap = false;
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(@"ATK and DEF combined
0-699 = 1 star
700-1399 = 2 star
1400-2099 = 3 star
2100-2799 = 4 star
2800-3499 = 5 star
3500-4199 = 6 star
4200-4899 = 7 star
4900-5599 = 8 star
5600-6299 = 9 star
6300-6999 = 10 star
7000-8000 = 11 star
>8000 = 12 star 
");
            ImGui.Unindent();
        }

        ImGui.Checkbox("Randomise card attribute", ref randomiseAttributes);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Gives monsters a random Attribute, Fire, Earth, Water..");
        ImGui.Checkbox("Randomise card kind", ref randomiseKinds);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises monsters to other monster kinds");

        ImGui.Checkbox("Randomise valid power-ups", ref randomiseEquip);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises what equips will work ");
        if (randomiseEquip)
        {
            ImGui.SliderInt("Roll chance per power up", ref EquipPercentChance, 0, 100);
            {
            }
        }
        ImGui.Checkbox("Randomise power-up values", ref randomisePowerUpValues);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Gives power increase power ups a random value within a range of x of its original value");
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
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("If banned these card cannot be randomised into by you or opponents");
        if (banSpecificCards)
        {
            ImGui.Indent();

            if (ImGui.Checkbox("Ban for player only", ref banForPlayerOnly))
            {
                banForEnemyOnly = false;
            }

            if (ImGui.Checkbox("Ban for enemy only", ref banForEnemyOnly))
            {
                banForPlayerOnly = false;
            }

            ImGui.Checkbox("Bosses can have banned cards", ref bossesBypassBan);
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
        ImGui.TextColored(new GuiColour(Color.Red).value, "Not recommended due to lack of text changes");
        ImGui.Separator();
        ImGui.Checkbox("Randomise monster effects", ref randomiseMonsterEffects);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(@"Will give monsters a X/100 chance top be assigned a random monster effect. ");
        if (randomiseMonsterEffects)
        {
            ImGui.SliderInt("Effect chance", ref randomEffectChance, 1, 100);
        }
        ImGui.Checkbox("Randomise magic effects", ref randomiseMagicEffects);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randomises the effect of a spell with another spell or monster flip effect");
        ImGui.Checkbox("Randomise strong on toon", ref randomiseStrongOnToon);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Randoms whether a monster should be strong on too");
        if (randomiseStrongOnToon)
        {
            ImGui.SliderInt("Strong on toon chance", ref strongOnToonChance, 0, 100);
        }
        ImGui.Checkbox("Randomise enemy background music tracks", ref randomiseMusic);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Not recommended because it forces fast intro mod to work");

        ImGui.PopFont();
    }

    void RandomiseMusic()
    {
        if (randomiseMusic)
        {
            foreach (var key in _musicEditorWindow.DuelistMusic.Keys)
            {

                _musicEditorWindow.DuelistMusic[key] = RandomNumberGenerator.GetInt32(0, _musicEditorWindow.DuelistMusic.Keys.Count);
                while (_musicEditorWindow.DuelistMusic[key] == 1 || _musicEditorWindow.DuelistMusic[key] == 6 ||
                       _musicEditorWindow.DuelistMusic[key] == 23 ||
                       _musicEditorWindow.DuelistMusic[key] == 39 || _musicEditorWindow.DuelistMusic[key] == 42 ||
                       _musicEditorWindow.DuelistMusic[key] == 43)
                {
                    _musicEditorWindow.DuelistMusic[key] = RandomNumberGenerator.GetInt32(2, 45);

                }
            }
            _musicEditorWindow.bSaveMusicChanges = true;
            GameplayPatchesWindow.Instance.bSaveMusic = true;
        }
    }


    void ChangeAllAi()
    {
        if (randomiseAI)
        {
            foreach (var enemy in Enemies.EnemyList)
            {
                int randomValue = RandomNumberGenerator.GetInt32(0, Enemies.EnemyList.Count);
                while (randomValue == 5 || randomValue == 16)
                {
                    randomValue = RandomNumberGenerator.GetInt32(0, Enemies.EnemyList.Count);
                }
                enemy.AiId = Enemies.EnemyList[randomValue].AiId;
            }
        }
        else
        {
            foreach (var enemy in Enemies.EnemyList)
            {
                enemy.AiId = Enemies.EnemyList[22].AiId;
            }
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

            int assignedAbilities = 0;
            StartingDeckData.StarterDeckDataEnums.Kind cardKind =
                (StartingDeckData.StarterDeckDataEnums.Kind)CardConstant.Monsters[deckLeaderAbilityInstance.CardId].CardKind.Id + 1;
            for (int i = 0; i < deckLeaderAbilityInstance.Abilities.Length; i++)
            {
                deckLeaderAbilityInstance.Abilities[i].SetEnabled(false);
            }

            for (int i = 0; i < deckLeaderAbilityInstance.Abilities.Length && assignedAbilities < maxLeaderAbilities; i++)
            {
                DeckLeaderAbilityType currentAbility = (DeckLeaderAbilityType)i;
                if (currentAbility == DeckLeaderAbilityType.HiddenCard)
                {
                    deckLeaderAbilityInstance.Abilities[i].SetEnabled(true);
                    deckLeaderAbilityInstance.Abilities[i].RankRequired = 1;
                    assignedAbilities++;
                    continue;
                }
                bool isValid = true;
                if (abilityValidations.TryGetValue(currentAbility, out var validationFunc))
                {
                    isValid = validationFunc(cardKind);
                }

                if (isValid)
                {
                    assignedAbilities++;
                    int chanceToEnable = 0;

                    if (weakLeaderAbilities.Contains(currentAbility))
                    {
                        chanceToEnable = weakLeaderAbilityChance;
                    }
                    else if (strongLeaderAbilities.Contains(currentAbility))
                    {
                        chanceToEnable = strongLeaderAbilityChance;
                    }
                    else if (OPLeaderAbilities.Contains(currentAbility))
                    {
                        chanceToEnable = OPLeaderAbilityChance;
                    }
                    if (RandomBoolIn100(chanceToEnable))
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
            else if (randomiseOpponentLeaderRanks && index >= 27)
            {

                if (strongBossDeck && IsBossEnemy(index))
                {
                    Deck.DeckList[index].DeckLeader.Rank = DeckLeaderRank.SD;
                }
                else
                {
                    Deck.DeckList[index].DeckLeader.Rank = (DeckLeaderRank)RandomNumberGenerator.GetInt32(1, 13);
                }

            }
            else
            {
                Deck.DeckList[index].DeckLeader.Rank = leaderRanksOriginal[index];
            }
        }
    }

    bool IsBossEnemy(int index)
    {
        if (bossDecks.Contains(index))
        {
            return true;
        }
        return false;
    }

    void RandomiseDecks()
    {
        if (strongBossDeck)
        {
            GetStrongCards();
        }
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
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "monster", true, deckIndex);
                                    currentMonsterCount++;
                                }
                                else if (currentSpellCount < spellCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "spell", true, deckIndex);
                                    currentSpellCount++;
                                }
                                else if (currentEquipCount < equipCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "equip", true, deckIndex);
                                    currentEquipCount++;
                                }
                                else if (currentTrapCount < trapCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "trap", true, deckIndex);
                                    currentTrapCount++;
                                }
                                else if (currentRitualCount < ritualCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "ritual", true, deckIndex);
                                    currentRitualCount++;
                                }
                            }
                        }
                        if (randomiseOpponentDecks)
                        {
                            if (deckIndex >= 27)
                            {
                                deck.DeckLeader = CreateRandomCard("monster", false, leaderRanksOriginal[deckIndex]);
                                if (currentMonsterCount < monsterCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "monster", false, deckIndex);
                                    currentMonsterCount++;
                                }
                                else if (currentSpellCount < spellCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "spell", false, deckIndex);
                                    currentSpellCount++;
                                }
                                else if (currentEquipCount < equipCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "equip", false, deckIndex);
                                    currentEquipCount++;
                                }
                                else if (currentTrapCount < trapCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "trap", false, deckIndex);
                                    currentTrapCount++;
                                }
                                else if (currentRitualCount < ritualCount)
                                {
                                    FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "ritual", false, deckIndex);
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
                                FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "", false, deckIndex);
                            }
                        }
                        if (randomiseOpponentDecks)
                        {
                            if (deckIndex >= 27)
                            {
                                deck.DeckLeader = CreateRandomCard("monster");
                                FillDeckWithType(CurrentDeckCardCount, deck, deckSlot, "", false, deckIndex);
                            }
                        }
                    }
                }
            }
            if (strongBossDeck)
            {
                GiveBossesEquips();
            }
        }

    }

    void FixEquipTargets()
    {
        if (randomiseEquip)
        {
            for (var i = 0; i < MonsterEnchantData.MonsterEnchantDataList.Count; i++)
            {
                MonsterEnchantData? monsterEnchantData = MonsterEnchantData.MonsterEnchantDataList[i];
                monsterEnchantData.Flags[43] = true;
                monsterEnchantData.Flags[44] = true;
            }
        }
        else
        {
            for (var i = 0; i < MonsterEnchantData.MonsterEnchantDataList.Count; i++)
            {
                MonsterEnchantData? monsterEnchantData = MonsterEnchantData.MonsterEnchantDataList[i];
                for (int flagIndex = 0; flagIndex < 49; flagIndex++)
                {
                    switch (flagIndex)
                    {
                        case 0: // 752 Legendary Sword
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Warrior &&
                                CardConstant.List[i].Attribute is (int)AttributeVisual.Light or (int)AttributeVisual.Earth)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 1: // 753 Sword of Dark Destruction
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Warrior &&
                                CardConstant.List[i].Attribute is (int)AttributeVisual.Dark or (int)AttributeVisual.Fire
                                    or (int)AttributeVisual.Water and (int)AttributeVisual.Wind)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 2: // 754 Dark Energy
                            if (CardConstant.List[i].Attribute == (int)AttributeVisual.Dark)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 3: // 755 Axe of Despair
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.BeastWarrior or (int)CardKind.CardKindEnum.Fiend)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 4: // 756 Laser Cannon Armor
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Insect)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 5: // 757 Insect Armor with Laser Cannon
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Insect)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 6: // 758 Elf's Light
                            if (CardConstant.List[i].Name.ToLower().Contains("elf"))
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 7: // 759 Beast Fangs
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Beast)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 8: // 760 Steel Shell
                            break;
                        case 9: // 761 Vile Germs
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Plant)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 10: // 762 Black Pendant
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Spellcaster &&
                                CardConstant.List[i].Attribute is (int)AttributeVisual.Dark)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 11: // 763 Silver Bow and Arrow
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Fairy)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 12: // 764 Horn of Light
                            break;
                        case 13: // 765 Horn of the Unicorn
                            break;
                        case 14: // 766 Dragon Treasure
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Dragon or (int)CardKind.CardKindEnum.SeaSerpent)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 15: // 767 Electro-Whip
                            break;
                        case 16: // 768 Cyber Shield
                            break;
                        case 17: // 769 Mystical Moon
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Beast or (int)CardKind.CardKindEnum.BeastWarrior)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 18: // 770 Malevolent Nuzzler
                            break;
                        case 19: // 771 Book of Secret Arts
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Spellcaster &&
                                CardConstant.List[i].Attribute != (int)AttributeVisual.Dark)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 20: // 772 Violet Crystal
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Zombie)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 21: // 773 Invigoration
                            if (CardConstant.List[i].Attribute == (int)AttributeVisual.Earth)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 22: // 774 Machine Conversion Factory
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Machine)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 23: // 775 Raise Body Heat
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Dinosaur or (int)CardKind.CardKindEnum.Reptile)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 24: // 776 Follow Wind
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.WingedBeast)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 25: // 777 Power of Kaishin
                            if (CardConstant.List[i].Attribute == (int)AttributeVisual.Water)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 26: // 778 Kunai with Chain
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Warrior or (int)CardKind.CardKindEnum.BeastWarrior)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 27: // 779 Salamandra
                            if (CardConstant.List[i].Attribute == (int)AttributeVisual.Fire)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 28: // 780 Megamorph
                            monsterEnchantData.Flags[flagIndex] = true;
                            break;
                        case 29: // 781 Bright Castle
                            if (CardConstant.List[i].Attribute == (int)AttributeVisual.Light)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 30: // 782 Fiend Castle
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Fiend)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 31: // 783 Hightide
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Fish or (int)CardKind.CardKindEnum.SeaSerpent)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 32: // 784 Spring of Rebirth
                            if (CardConstant.List[i].CardKind.Id is (int)CardKind.CardKindEnum.Pyro or (int)CardKind.CardKindEnum.Thunder
                                or (int)CardKind.CardKindEnum.Aqua or (int)CardKind.CardKindEnum.Rock)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 33: // 785 Gust Fan
                            if (CardConstant.List[i].Attribute == (int)AttributeVisual.Wind)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 34: // 786 Burning Spear
                            if (CardConstant.List[i].CardKind.Id == (int)CardKind.CardKindEnum.Pyro)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 35: // 787 7 Completed
                            if (CardConstant.List[i].Index == 505) //Slot machine
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 36: // 788 Nails of Bane
                            if (CardConstant.List[i].CardKind.Id == (int)CardKind.CardKindEnum.Dragon &&
                                CardConstant.List[i].Attribute == (int)AttributeVisual.Dark)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 37: // 789 Riryoku
                            monsterEnchantData.Flags[flagIndex] = true;
                            break;
                        case 38: // 790 Multiply
                            break;
                        case 39: // 791 Sword of Dragon's Soul
                            if (CardConstant.List[i].CardKind.Id == (int)CardKind.CardKindEnum.Warrior)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 40: // 792 Enchanted Javelin\
                            if (CardConstant.List[i].CardKind.Id == (int)CardKind.CardKindEnum.Fairy)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 41: // 793 Anti-Magic Fragrance
                            if (CardConstant.List[i].CardKind.Id == (int)CardKind.CardKindEnum.Plant)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 42: // 794 Crush Card
                            if (CardConstant.List[i].Attack <= 1000)
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            break;
                        case 43: // 795 Paralyzing Potion
                            if (CardConstant.List[i].CardKind.Id == (int)CardKind.CardKindEnum.Machine)
                            {
                                monsterEnchantData.Flags[flagIndex] = false;
                            }
                            else
                            {
                                monsterEnchantData.Flags[flagIndex] = true;
                            }
                            break;
                        case 44: // 796 Cursebreaker
                            monsterEnchantData.Flags[flagIndex] = true;
                            break;
                        case 45: // 797 "Elegant Egotist"
                            break;
                        case 46: // 798 "Cocoon of Evolution"
                            break;
                        case 47: // 799 "Metalmorph"
                            break;
                        case 48: // 800 "Insect Imitation"
                            break;
                    }
                }
            }
        }
    }

    void GiveBossesEquips()
    {
        foreach (var bossDeckIndex in bossDecks)
        {
            HashSet<int> validEquipsIndex = new HashSet<int>();
            List<DeckCard> deck = Deck.DeckList[bossDeckIndex].CardList;
            foreach (var card in deck)
            {
                if (!card.CardConstant.CardKind.isMonster())
                    continue;
                for (int i = 0; i < MonsterEnchantData.MonsterEnchantDataList[card.CardConstant.Index].Flags.Count - 1; i++)
                {
                    if (MonsterEnchantData.MonsterEnchantDataList[card.CardConstant.Index].Flags[i])
                    {
                        validEquipsIndex.Add(i + 752);
                    }
                }
            }
            var bestEquips = validEquipsIndex
                .Select(i => (index: i, powerValue: (int)EnchantData.EnchantScores[i - 752]))
                .OrderByDescending(e => e.powerValue)
                .Take(Math.Min(8, validEquipsIndex.Count))
                .Select(e => e.index)
                .ToList();
            bestEquips.Add(789);
            //Console.WriteLine($"Best equips for {Deck.CharacterNameDictionary[bossDeckIndex]}");
            //foreach (var equipIndex in bestEquips)
            //{
            //    Console.WriteLine(CardConstant.List[equipIndex].Name);
            //}
            if (bestEquips.Count == 0)
                continue;

            for (var index = 0; index < deck.Count; index++)
            {
                DeckCard? card = deck[index];

                if (!card.CardConstant.CardKind.isPowerUp())
                    continue;
                int randomStrongEquipIndex = bestEquips[RandomNumberGenerator.GetInt32(0, bestEquips.Count)];
                deck[index] = new DeckCard(CardConstant.List[randomStrongEquipIndex], 0);
            }
        }
    }

    void FillDeckWithType(Dictionary<int, int> CurrentDeckCardCount, Deck deck, int deckSlot, string type, bool isStarterDeck, int index)
    {
        bool cardAdded = false;
        DeckCard card;
        while (!cardAdded)
        {
            if (strongBossDeck && !isStarterDeck && IsBossEnemy(index))
            {
                card = CreateStrongBossCard(type);
            }
            else
            {
                card = CreateRandomCard(type, isStarterDeck);
            }
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

    DeckCard CreateStrongBossCard(string type)
    {
        int cardIndex;
        do
        {
            cardIndex = GetRandomStrongCardByType(type);
        } while (banSpecificCards && !bossesBypassBan && bannedCards.Contains(cardIndex));

        return new DeckCard(CardConstant.List[cardIndex], 0);
    }

    int GetRandomStrongCardByType(string type)
    {
        int cardIndex;
        switch (type)
        {
            case "monster":
                return GetRandomStrongMonster();
            case "spell":
                do
                {
                    cardIndex = GetRandomStrongSpell();
                } while (!CardConstant.List[cardIndex].CardKind.isMagic());
                return cardIndex;
            case "trap":
                do
                {
                    cardIndex = GetRandomStrongTrap();
                } while (!CardConstant.List[cardIndex].CardKind.isTrap());
                return cardIndex;
            case "equip":
                return GiveDefaultEquip();
            case "ritual":
                return RandomNumberGenerator.GetInt32(830, 854);
            default:
                int cardType = RandomNumberGenerator.GetInt32(0, 4);
                switch (cardType)
                {
                    case 0:
                        return GetRandomStrongMonster();
                    case 1:
                        return GetRandomStrongSpell();
                    case 2:
                        return GetRandomStrongTrap();
                    case 3:
                        return GiveDefaultEquip();
                    default:
                        return GetRandomStrongMonster();
                }
        }
    }

    int GetRandomStrongMonster()
    {
        int randomIndex = RandomNumberGenerator.GetInt32(0, strongestMonsters.Count);
        return strongestMonsters[randomIndex].index;
    }

    int GetRandomStrongSpell()
    {
        int randomIndex = RandomNumberGenerator.GetInt32(0, strongestSpells.Count);
        return strongestSpells[randomIndex].index;
    }

    int GetRandomStrongTrap()
    {
        int randomIndex = RandomNumberGenerator.GetInt32(0, strongestTraps.Count);
        return strongestTraps[randomIndex].index;
    }

    int GiveDefaultEquip()
    {
        return CardConstant.List[800].Index;
    }

    void GetStrongCards()
    {
        var monsters = new List<(int index, float strength)>();
        var spells = new List<(int index, byte deckcost)>();
        var traps = new List<(int index, byte deckcost)>();

        for (int i = 0; i < 853; i++)
        {
            if (i == 671)
                continue;
            var card = CardConstant.List[i];
            if (i < 683)
            {
                float strength;
                if (card.Level == 0)
                {
                    int strengthRatio = (card.Attack + card.Defense);
                    float levelFactor = (float)(10.0 / Math.Log(2));
                    strength = strengthRatio * levelFactor / 100f;
                }
                else
                {
                    int strengthRatio = (card.Attack + card.Defense);
                    float levelFactor = (float)(10.0 / Math.Log(card.Level + 1));
                    strength = strengthRatio * levelFactor / 100f;
                }
                monsters.Add((i, strength));
            }
            else if (i is >= 683 and < 752)
            {

                spells.Add((i, card.DeckCost));
            }
            else if (i is >= 801 and < 830)
            {
                traps.Add((i, card.DeckCost));
            }

        }
        strongestMonsters = monsters
            .OrderByDescending(c => c.strength)
            .Take(250)
            .ToList();
        //foreach (var score in strongestMonsters)
        //{
        //    Console.WriteLine($"{CardConstant.Monsters[score.index].Name} : power level {score.strength}");
        //}
        strongestSpells = spells
            .OrderByDescending(c => c.deckcost)
            .Take(20)
            .ToList();

        strongestTraps = traps
            .OrderByDescending(c => c.deckcost)
            .Take(20)
            .ToList();
        Console.WriteLine("Strong spells and traps");
        foreach (var score in strongestSpells)
        {
            Console.WriteLine($"{CardConstant.List[score.index].Name}");
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
                if (banForPlayerOnly && isStarterDeck)
                {
                    if (bannedCards.Contains(cardIndex))
                        continue;
                }
                else if (banForEnemyOnly && !isStarterDeck)
                {
                    if (bannedCards.Contains(cardIndex))
                        continue;
                }
                else
                {
                    if (bannedCards.Contains(cardIndex))
                        continue;
                }
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
        if (randomiseMaps)
        {
            DotrMap[] maps = DataAccess.Instance.maps;
            DotrMap[] newMapOrder = new DotrMap[maps.Length];
            Array.Copy(maps, newMapOrder, maps.Length);
            List<int> mapIndicesToSkip = new List<int>();
            
            bool keptOneSingleTerrainMap = false;
            for (int i = 0; i < newMapOrder.Length; i++)
            {
                if (newMapOrder[i].tiles.Length > 0)
                {
                    int firstTerrain = (int)newMapOrder[i].tiles[0,0];
                    bool singleTerrainMap = true;
                    
                    foreach (var terrain in newMapOrder[i].tiles)
                    {
                        if ((int)terrain != firstTerrain)
                        {
                            singleTerrainMap = false;
                            break;
                        }
                    }
                    if (singleTerrainMap)
                    {
                        if (keptOneSingleTerrainMap)
                        {
                            mapIndicesToSkip.Add(i);
                        }
                        else
                        {
                            keptOneSingleTerrainMap = true;
                        }
                    }
                }
            }

            // Fisher-Yates shuffle, skipping single-terrain maps (except the first one)
            for (int i = newMapOrder.Length - 1; i > 0; i--)
            {
                if (mapIndicesToSkip.Contains(i))
                {
                    continue;
                }
                int j;
                do
                {
                    j = RandomNumberGenerator.GetInt32(0, i + 1);
                } while (mapIndicesToSkip.Contains(j));
                // ReSharper disable once SwapViaDeconstruction
                DotrMap temp = newMapOrder[i];
                newMapOrder[i] = newMapOrder[j];
                newMapOrder[j] = temp;
            }
            Array.Copy(newMapOrder, DataAccess.Instance.maps, newMapOrder.Length);
        }
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
                    Dictionary<Terrain, Terrain> terrainSwapMap = new Dictionary<Terrain, Terrain>();
                    for (int i = 0; i < 10; i++)
                    {
                        Terrain originalTerrain = (Terrain)i;
                        Terrain newTerrain = GetRandomTerrain();
                        terrainSwapMap[originalTerrain] = newTerrain;
                    }

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
                EnsureTraversableMap(map);
            }
        }

        foreach (var treasureCard in TreasureCards.Instance.Treasures)
        {
            if (randomiseHiddenCardLocation)
            {
                treasureCard.Column = (byte)RandomNumberGenerator.GetInt32(0, 7);
                treasureCard.Row = (byte)RandomNumberGenerator.GetInt32(0, 7);
            }
            if (randomiseHiddenCardValue)
            {
                treasureCard.CardIndex = CreateRandomCard("").CardConstant.Index;
            }
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
                cardConstant.AppearsInSlotReels = RandomBoolIn100(slotChance);
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
                    else if (randomSPRange)
                    {
                        cardConstant.Level = (byte)GetRandomAroundTarget(cardConstant.Level, levelDelta);

                    }
                    else if (SpBasedOnPower)
                    {
                        cardConstant.Level = GetSpValue(cardConstant.Attack + cardConstant.Defense);
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
                if (randomiseEquip)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (i == 45 || i == 46 || i == 47 || i == 48 || i == 49)
                        {
                            continue;
                        }
                        MonsterEnchantData.MonsterEnchantDataList[cardConstant.Index].Flags[i] = RandomBoolIn100(EquipPercentChance);
                    }
                }
                if (randomiseStrongOnToon)
                {
                    MonsterEnchantData.MonsterEnchantDataList[cardConstant.Index].Flags[49] = RandomBoolIn100(strongOnToonChance);
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
                    if (i == 37)
                    {
                        continue;
                    }
                    EnchantData.EnchantScores[i] = (ushort)Math.Clamp(
                        Math.Round(GetRandomAroundTarget(EnchantData.EnchantScores[i], powerUpDelta) / 50.0) * 50,
                        50, 8000);
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

    byte GetSpValue(int combinedPower)
    {
        if (combinedPower < 0)
        {
            return 0;
        }
        if (combinedPower < 700)
        {
            return 1;
        }
        if (combinedPower < 1400)
        {
            return 2;
        }
        if (combinedPower < 2100)
        {
            return 3;
        }
        if (combinedPower < 2800)
        {
            return 4;
        }
        if (combinedPower < 3500)
        {
            return 5;
        }
        if (combinedPower < 4200)
        {
            return 6;
        }
        if (combinedPower < 4900)
        {
            return 7;
        }
        if (combinedPower < 5600)
        {
            return 8;
        }
        if (combinedPower < 6300)
        {
            return 9;
        }
        if (combinedPower < 7000)
        {
            return 10;
        }
        if (combinedPower <= 8000)
        {
            return 11;
        }
        return 12;
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


    //AI Code
    void EnsureTraversableMap(DotrMap dotrMap)
    {
        // Step 0: Ensure starting positions are not labyrinth
        var startPositions = new List<(int, int)> { (3, 0), (3, 6) };

        foreach (var startPos in startPositions)
        {
            if (IsValidTile(startPos) && dotrMap.tiles[startPos.Item1, startPos.Item2] == Terrain.Labyrinth)
            {
                Terrain terrainToUse = GetAdjacentNonLabyrinthTerrain(dotrMap, startPos);
                Console.WriteLine($"Converting starting position at ({startPos.Item1},{startPos.Item2}) from labyrinth to {terrainToUse}");
                dotrMap.tiles[startPos.Item1, startPos.Item2] = terrainToUse;
            }
        }

        // Step 1: Identify all non-labyrinth regions
        Dictionary<(int, int), int> tileToRegion = new Dictionary<(int, int), int>();
        int regionCount = 0;

        for (int i = 0; i < dotrMap.tiles.GetLength(0); i++)
        {
            for (int j = 0; j < dotrMap.tiles.GetLength(1); j++)
            {
                if (dotrMap.tiles[i, j] != Terrain.Labyrinth && !tileToRegion.ContainsKey((i, j)))
                {
                    // Found a new region, flood fill it
                    regionCount++;
                    Queue<(int, int)> queue = new Queue<(int, int)>();
                    queue.Enqueue((i, j));
                    tileToRegion[(i, j)] = regionCount;

                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();

                        var adjacentTiles = new[] {
                            (current.Item1, current.Item2 + 1),
                            (current.Item1 + 1, current.Item2),
                            (current.Item1, current.Item2 - 1),
                            (current.Item1 - 1, current.Item2)
                        };

                        foreach (var next in adjacentTiles)
                        {
                            if (IsValidTile(next) &&
                                dotrMap.tiles[next.Item1, next.Item2] != Terrain.Labyrinth &&
                                !tileToRegion.ContainsKey(next))
                            {
                                queue.Enqueue(next);
                                tileToRegion[next] = regionCount;
                            }
                        }
                    }
                }
            }
        }

        // If only one region, the map is already valid
        if (regionCount <= 1)
        {
            Console.WriteLine("Map is valid: All non-labyrinth tiles are connected");
            return;
        }

        Console.WriteLine($"Invalid: Map has {regionCount} disconnected regions");

        // Step 2: For each pair of regions, find the shortest path between them
        List<(int regionA, int regionB, List<(int, int)> path)> allPaths = new List<(int, int, List<(int, int)>)>();

        // Get a representative tile from each region
        Dictionary<int, (int, int)> regionToTile = new Dictionary<int, (int, int)>();
        foreach (var entry in tileToRegion)
        {
            var tile = entry.Key;
            int region = entry.Value;

            if (!regionToTile.ContainsKey(region))
            {
                regionToTile[region] = tile;
            }
        }

        // Find paths between all regions
        for (int r1 = 1; r1 <= regionCount; r1++)
        {
            for (int r2 = r1 + 1; r2 <= regionCount; r2++)
            {
                var source = regionToTile[r1];
                var target = regionToTile[r2];

                var path = FindShortestPathBFS(dotrMap, source, target);
                if (path != null)
                {
                    allPaths.Add((r1, r2, path));
                }
            }
        }

        // Step 3: Apply Kruskal's algorithm to find minimum spanning tree
        allPaths.Sort((a, b) => a.path.Count.CompareTo(b.path.Count));

        // Union-find data structure for MST
        Dictionary<int, int> parent = new Dictionary<int, int>();
        for (int i = 1; i <= regionCount; i++)
            parent[i] = i;

        // Find function for union-find
        Func<int, int> find = null;
        find = r =>
        {
            if (parent[r] != r)
                parent[r] = find(parent[r]);
            return parent[r];
        };

        // Apply MST (Kruskal's)
        int tilesConverted = 0;

        foreach (var conn in allPaths)
        {
            int rootA = find(conn.regionA);
            int rootB = find(conn.regionB);

            if (rootA != rootB)
            {
                // Union
                parent[rootB] = rootA;

                // Convert labyrinth tiles in the path
                foreach (var tile in conn.path)
                {
                    if (dotrMap.tiles[tile.Item1, tile.Item2] == Terrain.Labyrinth)
                    {
                        // Use terrain from one of the connected regions
                        Terrain terrainToUse;
                        var representativeTile = regionToTile[conn.regionA];
                        terrainToUse = dotrMap.tiles[representativeTile.Item1, representativeTile.Item2];

                        dotrMap.tiles[tile.Item1, tile.Item2] = terrainToUse;
                        tilesConverted++;
                    }
                }
            }
        }

        Console.WriteLine($"Map fixed: Converted {tilesConverted} labyrinth tiles to connect all regions");

        // Verify the map is now fully connected
        Dictionary<(int, int), bool> visited = new Dictionary<(int, int), bool>();
        Queue<(int, int)> verificationQueue = new Queue<(int, int)>();

        // Find the first non-labyrinth tile
        (int, int)? firstTile = null;
        for (int i = 0; i < dotrMap.tiles.GetLength(0) && firstTile == null; i++)
        {
            for (int j = 0; j < dotrMap.tiles.GetLength(1) && firstTile == null; j++)
            {
                if (dotrMap.tiles[i, j] != Terrain.Labyrinth)
                {
                    firstTile = (i, j);
                }
            }
        }

        if (firstTile != null)
        {
            verificationQueue.Enqueue(firstTile.Value);
            visited[firstTile.Value] = true;

            while (verificationQueue.Count > 0)
            {
                var current = verificationQueue.Dequeue();

                var adjacentTiles = new[] {
                    (current.Item1, current.Item2 + 1),
                    (current.Item1 + 1, current.Item2),
                    (current.Item1, current.Item2 - 1),
                    (current.Item1 - 1, current.Item2)
                };

                foreach (var next in adjacentTiles)
                {
                    if (IsValidTile(next) &&
                        dotrMap.tiles[next.Item1, next.Item2] != Terrain.Labyrinth &&
                        !visited.ContainsKey(next))
                    {
                        verificationQueue.Enqueue(next);
                        visited[next] = true;
                    }
                }
            }

            // Check if any non-labyrinth tiles weren't visited
            for (int i = 0; i < dotrMap.tiles.GetLength(0); i++)
            {
                for (int j = 0; j < dotrMap.tiles.GetLength(1); j++)
                {
                    if (dotrMap.tiles[i, j] != Terrain.Labyrinth && !visited.ContainsKey((i, j)))
                    {
                        // We found a disconnected tile - create a direct path to it
                        Console.WriteLine($"Found unreachable tile at ({i},{j}) even after MST, forcing connection");

                        // Connect to the nearest visited tile with a straight path
                        (int, int) nearestVisited = FindNearestVisitedTile(visited, (i, j));
                        ConnectTiles(dotrMap, nearestVisited, (i, j));
                    }
                }
            }
        }
    }

// Helper method to find shortest path using BFS
    List<(int, int)> FindShortestPathBFS(DotrMap dotrMap, (int, int) start, (int, int) end)
    {
        Queue<(int, int)> queue = new Queue<(int, int)>();
        Dictionary<(int, int), (int, int)> cameFrom = new Dictionary<(int, int), (int, int)>();

        queue.Enqueue(start);
        cameFrom[start] = (-1, -1); // Special marker for start

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Equals(end))
            {
                // Found path, reconstruct it
                List<(int, int)> path = new List<(int, int)>();
                var curr = current;

                while (!curr.Equals(start))
                {
                    path.Add(curr);
                    curr = cameFrom[curr];
                }

                path.Reverse();
                return path;
            }

            var adjacentTiles = new[] {
                (current.Item1, current.Item2 + 1),
                (current.Item1 + 1, current.Item2),
                (current.Item1, current.Item2 - 1),
                (current.Item1 - 1, current.Item2)
            };

            foreach (var next in adjacentTiles)
            {
                if (IsValidTile(next) && !cameFrom.ContainsKey(next))
                {
                    queue.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        return null; // No path found
    }

// Helper method to find nearest visited tile
    (int, int) FindNearestVisitedTile(Dictionary<(int, int), bool> visited, (int, int) target)
    {
        (int, int) nearest = (-1, -1);
        int minDistance = int.MaxValue;

        foreach (var tile in visited.Keys)
        {
            int distance = Math.Abs(tile.Item1 - target.Item1) + Math.Abs(tile.Item2 - target.Item2);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = tile;
            }
        }

        return nearest;
    }

// Helper method to connect two tiles with a straight path
    void ConnectTiles(DotrMap dotrMap, (int, int) start, (int, int) end)
    {
        // Get terrain type from the end tile
        Terrain terrainToUse = dotrMap.tiles[end.Item1, end.Item2];

        // Draw a simple L-shaped path
        // First horizontal
        int minX = Math.Min(start.Item1, end.Item1);
        int maxX = Math.Max(start.Item1, end.Item1);

        for (int x = minX; x <= maxX; x++)
        {
            if (dotrMap.tiles[x, start.Item2] == Terrain.Labyrinth)
            {
                dotrMap.tiles[x, start.Item2] = terrainToUse;
            }
        }

        // Then vertical
        int minY = Math.Min(start.Item2, end.Item2);
        int maxY = Math.Max(start.Item2, end.Item2);

        for (int y = minY; y <= maxY; y++)
        {
            if (dotrMap.tiles[end.Item1, y] == Terrain.Labyrinth)
            {
                dotrMap.tiles[end.Item1, y] = terrainToUse;
            }
        }
    }

    Terrain GetAdjacentNonLabyrinthTerrain(DotrMap dotrMap, (int, int) tile)
    {
        var adjacentTiles = new[] {
            (tile.Item1, tile.Item2 + 1),
            (tile.Item1 + 1, tile.Item2),
            (tile.Item1, tile.Item2 - 1),
            (tile.Item1 - 1, tile.Item2)
        };

        // Collect all adjacent non-labyrinth terrain types
        List<Terrain> availableTerrains = new List<Terrain>();

        foreach (var adj in adjacentTiles)
        {
            if (IsValidTile(adj) && dotrMap.tiles[adj.Item1, adj.Item2] != Terrain.Labyrinth)
            {
                availableTerrains.Add(dotrMap.tiles[adj.Item1, adj.Item2]);
            }
        }

        // If no adjacent non-labyrinth terrain, default to normal
        if (availableTerrains.Count == 0)
            return Terrain.Normal;

        // Pick a random terrain from available options
        Random random = new Random();
        return availableTerrains[random.Next(availableTerrains.Count)];
    }

    bool IsValidTile((int, int) tile)
    {
        return tile.Item1 >= 0 && tile.Item1 <= 6 && tile.Item2 >= 0 && tile.Item2 <= 6;
    }
}