using System.Collections;
using System.Diagnostics;
namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

public class CardDataDiffChecker : IDiffChecker<CardConstSnapshot>
{
    public DiffResult CompareSnapshots(CardConstSnapshot oldSnapshot, CardConstSnapshot currentSnapshot)
    {
        DiffResult result = new DiffResult { Name = "Card Constant Changes" };
        for (var i = 0; i < oldSnapshot.CardConstants.Count; i++)
        {
            CardConstant oldCard = oldSnapshot.CardConstants[i];
            CardConstant currentCard = currentSnapshot.CardConstants[i];
            string title = $"{oldCard.Name.Current}:";
            List<string> diffs = new List<string>();

            if (oldSnapshot.Names[i].Current != currentSnapshot.Names[i].Current)
            {
                diffs.Add("Name changes:");
                diffs.Add($"  original name: {oldSnapshot.Names[i].Default}");
                diffs.Add($"  name: {oldSnapshot.Names[i].Current} -> {currentSnapshot.Names[i].Current}");
            }
            if (!oldCard.Bytes.SequenceEqual(currentCard.Bytes))
            {
                List<string> basicDiffs = new();
                ChangelogManager.Check("  Attack:", oldCard.Attack, currentCard.Attack, basicDiffs);
                ChangelogManager.Check("  Defense:", oldCard.Defense, currentCard.Defense, basicDiffs);
                ChangelogManager.Check("  Attribute:", oldCard.AttributeName, currentCard.AttributeName, basicDiffs);
                ChangelogManager.Check("  Kind:", oldCard.Type, currentCard.Type, basicDiffs);
                ChangelogManager.Check("  Level:", oldCard.Level, currentCard.Level, basicDiffs);
                ChangelogManager.Check("  Deck Cost:", oldCard.DeckCost, currentCard.DeckCost, basicDiffs);
                if (basicDiffs.Count > 0)
                {
                    diffs.Add("Basic Stats:");
                    diffs.AddRange(basicDiffs);
                }


                List<string> acqDiffs = new();
                ChangelogManager.Check("  Graveyard:", oldCard.AppearsInSlotReels, currentCard.AppearsInSlotReels, acqDiffs);
                ChangelogManager.Check("  Slot Rare:", oldCard.IsRareDrop, currentCard.IsRareDrop, acqDiffs);
                ChangelogManager.Check("  Reincarnation:", oldCard.AppearsInReincarnation, currentCard.AppearsInReincarnation, acqDiffs);
                ChangelogManager.Check("  Enable Password:", oldCard.PasswordWorks, currentCard.PasswordWorks, acqDiffs);
                ChangelogManager.Check("  Password:", oldCard.Password, currentCard.Password, acqDiffs);
                if (acqDiffs.Count > 0)
                {
                    diffs.Add("Acquisition:");
                    diffs.AddRange(acqDiffs);
                }
            }



            List<string> leaderDiffs = new();
            if (oldCard.Index < 683)
            {
                DeckLeaderAbilityInstance oldAbilities = oldSnapshot.LeaderAbilities[oldCard.Index];
                DeckLeaderAbilityInstance newAbilities = currentSnapshot.LeaderAbilities[oldCard.Index];

                for (int lb = 0; lb < DataAccess.CardLeaderAbilityTypeCount; lb++)
                {
                    DeckLeaderAbility oldAbility = oldAbilities.Abilities[lb];
                    DeckLeaderAbility currentAbility = newAbilities.Abilities[lb];
                    if (!oldAbility.Bytes.SequenceEqual(currentAbility.Bytes))
                    {
                        ChangelogManager.Check($"  {oldAbility.Name} :", oldAbility.IsEnabled, currentAbility.IsEnabled, leaderDiffs);
                        if (currentAbility.RankRequired != 65535)
                        {
                            ChangelogManager.Check($"  {oldAbility.Name} rank :", (DeckLeaderRank)oldAbility.RankRequired, (DeckLeaderRank)currentAbility.RankRequired, leaderDiffs);
                        }

                    }
                }
            }
            if (leaderDiffs.Count > 0)
            {
                diffs.Add("Leader Abilities:");
                diffs.AddRange(leaderDiffs);
            }

            List<string> equipDiffs = new();
            if (oldCard.Index < 683)
            {
                MonsterEnchantData oldEnchants = oldSnapshot.MonsterEnchants[i];
                MonsterEnchantData newEnchants = currentSnapshot.MonsterEnchants[i];

                for (var e = 0; e < oldEnchants.Flags.Count; e++)
                {
                    if (e == 47 || e == 48 || e == 49)
                        continue;

                    ChangelogManager.Check($"  {oldEnchants.GetEquipName(e)}", oldEnchants.Flags[e], newEnchants.Flags[e], equipDiffs);
                }
            }
            if (equipDiffs.Count > 0)
            {
                diffs.Add("Equips:");
                diffs.AddRange(equipDiffs);
            }


            List<string> effectDiffs = new();
            if (oldCard.Index < Card.EquipCardStartIndex || oldCard.Index > Card.EquipCardEndIndex)
            {


                if (oldCard.Index < 683)
                {
                    ChangelogManager.Check("  Effect Type:", oldCard.CardColor, currentCard.CardColor, effectDiffs);

                    string oldEffectName;
                    if (oldCard.EffectId == 65535)
                    {
                        oldEffectName = "No Effect";
                    }
                    else
                    {
                        oldEffectName = Effects.MonsterEffectOwners.ElementAt(oldCard.EffectId).Value.Current;
                    }

                    string currentEffectName;
                    if (currentCard.EffectId == 65535)
                    {
                        currentEffectName = "No Effect";
                    }
                    else
                    {
                        currentEffectName = Effects.MonsterEffectOwners.ElementAt(currentCard.EffectId).Value.Current;
                    }
                    ChangelogManager.Check("  Effect ID:", oldEffectName, currentEffectName, effectDiffs);

                    ChangelogManager.Check("  Toon:", oldSnapshot.MonsterEnchants[i].Flags[49], currentSnapshot.MonsterEnchants[i].Flags[49], effectDiffs
                    );
                }
                else
                {
                    ChangelogManager.Check("  Effect Type:", oldCard.CardColor, currentCard.CardColor, effectDiffs);
                    ChangelogManager.Check("  Effect ID:", Effects.NonMonsterOwners.ElementAt(oldCard.EffectId).Value.Current, Effects.NonMonsterOwners.ElementAt(currentCard.EffectId).Value.Current,effectDiffs);
                }
            }
            else
            {
                ChangelogManager.Check($"  {Card.GetNameByIndex(oldCard.Index)} Id:",
                    oldSnapshot.EnchantIds[oldCard.Index - Card.EquipCardStartIndex],
                    currentSnapshot.EnchantIds[oldCard.Index - Card.EquipCardStartIndex],
                    effectDiffs);

                ChangelogManager.Check($"  {Card.GetNameByIndex(oldCard.Index)} Score:",
                    EnchantData.GetEnchantScoreName(oldSnapshot.EnchantScores[oldCard.Index - Card.EquipCardStartIndex]),
                    EnchantData.GetEnchantScoreName(currentSnapshot.EnchantScores[oldCard.Index - Card.EquipCardStartIndex]),
                    effectDiffs);
            }
            if (effectDiffs.Count > 0)
            {
                diffs.Add("Effects / Enchant Data:");
                diffs.AddRange(effectDiffs);
            }


            if (diffs.Count > 0)
            {
                foreach (var diff in diffs)
                    result.Add(title, diff);
            }
        }


        return result;
    }
}

public class CardConstSnapshot
{
    CardConstant CloneCardConst(CardConstant original)
    {
        return new CardConstant(original.Index, original.Bytes);
    }

    MonsterEnchantData CloneMonsterEnchant(MonsterEnchantData original)
    {
        return new MonsterEnchantData(new BitArray(original.Flags.ToByteArray()));
    }

    DeckLeaderAbility CloneDeckLeaderAbility(DeckLeaderAbility original)
    {
        //Because its just references internally
        return new DeckLeaderAbility(original.AbilityIndex, (byte[])original.Bytes.Clone());
    }

    DeckLeaderAbilityInstance CloneDeckLeaderAbilityInstance(DeckLeaderAbilityInstance original)
    {
        return new DeckLeaderAbilityInstance(original.CardId, original.Abilities.Select(la => CloneDeckLeaderAbility(la)).ToArray());
    }

    ModdedStringName CloneModdedString(ModdedStringName original)
    {
        return new ModdedStringName(original.Default, original.Edited);
    }


    public List<CardConstant> CardConstants;
    public List<DeckLeaderAbilityInstance> LeaderAbilities;
    public List<MonsterEnchantData> MonsterEnchants;
    public List<ModdedStringName> Names;
    public List<byte> EnchantIds;
    public List<ushort> EnchantScores;

    public CardConstSnapshot(List<CardConstant> consts, List<DeckLeaderAbilityInstance> abilities, List<MonsterEnchantData> monsterEnchant, List<byte> enchIds, List<ushort> enchScores, ModdedStringName[] moddedNames)
    {
        CardConstants = consts.Select(c => CloneCardConst(c)).ToList();
        LeaderAbilities = abilities.Select(la => CloneDeckLeaderAbilityInstance(la)).ToList();
        MonsterEnchants = monsterEnchant.Select(c => CloneMonsterEnchant(c)).ToList();
        EnchantIds = new List<byte>(enchIds);
        EnchantScores = new List<ushort>(enchScores);
        Names = moddedNames.Select(c => CloneModdedString(c)).ToList();
    }
}