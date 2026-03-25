namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

class AiDiffChecker : IDiffChecker<AiSnapshot>
{
    public DiffResult CompareSnapshots(AiSnapshot oldSnapshot, AiSnapshot currentSnapshot)
    {
        DiffResult result = new DiffResult { Name = "Ai Changes" };
        for (var i = 0; i < oldSnapshot.AiIds.Count; i++)
        {
            int oldAi = oldSnapshot.AiIds[i];
            int newAi = currentSnapshot.AiIds[i];
            if (oldAi != newAi)
            {
                result.Add("",$"{Ai.GetAiById(oldAi).Name} -> {Ai.GetAiById(newAi).Name}");
            }
        }
        return result;
    }
}

public class AiSnapshot
{
    public List<int> AiIds;

    public AiSnapshot()
    {
        AiIds = new List<int>();
        foreach (var enemy in Enemies.EnemyList)
        {
            AiIds.Add(enemy.AiId);
        }
    }

 
}