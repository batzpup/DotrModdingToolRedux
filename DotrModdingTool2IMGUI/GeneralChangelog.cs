using System.Text;
using System.Text.Json.Serialization;
namespace DotrModdingTool2IMGUI;

public class GeneralChangelog
{
    [JsonPropertyName("duel_changes")] public DuelChanges DuelChanges { get; set; } = new();
    [JsonPropertyName("banned_cards")] public List<string> BannedCards { get; set; } = new();
    [JsonPropertyName("card_changes")] public Dictionary<string, CardChanges> CardChanges { get; set; } = new();
    [JsonPropertyName("deck_changes")] public Dictionary<string, DeckChange> DeckChanges { get; set; } = new();
    [JsonPropertyName("map_changes")] public List<GeneralMapChanges> MapChanges { get; set; } = new();
    [JsonPropertyName("ai_changes")] public List<AiChange> AiChanges { get; set; } = new();
    [JsonPropertyName("music_changes")] public List<MusicChange> MusicChanges { get; set; } = new();

/*
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

        sb.AppendLine("Mod Summary");
        sb.AppendLine($"Cards Modified: {CardChanges.Count}");
        sb.AppendLine($"Decks Modified: {DeckChanges.Count}");
        sb.AppendLine($"AI Changes: {AiChanges.Count}");
        sb.AppendLine($"Terrain Changes: {MapChanges?.TerrainChanges?.Count ?? 0}");
        sb.AppendLine($"Hidden Card Changes: {MapChanges?.HiddenCardChanges?.Count ?? 0}");
        sb.AppendLine($"Music Changes: {MusicChanges.Count}");

        return sb.ToString();
    }
*/
    public void FillInitData()
    {
        //Get init card 

        //Get init decks

        //Get init AI

        //Get Init Map 

        //Get init Music
    }
}

public class GeneralMapChanges
{
    public static Dictionary<Terrain, string> CharacterMapping = new Dictionary<Terrain, string>() {

        { Terrain.Forest, "F" },
        { Terrain.Wasteland, "W" },
        { Terrain.Mountain, "M" },
        { Terrain.Sea, "S" },
        { Terrain.Dark, "Y" },
        { Terrain.Toon, "T" },
        { Terrain.Normal, "N" },
        { Terrain.Labyrinth, "L" },
        { Terrain.Crush, "C" },

    };

    List<Map> OldMaps = new List<Map>();
    List<Map> NewMaps = new List<Map>();

    
}