using System.Runtime.InteropServices;
using System.Text;
namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

public static class ChangelogManager
{
    public static ModSnapshot OldSnapshot;
    public static ModSnapshot NewSnapshot;

    static DeckDiffChecker Decks = new DeckDiffChecker();
    static CardDataDiffChecker CardData = new CardDataDiffChecker();
    static MapDiffChecker Maps = new MapDiffChecker();
    static FusionDiffChecker Fusions = new FusionDiffChecker();

    static CustomPatchDiff CustomPatches = new CustomPatchDiff();
    static StringDiffChecker Strings = new StringDiffChecker();
    static MusicDiffChecker Music = new MusicDiffChecker();


    /// <summary>
    /// Returns true if there is a difference and adds it to diff
    /// </summary>
    public static bool Check<T>(string name, T oldVal, T newVal, List<string> diff)
    {
        if (!Equals(oldVal, newVal))
        {
            diff.Add($"{name} {oldVal} → {newVal}");
            return true;
        }
        return false;
    }

    public static void Check(
        string name,
        int oldId,
        int newId,
        List<string> diffs,
        Func<int, string>? nameResolver = null)
    {
        if (oldId != newId)
        {
            string oldName = nameResolver != null ? nameResolver(oldId) : oldId.ToString();
            string newName = nameResolver != null ? nameResolver(newId) : newId.ToString();
            diffs.Add($"{name} {oldName} → {newName}");
        }
    }

    public static List<DiffResult> CompareAll()
    {
        var results = new List<DiffResult>();
        results.Add(Decks.CompareSnapshots(OldSnapshot.DeckSnapshot, NewSnapshot.DeckSnapshot));
        results.Add(CardData.CompareSnapshots(OldSnapshot.CardConstantSnapshot, NewSnapshot.CardConstantSnapshot));
        results.Add(Maps.CompareSnapshots(OldSnapshot.MapSnapshot, NewSnapshot.MapSnapshot));
        results.Add(CustomPatches.CompareSnapshots(OldSnapshot.CustomPatchSnapshot, NewSnapshot.CustomPatchSnapshot));
        results.Add(Music.CompareSnapshots(OldSnapshot.MusicSnapshot, NewSnapshot.MusicSnapshot));
        results.Add(Fusions.CompareSnapshots(OldSnapshot.FusionSnapshot, NewSnapshot.FusionSnapshot));
        results.Add(Strings.CompareSnapshots(OldSnapshot.StringSnapshot, NewSnapshot.StringSnapshot));
        return results.Where(r => r.HasChanges).ToList();
    }

    public static string FormatResultsAsString(List<DiffResult> results)
    {
        var sb = new StringBuilder();
        try
        {
            if (results.Count == 0)
            {
                return "No changes detected.";
            }


            for (var i = 0; i < results.Count; i++)
            {
                if (!results[i].HasChanges)
                {
                    continue;
                }

                DiffResult? result = results[i];
                sb.Append(result.ToString());
                sb.AppendLine();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        return sb.ToString().TrimEnd();
    }
}

public class ModSnapshot
{
    public DeckSnapshot DeckSnapshot;
    public CardConstSnapshot CardConstantSnapshot;
    public MapSnapshot MapSnapshot;
    public FusionSnapshot FusionSnapshot;

    public CustomPatchSnapshot CustomPatchSnapshot;

    public StringSnapshot StringSnapshot;
    public MusicSnapshot MusicSnapshot;

    public ModSnapshot()
    {
        DeckSnapshot = new DeckSnapshot(Deck.DeckList);
        CardConstantSnapshot = new(
            CardConstant.List, CardDeckLeaderAbilities.MonsterAbilities, MonsterEnchantData.MonsterEnchantDataList,
            EnchantData.EnchantIds, EnchantData.EnchantScores, Card.cardNameList);
        CustomPatchSnapshot = new CustomPatchSnapshot(GameplayPatchesWindow.Instance);
        FusionSnapshot = new(FusionData.FusionTableData);
        MapSnapshot = new MapSnapshot(DataAccess.Instance.maps);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
             MusicSnapshot = new MusicSnapshot(MusicEditorWindow.Instance.DuelistMusic, MusicEditorWindow.MusicTargets);
        }
       
        StringSnapshot = new StringSnapshot(StringEditor.StringTable.Values.ToList());
    }
}