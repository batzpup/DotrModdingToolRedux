namespace DotrModdingTool2IMGUI;

public static class Effects
{
    public static List<MonsterEffects> MonsterEffectsList = new List<MonsterEffects>();
    public static List<Effect> NonMonsterEffectsList = new List<Effect>();
        
    

    public static Dictionary<int, ModdedStringName> MonsterEffectOwners = new Dictionary<int, ModdedStringName> {
        { 1, new ModdedStringName("Seiyaryu", "Seiyaryu") },
        { 6, new ModdedStringName("Curse of Dragon", "Curse of Dragon") },
        { 11, new ModdedStringName("Serpent Night Dragon", "Serpent Night Dragon") },
        { 13, new ModdedStringName("Yamadron", "Yamadron") },
        { 25, new ModdedStringName("Yamatano Dragon Scroll", "Yamatano Dragon Scroll") },
        { 35, new ModdedStringName("Mystical Elf", "Mystical Elf") },
        { 36, new ModdedStringName("Time Wizard", "Time Wizard") },
        { 37, new ModdedStringName("Rogue Doll", "Rogue Doll") },
        { 38, new ModdedStringName("White Magical Hat", "White Magical Hat") },
        { 40, new ModdedStringName("Lucky Trinket", "Lucky Trinket") },
        { 41, new ModdedStringName("Genin", "Genin") },
        { 42, new ModdedStringName("Fairy's Gift", "Fairy's Gift") },
        { 43, new ModdedStringName("Magician of Faith", "Magician of Faith") },
        { 45, new ModdedStringName("Maha Vailo", "Maha Vailo") },
        { 51, new ModdedStringName("The Stern Mystic", "The Stern Mystic") },
        { 53, new ModdedStringName("The Unhappy Maiden", "The Unhappy Maiden") },
        { 61, new ModdedStringName("Illusionist Faceless Mage", "Illusionist Faceless Mage") },
        { 62, new ModdedStringName("Curtain of the Dark Ones", "Curtain of the Dark Ones") },
        { 64, new ModdedStringName("Nemuriko", "Nemuriko") },
        { 66, new ModdedStringName("The Bewitching Phantom Thief", "The Bewitching Phantom Thief") },
        { 67, new ModdedStringName("Phantom Dewan", "Phantom Dewan") },
        { 72, new ModdedStringName("Sectarian of Secrets", "Sectarian of Secrets") },
        { 73, new ModdedStringName("Mystic Lamp", "Mystic Lamp") },
        { 74, new ModdedStringName("Boo Koo", "Boo Koo") },
        { 76, new ModdedStringName("Cosmo Queen", "Cosmo Queen") },
        { 77, new ModdedStringName("Mask of Shine & Dark", "Mask of Shine & Dark") },
        { 79, new ModdedStringName("Dark Elf", "Dark Elf") },
        { 80, new ModdedStringName("Witch of the Black Forest", "Witch of the Black Forest") },
        { 83, new ModdedStringName("Lord of D.", "Lord of D.") },
        { 84, new ModdedStringName("Invitation to a Dark Sleep", "Invitation to a Dark Sleep") },
        { 85, new ModdedStringName("Hannibal Necromancer", "Hannibal Necromancer") },
        { 87, new ModdedStringName("Dark Magician Girl", "Dark Magician Girl") },
        { 89, new ModdedStringName("Dryad", "Dryad") },
        { 90, new ModdedStringName("Tao the Chanter", "Tao the Chanter") },
        { 95, new ModdedStringName("Injection Fairy Lily", "Injection Fairy Lily") },
        { 98, new ModdedStringName("Hurricail", "Hurricail") },
        { 99, new ModdedStringName("Kazejin", "Kazejin") },
        { 101, new ModdedStringName("Shadow Specter", "Shadow Specter") },
        { 102, new ModdedStringName("Skull Servant", "Skull Servant") },
        { 104, new ModdedStringName("The Snake Hair", "The Snake Hair") },
        { 108, new ModdedStringName("Pumpking the King of Ghosts", "Pumpking the King of Ghosts") },
        { 109, new ModdedStringName("Graveyard and the Hand of Invitation", "Graveyard and the Hand of Invitation") },
        { 111, new ModdedStringName("Fiend's Hand", "Fiend's Hand") },
        { 112, new ModdedStringName("Blue-Eyed Silver Zombie", "Blue-Eyed Silver Zombie") },
        { 113, new ModdedStringName("Temple of Skulls", "Temple of Skulls") },
        { 114, new ModdedStringName("Dokuroizo the Grim Reaper", "Dokuroizo the Grim Reaper") },
        { 115, new ModdedStringName("Fire Reaper", "Fire Reaper") },
        { 116, new ModdedStringName("Mech Mole Zombie", "Mech Mole Zombie") },
        { 118, new ModdedStringName("Flame Ghost", "Flame Ghost") },
        { 119, new ModdedStringName("Wood Remains", "Wood Remains") },
        { 123, new ModdedStringName("Shadow Ghoul", "Shadow Ghoul") },
        { 127, new ModdedStringName("Bone Mouse", "Bone Mouse") },
        { 128, new ModdedStringName("Dokurorider", "Dokurorider") },
        { 135, new ModdedStringName("Skull Guardian", "Skull Guardian") },
        { 137, new ModdedStringName("Kageningen", "Kageningen") },
        { 140, new ModdedStringName("Skull Stalker", "Skull Stalker") },
        { 141, new ModdedStringName("Vishwar Randi", "Vishwar Randi") },
        { 143, new ModdedStringName("Black Luster Soldier", "Black Luster Soldier") },
        { 144, new ModdedStringName("Wall Shadow", "Wall Shadow") },
        { 145, new ModdedStringName("Gate Guardian", "Gate Guardian") },
        { 146, new ModdedStringName("Swordstalker", "Swordstalker") },
        { 147, new ModdedStringName("Hungry Burger", "Hungry Burger") },
        { 148, new ModdedStringName("Garma Sword", "Garma Sword") },
        { 149, new ModdedStringName("Greenkappa", "Greenkappa") },
        { 151, new ModdedStringName("Flame Swordsman", "Flame Swordsman") },
        { 152, new ModdedStringName("Tactical Warrior", "Tactical Warrior") },
        { 153, new ModdedStringName("Swamp Battleguard", "Swamp Battleguard") },
        { 156, new ModdedStringName("Celtic Guardian", "Celtic Guardian") },
        { 161, new ModdedStringName("Battle Warrior", "Battle Warrior") },
        { 163, new ModdedStringName("Supporter in the Shadows", "Supporter in the Shadows") },
        { 164, new ModdedStringName("Dream Clown", "Dream Clown") },
        { 170, new ModdedStringName("M-Warrior #1", "M-Warrior #1") },
        { 171, new ModdedStringName("M-Warrior #2", "M-Warrior #2") },
        { 175, new ModdedStringName("Eyearmor", "Eyearmor") },
        { 176, new ModdedStringName("Doron", "Doron") },
        { 178, new ModdedStringName("Trap Master", "Trap Master") },
        { 181, new ModdedStringName("Wodan the Resident of the Forest", "Wodan the Resident of the Forest") },
        { 183, new ModdedStringName("Dimensional Warrior", "Dimensional Warrior") },
        { 186, new ModdedStringName("Sonic Maid", "Sonic Maid") },
        { 189, new ModdedStringName("Millennium Shield", "Millennium Shield") },
        { 190, new ModdedStringName("Monster Tamer", "Monster Tamer") },
        { 191, new ModdedStringName("Swordsman from a Foreign Land", "Swordsman from a Foreign Land") },
        { 192, new ModdedStringName("Beautiful Beast Trainer", "Beautiful Beast Trainer") },
        { 193, new ModdedStringName("Armed Ninja", "Armed Ninja") },
        { 195, new ModdedStringName("Performance of Sword", "Performance of Sword") },
        { 197, new ModdedStringName("Lava Battleguard", "Lava Battleguard") },
        { 201, new ModdedStringName("Queen's Double", "Queen's Double") },
        { 203, new ModdedStringName("Hibikime", "Hibikime") },
        { 206, new ModdedStringName("Hyo", "Hyo") },
        { 213, new ModdedStringName("Mountain Warrior", "Mountain Warrior") },
        { 216, new ModdedStringName("Solitude", "Solitude") },
        { 217, new ModdedStringName("One Who Hunts Souls", "One Who Hunts Souls") },
        { 220, new ModdedStringName("Sengenjin", "Sengenjin") },
        { 224, new ModdedStringName("Gate Deeg", "Gate Deeg") },
        { 228, new ModdedStringName("The Wicked Worm Beast", "The Wicked Worm Beast") },
        { 236, new ModdedStringName("Larvas", "Larvas") },
        { 242, new ModdedStringName("Air Marmot of Nefariousness", "Air Marmot of Nefariousness") },
        { 247, new ModdedStringName("Mystical Sheep #2", "Mystical Sheep #2") },
        { 248, new ModdedStringName("Super War-Lion", "Super War-Lion") },
        { 255, new ModdedStringName("Milus Radiant", "Milus Radiant") },
        { 257, new ModdedStringName("Hane-Hane", "Hane-Hane") },
        { 266, new ModdedStringName("King Tiger Wanghu", "King Tiger Wanghu") },
        { 268, new ModdedStringName("Fiend Reflection #2", "Fiend Reflection #2") },
        { 270, new ModdedStringName("Niwatori", "Niwatori") },
        { 272, new ModdedStringName("Harpie Lady", "Harpie Lady") },
        { 273, new ModdedStringName("Harpie Lady Sisters", "Harpie Lady Sisters") },
        { 274, new ModdedStringName("Spirit of the Books", "Spirit of the Books") },
        { 276, new ModdedStringName("Droll Bird", "Droll Bird") },
        { 291, new ModdedStringName("Birdface", "Birdface") },
        { 295, new ModdedStringName("Horn Imp", "Horn Imp") },
        { 297, new ModdedStringName("Kuriboh", "Kuriboh") },
        { 298, new ModdedStringName("Castle of Dark Illusions", "Castle of Dark Illusions") },
        { 299, new ModdedStringName("Reaper of the Cards", "Reaper of the Cards") },
        { 305, new ModdedStringName("Mask of Darkness", "Mask of Darkness") },
        { 310, new ModdedStringName("Mystery Hand", "Mystery Hand") },
        { 311, new ModdedStringName("The Shadow Who Controls the Dark", "The Shadow Who Controls the Dark") },
        { 313, new ModdedStringName("Tainted Wisdom", "Tainted Wisdom") },
        { 316, new ModdedStringName("Big Eye", "Big Eye") },
        { 317, new ModdedStringName("Dark Prisoner", "Dark Prisoner") },
        { 322, new ModdedStringName("Midnight Fiend", "Midnight Fiend") },
        { 325, new ModdedStringName("The Drdek", "The Drdek") },
        { 326, new ModdedStringName("Candle of Fate", "Candle of Fate") },
        { 328, new ModdedStringName("Embryonic Beast", "Embryonic Beast") },
        { 338, new ModdedStringName("Fiend's Mirror", "Fiend's Mirror") },
        { 344, new ModdedStringName("Monster Eye", "Monster Eye") },
        { 346, new ModdedStringName("Needle Ball", "Needle Ball") },
        { 348, new ModdedStringName("Dragon Seeker", "Dragon Seeker") },
        { 349, new ModdedStringName("Fungi of the Musk", "Fungi of the Musk") },
        { 352, new ModdedStringName("Chakra", "Chakra") },
        { 353, new ModdedStringName("Psycho-Puppet", "Psycho-Puppet") },
        { 357, new ModdedStringName("Hiro's Shadow Scout", "Hiro's Shadow Scout") },
        { 361, new ModdedStringName("Wall of Illusion", "Wall of Illusion") },
        { 364, new ModdedStringName("Berfomet", "Berfomet") },
        { 365, new ModdedStringName("Kryuel", "Kryuel") },
        { 368, new ModdedStringName("Gyakutenno Megami", "Gyakutenno Megami") },
        { 371, new ModdedStringName("Weather Control", "Weather Control") },
        { 372, new ModdedStringName("Mystical Capture Chain", "Mystical Capture Chain") },
        { 374, new ModdedStringName("Key Mace", "Key Mace") },
        { 375, new ModdedStringName("Happy Lover", "Happy Lover") },
        { 376, new ModdedStringName("Petit Angel", "Petit Angel") },
        { 377, new ModdedStringName("Hourglass of Life", "Hourglass of Life") },
        { 380, new ModdedStringName("Ray & Temperature", "Ray & Temperature") },
        { 382, new ModdedStringName("Goddess of Whim", "Goddess of Whim") },
        { 383, new ModdedStringName("Hoshiningen", "Hoshiningen") },
        { 384, new ModdedStringName("Skelengel", "Skelengel") },
        { 386, new ModdedStringName("Binding Chain", "Binding Chain") },
        { 387, new ModdedStringName("Muse-A", "Muse-A") },
        { 388, new ModdedStringName("Tenderness", "Tenderness") },
        { 389, new ModdedStringName("Shining Friendship", "Shining Friendship") },
        { 390, new ModdedStringName("Hourglass of Courage", "Hourglass of Courage") },
        { 395, new ModdedStringName("Spiked Snail", "Spiked Snail") },
        { 402, new ModdedStringName("Great Moth", "Great Moth") },
        { 403, new ModdedStringName("Perfectly Ultimate Great Moth", "Perfectly Ultimate Great Moth") },
        { 404, new ModdedStringName("Nightmare Scorpion", "Nightmare Scorpion") },
        { 408, new ModdedStringName("Jirai Gumo", "Jirai Gumo") },
        { 409, new ModdedStringName("Dungeon Worm", "Dungeon Worm") },
        { 410, new ModdedStringName("Leghul", "Leghul") },
        { 411, new ModdedStringName("Ganigumo", "Ganigumo") },
        { 415, new ModdedStringName("Korogashi", "Korogashi") },
        { 417, new ModdedStringName("Man-Eater Bug", "Man-Eater Bug") },
        { 418, new ModdedStringName("Gale Dogra", "Gale Dogra") },
        { 422, new ModdedStringName("Javelin Beetle", "Javelin Beetle") },
        { 427, new ModdedStringName("Larva of Moth", "Larva of Moth") },
        { 428, new ModdedStringName("Pupa of Moth", "Pupa of Moth") },
        { 429, new ModdedStringName("Arsenal Bug", "Arsenal Bug") },
        { 432, new ModdedStringName("Bladefly", "Bladefly") },
        { 447, new ModdedStringName("Serpent Marauder", "Serpent Marauder") },
        { 456, new ModdedStringName("Sinister Serpent", "Sinister Serpent") },
        { 457, new ModdedStringName("Mechaleon", "Mechaleon") },
        { 458, new ModdedStringName("Serpentine Princess", "Serpentine Princess") },
        { 461, new ModdedStringName("Root Water", "Root Water") },
        { 469, new ModdedStringName("Misairuzame", "Misairuzame") },
        { 470, new ModdedStringName("Tongyo", "Tongyo") },
        { 472, new ModdedStringName("Fortress Whale", "Fortress Whale") },
        { 476, new ModdedStringName("Kairyu-Shin", "Kairyu-Shin") },
        { 478, new ModdedStringName("Aqua Dragon", "Aqua Dragon") },
        { 480, new ModdedStringName("Spike Seadra", "Spike Seadra") },
        { 486, new ModdedStringName("Labyrinth Tank", "Labyrinth Tank") },
        { 490, new ModdedStringName("Yaiba Robo", "Yaiba Robo") },
        { 493, new ModdedStringName("Blocker", "Blocker") },
        { 495, new ModdedStringName("Cyber-Stein", "Cyber-Stein") },
        { 496, new ModdedStringName("Cyber Commander", "Cyber Commander") },
        { 498, new ModdedStringName("Cannon Soldier", "Cannon Soldier") },
        { 500, new ModdedStringName("Dharma Cannon", "Dharma Cannon") },
        { 502, new ModdedStringName("Barrel Dragon", "Barrel Dragon") },
        { 506, new ModdedStringName("Blast Sphere", "Blast Sphere") },
        { 509, new ModdedStringName("Blast Juggler", "Blast Juggler") },
        { 510, new ModdedStringName("Robotic Knight", "Robotic Knight") },
        { 514, new ModdedStringName("Machine King", "Machine King") },
        { 519, new ModdedStringName("Golgoil", "Golgoil") },
        { 523, new ModdedStringName("Patrol Robo", "Patrol Robo") },
        { 528, new ModdedStringName("Kinetic Soldier", "Kinetic Soldier") },
        { 530, new ModdedStringName("Bat", "Bat") },
        { 533, new ModdedStringName("Oscillo Hero #2", "Oscillo Hero #2") },
        { 534, new ModdedStringName("Sanga of the Thunder", "Sanga of the Thunder") },
        { 536, new ModdedStringName("The Immortal of Thunder", "The Immortal of Thunder") },
        { 537, new ModdedStringName("Electric Snake", "Electric Snake") },
        { 539, new ModdedStringName("Thunder Nyan Nyan", "Thunder Nyan Nyan") },
        { 541, new ModdedStringName("Electric Lizard", "Electric Lizard") },
        { 542, new ModdedStringName("LaLa Li-oon", "LaLa Li-oon") },
        { 545, new ModdedStringName("Mega Thunderball", "Mega Thunderball") },
        { 551, new ModdedStringName("Jellyfish", "Jellyfish") },
        { 552, new ModdedStringName("Catapult Turtle", "Catapult Turtle") },
        { 554, new ModdedStringName("Toad Master", "Toad Master") },
        { 560, new ModdedStringName("Penguin Knight", "Penguin Knight") },
        { 561, new ModdedStringName("Dorover", "Dorover") },
        { 563, new ModdedStringName("Roaring Ocean Snake", "Roaring Ocean Snake") },
        { 564, new ModdedStringName("Hitodenchak", "Hitodenchak") },
        { 565, new ModdedStringName("Water Element", "Water Element") },
        { 567, new ModdedStringName("Beastking of the Swamps", "Beastking of the Swamps") },
        { 568, new ModdedStringName("The Furious Sea King", "The Furious Sea King") },
        { 570, new ModdedStringName("Change Slime", "Change Slime") },
        { 571, new ModdedStringName("Psychic Kappa", "Psychic Kappa") },
        { 573, new ModdedStringName("Suijin", "Suijin") },
        { 574, new ModdedStringName("Zone Eater", "Zone Eater") },
        { 575, new ModdedStringName("Ooguchi", "Ooguchi") },
        { 580, new ModdedStringName("Turu-Purun", "Turu-Purun") },
        { 582, new ModdedStringName("Aqua Snake", "Aqua Snake") },
        { 589, new ModdedStringName("Ameba", "Ameba") },
        { 591, new ModdedStringName("Turtle Raccoon", "Turtle Raccoon") },
        { 593, new ModdedStringName("Star Boy", "Star Boy") },
        { 594, new ModdedStringName("Frog The Jam", "Frog The Jam") },
        { 599, new ModdedStringName("Violent Rain", "Violent Rain") },
        { 600, new ModdedStringName("Penguin Soldier", "Penguin Soldier") },
        { 612, new ModdedStringName("Maiden of The Aqua", "Maiden of The Aqua") },
        { 613, new ModdedStringName("Dragon Piper", "Dragon Piper") },
        { 616, new ModdedStringName("Fire Eye", "Fire Eye") },
        { 617, new ModdedStringName("Hinotama Soul", "Hinotama Soul") },
        { 621, new ModdedStringName("Jigen Bakudan", "Jigen Bakudan") },
        { 622, new ModdedStringName("Molten Behemoth", "Molten Behemoth") },
        { 624, new ModdedStringName("Prisman", "Prisman") },
        { 627, new ModdedStringName("Ancient Jar", "Ancient Jar") },
        { 629, new ModdedStringName("Dissolverock", "Dissolverock") },
        { 635, new ModdedStringName("Barrel Rock", "Barrel Rock") },
        { 639, new ModdedStringName("Muka Muka", "Muka Muka") },
        { 642, new ModdedStringName("Pot the Trick", "Pot the Trick") },
        { 647, new ModdedStringName("Dark Plant", "Dark Plant") },
        { 649, new ModdedStringName("Mushroom Man", "Mushroom Man") },
        { 652, new ModdedStringName("Man Eater", "Man Eater") },
        { 654, new ModdedStringName("Yashinoki", "Yashinoki") },
        { 655, new ModdedStringName("Ancient Tree of Enlightenment", "Ancient Tree of Enlightenment") },
        { 656, new ModdedStringName("Green Phantom King", "Green Phantom King") },
        { 659, new ModdedStringName("Laughing Flower", "Laughing Flower") },
        { 668, new ModdedStringName("Woodland Sprite", "Woodland Sprite") },
        { 670, new ModdedStringName("Fairy King Truesdale", "Fairy King Truesdale") },
        { 672, new ModdedStringName("Jowls of Dark Demise", "Jowls of Dark Demise") },
        { 673, new ModdedStringName("Souleater", "Souleater") },
        { 674, new ModdedStringName("Slate Warrior", "Slate Warrior") },
        { 675, new ModdedStringName("Shapesnatch", "Shapesnatch") },
        { 676, new ModdedStringName("Carat Idol", "Carat Idol") },
        { 677, new ModdedStringName("Electromagnetic Bagworm", "Electromagnetic Bagworm") },
        { 678, new ModdedStringName("Timeater", "Timeater") },
        { 679, new ModdedStringName("Mucus Yolk", "Mucus Yolk") },
        { 680, new ModdedStringName("Servant of Catabolism", "Servant of Catabolism") },
        { 681, new ModdedStringName("Rigras Leever", "Rigras Leever") },
        { 682, new ModdedStringName("Moisture Creature", "Moisture Creature") },
    };

    public static Dictionary<int, ModdedStringName> NonMonsterOwners = new Dictionary<int, ModdedStringName> {
        { 683, new ModdedStringName("Dragon Capture Jar", "Dragon Capture Jar") },
        { 684, new ModdedStringName("Time Seal", "Time Seal") },
        { 685, new ModdedStringName("Monster Reborn", "Monster Reborn") },
        { 686, new ModdedStringName("Copycat", "Copycat") },
        { 687, new ModdedStringName("Mimicat", "Mimicat") },
        { 688, new ModdedStringName("Graverobber", "Graverobber") },
        { 689, new ModdedStringName("Forest", "Forest") },
        { 690, new ModdedStringName("Wasteland", "Wasteland") },
        { 691, new ModdedStringName("Mountain", "Mountain") },
        { 692, new ModdedStringName("Sogen", "Sogen") },
        { 693, new ModdedStringName("Umi", "Umi") },
        { 694, new ModdedStringName("Yami", "Yami") },
        { 695, new ModdedStringName("Toon World", "Toon World") },
        { 696, new ModdedStringName("Burning Land", "Burning Land") },
        { 697, new ModdedStringName("Labyrinth Wall", "Labyrinth Wall") },
        { 698, new ModdedStringName("Magical Labyrinth", "Magical Labyrinth") },
        { 699, new ModdedStringName("Dark Hole", "Dark Hole") },
        { 700, new ModdedStringName("Raigeki", "Raigeki") },
        { 701, new ModdedStringName("Heavy Storm", "Heavy Storm") },
        { 702, new ModdedStringName("Harpie's Feather Duster", "Harpie's Feather Duster") },
        { 703, new ModdedStringName("Mooyan Curry", "Mooyan Curry") },
        { 704, new ModdedStringName("Red Medicine", "Red Medicine") },
        { 705, new ModdedStringName("Goblin's Secret Remedy", "Goblin's Secret Remedy") },
        { 706, new ModdedStringName("Soul of the Pure", "Soul of the Pure") },
        { 707, new ModdedStringName("Dian Keto the Cure Master", "Dian Keto the Cure Master") },
        { 708, new ModdedStringName("Gift of The Mystical Elf", "Gift of The Mystical Elf") },
        { 709, new ModdedStringName("Sparks", "Sparks") },
        { 710, new ModdedStringName("Hinotama", "Hinotama") },
        { 711, new ModdedStringName("Final Flame", "Final Flame") },
        { 712, new ModdedStringName("Ookazi", "Ookazi") },
        { 713, new ModdedStringName("Tremendous Fire", "Tremendous Fire") },
        { 714, new ModdedStringName("Just Desserts", "Just Desserts") },
        { 715, new ModdedStringName("Swords of Revealing Light", "Swords of Revealing Light") },
        { 716, new ModdedStringName("Dark-Piercing Light", "Dark-Piercing Light") },
        { 717, new ModdedStringName("Darkness Approaches", "Darkness Approaches") },
        { 718, new ModdedStringName("The Eye of Truth", "The Eye of Truth") },
        { 719, new ModdedStringName("The Inexperienced Spy", "The Inexperienced Spy") },
        { 720, new ModdedStringName("Warrior Elimination", "Warrior Elimination") },
        { 721, new ModdedStringName("Eternal Rest", "Eternal Rest") },
        { 722, new ModdedStringName("Stain Storm", "Stain Storm") },
        { 723, new ModdedStringName("Eradicating Aerosol", "Eradicating Aerosol") },
        { 724, new ModdedStringName("Breath of Light", "Breath of Light") },
        { 725, new ModdedStringName("Eternal Draught", "Eternal Draught") },
        { 726, new ModdedStringName("Fissure", "Fissure") },
        { 727, new ModdedStringName("Last Day of Witch", "Last Day of Witch") },
        { 728, new ModdedStringName("Exile of the Wicked", "Exile of the Wicked") },
        { 729, new ModdedStringName("Dust Tornado", "Dust Tornado") },
        { 730, new ModdedStringName("Cold Wave", "Cold Wave") },
        { 731, new ModdedStringName("Fairy Meteor Crush", "Fairy Meteor Crush") },
        { 732, new ModdedStringName("Change of Heart", "Change of Heart") },
        { 733, new ModdedStringName("Brain Control", "Brain Control") },
        { 734, new ModdedStringName("Magical Neutralizing Force Field", "Magical Neutralizing Force Field") },
        { 735, new ModdedStringName("Winged Trumpeter", "Winged Trumpeter") },
        { 736, new ModdedStringName("Shield & Sword", "Shield & Sword") },
        { 737, new ModdedStringName("Yellow Luster Shield", "Yellow Luster Shield") },
        { 738, new ModdedStringName("Limiter Removal", "Limiter Removal") },
        { 739, new ModdedStringName("Rain of Mercy", "Rain of Mercy") },
        { 740, new ModdedStringName("Windstorm of Etaqua", "Windstorm of Etaqua") },
        { 741, new ModdedStringName("Sebek's Blessing", "Sebek's Blessing") },
        { 742, new ModdedStringName("Aqua Chorus", "Aqua Chorus") },
        { 743, new ModdedStringName("Stop Defense", "Stop Defense") },
        { 744, new ModdedStringName("Monster Recovery", "Monster Recovery") },
        { 745, new ModdedStringName("Call Of The Haunted", "Call Of The Haunted") },
        { 746, new ModdedStringName("Shift", "Shift") },
        { 747, new ModdedStringName("Solomon's Lawbook", "Solomon's Lawbook") },
        { 748, new ModdedStringName("Magic Drain", "Magic Drain") },
        { 749, new ModdedStringName("Dimensionhole", "Dimensionhole") },
        { 750, new ModdedStringName("Earthshaker", "Earthshaker") },
        { 751, new ModdedStringName("Creature Swap", "Creature Swap") },
        { 752, new ModdedStringName("Legendary Sword", "Legendary Sword") },
        { 753, new ModdedStringName("Sword of Dark Destruction", "Sword of Dark Destruction") },
        { 754, new ModdedStringName("Dark Energy", "Dark Energy") },
        { 755, new ModdedStringName("Axe of Despair", "Axe of Despair") },
        { 756, new ModdedStringName("Laser Cannon Armor", "Laser Cannon Armor") },
        { 757, new ModdedStringName("Insect Armor with Laser Cannon", "Insect Armor with Laser Cannon") },
        { 758, new ModdedStringName("Elf's Light", "Elf's Light") },
        { 759, new ModdedStringName("Beast Fangs", "Beast Fangs") },
        { 760, new ModdedStringName("Steel Shell", "Steel Shell") },
        { 761, new ModdedStringName("Vile Germs", "Vile Germs") },
        { 762, new ModdedStringName("Black Pendant", "Black Pendant") },
        { 763, new ModdedStringName("Silver Bow and Arrow", "Silver Bow and Arrow") },
        { 764, new ModdedStringName("Horn of Light", "Horn of Light") },
        { 765, new ModdedStringName("Horn of the Unicorn", "Horn of the Unicorn") },
        { 766, new ModdedStringName("Dragon Treasure", "Dragon Treasure") },
        { 767, new ModdedStringName("Electro-Whip", "Electro-Whip") },
        { 768, new ModdedStringName("Cyber Shield", "Cyber Shield") },
        { 769, new ModdedStringName("Mystical Moon", "Mystical Moon") },
        { 770, new ModdedStringName("Malevolent Nuzzler", "Malevolent Nuzzler") },
        { 771, new ModdedStringName("Book of Secret Arts", "Book of Secret Arts") },
        { 772, new ModdedStringName("Violet Crystal", "Violet Crystal") },
        { 773, new ModdedStringName("Invigoration", "Invigoration") },
        { 774, new ModdedStringName("Machine Conversion Factory", "Machine Conversion Factory") },
        { 775, new ModdedStringName("Raise Body Heat", "Raise Body Heat") },
        { 776, new ModdedStringName("Follow Wind", "Follow Wind") },
        { 777, new ModdedStringName("Power of Kaishin", "Power of Kaishin") },
        { 778, new ModdedStringName("Kunai with Chain", "Kunai with Chain") },
        { 779, new ModdedStringName("Salamandra", "Salamandra") },
        { 780, new ModdedStringName("Megamorph", "Megamorph") },
        { 781, new ModdedStringName("Bright Castle", "Bright Castle") },
        { 782, new ModdedStringName("Fiend Castle", "Fiend Castle") },
        { 783, new ModdedStringName("Hightide", "Hightide") },
        { 784, new ModdedStringName("Spring of Rebirth", "Spring of Rebirth") },
        { 785, new ModdedStringName("Gust Fan", "Gust Fan") },
        { 786, new ModdedStringName("Burning Spear", "Burning Spear") },
        { 787, new ModdedStringName("7 Completed", "7 Completed") },
        { 788, new ModdedStringName("Nails of Bane", "Nails of Bane") },
        { 789, new ModdedStringName("Riryoku", "Riryoku") },
        { 790, new ModdedStringName("Multiply", "Multiply") },
        { 791, new ModdedStringName("Sword of Dragon's Soul", "Sword of Dragon's Soul") },
        { 792, new ModdedStringName("Enchanted Javelin", "Enchanted Javelin") },
        { 793, new ModdedStringName("Anti-Magic Fragrance", "Anti-Magic Fragrance") },
        { 794, new ModdedStringName("Crush Card", "Crush Card") },
        { 795, new ModdedStringName("Paralyzing Potion", "Paralyzing Potion") },
        { 796, new ModdedStringName("Cursebreaker", "Cursebreaker") },
        { 797, new ModdedStringName("Elegant Egotist", "Elegant Egotist") },
        { 798, new ModdedStringName("Cocoon of Evolution", "Cocoon of Evolution") },
        { 799, new ModdedStringName("Metalmorph", "Metalmorph") },
        { 800, new ModdedStringName("Insect Imitation", "Insect Imitation") },
        { 801, new ModdedStringName("Spellbinding Circle", "Spellbinding Circle") },
        { 802, new ModdedStringName("Shadow Spell", "Shadow Spell") },
        { 803, new ModdedStringName("Mesmeric Control", "Mesmeric Control") },
        { 804, new ModdedStringName("Tears of the Mermaid", "Tears of the Mermaid") },
        { 805, new ModdedStringName("Infinite Dismissal", "Infinite Dismissal") },
        { 806, new ModdedStringName("Gravity Bind", "Gravity Bind") },
        { 807, new ModdedStringName("House of Adhesive Tape", "House of Adhesive Tape") },
        { 808, new ModdedStringName("Eatgaboon", "Eatgaboon") },
        { 809, new ModdedStringName("Bear Trap", "Bear Trap") },
        { 810, new ModdedStringName("Invisible Wire", "Invisible Wire") },
        { 811, new ModdedStringName("Acid Trap Hole", "Acid Trap Hole") },
        { 812, new ModdedStringName("Widespread Ruin", "Widespread Ruin") },
        { 813, new ModdedStringName("Type Zero Magic Crusher", "Type Zero Magic Crusher") },
        { 814, new ModdedStringName("Goblin Fan", "Goblin Fan") },
        { 815, new ModdedStringName("Bad Reaction to Simochi", "Bad Reaction to Simochi") },
        { 816, new ModdedStringName("Reverse Trap", "Reverse Trap") },
        { 817, new ModdedStringName("Block Attack", "Block Attack") },
        { 818, new ModdedStringName("Shadow of Eyes", "Shadow of Eyes") },
        { 819, new ModdedStringName("Gorgon's Eye", "Gorgon's Eye") },
        { 820, new ModdedStringName("Fake Trap", "Fake Trap") },
        { 821, new ModdedStringName("Anti Raigeki", "Anti Raigeki") },
        { 822, new ModdedStringName("Call of the Grave", "Call of the Grave") },
        { 823, new ModdedStringName("Magic Jammer", "Magic Jammer") },
        { 824, new ModdedStringName("White Hole", "White Hole") },
        { 825, new ModdedStringName("Royal Decree", "Royal Decree") },
        { 826, new ModdedStringName("Seal of the Ancients", "Seal of the Ancients") },
        { 827, new ModdedStringName("Mirror Force", "Mirror Force") },
        { 828, new ModdedStringName("Negate Attack", "Negate Attack") },
        { 829, new ModdedStringName("Mirror Wall", "Mirror Wall") },
        { 830, new ModdedStringName("Curse of Millennium Shield", "Curse of Millennium Shield") },
        { 831, new ModdedStringName("Yamadron Ritual", "Yamadron Ritual") },
        { 832, new ModdedStringName("Gate Guardian Ritual", "Gate Guardian Ritual") },
        { 833, new ModdedStringName("Black Luster Ritual", "Black Luster Ritual") },
        { 834, new ModdedStringName("Zera Ritual", "Zera Ritual") },
        { 835, new ModdedStringName("War-Lion Ritual", "War-Lion Ritual") },
        { 836, new ModdedStringName("Beastly Mirror Ritual", "Beastly Mirror Ritual") },
        { 837, new ModdedStringName("Ultimate Dragon", "Ultimate Dragon") },
        { 838, new ModdedStringName("Commencement Dance", "Commencement Dance") },
        { 839, new ModdedStringName("Hamburger Recipe", "Hamburger Recipe") },
        { 840, new ModdedStringName("Revival of Sennen Genjin", "Revival of Sennen Genjin") },
        { 841, new ModdedStringName("Novox's Prayer", "Novox's Prayer") },
        { 842, new ModdedStringName("Curse of Tri-Horned Dragon", "Curse of Tri-Horned Dragon") },
        { 843, new ModdedStringName("Revived Serpent Night Dragon", "Revived Serpent Night Dragon") },
        { 844, new ModdedStringName("Turtle Oath", "Turtle Oath") },
        { 845, new ModdedStringName("Contruct of Mask", "Contruct of Mask") },
        { 846, new ModdedStringName("Resurrection of Chakra", "Resurrection of Chakra") },
        { 847, new ModdedStringName("Puppet Ritual", "Puppet Ritual") },
        { 848, new ModdedStringName("Javelin Beetle Pact", "Javelin Beetle Pact") },
        { 849, new ModdedStringName("Garma Sword Oath", "Garma Sword Oath") },
        { 850, new ModdedStringName("Cosmo Queen's Prayer", "Cosmo Queen's Prayer") },
        { 851, new ModdedStringName("Revival of Dokurorider", "Revival of Dokurorider") },
        { 852, new ModdedStringName("Fortress Whale's Oath", "Fortress Whale's Oath") },
        { 853, new ModdedStringName("Dark Magic Ritual", "Dark Magic Ritual") },
    };


    public static byte[] MonsterEffectBytes
    {
        get { return MonsterEffectsList.SelectMany(a => a.Effects.SelectMany(b => b.Bytes)).ToArray(); }
    }

    public static byte[] MagicEffectBytes
    {
        get { return NonMonsterEffectsList.SelectMany(a => a.Bytes).ToArray(); }
    }

    public static void ReloadStrings()
    {
        foreach (var mEffect in MonsterEffectOwners)
        {
            mEffect.Value.Edited = StringEditor.StringTable[StringEditor.CardNamesOffsetStart + mEffect.Key];
        }
        foreach (var mEffect in NonMonsterOwners)
        {
            mEffect.Value.Edited = StringEditor.StringTable[StringEditor.CardNamesOffsetStart + mEffect.Key];
        }
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