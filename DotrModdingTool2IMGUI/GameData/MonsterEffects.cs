namespace DotrModdingTool2IMGUI;

public static class Effects
{
    public static List<MonsterEffects> MonsterEffectsList = new List<MonsterEffects>();
    public static List<Effect> MagicEffectsList = new List<Effect>();

    public static byte[] MonsterEffectBytes
    {
        get { return MonsterEffectsList.SelectMany(a => a.Effects.SelectMany(b => b.Bytes)).ToArray(); }
    }

    public static byte[] MagicEffectBytes
    {
        get { return MagicEffectsList.SelectMany(a => a.Bytes).ToArray(); }
    }
}

public class MonsterEffects
{
    public enum MonsterEffectType
    {
        Attack,
        Movement,
        Nature,
        Flip,
        Destruction
    }

    public Effect[] Effects = new Effect[5];
}

public class Effect
{
    public static uint NoEffect = 0xFFFFFFFF;

    public byte[] Bytes;

    //Together these make effect type
    public EffectId EffectId;

    public SearchMode SearchMode;

    //The parameter data
    ushort effectDataUpper;
    ushort effectDataLower;
    public string effectName;
    public string searchModeName;

    public ushort EffectDataUpper
    {
        get => effectDataUpper;
        set
        {
            effectDataUpper = value;
            BitConverter.GetBytes(value).CopyTo(Bytes, 6);
        }
    }

    public ushort EffectDataLower
    {
        get => effectDataLower;
        set
        {
            effectDataLower = value;
            BitConverter.GetBytes(value).CopyTo(Bytes, 4);
        }
    }


    public Effect(byte[] EffectData)
    {
        Bytes = EffectData;
        EffectId = (EffectId)Bytes[1];
        byte searchModeByte = Bytes[0];
        effectDataLower = BitConverter.ToUInt16(Bytes, 4);
        effectDataUpper = BitConverter.ToUInt16(Bytes, 6);
        byte baseSearchMode = (byte)(searchModeByte & 0x3F);
        SearchModeTargeting sideTarget = (SearchModeTargeting)(searchModeByte & 0xC0);


        if (BitConverter.ToUInt32(Bytes) == NoEffect)
        {
            effectName = "";
            searchModeName = "";
            SearchMode = (SearchMode)0xff;
        }
        else
        {
            SearchMode = (SearchMode)baseSearchMode;
            effectName = Enum.GetName(typeof(EffectId), EffectId) ?? "";
            searchModeName = Enum.GetName(typeof(SearchMode), SearchMode) ?? "";

            if (sideTarget != 0)
            {
                searchModeName += $" ({Enum.GetName(typeof(SearchModeTargeting), sideTarget) ?? "No Targeting type"})";
            }
        }

    }
}

public enum SearchModeTargeting
{
    Friendly = 0x40,
    Enemy = 0x80,
    BothSides = 0xc0
}

public enum EffectId : byte
{
    CancelPowerups = 0x01,
    TerrainChange = 0x02,
    Destruction = 0x04,
    NoCombatDamage = 0x05,
    DoubleMovement = 0x06,
    DontTriggerTraps = 0x07,
    LabMovement = 0x08,
    TransformOnLab = 0x09,
    MoveOnLabAndMakeNormal = 0x0A,
    GrantImmunityToSpells = 0x0B,
    Transform = 0x0C,
    AlterLifePoints = 0x0D,
    DenyMagicAndRitual = 0x0E,
    StopTrapActivation = 0x0F,
    Unknown_10 = 0x10,
    DenyRitual = 0x11,
    APDPBuff = 0x12,
    APDPReduction = 0x13,
    Revive = 0x14,
    Unknown_15 = 0x15,
    GrowStatsToValue = 0x16,
    Spellbind = 0x17,
    Flip = 0x18,
    Unknown_19 = 0x19,
    SetSummoningPower = 0x1A,
    BonusVsMonsterType = 0x1B,
    Unknown_1C = 0x1C,
    RevealHand = 0x1D,
    RevealTypes = 0x1E,
    RevoltOrSteal = 0x1F,
    MagicalNeutralizingForceField = 0x20,
    SwapAllAttackAndDefense = 0x21,
    BuffDP = 0x22,
    ShiftToAttack = 0x23,
    ReturnToDeck = 0x24,
    Teleport = 0x25,
    SummonAgain = 0x26,
    StealSP = 0x27,
    EarthshakerEffect = 0x28,
    ReflectSpellDamage = 0x29,
    TurnHealIntoDamage = 0x2A,
    ReversePowerups = 0x2B,
    StopAttackMove = 0x2C,
    WhiteHole = 0x2D,
    DenyTraps = 0x2E,
    Unknown_2F = 0x2F,
    NegateAttack = 0x30,
    MirrorWall = 0x31,
    Ritual = 0x32,
    TimeWizard = 0x33,
    BoostPowerUps = 0x34,
    BuffAttackFromGraveyard = 0x35,
    TransformCardKind = 0x36,
    TransformCardAttribute = 0x37,
    Unknown_38 = 0x38,
    DenyRevoltOrSteal = 0x39,
    DenyRevivalFromCombatDeath = 0x3A,
    DoubleLifePointRecovery = 0x3B,
    TeleportJigenBakudan = 0x3C,
    TaintedWisdom = 0x3D,
    ReviveHighestAttackFor1kLP = 0x3E,
    GoddessOfWhim = 0x3F,
    Unknown_40 = 0x40,
    BuffMonsterBy1kFor1kLP = 0x41,
    LoseLPPerSquare = 0x42,
    CancelMovementBonus = 0x43,
    Unknown_44 = 0x44,
    Unknown_45 = 0x45,
    JigenBakudan = 0x46,
    PauseSpellboundTimer = 0x47,
    MagicalLab = 0x48,
    FlipAndSpellbind = 0x49,
    GorgonsEye = 0x4A,
    FakeTrap = 0x4B,
    DenyRevive = 0x4C,
    WeakenMonsters = 0x4D,
    BuffMonsters = 0x4E,
    CantMove = 0x4F,
    Unknown_50 = 0x50,
    JowlsOfDarkDemise = 0x51,
    MimicDeckLeaderStats = 0x52,
    RemoveGraveyardAndBuff = 0x53,
    MoveAllCardsTowardsMe = 0x54,
    DoubleSpellbindDuration = 0x55,
    ReduceSummoningPower = 0x56,
    DiscardHands = 0x57,
    DenyLeaderAbilities = 0x58
}