using System.Runtime.Serialization.Formatters.Binary;
namespace DotrModdingTool2IMGUI;

using System;

public class DeckLeaderAbilityInstance
{
    public int CardId;
    public DeckLeaderAbility[] Abilities;

    public DeckLeaderAbilityInstance(int cardId, DeckLeaderAbility[] abilities)
    {
        CardId = cardId;
        Abilities = abilities;
    }
}

public class DeckLeaderAbility
{
    public static readonly ushort DisabledBytesValue = 0xFFFF;

    public byte[] Bytes { get; set; }
    public int AbilityIndex { get; }
    public DeckLeaderAbilityType AbilityType { get; }
    public string Name { get; }
    public string Description { get; }
    bool enabled;

    int rankRequired;

    public int RankRequired
    {
        get => rankRequired;
        set
        {
            rankRequired = value;
            if (IsRankLowerByte((DeckLeaderAbilityType)AbilityIndex))
            {
                Bytes[1] = (byte)rankRequired;
            }
            else
            {
                Bytes[0] = (byte)rankRequired;
            }

        }
    }


    public DeckLeaderAbility(int abilityIndex, byte[] bytes)
    {
        Bytes = bytes;
        AbilityIndex = abilityIndex;
        AbilityType = (DeckLeaderAbilityType)this.AbilityIndex;
        Name = DeckLeaderAbilityInfo.NameAndDescriptions[abilityIndex][0];
        Description = DeckLeaderAbilityInfo.NameAndDescriptions[abilityIndex][1];
        enabled = BitConverter.ToUInt16(this.Bytes, 0) != DisabledBytesValue;
        if (enabled)
        {
            if (IsRankLowerByte((DeckLeaderAbilityType)abilityIndex))
            {
                rankRequired = bytes[1];
            }
            else
            {
                rankRequired = bytes[0];
            }
        }
        else
        {
            rankRequired = DisabledBytesValue;
        }

    }

    public override string ToString()
    {
        return Name;
    }

    public bool IsEnabled => enabled;

    public void ToggleEnabled()
    {
        enabled = !enabled;
        if (!enabled)
        {
            RankRequired = DisabledBytesValue;
        }
        else
        {
            RankRequired = 12;
        }
    }

    public void SetEnabled(bool enable)
    {
        enabled = enable;
        if (enable)
        {
            RankRequired = 12;
        }
        else
        {
            RankRequired = DisabledBytesValue;
        }
    }


    public static bool IsRankLowerByte(DeckLeaderAbilityType ability)
    {
        switch (ability)
        {
            case DeckLeaderAbilityType.HiddenCard:
            case DeckLeaderAbilityType.ExtraSlots:
            case DeckLeaderAbilityType.TerrainChange:
            case DeckLeaderAbilityType.LevelCostReduction:
            case DeckLeaderAbilityType.FriendlyIncreasedStrength:
            case DeckLeaderAbilityType.WeakenSpecificEnemyType:
                return false;
            case DeckLeaderAbilityType.DestinyDraw:
            case DeckLeaderAbilityType.LPRecovery:
            case DeckLeaderAbilityType.IncreasedMovement:
            case DeckLeaderAbilityType.DirectDamageHalved:
            case DeckLeaderAbilityType.ExtendedSupportRange:
            case DeckLeaderAbilityType.FriendlyImprovedResistance:
            case DeckLeaderAbilityType.OpenCard:
            case DeckLeaderAbilityType.FriendlyMovementBoost:
            case DeckLeaderAbilityType.SpellbindSpecificEnemyType:
            case DeckLeaderAbilityType.DestroySpecificEnemyType:
                return true;
            default:
                return false;

        }
    }
}