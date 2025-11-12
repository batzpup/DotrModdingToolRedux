namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

public class MusicSnapshot
{
    public Dictionary<string, int> music;

    public MusicSnapshot(Dictionary<int, int> duelistMusic, ModdedStringName[] musicTargets)
    {
        music = new Dictionary<string, int>();
        for (var index = 0; index < musicTargets.Length; index++)
        {
            music.Add(new String(musicTargets[index].Current), duelistMusic[index]);
        }
    }
}

public class MusicDiffChecker : IDiffChecker<MusicSnapshot>
{
    public DiffResult CompareSnapshots(MusicSnapshot oldSnapshot, MusicSnapshot currentSnapshot)
    {

        DiffResult result = new DiffResult { Name = "Music" };
        for (int i = 0; i < oldSnapshot.music.Count; i++)
        {
            var oldKvp = oldSnapshot.music.ElementAt(i);
            var currentKvp = currentSnapshot.music.ElementAt(i);
            if (oldKvp.Value != currentKvp.Value)
            {
                string oldTrack = MusicEditorWindow.musicTracks[oldKvp.Value - 1].Remove(0, 2);
                string newTrack = MusicEditorWindow.musicTracks[currentKvp.Value - 1].Remove(0, 2);
                result.Add(currentKvp.Key, $"{oldTrack} -> {newTrack}");
            }
        }
        return result;
    }
}