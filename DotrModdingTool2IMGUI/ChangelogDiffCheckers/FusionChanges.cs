namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

public class FusionDiffChecker : IDiffChecker<FusionSnapshot>
{
    public DiffResult CompareSnapshots(FusionSnapshot oldSnapshot, FusionSnapshot currentSnapshot)
    {
        DiffResult result = new DiffResult { Name = "Fusions" };
        //TODO some sort of sorting to comparing to make both new and old ordered the same then compare changes
        foreach (var kvp in oldSnapshot.Fusions)
        {
            string title = $"Fusion id {kvp.Key}:";
            List<string> diffs = new List<string>();
            FusionData oldFusion = kvp.Value;
            FusionData newFusion = currentSnapshot.Fusions[kvp.Key];
            if (!oldFusion.Bytes.SequenceEqual(newFusion.Bytes))
            {
                ChangelogManager.Check("Lower Id:", oldFusion.lowerCardName.Edited, newFusion.lowerCardName.Edited, diffs);
                ChangelogManager.Check("High Id:", oldFusion.higherCardName.Edited, newFusion.higherCardName.Edited, diffs);
                ChangelogManager.Check("result Id:", oldFusion.cardResultName.Edited, newFusion.cardResultName.Edited, diffs);
                diffs.Add($"Fusion: {newFusion.cardResultName} = {newFusion.lowerCardName.Edited} + {newFusion.higherCardName.Edited}");
            }
            if (diffs.Count > 0)
            {
                foreach (var diff in diffs)
                {
                    result.Add(title, diff);
                }
            }
        }
        return result;
    }
}

public class FusionSnapshot
{
    public Dictionary<int, FusionData> Fusions;

    public FusionSnapshot(Dictionary<int, FusionData> fusionData)
    {
        Fusions = fusionData.ToDictionary(
            pair => pair.Key,
            pair => CloneFusionData(pair.Value)
        );
    }

    FusionData CloneFusionData(FusionData original)
    {
        return new FusionData {
            lowerCardId = original.lowerCardId,
            higherCardId = original.higherCardId,
            resultId = original.resultId,
            fusionData = original.fusionData,
            lowerCardName = new ModdedStringName(original.lowerCardName.Default, original.lowerCardName.Current),
            higherCardName = new ModdedStringName(original.higherCardName.Default, original.higherCardName.Current),
            cardResultName = new ModdedStringName(original.cardResultName.Default, original.cardResultName.Current)
        };
    }
}