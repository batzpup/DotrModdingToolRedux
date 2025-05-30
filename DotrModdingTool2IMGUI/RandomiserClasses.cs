using System.Text;
using System.Text.Json.Serialization;
namespace DotrModdingTool2IMGUI;

public class RandomiserChangeLog
{
    [JsonPropertyName("seed")] public int Seed { get; set; }
    [JsonPropertyName("duel_changes")] public DuelChanges DuelChanges { get; set; } = new();
    [JsonPropertyName("banned_cards")] public List<string> BannedCards { get; set; } = new();
    [JsonPropertyName("card_changes")] public Dictionary<string, CardChanges> CardChanges { get; set; } = new();
    [JsonPropertyName("deck_changes")] public Dictionary<string, DeckChange> DeckChanges { get; set; } = new();
    [JsonPropertyName("map_changes")] public MapChanges MapChanges { get; set; } = new();
    [JsonPropertyName("ai_changes")] public List<AiChange> AiChanges { get; set; } = new();
    [JsonPropertyName("music_changes")] public List<MusicChange> MusicChanges { get; set; } = new();


    public string ToReadableFormat()
    {
        var sb = new StringBuilder();
        GetSummary(sb);
        sb.AppendLine();

        bool hasDuelChanges = DuelChanges.StartingSp.HasValue ||
                              DuelChanges.SpRecovery.HasValue ||
                              DuelChanges.StartingLp.HasValue ||
                              DuelChanges.TerrainBuff.HasValue;
        if (hasDuelChanges)
        {
            sb.AppendLine("Duel Changes:");
            sb.AppendLine();

            if (DuelChanges.StartingSp.HasValue)
                sb.AppendLine($"  Starting SP: {DuelChanges.StartingSp.Value}");

            if (DuelChanges.SpRecovery.HasValue)
                sb.AppendLine($"  SP Recovery: {DuelChanges.SpRecovery.Value}");

            if (DuelChanges.StartingLp.HasValue)
                sb.AppendLine($"  Starting LP: {DuelChanges.StartingLp.Value}");

            if (DuelChanges.TerrainBuff.HasValue)
                sb.AppendLine($"  Terrain Buff: {DuelChanges.TerrainBuff.Value}");

            sb.AppendLine();
        }

        if (BannedCards.Any())
        {
            sb.AppendLine("Banned Cards:");
            sb.AppendLine();

            foreach (var bannedCard in BannedCards)
            {
                sb.AppendLine($"  {bannedCard}");
            }
            sb.AppendLine();
        }
        if (CardChanges.Any())
        {
            sb.AppendLine("Card Changes:");
            sb.AppendLine();

            foreach (var kvp in CardChanges)
            {
                string cardName = kvp.Key;
                var changes = kvp.Value;
                sb.AppendLine($"  {cardName}:");

                sb.AppendLine("      Acquistion:");
                if (changes.Acquisition.AppearsInSlots == true)
                    sb.AppendLine("         in slots");
                if (changes.Acquisition.IsRareDrop == true)
                    sb.AppendLine("         in rare drops");
                if (changes.Acquisition.AppearsInReincarnation == true)
                    sb.AppendLine("         in reincarnation");

 
                var parts = new List<string>();
                parts.Add($"Atk = {changes.Stats.Attack}");
                parts.Add($"DEF = {changes.Stats.Defense}");
                sb.AppendLine($"      {string.Join("/ ", parts)}");
                sb.AppendLine($"      level: {changes.Stats.Level}");
                sb.AppendLine($"      Type: {changes.Properties.Kind}");
                sb.AppendLine($"      Attribute: {changes.Properties.Attribute}");
                sb.AppendLine($"      Effect: {changes.Properties.Effect}");
                if (changes.Properties.StrongOnToon.HasValue && changes.Properties.StrongOnToon.Value)
                {
                    sb.AppendLine("      Strong on toon");
                }

                if (changes.Equipment?.CanEquip?.Any() == true)
                {
                    sb.AppendLine("      Equips:");
                    foreach (var equip in changes.Equipment.CanEquip)
                    {
                        sb.AppendLine($"          {equip}");
                    }
                }
                if (changes.LeaderAbilities?.Any() == true)
                {
                    sb.AppendLine("      Leader Abilities:");
                    foreach (var ability in changes.LeaderAbilities)
                    {
                        sb.AppendLine($"          {ability.Ability}: {ability.RankRequired}");
                    }
                }
                if (changes.PowerUpValue.HasValue)
                {
                    sb.AppendLine($"      power-up value: {changes.PowerUpValue.Value}");
                }

                sb.AppendLine();
            }
        }
        if (DeckChanges.Any())
        {
            sb.AppendLine("Deck Changes:");
            sb.AppendLine();

            foreach (var deckChange in DeckChanges)
            {
                sb.AppendLine($"  {deckChange.Key}:");
                if (!string.IsNullOrEmpty(deckChange.Value.LeaderChange))
                    sb.AppendLine($"      Leader: {deckChange.Value.LeaderChange}:{deckChange.Value.LeaderRank}");

                if (deckChange.Value.CardsAdded.Any())
                {
                    sb.AppendLine("      Cards Added:");
                    foreach (var card in deckChange.Value.CardsAdded)
                    {
                        sb.AppendLine($"          {card}");
                    }
                }
                sb.AppendLine();
            }
        }

        if (AiChanges.Any())
        {
            sb.AppendLine("AI Changes:");
            sb.AppendLine();
            foreach (var aiChange in AiChanges)
            {
                sb.AppendLine($"  {aiChange.EnemyName}: {aiChange.NewAi}");
            }
            sb.AppendLine();
        }

        if (MusicChanges.Any())
        {
            sb.AppendLine("Music Changes:");
            sb.AppendLine();

            foreach (var musicChange in MusicChanges)
            {
                sb.AppendLine($"  {musicChange.Duelist}: {musicChange.NewTrack}");
            }
            sb.AppendLine();
        }


        if (MapChanges.MapSwaps?.Any() == true)
        {
            sb.AppendLine("Map Swaps:");
            sb.AppendLine();

            foreach (var swap in MapChanges.MapSwaps)
            {
                sb.AppendLine($"  {swap.Map1} <-> {swap.Map2}");
            }
            sb.AppendLine();
        }

        if (MapChanges.TerrainChanges?.Any() == true)
        {
            sb.AppendLine("Terrain Changes:");
            sb.AppendLine();
            foreach (var terrainChange in MapChanges.TerrainChanges)
            {
                sb.AppendLine($"  {terrainChange.Map}: {terrainChange.FromTerrain} -> {terrainChange.ToTerrain}");
            }
            sb.AppendLine();
        }

        if (MapChanges.HiddenCardChanges?.Any() == true)
        {
            sb.AppendLine("Hidden Card Changes:");
            sb.AppendLine();

            foreach (var hiddenChange in MapChanges.HiddenCardChanges)
            {
                sb.AppendLine($"  {hiddenChange.Key}:");
                if (!string.IsNullOrEmpty(hiddenChange.Value.NewCard))
                    sb.AppendLine($"      New Card: {hiddenChange.Value.NewCard}");
                if (!string.IsNullOrEmpty(hiddenChange.Value.NewLocation))
                    sb.AppendLine($"      New Location: {hiddenChange.Value.NewLocation}");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public string GetSummary(StringBuilder sb)
    {
        sb.AppendLine($"Randomizer Summary - Seed: {Seed}");
        sb.AppendLine();
        sb.AppendLine($"Cards Modified: {CardChanges.Count}");
        sb.AppendLine($"Decks Modified: {DeckChanges.Count}");
        sb.AppendLine($"AI Changes: {AiChanges.Count}");
        sb.AppendLine($"Music Changes: {MusicChanges.Count}");
        sb.AppendLine($"Map Swaps: {MapChanges?.MapSwaps?.Count ?? 0}");
        sb.AppendLine($"Terrain Changes: {MapChanges?.TerrainChanges?.Count ?? 0}");
        sb.AppendLine($"Hidden Card Changes: {MapChanges?.HiddenCardChanges?.Count ?? 0}");
        sb.AppendLine($"Banned Cards: {BannedCards.Count}");
        return sb.ToString();
    }
}

public class CardChanges
{
    [JsonPropertyName("acquisition")] public AcquisitionChanges Acquisition { get; set; } = new();
    [JsonPropertyName("stats")] public StatChanges Stats { get; set; } = new();
    [JsonPropertyName("properties")] public PropertyChanges Properties { get; set; } = new();
    [JsonPropertyName("equipment")] public EquipmentChanges Equipment { get; set; } = new();
    [JsonPropertyName("leader_abilities")] public List<LeaderAbilityChange> LeaderAbilities { get; set; } = new();
    [JsonPropertyName("power_up_value")] public int? PowerUpValue { get; set; }
}

public class AcquisitionChanges
{
    [JsonPropertyName("appears_in_slots")] public bool? AppearsInSlots { get; set; }
    [JsonPropertyName("is_rare_drop")] public bool? IsRareDrop { get; set; }

    [JsonPropertyName("appears_in_reincarnation")]
    public bool? AppearsInReincarnation { get; set; }
}

public class StatChanges
{
    [JsonPropertyName("attack")] public ushort Attack { get; set; } = new();
    [JsonPropertyName("defense")] public ushort Defense { get; set; } = new();
    [JsonPropertyName("level")] public byte Level { get; set; } = new();
}

public class StatChange
{
    [JsonPropertyName("old_value")] public int OldValue { get; set; }
    [JsonPropertyName("new_value")] public int NewValue { get; set; }
}

public class PropertyChanges
{
    [JsonPropertyName("attribute")] public string Attribute { get; set; }
    [JsonPropertyName("kind")] public string Kind { get; set; }
    [JsonPropertyName("effect")] public string Effect { get; set; }
    [JsonPropertyName("strong_on_toon")] public bool? StrongOnToon { get; set; } = new();
}

public class EquipmentChanges
{
    [JsonPropertyName("can_equip")] public List<string> CanEquip { get; set; } = new();
}

public class LeaderAbilityChange
{
    [JsonPropertyName("ability")] public string Ability { get; set; }
    [JsonPropertyName("rank_required")] public string RankRequired { get; set; }

    public LeaderAbilityChange(string ability, string rankRequired)
    {
        Ability = ability;
        RankRequired = rankRequired;

    }
}

public class DeckChange
{
    [JsonPropertyName("leader_change")] public string LeaderChange { get; set; }
    [JsonPropertyName("leader_rank")] public string LeaderRank { get; set; }
    [JsonPropertyName("cards_added")] public List<string> CardsAdded { get; set; } = new();
}

public class MapChanges
{
    [JsonPropertyName("map_swaps")] public List<MapSwap> MapSwaps { get; set; } = new();
    [JsonPropertyName("terrain_changes")] public List<TerrainChange> TerrainChanges { get; set; } = new();
    [JsonPropertyName("hidden_card_changes")]
    public Dictionary<string, HiddenCardChange> HiddenCardChanges { get; set; } = new();
}

public class MapSwap
{
    public MapSwap(string map1, string map2)
    {
        Map1 = map1;
        Map2 = map2;

    }

    [JsonPropertyName("map1")] public string Map1 { get; set; }
    [JsonPropertyName("map2")] public string Map2 { get; set; }
}

public class TerrainChange
{
    public TerrainChange(string map, string fromTerrain, string toTerrain)
    {
        Map = map;
        FromTerrain = fromTerrain;
        ToTerrain = toTerrain;

    }

    [JsonPropertyName("map")] public string Map { get; set; }
    [JsonPropertyName("from_terrain")] public string FromTerrain { get; set; }
    [JsonPropertyName("to_terrain")] public string ToTerrain { get; set; }
}

public class HiddenCardChange
{
    [JsonPropertyName("new_card")] public string NewCard { get; set; }
    [JsonPropertyName("new_location")] public string NewLocation { get; set; }
}

public class DuelChanges
{
    [JsonPropertyName("starting_sp")] public int? StartingSp { get; set; }
    [JsonPropertyName("sp_recovery")] public int? SpRecovery { get; set; }
    [JsonPropertyName("starting_lp")] public int? StartingLp { get; set; }
    [JsonPropertyName("terrain_buff")] public int? TerrainBuff { get; set; }
    [JsonPropertyName("exp_changes")] public List<int> ExpChanges { get; set; }
}

public class AiChange
{
    [JsonPropertyName("enemy_name")] public string EnemyName { get; set; }
    [JsonPropertyName("new_ai")] public string NewAi { get; set; }

    public AiChange(string enemyName, string newAi)
    {
        EnemyName = enemyName;
        NewAi = newAi;

    }
}

public class MusicChange
{
    public MusicChange(string duelist, string newTrack)
    {
        Duelist = duelist;
        NewTrack = newTrack;
    }

    [JsonPropertyName("duelist")] public string Duelist { get; set; }
    [JsonPropertyName("new_track")] public string NewTrack { get; set; }
}