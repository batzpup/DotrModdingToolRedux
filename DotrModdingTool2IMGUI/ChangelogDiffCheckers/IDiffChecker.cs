using System.Text;
namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

public interface IDiffChecker<TSnapshot>
{
    DiffResult CompareSnapshots(TSnapshot oldSnapshot, TSnapshot currentSnapshot);
}

public class DiffResult
{
    public string Name = "";

    public bool HasChanges => Changes.Values.Any(list => list.Count > 0);

    public Dictionary<string, List<string>> Changes { get; } = new();

    public void Add(string title, string entry)
    {
        if (!Changes.TryGetValue(title, out var list))
        {
            list = new List<string>();
            Changes[title] = list;
        }
        list.Add(entry);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Name}");
        foreach (var kvp in Changes)
        {
            sb.AppendLine(kvp.Key);
            foreach (var entry in kvp.Value)
            {
                sb.AppendLine("  " + entry);
            }

            sb.AppendLine();
        }
        return sb.ToString();
    }
}