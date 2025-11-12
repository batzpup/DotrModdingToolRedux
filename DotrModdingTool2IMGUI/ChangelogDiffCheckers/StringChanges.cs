using System.Diagnostics;
namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

public class StringSnapshot
{
    public List<string> strings;

    public StringSnapshot(List<string> stringList)
    {
        strings = CloneStrings(stringList);
    }


    List<string> CloneStrings(List<string> original)
    {
        List<string> temp = new List<string>();
        foreach (var str in original)
        {
            temp.Add(new string(str));
        }
        return temp;
    }
}

public class StringDiffChecker : IDiffChecker<StringSnapshot>
{
    public DiffResult CompareSnapshots(StringSnapshot oldSnapshot, StringSnapshot currentSnapshot)
    {

        DiffResult result = new DiffResult{Name = "Strings"};
        return result;
    }
}