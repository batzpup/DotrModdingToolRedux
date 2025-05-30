namespace DotrModdingTool2IMGUI;

public static class Effects
{
    public static List<MonsterEffects> MonsterEffectsList = new List<MonsterEffects>();
    public static List<Effect> MagicEffectsList = new List<Effect>();


    public static List<string> MonsterEffectOwnerNames = new List<string> {
        "Seiyaryu",
        "Curse of Dragon",
        "Serpent Night Dragon",
        "Yamadron",
        "Yamatano Dragon Scroll",
        "Mystical Elf",
        "Time Wizard",
        "Rogue Doll",
        "White Magical Hat",
        "Lucky Trinket",
        "Genin",
        "Fairy's Gift",
        "Magician of Faith",
        "Maha Vailo",
        "The Stern Mystic",
        "The Unhappy Maiden",
        "Illusionist Faceless Mage",
        "Curtain of the Dark Ones",
        "Nemuriko",
        "The Bewitching Phantom Thief",
        "Phantom Dewan",
        "Sectarian of Secrets",
        "Mystic Lamp",
        "Boo Koo",
        "Cosmo Queen",
        "Mask of Shine & Dark",
        "Dark Elf",
        "Witch of the Black Forest",
        "Lord of D.",
        "Invitation to a Dark Sleep",
        "Hannibal Necromancer",
        "Dark Magician Girl",
        "Dryad",
        "Tao the Chanter",
        "Injection Fairy Lily",
        "Hurricail",
        "Kazejin",
        "Shadow Specter",
        "Skull Servant",
        "The Snake Hair",
        "Pumpking the King of Ghosts",
        "Graveyard and the Hand of Invitation",
        "Fiend's Hand",
        "Blue-Eyed Silver Zombie",
        "Temple of Skulls",
        "Dokuroizo the Grim Reaper",
        "Fire Reaper",
        "Mech Mole Zombie",
        "Flame Ghost",
        "Wood Remains",
        "Shadow Ghoul",
        "Bone Mouse",
        "Dokurorider",
        "Skull Guardian",
        "Kageningen",
        "Skull Stalker",
        "Vishwar Randi",
        "Black Luster Soldier",
        "Wall Shadow",
        "Gate Guardian",
        "Swordstalker",
        "Hungry Burger",
        "Garma Sword",
        "Greenkappa",
        "Flame Swordsman",
        "Tactical Warrior",
        "Swamp Battleguard",
        "Celtic Guardian",
        "Battle Warrior",
        "Supporter in the Shadows",
        "Dream Clown",
        "M-Warrior #1",
        "M-Warrior #2",
        "Eyearmor",
        "Doron",
        "Trap Master",
        "Wodan the Resident of the Forest",
        "Dimensional Warrior",
        "Sonic Maid",
        "Millennium Shield",
        "Monster Tamer",
        "Swordsman from a Foreign Land",
        "Beautiful Beast Trainer",
        "Armed Ninja",
        "Performance of Sword",
        "Lava Battleguard",
        "Queen's Double",
        "Hibikime",
        "Hyo",
        "Mountain Warrior",
        "Solitude",
        "One Who Hunts Souls",
        "Sengenjin",
        "Gate Deeg",
        "The Wicked Worm Beast",
        "Larvas",
        "Air Marmot of Nefariousness",
        "Mystical Sheep #2",
        "Super War-Lion",
        "Milus Radiant",
        "Hane-Hane",
        "King Tiger Wanghu",
        "Fiend Reflection #2",
        "Niwatori",
        "Harpie Lady",
        "Harpie Lady Sisters",
        "Spirit of the Books",
        "Droll Bird",
        "Birdface",
        "Horn Imp",
        "Kuriboh",
        "Castle of Dark Illusions",
        "Reaper of the Cards",
        "Mask of Darkness",
        "Mystery Hand",
        "The Shadow Who Controls the Dark",
        "Tainted Wisdom",
        "Big Eye",
        "Dark Prisoner",
        "Midnight Fiend",
        "The Drdek",
        "Candle of Fate",
        "Embryonic Beast",
        "Fiend's Mirror",
        "Monster Eye",
        "Needle Ball",
        "Dragon Seeker",
        "Fungi of the Musk",
        "Chakra",
        "Psycho-Puppet",
        "Hiro's Shadow Scout",
        "Wall of Illusion",
        "Berfomet",
        "Kryuel",
        "Gyakutenno Megami",
        "Weather Control",
        "Mystical Capture Chain",
        "Key Mace",
        "Happy Lover",
        "Petit Angel",
        "Hourglass of Life",
        "Ray & Temperature",
        "Goddess of Whim",
        "Hoshiningen",
        "Skelengel",
        "Binding Chain",
        "Muse-A",
        "Tenderness",
        "Shining Friendship",
        "Hourglass of Courage",
        "Spiked Snail",
        "Great Moth",
        "Perfectly Ultimate Great Moth",
        "Nightmare Scorpion",
        "Jirai Gumo",
        "Dungeon Worm",
        "Leghul",
        "Ganigumo",
        "Korogashi",
        "Man-Eater Bug",
        "Gale Dogra",
        "Javelin Beetle",
        "Larva of Moth",
        "Pupa of Moth",
        "Arsenal Bug",
        "Bladefly",
        "Serpent Marauder",
        "Sinister Serpent",
        "Mechaleon",
        "Serpentine Princess",
        "Root Water",
        "Misairuzame",
        "Tongyo",
        "Fortress Whale",
        "Kairyu-Shin",
        "Aqua Dragon",
        "Spike Seadra",
        "Labyrinth Tank",
        "Yaiba Robo",
        "Blocker",
        "Cyber-Stein",
        "Cyber Commander",
        "Cannon Soldier",
        "Dharma Cannon",
        "Barrel Dragon",
        "Blast Sphere",
        "Blast Juggler",
        "Robotic Knight",
        "Machine King",
        "Golgoil",
        "Patrol Robo",
        "Kinetic Soldier",
        "Bat",
        "Oscillo Hero #2",
        "Sanga of the Thunder",
        "The Immortal of Thunder",
        "Electric Snake",
        "Thunder Nyan Nyan",
        "Electric Lizard",
        "LaLa Li-oon",
        "Mega Thunderball",
        "Jellyfish",
        "Catapult Turtle",
        "Toad Master",
        "Penguin Knight",
        "Dorover",
        "Roaring Ocean Snake",
        "Hitodenchak",
        "Water Element",
        "Beastking of the Swamps",
        "The Furious Sea King",
        "Change Slime",
        "Psychic Kappa",
        "Suijin",
        "Zone Eater",
        "Ooguchi",
        "Turu-Purun",
        "Aqua Snake",
        "Ameba",
        "Turtle Raccoon",
        "Star Boy",
        "Frog The Jam",
        "Violent Rain",
        "Penguin Soldier",
        "Maiden of The Aqua",
        "Dragon Piper",
        "Fire Eye",
        "Hinotama Soul",
        "Jigen Bakudan",
        "Molten Behemoth",
        "Prisman",
        "Ancient Jar",
        "Dissolverock",
        "Barrel Rock",
        "Muka Muka",
        "Pot the Trick",
        "Dark Plant",
        "Mushroom Man",
        "Man Eater",
        "Yashinoki",
        "Ancient Tree of Enlightenment",
        "Green Phanton King",
        "Laughing Flower",
        "Woodland Sprite",
        "Fairy King Truesdale",
        "Jowls of Dark Demise",
        "Souleater",
        "Slate Warrior",
        "Shapesnatch",
        "Carat Idol",
        "Electromagnetic Bagworm",
        "Timeater",
        "Mucus Yolk",
        "Servant of Catabolism",
        "Rigras Leever",
        "Moisture Creature"
    };

    public static List<string> MagicEffectOwnerNames = new List<string> {
        "Dragon Capture Jar",
        "Time Seal",
        "Monster Reborn",
        "Copycat",
        "Mimicat",
        "Graverobber",
        "Forest",
        "Wasteland",
        "Mountain",
        "Sogen",
        "Umi",
        "Yami",
        "Toon World",
        "Burning Land",
        "Labyrinth Wall",
        "Magical Labyrinth",
        "Dark Hole",
        "Raigeki",
        "Heavy Storm",
        "Harpie's Feather Duster",
        "Mooyan Curry",
        "Red Medicine",
        "Goblin's Secret Remedy",
        "Soul of the Pure",
        "Dian Keto the Cure Master",
        "Gift of The Mystical Elf",
        "Sparks",
        "Hinotama",
        "Final Flame",
        "Ookazi",
        "Tremendous Fire",
        "Just Desserts",
        "Swords of Revealing Light",
        "Dark-Piercing Light",
        "Darkness Approaches",
        "The Eye of Truth",
        "The Inexperienced Spy",
        "Warrior Elimination",
        "Eternal Rest",
        "Stain Storm",
        "Eradicating Aerosol",
        "Breath of Light",
        "Eternal Draught",
        "Fissure",
        "Last Day of Witch",
        "Exile of the Wicked",
        "Dust Tornado",
        "Cold Wave",
        "Fairy Meteor Crush",
        "Change of Heart",
        "Brain Control",
        "Magical Neutralizing Force Field",
        "Winged Trumpeter",
        "Shield & Sword",
        "Yellow Luster Shield",
        "Limiter Removal",
        "Rain of Mercy",
        "Windstorm of Etaqua",
        "Sebek's Blessing",
        "Aqua Chorus",
        "Stop Defense",
        "Monster Recovery",
        "Call Of The Haunted",
        "Shift",
        "Solomon's Lawbook",
        "Magic Drain",
        "Dimensionhole",
        "Earthshaker",
        "Creature Swap",
        "Legendary Sword",
        "Sword of Dark Destruction",
        "Dark Energy",
        "Axe of Despair",
        "Laser Cannon Armor",
        "Insect Armor with Laser Cannon",
        "Elf's Light",
        "Beast Fangs",
        "Steel Shell",
        "Vile Germs",
        "Black Pendant",
        "Silver Bow and Arrow",
        "Horn of Light",
        "Horn of the Unicorn",
        "Dragon Treasure",
        "Electro-Whip",
        "Cyber Shield",
        "Mystical Moon",
        "Malevolent Nuzzler",
        "Book of Secret Arts",
        "Violet Crystal",
        "Invigoration",
        "Machine Conversion Factory",
        "Raise Body Heat",
        "Follow Wind",
        "Power of Kaishin",
        "Kunai with Chain",
        "Salamandra",
        "Megamorph",
        "Bright Castle",
        "Fiend Castle",
        "Hightide",
        "Spring of Rebirth",
        "Gust Fan",
        "Burning Spear",
        "7 Completed",
        "Nails of Bane",
        "Riryoku",
        "Multiply",
        "Sword of Dragon's Soul",
        "Enchanted Javelin",
        "Anti-Magic Fragrance",
        "Crush Card",
        "Paralyzing Potion",
        "Cursebreaker",
        "Elegant Egotist",
        "Cocoon of Evolution",
        "Metalmorph",
        "Insect Imitation",
        "Spellbinding Circle",
        "Shadow Spell",
        "Mesmeric Control",
        "Tears of the Mermaid",
        "Infinite Dismissal",
        "Gravity Bind",
        "House of Adhesive Tape",
        "Eatgaboon",
        "Bear Trap",
        "Invisible Wire",
        "Acid Trap Hole",
        "Widespread Ruin",
        "Type Zero Magic Crusher",
        "Goblin Fan",
        "Bad Reaction to Simochi",
        "Reverse Trap",
        "Block Attack",
        "Shadow of Eyes",
        "Gorgon's Eye",
        "Fake Trap",
        "Anti Raigeki",
        "Call of the Grave",
        "Magic Jammer",
        "White Hole",
        "Royal Decree",
        "Seal of the Ancients",
        "Mirror Force",
        "Negate Attack",
        "Mirror Wall",
        "Curse of Millennium Shield",
        "Yamadron Ritual",
        "Gate Guardian Ritual",
        "Black Luster Ritual",
        "Zera Ritual",
        "War-Lion Ritual",
        "Beastly Mirror Ritual",
        "Ultimate Dragon",
        "Commencement Dance",
        "Hamburger Recipe",
        "Revival of Sennen Genjin",
        "Novox's Prayer",
        "Curse of Tri-Horned Dragon",
        "Revived Serpent Night Dragon",
        "Turtle Oath",
        "Construct of Mask",
        "Resurrection of Chakra",
        "Puppet Ritual",
        "Javelin Beetle Pact",
        "Garma Sword Oath",
        "Cosmo Queen's Prayer",
        "Revival of Dokurorider",
        "Fortress Whale's Oath",
        "Dark Magic Ritual"
    };

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
    EffectId effectId;
    SearchMode searchMode;
    SearchModeTargeting searchModeTargeting;

    public string SearchModeName;
    public string SearchModeTargetingName;


    public void DisableEffect()
    {
        SearchModeTargeting = (SearchModeTargeting)0xff;
        SearchModeTargetingName = "No Effect";
        SearchMode = (SearchMode)0xff;
        SearchModeName = "No Effect";
        effectName = "No Effect";
        EffectId = (EffectId)0xff;
        EffectDataLower = 0xffff;
        EffectDataUpper = 0xffff;
       Array.Fill(Bytes, (byte)0xFF);
    }

    public EffectId EffectId
    {
        get => effectId;
        set
        {
            if (value != (EffectId)0xff)
            {
                Bytes[2] = 0x00;
                Bytes[3] = 0x00;
            }
            effectId = value;
            Bytes[1] = (byte)value;
            effectName = Enum.GetName(typeof(EffectId), effectId) ?? "No Effect";
        }
    }

    public SearchMode SearchMode
    {
        get => searchMode;
        set
        {
            searchMode = value;
            Bytes[0] = (byte)((byte)value | (byte)searchModeTargeting);
            SearchModeName = Enum.GetName(typeof(SearchMode), searchMode) ?? "Unknown Target";
        }
    }

    public SearchModeTargeting SearchModeTargeting
    {
        get => searchModeTargeting;
        set
        {
            searchModeTargeting = value;
            Bytes[0] = (byte)((byte)value | (byte)searchMode);
            SearchModeTargetingName = Enum.GetName(typeof(SearchModeTargeting), searchModeTargeting) ?? "No target type";

        }
    }


    //The parameter data
    ushort effectDataUpper;
    ushort effectDataLower;
    public string effectName;


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
        effectId = (EffectId)Bytes[1];
        byte searchModeByte = Bytes[0];
        effectDataLower = BitConverter.ToUInt16(Bytes, 4);
        effectDataUpper = BitConverter.ToUInt16(Bytes, 6);
        searchMode = (SearchMode)(searchModeByte & 0x3F);
        searchModeTargeting = (SearchModeTargeting)(searchModeByte & 0xC0);


        if (BitConverter.ToUInt32(Bytes) == NoEffect)
        {
            effectName = "No Effect";
            SearchModeName = "No Effect";
            SearchModeTargetingName = "No Effect";
            searchMode = (SearchMode)0xff;
            searchModeTargeting = (SearchModeTargeting)0xff;
        }
        else
        {
            effectName = Enum.GetName(typeof(EffectId), effectId) ?? "No Effect";
            SearchModeName = Enum.GetName(typeof(SearchMode), searchMode) ?? "Unknown Target";
            SearchModeTargetingName = $"{Enum.GetName(typeof(SearchModeTargeting), searchModeTargeting)}";
        }

    }
}

public enum SearchModeTargeting
{
    No_Target = 0x0,
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
    IncreasedMovement = 0x06,
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
    SetSpellbindTimer = 0x17,
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
    BuffAttackBasedOnTarget = 0x35,
    TransformCardKind = 0x36,
    TransformCardAttribute = 0x37,
    SummonCopyOfCard = 0x38,
    DenyRevoltOrSteal = 0x39,
    DenyRevivalFromCombatDeath = 0x3A,
    AlterLifePointRecovery = 0x3B,
    TeleportOther = 0x3C,
    TaintedWisdom = 0x3D,
    ReviveHighestAttackForCost = 0x3E,
    GoddessOfWhim = 0x3F,
    Unknown_40 = 0x40,
    BuffMonsterBy1kFor1kLP = 0x41,
    LoseLPPerSquare = 0x42,
    CancelMovementBonus = 0x43,
    Unknown_44 = 0x44,
    Unknown_45 = 0x45,
    DestroyCardAfterTimer = 0x46,
    PauseSpellboundTimer = 0x47,
    MagicalLab = 0x48,
    FlipAndSpellbind = 0x49,
    GorgonsEye = 0x4A,
    FakeTrap = 0x4B,
    DenyRevive = 0x4C,
    WeakenMonstersPerTurn = 0x4D,
    BuffMonstersPerTurn = 0x4E,
    CantMove = 0x4F,
    AntiRaigeki = 0x50,
    JowlsOfDarkDemise = 0x51,
    MimicDeckLeaderStats = 0x52,
    RemoveGraveyardAndBuff = 0x53,
    MoveAllCardsTowardsMe = 0x54,
    DoubleSpellbindDuration = 0x55,
    ReduceSummoningPower = 0x56,
    DiscardHands = 0x57,
    DenyLeaderAbilities = 0x58
}