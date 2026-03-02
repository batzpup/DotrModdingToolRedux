namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

public class CustomPatchSnapshot
{
    public bool bAiDoubleTap;
    public bool bFastIntro;
    public bool bUserToggledFastIntro = false;
    public bool bUnlockFusions;
    public bool bCameraFix;
    public bool bExpandedZoom;
    public bool bRemoveExpLoss;
    public bool bRemoveSlotRng;
    public bool bAllCustomDuels;
    public bool bKeepReincarnatedCard;
    public bool bMaxCardLimitInDeck;
    public bool bToonLeaderLandChange;
    public bool bAllKindsExtraSlots;
    public bool bSaveMusic;
    public int CurrentRule;
    public int[] rankExp;
    public int[] SpecialThreeInARows;

    public int[] SpecialSlotRewards;

    //Ai patches:
    public bool bFixDarkHole;
    public bool bMaiFeatherDuster;
    public bool bFixGetMaxApList;
    public bool bMakoHeavyStorm;
    public bool bStopPegasusFusion;
    public bool bDmkFixRevive;
    public bool bDontReviveEquips;
    public bool bGiveJoeyReviveMission;
    public bool bYugiRaigeki;
    public bool bTeaInsectImitation;

    //Value patches:
    public int currentSideToGoFirst;
    public bool bSideToGoFirst;
    public bool bForceNewStartSide;
    public bool bLpCap;
    public bool bReincarnationCount;
    public bool bTerrainBuff;
    public bool bDeckLeaderRecovery;

    public bool bStartingLpRed;
    public bool bSpRecoveryRed;
    public bool bStartingSpRed;

    public int startingSpRed;
    public int startingLpRed;
    public int spRecoveryRed;

    public bool bStartingLpWhite;
    public bool bSpRecoveryWhite;
    public bool bStartingSpWhite;
    public int startingSpWhite;
    public int startingLpWhite;
    public int spRecoveryWhite;

    public int forceSideIndex;
    public int lpCap;
    public int reincarnationCount;
    public int terrainBuffAmount;
    public int leaderRecovery;
    public int maxCardInDeck;

    public CustomPatchSnapshot(GameplayPatchesWindow patches)
    {
        // General patches
        bAiDoubleTap = patches.bAiDoubleTap;
        bFastIntro = patches.bFastIntro;
        bUserToggledFastIntro = patches.bUserToggledFastIntro;
        bUnlockFusions = patches.bUnlockFusions;
        bCameraFix = patches.bCameraFix;
        bExpandedZoom = patches.bExpandedZoom;
        bRemoveExpLoss = patches.bRemoveExpLoss;
        bRemoveSlotRng = patches.bRemoveSlotRng;
        bAllCustomDuels = patches.bAllCustomDuels;
        bKeepReincarnatedCard = patches.bKeepReincarnatedCard;
        
        bToonLeaderLandChange = patches.bToonLeaderLandChange;
        bAllKindsExtraSlots = patches.bAllKindsExtraSlots;
        bSaveMusic = patches.bSaveMusic;
        CurrentRule = patches.CurrentRule;

        // Arrays
        rankExp = CloneIntArray(GameplayPatchesWindow.rankExp);
        SpecialThreeInARows = CloneIntArray(patches.SpecialThreeInARows);
        SpecialSlotRewards = CloneIntArray(patches.SpecialSlotRewards);

        // AI patches
        bFixDarkHole = patches.bFixDarkHole;
        bMaiFeatherDuster = patches.bMaiFeatherDuster;
        bFixGetMaxApList = patches.bFixGetMaxApList;
        bMakoHeavyStorm = patches.bMakoHeavyStorm;
        bStopPegasusFusion = patches.bStopPegasusFusion;
        bDmkFixRevive = patches.bDmkFixRevive;
        bDontReviveEquips = patches.bDontReviveEquips;
        bGiveJoeyReviveMission = patches.bGiveJoeyReviveMission;
        bYugiRaigeki = patches.bYugiRaigeki;
        bTeaInsectImitation = patches.bTeaInsectImitation;

        // Value patches
        currentSideToGoFirst = patches.currentSideToGoFirst;
        bSideToGoFirst = patches.bSideToGoFirst;
        bForceNewStartSide = patches.bForceNewStartSide;
        bLpCap = patches.bChangeLpCap;
        bReincarnationCount = patches.bReincarnationCount;
        bTerrainBuff = patches.bTerrainBuff;
        bDeckLeaderRecovery = patches.bDeckLeaderRecovery;

        bStartingLpRed = patches.bStartingLpRed;
        bSpRecoveryRed = patches.bSpRecoveryRed;
        bStartingSpRed = patches.bStartingSpRed;

        bMaxCardLimitInDeck = patches.bMaxCardLimitInDeck;
        
        startingSpRed = patches.startingSpRed;
        startingLpRed = patches.startingLpRed;
        spRecoveryRed = patches.spRecoveryRed;

        bStartingLpWhite = patches.bStartingLpWhite;
        bSpRecoveryWhite = patches.bSpRecoveryWhite;
        bStartingSpWhite = patches.bStartingSpWhite;

        startingSpWhite = patches.startingSpWhite;
        startingLpWhite = patches.startingLpWhite;
        spRecoveryWhite = patches.spRecoveryWhite;

        forceSideIndex = patches.forceSideIndex;
        lpCap = patches.lpCap;
        reincarnationCount = patches.reincarnationCount;
        terrainBuffAmount = patches.terrainBuffAmount;
        leaderRecovery = patches.leaderRecovery;
        maxCardInDeck = patches.maxCardLimitInDeck;
    }

    int[] CloneIntArray(int[] source)
    {
        return (int[])source.Clone();
    }
}

public class CustomPatchDiff : IDiffChecker<CustomPatchSnapshot>
{
    public DiffResult CompareSnapshots(CustomPatchSnapshot oldSnapshot, CustomPatchSnapshot currentSnapshot)
    {
        DiffResult result = new DiffResult { Name = "Custom Patches" };
        List<string> diffs = new();
        List<string> normalDiffs = new();
        ChangelogManager.Check("  AI Double Tap", oldSnapshot.bAiDoubleTap, currentSnapshot.bAiDoubleTap, normalDiffs);
        ChangelogManager.Check("  Fast Intro", oldSnapshot.bFastIntro, currentSnapshot.bFastIntro, normalDiffs);
        ChangelogManager.Check("  User Toggled Fast Intro", oldSnapshot.bUserToggledFastIntro, currentSnapshot.bUserToggledFastIntro, normalDiffs);
        ChangelogManager.Check("  Unlock Fusions", oldSnapshot.bUnlockFusions, currentSnapshot.bUnlockFusions, normalDiffs);
        ChangelogManager.Check("  Camera Fix", oldSnapshot.bCameraFix, currentSnapshot.bCameraFix, normalDiffs);
        ChangelogManager.Check("  Expanded Zoom", oldSnapshot.bExpandedZoom, currentSnapshot.bExpandedZoom, normalDiffs);
        ChangelogManager.Check("  Remove EXP Loss", oldSnapshot.bRemoveExpLoss, currentSnapshot.bRemoveExpLoss, normalDiffs);
        ChangelogManager.Check("  Remove Slot RNG", oldSnapshot.bRemoveSlotRng, currentSnapshot.bRemoveSlotRng, normalDiffs);
        ChangelogManager.Check("  All Custom Duels", oldSnapshot.bAllCustomDuels, currentSnapshot.bAllCustomDuels, normalDiffs);
        ChangelogManager.Check("  Keep Reincarnated Card", oldSnapshot.bKeepReincarnatedCard, currentSnapshot.bKeepReincarnatedCard, normalDiffs);
        ChangelogManager.Check("  Nine Card Limit", oldSnapshot.bMaxCardLimitInDeck, currentSnapshot.bMaxCardLimitInDeck, normalDiffs);
        ChangelogManager.Check("  Toon Leader Land Change", oldSnapshot.bToonLeaderLandChange, currentSnapshot.bToonLeaderLandChange, normalDiffs);
        ChangelogManager.Check("  All Kinds Extra Slots", oldSnapshot.bAllKindsExtraSlots, currentSnapshot.bAllKindsExtraSlots, normalDiffs);
        ChangelogManager.Check("  Save Music", oldSnapshot.bSaveMusic, currentSnapshot.bSaveMusic, normalDiffs);
        ChangelogManager.Check("  Current DC Rule", GameplayPatchesWindow.RuleList[oldSnapshot.CurrentRule], GameplayPatchesWindow.RuleList[currentSnapshot.CurrentRule], normalDiffs);
        ChangelogManager.Check("  Force side first", oldSnapshot.bSideToGoFirst, currentSnapshot.bSideToGoFirst, normalDiffs);
        ChangelogManager.Check("  Side to go first", GameplayPatchesWindow.sideStrings[oldSnapshot.currentSideToGoFirst], GameplayPatchesWindow.sideStrings[currentSnapshot.currentSideToGoFirst], normalDiffs);
        ChangelogManager.Check("  new game force side", oldSnapshot.bForceNewStartSide, currentSnapshot.bForceNewStartSide, normalDiffs);
        ChangelogManager.Check("  New game side", GameplayPatchesWindow.sideStrings[oldSnapshot.forceSideIndex], GameplayPatchesWindow.sideStrings[currentSnapshot.forceSideIndex], normalDiffs);

        ChangelogManager.Check("  Change LP cap", oldSnapshot.bLpCap, currentSnapshot.bLpCap, normalDiffs);
        ChangelogManager.Check("  LP cap", oldSnapshot.lpCap, currentSnapshot.lpCap, normalDiffs);

        ChangelogManager.Check("  Change reincarnation amount", oldSnapshot.bReincarnationCount, currentSnapshot.bReincarnationCount, normalDiffs);
        ChangelogManager.Check("  reincarnation amount", oldSnapshot.reincarnationCount, currentSnapshot.reincarnationCount, normalDiffs);

        ChangelogManager.Check("  Change terrain buff", oldSnapshot.bTerrainBuff, currentSnapshot.bTerrainBuff, normalDiffs);
        ChangelogManager.Check("  Terrain buff amount", oldSnapshot.terrainBuffAmount, currentSnapshot.terrainBuffAmount, normalDiffs);

        ChangelogManager.Check("  Change DL Recovery", oldSnapshot.bDeckLeaderRecovery, currentSnapshot.bDeckLeaderRecovery, normalDiffs);
        ChangelogManager.Check("  DL recovery amount", oldSnapshot.leaderRecovery, currentSnapshot.leaderRecovery, normalDiffs);

        ChangelogManager.Check("  Change red starting Lp ", oldSnapshot.bStartingLpRed, currentSnapshot.bStartingLpRed, normalDiffs);
        ChangelogManager.Check("  Red starting lp", oldSnapshot.startingLpRed, currentSnapshot.startingLpRed, normalDiffs);

        ChangelogManager.Check("  Change white starting Lp ", oldSnapshot.bStartingLpWhite, currentSnapshot.bStartingLpWhite, normalDiffs);
        ChangelogManager.Check("  White starting lp", oldSnapshot.startingLpWhite, currentSnapshot.startingLpWhite, normalDiffs);

        ChangelogManager.Check("  Change red starting SP", oldSnapshot.bStartingSpRed, currentSnapshot.bStartingSpRed, normalDiffs);
        ChangelogManager.Check("  Red starting SP", oldSnapshot.startingSpRed, currentSnapshot.startingSpRed, normalDiffs);

        ChangelogManager.Check("  Change white starting SP", oldSnapshot.bStartingSpWhite, currentSnapshot.bStartingSpWhite, normalDiffs);
        ChangelogManager.Check("  White starting SP", oldSnapshot.startingSpWhite, currentSnapshot.startingSpWhite, normalDiffs);

        ChangelogManager.Check("  Change red recovery SP", oldSnapshot.bSpRecoveryRed, currentSnapshot.bSpRecoveryRed, normalDiffs);
        ChangelogManager.Check("  Red recovery SP", oldSnapshot.spRecoveryRed, currentSnapshot.spRecoveryRed, normalDiffs);

        ChangelogManager.Check("  Change white recovery SP", oldSnapshot.bSpRecoveryWhite, currentSnapshot.bSpRecoveryWhite, normalDiffs);
        ChangelogManager.Check("  White starting SP", oldSnapshot.spRecoveryWhite, currentSnapshot.spRecoveryWhite, normalDiffs);
        
        ChangelogManager.Check("  Max card limit per deck", oldSnapshot.maxCardInDeck, currentSnapshot.maxCardInDeck, normalDiffs);
        
        if (normalDiffs.Count > 0)
        {
            diffs.Add("Basic patches");
            diffs.AddRange(normalDiffs);
            diffs.Add("\n");
        }
        //Ai patches
        List<string> AiDiffs = new();
        ChangelogManager.Check("  Fix Darkhole", oldSnapshot.bFixDarkHole, currentSnapshot.bFixDarkHole, AiDiffs);
        ChangelogManager.Check("  Mai Feather Duster", oldSnapshot.bMaiFeatherDuster, currentSnapshot.bMaiFeatherDuster, AiDiffs);
        ChangelogManager.Check("  Fix Get Max AP List", oldSnapshot.bFixGetMaxApList, currentSnapshot.bFixGetMaxApList, AiDiffs);
        ChangelogManager.Check("  Mako Heavy Storm", oldSnapshot.bMakoHeavyStorm, currentSnapshot.bMakoHeavyStorm, AiDiffs);
        ChangelogManager.Check("  Stop Pegasus Fusion", oldSnapshot.bStopPegasusFusion, currentSnapshot.bStopPegasusFusion, AiDiffs);
        ChangelogManager.Check("  DMK Fix Revive", oldSnapshot.bDmkFixRevive, currentSnapshot.bDmkFixRevive, AiDiffs);
        ChangelogManager.Check("  Don't Revive Equips", oldSnapshot.bDontReviveEquips, currentSnapshot.bDontReviveEquips, AiDiffs);
        ChangelogManager.Check("  Give Joey Revive Mission", oldSnapshot.bGiveJoeyReviveMission, currentSnapshot.bGiveJoeyReviveMission, AiDiffs);
        ChangelogManager.Check("  Yugi Raigeki", oldSnapshot.bYugiRaigeki, currentSnapshot.bYugiRaigeki, AiDiffs);
        ChangelogManager.Check("  Tea Insect Imitation", oldSnapshot.bTeaInsectImitation, currentSnapshot.bTeaInsectImitation, AiDiffs);
        if (AiDiffs.Count > 0)
        {
            diffs.Add("AI patches");
            diffs.AddRange(AiDiffs);
            diffs.Add("\n");
        }



        if (!oldSnapshot.rankExp.SequenceEqual(currentSnapshot.rankExp))
        {
            List<string> rankDiffs = new();
            for (int i = 0; i < oldSnapshot.rankExp.Length; i++)
            {
                if (oldSnapshot.rankExp[i] != currentSnapshot.rankExp[i])
                    rankDiffs.Add($"  Rank {(DeckLeaderRank)(i + 1)}: {oldSnapshot.rankExp[i]} → {currentSnapshot.rankExp[i]}");
            }
            if (rankDiffs.Count > 0)
            {
                diffs.Add("Rank Exp Required");
                diffs.AddRange(rankDiffs);
                diffs.Add("\n");
            }
        }

        List<string> specialSlotDiffs = new();
        if (!oldSnapshot.SpecialThreeInARows.SequenceEqual(currentSnapshot.SpecialThreeInARows) || oldSnapshot.SpecialSlotRewards.SequenceEqual(currentSnapshot.SpecialSlotRewards))
        {
            for (int i = 0; i < oldSnapshot.SpecialThreeInARows.Length; i++)
            {
                if (oldSnapshot.SpecialThreeInARows[i] != currentSnapshot.SpecialThreeInARows[i] || oldSnapshot.SpecialSlotRewards[i] != currentSnapshot.SpecialSlotRewards[i])
                {
                    specialSlotDiffs.Add($"  Special Rare {i}:");
                    if (oldSnapshot.SpecialThreeInARows[i] == 0 && oldSnapshot.SpecialSlotRewards[i] == 673)
                    {
                        specialSlotDiffs.Add($"    Added Target: {Card.cardNameList[currentSnapshot.SpecialThreeInARows[i]]}");
                        specialSlotDiffs.Add($"    Added Reward: {Card.cardNameList[currentSnapshot.SpecialSlotRewards[i]]}");
                    }
                    else
                    {
                        specialSlotDiffs.Add($"    Special Rare Target: {Card.cardNameList[oldSnapshot.SpecialThreeInARows[i]]} → {Card.cardNameList[currentSnapshot.SpecialThreeInARows[i]]}");
                        specialSlotDiffs.Add($"    Special Rare Reward: {Card.cardNameList[oldSnapshot.SpecialSlotRewards[i]]} → {Card.cardNameList[currentSnapshot.SpecialSlotRewards[i]]}");
                    }
                    if (i != oldSnapshot.SpecialThreeInARows.Length - 1)
                    {
                        specialSlotDiffs.Add("");
                    }

                }
            }
            if (specialSlotDiffs.Count > 0)
            {
                diffs.Add("Special Rares");
                diffs.AddRange(specialSlotDiffs);
            }

        }


        if (diffs.Count > 0)
        {
            foreach (var diff in diffs)
            {
                result.Add("", diff);
            }
        }
        return result;
    }
}