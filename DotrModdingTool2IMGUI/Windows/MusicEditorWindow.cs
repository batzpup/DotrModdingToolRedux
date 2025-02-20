using System.Drawing;
using System.Numerics;
using DiscUtils.Iso9660;
using GameplayPatches;
using ImGuiNET;
using NAudio.MediaFoundation;
using NAudio.Wave;
namespace DotrModdingTool2IMGUI;

public class MusicEditorWindow : IImGuiWindow
{
    //Music
    static int AddCustomMusicPtr = 0x17ac58;
    static int TaTuto_DrawTrapArea = 0x24f800;

    WaveOutEvent waveOut;
    Mp3FileReader mp3Reader;
    bool isMusicPlaying = false;
    DataAccess dataAccess = DataAccess.Instance;
    public Dictionary<int, int> DuelistMusic = new Dictionary<int, int>();
    string MusicDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Music");
    string playStopButton = "Play";
    int currentDuelistSelected;
    int currentTrackIndex;
    int currentSlotMusicIndex;
    bool bSaveMusicChanges;
    ImFontPtr monoSpaceFont;
    public static Action<bool> OnSaveCustomMusic;


    #region StringsArrays

    string[] musicTargets = new[] {
        "Tutorial",
        "Seto",
        "Weevil",
        "Rex",
        "Keith",
        "Ishtar",
        "Necromancer",
        "Darkness-ruler",
        "Labyrinth-ruler",
        "Pegasus",
        "Richard",
        "Tea",
        "Tristan",
        "Mai",
        "Mako",
        "Joey",
        "Shadi",
        "Jasper",
        "Bakura",
        "Yugi",
        "MFL Skull Knight",
        "MFL Chakra"
    };

    string[] musicTracks = new[] {
        "01OpeningCutscene",
        "02MainMenu",
        "03CustomDuel",
        "04MapMusic",
        "05DeckCreation",
        "06N/A",
        "07TutorialDuel",
        "08BattleMusicRed",
        "09BattleMusicWhite",
        "10Seto",
        "11Yugi",
        "12MFLWhite",
        "13NormalBattle",
        "14MeadowBattle",
        "15MountainBattle",
        "16SeaBattle",
        "17ForestBattle",
        "18WastelandBattle",
        "19CrushBattle",
        "20DarkBattle",
        "21ToonBattle",
        "22LabBattle",
        "23Victory",
        "24NewGame",
        "25SimonSummoning",
        "26SetosArrival",
        "27TalkWithYugi",
        "28YugiArrivesAtDover",
        "29PegasusBetrayal",
        "30DefeatRichardCutscene",
        "31SetoSummoningRitual1",
        "32SetoSummongRitual2",
        "33SummonMFLS",
        "34FindSetosBook",
        "35MakoTheme",
        "36SummonMFLC",
        "37SetoHandsYouBadge",
        "38EndGameYorkist",
        "39N/A",
        "40CustomDuel",
        "41Trade",
        "42Defeat",
        "43Exodia",
        "44MFLRed"
    };

    #endregion

    public static class MusicList
    {
        public static int[] defaultSongs = new int[22] { 7, 10, 8, 8, 8, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9, 9, 9, 9, 9, 11, 12, 44 };
    }

    public MusicEditorWindow()
    {
        LoadDefaultMusic();
        waveOut = new WaveOutEvent();
        waveOut.Volume = 0.1f;
        monoSpaceFont = Fonts.MonoSpace;
    }

    public void LoadMusicFromIso()
    {
        bSaveMusicChanges = new CustomMusicPatch().IsApplied();

        if (bSaveMusicChanges)
        {
            int j = 0;
            byte[] bytes = dataAccess.ReadBytes(TaTuto_DrawTrapArea, 288);
            for (int i = 12; j < 22; i += 12)
            {
                DuelistMusic[j] = bytes[i];
                j++;
            }
        }
        currentTrackIndex = DuelistMusic[currentDuelistSelected] - 1;

        currentSlotMusicIndex = dataAccess.ReadBytes(SlotMusicPatch.SlotTrackPtr, 1)[0] - 1;
    }

    public void Render()
    {
        ImGui.PushFont(monoSpaceFont);
        if (!dataAccess.IsIsoLoaded)
        {
            ImGui.Text("Please load ISO file");
            ImGui.PopFont();
            return;
        }

        Vector2 windowSize = ImGui.GetWindowSize();
        ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.Orange).value);
        ImGui.TextWrapped(
            "Note:\n Duelist music changes require fast intro to be enabled as it uses the spare bytes available from skipping the tutorial, if you enable custom music it will enable fast intro (see Mechanics Editor)");
        ImGui.PopStyleColor();
        if (ImGui.Checkbox("Save music changes", ref bSaveMusicChanges))
        {
            OnSaveCustomMusic?.Invoke(bSaveMusicChanges);
        }
        ImGui.PopFont();
        float remainingHeight = ImGui.GetContentRegionAvail().Y;
        ImGui.BeginChild("LeftThirdPanel", new Vector2(windowSize.X / 3f, remainingHeight),
            ImGuiChildFlags.Border | ImGuiChildFlags.NavFlattened | ImGuiChildFlags.AlwaysAutoResize);

        ImGui.PushFont(monoSpaceFont);
        ImGui.Text("Duelist Music");
        ImGui.PushItemWidth(windowSize.X / 3f);
        if (ImGui.ListBox("DuelistMusic", ref currentDuelistSelected, musicTargets, musicTargets.Length, musicTargets.Length))
        {
            currentTrackIndex = DuelistMusic[currentDuelistSelected] - 1;
            PlaySelectedTrack(currentTrackIndex);
        }
        ImGui.EndChild();

        ImGui.SameLine();


        ImGui.BeginChild("MiddleThirdPanel", new Vector2(windowSize.X / 3f, ImGui.GetContentRegionAvail().Y),
            ImGuiChildFlags.Border | ImGuiChildFlags.NavFlattened | ImGuiChildFlags.AlwaysAutoResize);
        ImGui.Text("Music Tracks");
        ImGui.PushItemWidth(windowSize.X / 3f);
        if (ImGui.ListBox("MusicTrack", ref currentTrackIndex, musicTracks, musicTracks.Length,
                (int)((ImGui.GetContentRegionAvail().Y / ImGui.GetTextLineHeightWithSpacing() - 1))))
        {
            DuelistMusic[currentDuelistSelected] = currentTrackIndex + 1;
            PlaySelectedTrack(currentTrackIndex);
        }
        ImGui.EndChild();

        ImGui.SameLine();
        ImGui.BeginChild("RightThirdPanel", new Vector2(windowSize.X / 3f, remainingHeight),
            ImGuiChildFlags.Border | ImGuiChildFlags.NavFlattened | ImGuiChildFlags.AlwaysAutoResize);
        ImGui.Text("Music Options");
        ImGui.Separator();
        ImGui.Text("Slot Music");
        if (ImGui.ListBox("##SlotMusic", ref currentSlotMusicIndex, musicTracks, musicTracks.Length))
        {
            PlaySelectedTrack(currentSlotMusicIndex);
        }

        if (ImGui.Button(playStopButton, new Vector2(windowSize.X / 3f, windowSize.Y / 8)))
        {
            if (!isMusicPlaying)
            {
                PlaySelectedTrack(currentTrackIndex);
            }
            else
            {
                StopMusic();
            }
        }
        if (ImGui.Button("Extract Music", new Vector2(windowSize.X / 3f, windowSize.Y / 8)))
        {
            
            
            Task.Run(async () => await StartMusicTask());

        }
        if (ImGui.Button("Reset to Default", new Vector2(windowSize.X / 3f, windowSize.Y / 8)))
        {
            LoadDefaultMusic();
            currentTrackIndex = DuelistMusic[currentDuelistSelected] - 1;
        }



        ImGui.PopFont();
        ImGui.EndChild();
    }


    public void Free()
    {

    }

    void LoadDefaultMusic()
    {
        DuelistMusic.Clear();
        for (int i = 0; i < 22; i++)
        {
            DuelistMusic.TryAdd(i, MusicList.defaultSongs[i]);
        }
        currentSlotMusicIndex = 20;
    }


    public async Task StartMusicTask()
    {
        EditorWindow.Disabled = true;
        EditorWindow._modalPopup.Show("Extracting Music Please wait", "Loading Music");
        await Task.Run(() => GetMusicFiles());
        EditorWindow.Disabled = false;
    }

    Task GetMusicFiles()
    {

        if (!Directory.Exists(MusicDirectory))
        {
            Directory.CreateDirectory(MusicDirectory);
        }

        MediaFoundationApi.Startup();
        CDReader isoFile = new CDReader(DataAccess.fileStream, true);
        // Get the file from inside the ISO

        for (int i = 0; i < 5; i++)
        {
            var fileEntry = isoFile.GetFiles($"SOUND\\BGM\\0{i}");
            if (fileEntry == null)
            {
                if (ImGui.BeginPopup("resultFailure"))
                {
                    ImGui.Text("File not found in ISO");
                    if (ImGui.Button("Close"))
                    {
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.EndPopup();
                }
                return Task.CompletedTask;
            }

            // Extract the file to the destination folder
            foreach (var file in fileEntry)
            {
                if (!Path.GetFileName(file).StartsWith("00"))
                    continue;
                string fileName = Path.GetFileName(file);
                string extractedFilePath = Path.Combine("Music", fileName.Substring(0, fileName.Length - 2));
                RawSourceWaveStream pcmReader;
                using (FileStream fileStream = File.OpenWrite(extractedFilePath))
                {
                    var pcm = isoFile.OpenFile(file, FileMode.Open);
                    pcm.CopyTo(fileStream);

                }
                using (FileStream fileStream = File.OpenRead(extractedFilePath))
                {
                    string mp3FilePath = Path.Combine(MusicDirectory, Path.GetFileNameWithoutExtension(file) + ".mp3");
                    var desiredBitRate = 0; // ask for lowest available bitrate
                    pcmReader = new RawSourceWaveStream(fileStream, new WaveFormat(48000, 16, 1));
                    MediaFoundationEncoder.EncodeToMp3(pcmReader, mp3FilePath, desiredBitRate);
                }
            }
        }

        CleanUpPcms();
        MediaFoundationApi.Shutdown();
        return Task.CompletedTask;
    }

    void CleanUpPcms()
    {
        string[] pcmFiles = Directory.GetFiles(MusicDirectory, "*.pcm");
        foreach (string pcmFile in pcmFiles)
        {
            File.Delete(pcmFile);
        }
    }

    void PlaySelectedTrack(int trackNumber)
    {
        StopMusic();
        string selectedMp3FilePath = Path.Combine(MusicDirectory, "00" + musicTracks[trackNumber].Substring(0, 2) + ".mp3");
        if (File.Exists(selectedMp3FilePath))
        {
            mp3Reader = new Mp3FileReader(selectedMp3FilePath);
            playStopButton = "Stop";


            // Play or stop the MP3 based on the current state
            waveOut.Init(mp3Reader);
            waveOut.Play();
            isMusicPlaying = true;
        }
    }

    public void StopMusic()
    {
        if (waveOut.PlaybackState != PlaybackState.Stopped)
        {
            waveOut.Stop();
            mp3Reader?.Dispose();
            isMusicPlaying = false;
            playStopButton = "Start";
        }
    }

    void ChangeSlotMusic()
    {
        new SlotMusicPatch().Apply((byte)(currentSlotMusicIndex + 1));
    }

    public void SaveMusicChanges()
    {
        new CustomMusicPatch().ApplyOrRemove(bSaveMusicChanges, DuelistMusic);
        ChangeSlotMusic();

    }
}