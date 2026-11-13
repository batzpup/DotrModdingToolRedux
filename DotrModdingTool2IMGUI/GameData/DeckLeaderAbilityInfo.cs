namespace DotrModdingTool2IMGUI;

public class DeckLeaderAbilityInfo
{
    public readonly static string[][] NameAndDescriptions;

    static DeckLeaderAbilityInfo()
    {
        NameAndDescriptions = new string[20][];

        for (int i = 0; i < NameAndDescriptions.Length; i++)
        {
            NameAndDescriptions[i] = new string[] { "???", "?????????", null };
        }

        NameAndDescriptions[(int)DeckLeaderAbilityType.HiddenCard] = new string[] {
            "Hidden Card",
            "Gains the ability to find hidden Treasures on Campaign maps.\n" +
            "At lower ranks, only finds card if directly on top of them.\n" +
            "At higher ranks, the range is increased to a 3x3 Zone."
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.ExtraSlots] = new string[] {
            "Extra Slots",
            "Enables multiple angles to win 3-in-a-row in Card Slots.\n" +
            "The higher the Deck Leader rank, the more additional angles become available.\n" +
            "WARNING : The actual logic is hardcoded to not work with many Monster Types.\n" +
            "Check the Patches section to fix that."
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.DestinyDraw] = new string[] {
            "Destiny Draw",
            "Gives chance to draw a special card when LP are below 1000, once per Duel.\n" +
            "See General Editor - Destiny Draw Editor for card pools."
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.IncreasedMovement] = new string[] {
            "Increased Movement",
            "Deck Leader can move 2 squares at a time instead of 1."
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.LPRecovery] = new string[] {
            "LP Recovery",
            "Restore 50 LP at the end of every turn."
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.DirectDamageHalved] = new string[] {
            "Direct Damage Halved",
            "Halve all Damage from Direct Attacks. (Excludes Spells.)"
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.TerrainChange] = new string[] {
            "Terrain Change",
            @"When the Deck Leader moves onto a tile, its Terrain changes to the specified type.
WARNING : The actual logic is hardcoded and can't be changed with this tool :
  Spellcaster -> DARK
  Fiend -> DARK
  Insect -> FOREST
  Dinosaur -> WASTELAND
  Aqua -> SEA
  Immortal -> CRUSH
  All other types -> NORMAL" // Code that changes to DARK is ignored in the game.
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.LevelCostReduction] = new string[] {
            "Level Cost Reduction",
            @"Reduce Monster Levels for same type friendlies (up to 2 Stars).
WARNING : The actual logic is hardcoded and can't be changed with this tool.
Reduction depends on the Leader's Type and Level (as a Monster) :
  Immortal -> always 1
  Beast-Warrior, Sea Serpent -> 2 if level is at least 6, else 1
  Dragon, Winged Beast, Fish, Machine -> 2 if level is at least 7, else 1
  Does not work with other Types."
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.ExtendedSupportRange] = new string[] {
            "Extended Support Range",
            "Extends the Range of Support Range Abilities, from a 3x3 Zone to 5x5."
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.FriendlyIncreasedStrength] = new string[] {
            "Improved Strength for Same Type Friendlies",
            "Increase ATK/DEF for same type friendlies.\n" +
            "(100x Leader's Level (as a Monster), up to 500 points.)"
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.FriendlyImprovedResistance] = new string[] {
            "Improved Resistance for Same Type Friendlies",
            @"Any of your monsters of the same type and within your Deck Leader's range
are immune to enemy Magic, Powerup and Trap cards.
(They are unaffected by such cards, also, they do not trigger them.
 Protection against Powerups also extends when Attacking them from within the range.
 Monster Effects, as well as select few Magic Cards, ignore this ability.)"
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.FriendlyMovementBoost] = new string[] {
            "Movement Boost for Same Type Friendlies",
            "Grants Increased Movement to same type friendlies. (Does not stack.)"
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.OpenCard] = new string[] {
            "Open Opponent's Card",
            "Flips face-up all enemy cards within the Support Range.\n" +
            "(Flip effects do not Activate.)"
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.WeakenSpecificEnemyType] = new string[] {
            "Weaken Specific Enemy Type",
            @"Weakens enemy Monsters of up to 2 specific Types, while in the Support Range.
(100x Leader's Level (as a Monster), up to 500 points.)
WARNING : The actual logic is hardcoded and can't be changed with this tool.
  Spellcaster powers down Fiend.
  Zombie powers down Warrior.
  Warrior powers down Dragon and Spellcaster.
  Beast powers down Fish.
  Winged-Beast powers down Insect and Fish.
  Fiend powers down Spellcaster.
  Insect powers down Sea Serpent.
  Reptile powers down Beast and Winged-Beast.
  Fish powers down Thunder.
  Thunder powers down Zombie and Machine.
  Aqua powers down Machine and Pyro.
  Pyro powers down Beast-Warrior.
  Does not work with other Types."
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.SpellbindSpecificEnemyType] = new string[] {
            "Spellbind Specific Enemy Type",
            @"Spellbinds enemy Monsters of one specific Type, while in the Support Range.
(Spellbind is permanent, but clears up if the condition no longer meets.)
WARNING : The actual logic is hardcoded and can't be changed with this tool.
 Plant Spellbinds Dragon.
 Pyro Spellbinds Beast.
 Reptile Spellbinds Aqua.
 Fairy Spellbinds Zombie.
 Does not work with other Types."
        };

        NameAndDescriptions[(int)DeckLeaderAbilityType.DestroySpecificEnemyType] = new string[] {
            "Destroy Specific Enemy Type",
            @"Destroys enemy Monsters of one specific Type.
WARNING : The actual logic is hardcoded and can't be changed with this tool.
  Fairy destroys Fiend
  Reptile destroys Insect
  Pyro destroys Zombie and Plant
  Rock destroys Dinosaur
  Plant destroys Rock
  Does not work with other Types."
        };
    }
}