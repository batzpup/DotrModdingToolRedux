using System.Drawing;
using System.Numerics;
using DiscUtils.Iso9660;
using GameplayPatches;
using ImGuiNET;
using NAudio;
using NAudio.MediaFoundation;
using NAudio.Wave;
namespace DotrModdingTool2IMGUI;

public class MusicEditorWindow : IImGuiWindow
{
    
    public static MusicEditorWindow Instance;
    
    //Music
    static int AddCustomMusicPtr = 0x17ac58;
    static int TaTuto_DrawTrapArea = 0x24f800;

    const float PlaybackVolume = 0.1f;
    WaveOutEvent? waveOut;
    Mp3FileReader? mp3Reader;
    bool isMusicPlaying = false;
    bool hasAudioDevice;

    volatile bool audioDeviceLost;
    DateTime nextAudioDeviceCheck = DateTime.MinValue;
    DataAccess dataAccess = DataAccess.Instance;
    public Dictionary<int, int> DuelistMusic = new Dictionary<int, int>();
    string MusicDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Music");
    string playStopButton = "Play";
    int currentDuelistSelected;
    int currentTrackIndex;
    int currentSlotMusicIndex;
    public bool bSaveMusicChanges;
    ImFontPtr font;
    public static Action<bool> OnSaveCustomMusic;


    #region StringsArrays

    public static ModdedStringName[] MusicTargets;

    public static string[] defaultMusicTargets = new[] {
        "Tutorial",
        "Seto",
        "Weevil Underwood",
        "Rex Raptor",
        "Keith",
        "Ishtar",
        "Necromancer",
        "Darkness-ruler",
        "Labyrinth-ruler",
        "Pegasus Crawford",
        "Richard Slysheen of York",
        "Tea",
        "T.Tristan Grey",
        "Margaret Mai Beaufort",
        "Mako",
        "Joey",
        "J.Shadi Morton",
        "Jasper Dice Tudor",
        "Bakura",
        "Yugi",
        "MFL Skull Knight",
        "MFL Chakra"
    };

    public static string[] musicTracks = new[] {
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
        font = FontManager.GetFont(FontManager.FontFamily.NotoSansJP,32);
        Instance = this;
    }

    static MusicEditorWindow()
    {
        MusicTargets = new ModdedStringName[defaultMusicTargets.Length];
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
        ReloadStrings();
    }

    public static void ReloadStrings()
    {
        for (int i = 0; i < MusicTargets.Length; i++)
        {
            if (i > 0 && i < 20)
            {
                MusicTargets[i] = new ModdedStringName(defaultMusicTargets[i], Enemies.EnemyNameList[i].Current);
            }
            else if (i == 20 || i == 21)
            {
                MusicTargets[i] = new ModdedStringName(defaultMusicTargets[i], $"{StringEditor.StringTable[i + StringEditor.DuelistNameOffsetStart]} ({Deck.DeckList[i + 26].DeckLeader.Name.Current})");
            }
            else
            {
                MusicTargets[i] = new ModdedStringName(defaultMusicTargets[i], defaultMusicTargets[i]);
            }

        }
        
    }

    public void Render()
    {
        ImGui.PushFont(font);
        if (!dataAccess.IsIsoLoaded)
        {
            ImGui.Text("Please load ISO file");
            ImGui.PopFont();
            return;
        }

        UpdateAudioDeviceState();
        if (!hasAudioDevice)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.Red).value);
            ImGui.TextWrapped("No audio output device detected. Music preview is disabled until an audio output device is connected.");
            ImGui.PopStyleColor();
        }

        Vector2 windowSize = ImGui.GetWindowSize();
        ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.Orange).value);
        ImGui.TextWrapped(
            "Note:\n Duelist music changes require fast intro to be enabled as it uses the spare bytes available from skipping the tutorial, if you enable custom music it will enable fast intro (see patches)");
        ImGui.PopStyleColor();
        if (ImGui.Checkbox("Save music changes", ref bSaveMusicChanges))
        {
            OnSaveCustomMusic?.Invoke(bSaveMusicChanges);
        }
        ImGui.PopFont();
        float remainingHeight = ImGui.GetContentRegionAvail().Y;
        ImGui.BeginChild("LeftThirdPanel", new Vector2(windowSize.X / 3f, remainingHeight),
            ImGuiChildFlags.Border | ImGuiChildFlags.NavFlattened | ImGuiChildFlags.AlwaysAutoResize);

        ImGui.PushFont(font);
        ImGui.Text("Duelist Music");
        ImGui.PushItemWidth(windowSize.X / 3f);
        if (ImGui.BeginListBox("##DuelistMusic", new Vector2(0, ImGui.GetTextLineHeightWithSpacing() * MusicTargets.Length)))
        {
            for (int i = 0; i < MusicTargets.Length; i++)
            {
                bool isSelected = (currentDuelistSelected == i);
                if (ImGui.Selectable(MusicTargets[i].Current, isSelected))
                {
                    currentDuelistSelected = i;
                    currentTrackIndex = DuelistMusic[currentDuelistSelected] - 1;
                    PlaySelectedTrack(currentTrackIndex);
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndListBox();
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

        ImGui.BeginDisabled(!hasAudioDevice);
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
        ImGui.EndDisabled();
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

    void UpdateAudioDeviceState()
    {
        if (audioDeviceLost)
        {
            audioDeviceLost = false;
            ReleaseAudioOutput();
            nextAudioDeviceCheck = DateTime.MinValue;
        }
     
        if (isMusicPlaying && waveOut?.PlaybackState == PlaybackState.Stopped)
        {
            mp3Reader?.Dispose();
            mp3Reader = null;
            isMusicPlaying = false;
            playStopButton = "Play";
        }
        if (DateTime.Now < nextAudioDeviceCheck)
        {
            return;
        }
        nextAudioDeviceCheck = DateTime.Now.AddSeconds(1);
        hasAudioDevice = WaveInterop.waveOutGetNumDevs() > 0;
        if (!hasAudioDevice)
        {
            ReleaseAudioOutput();
        }
    }

    void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        if (e.Exception != null)
        {
            audioDeviceLost = true;
        }
    }

    void ReleaseAudioOutput()
    {
        if (waveOut != null)
        {
            waveOut.PlaybackStopped -= OnPlaybackStopped;
            try
            {
                waveOut.Dispose();
            }
            catch (MmException)
            {
                
            }
            waveOut = null;
        }
        mp3Reader?.Dispose();
        mp3Reader = null;
        isMusicPlaying = false;
        playStopButton = "Play";
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
        
        var extractedFiles = new List<(string IsoFile, string PcmPath)>();
        lock (DataAccess.FileStreamLock)
        {
            // Own read-only handle so extraction doesn't share DataAccess.fileStream's position with other threads
            using FileStream isoStream = new FileStream(DataAccess.Instance.currentIsoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using CDReader isoFile = new CDReader(isoStream, true);
          
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

                
                foreach (var file in fileEntry)
                {
                    if (!Path.GetFileName(file).StartsWith("00"))
                        continue;
                    string fileName = Path.GetFileName(file);
                    string extractedFilePath = Path.Combine("Music", fileName.Substring(0, fileName.Length - 2));
                    using (FileStream fileStream = File.Create(extractedFilePath))
                    using (var pcm = isoFile.OpenFile(file, FileMode.Open))
                    {
                        pcm.CopyTo(fileStream);
                    }
                    extractedFiles.Add((file, extractedFilePath));
                }
            }
        }

        MediaFoundationApi.Startup();
        foreach (var (file, extractedFilePath) in extractedFiles)
        {
            using (FileStream fileStream = File.OpenRead(extractedFilePath))
            {
                string mp3FilePath = Path.Combine(MusicDirectory, Path.GetFileNameWithoutExtension(file) + ".mp3");
                var desiredBitRate = 0; // ask for lowest available bitrate
                var pcmReader = new RawSourceWaveStream(fileStream, new WaveFormat(48000, 16, 1));
                MediaFoundationEncoder.EncodeToMp3(pcmReader, mp3FilePath, desiredBitRate);
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
        if (!hasAudioDevice)
        {
            return;
        }
        string selectedMp3FilePath = Path.Combine(MusicDirectory, "00" + musicTracks[trackNumber].Substring(0, 2) + ".mp3");
        if (File.Exists(selectedMp3FilePath))
        {
            try
            {
                if (waveOut == null)
                {
                    waveOut = new WaveOutEvent();
                    waveOut.PlaybackStopped += OnPlaybackStopped;
                }
                mp3Reader = new Mp3FileReader(selectedMp3FilePath);
                waveOut.Init(mp3Reader);
                waveOut.Volume = PlaybackVolume;
                waveOut.Play();
                playStopButton = "Stop";
                isMusicPlaying = true;
            }
            catch (MmException)
            {
                ReleaseAudioOutput();
                hasAudioDevice = false;
                nextAudioDeviceCheck = DateTime.Now.AddSeconds(1);
            }
        }
        else
        {
             EditorWindow._modalPopup.Show("No local music found please extract the music from the game", "Music needs extracting");
        }
    }

    public void StopMusic()
    {
        if (waveOut != null && waveOut.PlaybackState != PlaybackState.Stopped)
        {
            try
            {
                waveOut.Stop();
            }
            catch (MmException)
            {
                ReleaseAudioOutput();
                return;
            }
            mp3Reader?.Dispose();
            isMusicPlaying = false;
            playStopButton = "Play";
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