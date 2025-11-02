using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Text;
using ImGuiNET;
using NativeFileDialogSharp;
using Raylib_cs;
using rlImGui_cs;
using Color = System.Drawing.Color;

namespace DotrModdingTool2IMGUI;

public class EditorWindow
{
    EditorContentMode currentMode = EditorContentMode.EnemyEditor;

    ImGuiIOPtr io = ImGui.GetIO();
    ImFontPtr menuBarFont = FontManager.GetFont(FontManager.FontFamily.NotoSansJP,24);
    ImFontPtr sideBarFont = FontManager.GetFont(FontManager.FontFamily.NotoSansJP,20);
    ImFontPtr mainFont = FontManager.GetFont(FontManager.FontFamily.NotoSansJP,28);
    public static bool Disabled = false;
    float disabledTimer = 0;
    float maxDisabledTime = 30;
    public static Vector2 AspectRatio
    {
        get { return ImGui.GetWindowSize() / new Vector2(2560, 1351f); }
    }

    float buttonWidthScaled = 200f;
    float buttonHeightRatio = 100f;
    float buttonSpacingScaled = 10f;

    EnemyEditorWindow _enemyEditorWindow;
    CardEditorWindow _cardEditorWindow;
    GameplayPatchesWindow _gameplayPatchesWindow;
    FusionEditorWindow _fusionEditorWindow;
    MiscEditorWindow _miscEditorWindow;
    RandomiserWindow _randomiserWindow;
    MusicEditorWindow _musicEditorWindow;
    StringEditorWindow _stringEditorWindow;

    bool isCreditsOpen = false;

    //Data Access
    DataAccess dataAccess = DataAccess.Instance;
    public static ImGuiModalPopup _modalPopup = new ImGuiModalPopup();
    public static Action OnIsoLoaded;
    bool reloadStrings = false;


    public static void PrintEmbeddedResources()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        // Get all the embedded resource names
        string[] resourceNames = assembly.GetManifestResourceNames();
        // Print each resource name
        Console.WriteLine("Embedded Resources:");
        foreach (string resourceName in resourceNames)
        {
            Console.WriteLine(resourceName);
        }
        Console.WriteLine("_________________________");
    }

    public EditorWindow()
    {
        //PrintEmbeddedResources();
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard | ImGuiConfigFlags.DpiEnableScaleFonts | ImGuiConfigFlags.DpiEnableScaleViewports;
        
      
        _enemyEditorWindow = new EnemyEditorWindow(FontManager.GetFont(FontManager.FontFamily.NotoSansJP,26));
        _cardEditorWindow = new CardEditorWindow();
        _gameplayPatchesWindow = new GameplayPatchesWindow();
        _musicEditorWindow = new MusicEditorWindow();
        _fusionEditorWindow = new FusionEditorWindow();
        _randomiserWindow = new RandomiserWindow(_enemyEditorWindow, _musicEditorWindow);
        _stringEditorWindow = new StringEditorWindow();
        _enemyEditorWindow.DeckEditorWindow.ViewCardInEditor += ViewCardInEditor;

        Updater.NeedsUpdate += HandleNeedsUpdate;
        //IS here so you can open an iso before the updater gives you a result
        Disabled = true;
        _modalPopup.Show($"Checking for updates",
            "Updater", null, ImGuiModalPopup.ShowType.NoButton);
        Task.Run(async () =>
        {
            await Updater.CheckForUpdates(true);
            Disabled = false;
           
        });


    }

    void HandleNeedsUpdate(bool needsUpdate, string changes,bool isStartup)
    {
        if (needsUpdate)
        {
            string output = string.Join("\n", changes.Split('\n').Select(line => "- " + line));
            _modalPopup.Show(
                $"An update is available to {Updater.latestVersion} from {Updater.currentVersion}\nChanges:\n{output}\nWould you like to update?",
                "Update", RequestDownload, ImGuiModalPopup.ShowType.YesNo);
        }
        else
        {
            if (!isStartup)
            {
                _modalPopup.Show($"You are up to date, you are on version {Updater.latestVersion}", "Update");
            }
            else
            {
                _modalPopup.Hide();
            }
            
        }

    }

    void RequestDownload()
    {
        Task.Run(async () =>
        {
            Disabled = true;
            _modalPopup.Show($"Downloading Update.\nThe tool will close, then command prompt will appear swap the files, and re-open the tool",
                "Updater");
            await Updater.DownloadUpdate();
            Disabled = false;
        });

    }

    void ViewCardInEditor(ModdedStringName name)
    {
        currentMode = EditorContentMode.CardEditor;
        _cardEditorWindow.SetCurrentCard(name);
    }

    public Dictionary<EditorContentMode, string> ButtonModeTable = new Dictionary<EditorContentMode, string>() {
        { EditorContentMode.EnemyEditor, "Enemy Editor" },
        { EditorContentMode.CardEditor, "Card Editor" },
        { EditorContentMode.FusionEditor, "Fusion Editor" },
        { EditorContentMode.Patches, "Patches" },
        { EditorContentMode.StringEditor, "String Editor" },
        { EditorContentMode.MusicEditor, "Music Editor" },
        { EditorContentMode.Randomiser, "Randomiser" },
    };


    public void Render()
    {
        rlImGui.Begin();
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10f, 10f));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10f, 10f));
        ImGui.Begin("Dotr Modding tool 2",
            ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse);
        ImGui.SetWindowPos(Vector2.Zero);
        ImGui.SetWindowSize(new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()));
        //Console.WriteLine(ImGui.GetWindowSize());
        CheckForHotkeys();
        ImGui.PopStyleVar(2);
        buttonWidthScaled = Math.Max(200f * AspectRatio.X, 100);
        buttonHeightRatio = Math.Max(100f * AspectRatio.Y, 50);
        buttonSpacingScaled = Math.Max(10f * AspectRatio.X, 5);
        
        if(disabledTimer > maxDisabledTime)
        {
            Disabled = false;
            disabledTimer = 0;
        }
        if (Disabled)
        {
            ImGui.BeginDisabled();
            disabledTimer += ImGui.GetIO().DeltaTime;
        }
        ImGui.Text($"FPS: {ImGui.GetIO().Framerate.ToString()}");

        ImGui.PushFont(menuBarFont);
        if (ImGui.BeginMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open Iso"))
                {
                    OpenIso();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Ctrl + O");
                    ImGui.EndTooltip();

                }
                ImGui.Spacing();

                if (!dataAccess.IsIsoLoaded)
                {
                    ImGui.BeginDisabled();
                }

                if (ImGui.MenuItem("Save"))
                {
                    if (dataAccess.IsIsoLoaded)
                    {
                        SaveChanges();
                    }
                    else
                    {
                        _modalPopup.Show("No ISO open to save to");
                    }
                }
                ImGui.Spacing();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Saves All data");
                    ImGui.Text("Ctrl + S");
                    ImGui.EndTooltip();
                }
                if (ImGui.MenuItem("Save Strings"))
                {
                    if (dataAccess.IsIsoLoaded)
                    {
                        SaveStringAsync();
                    }
                    else
                    {
                        _modalPopup.Show("No ISO open to save to");
                    }
                }
                ImGui.Spacing();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Saves string changes");
                    ImGui.EndTooltip();
                }
                if (ImGui.BeginMenu("Import"))
                {
                    if (ImGui.MenuItem("Import Strings"))
                    {
                        DialogResult result = Dialog.FileOpen("json");
                        if (result.IsOk)
                        {
                            if (StringEditor.ImportStringsToJSON(result.Path))
                            {
                                _modalPopup.Show("Successfully imported json", "String Import");

                            }
                            else
                            {
                                _modalPopup.Show("Failed to import json file");
                            }
                        }
                    }
                    if (ImGui.MenuItem("Import Card Data"))
                    {
                        DialogResult result = Dialog.FileOpen("csv");
                        if (result.IsOk)
                        {
                            CardEditorWindow.ImportMonstersFromCSV(result.Path);
                        }
                        else
                        {
                            _modalPopup.Show("Failed to card data");
                        }
                    }
                    if (ImGui.MenuItem("Import Fusion Data"))
                    {
                        DialogResult result = Dialog.FileOpen("csv");
                        if (result.IsOk)
                        {
                            string isoPath = result.Path;
                            if (result.Path.EndsWith(".csv"))
                            {
                                FusionData.ImportFromCSV(isoPath);
                                _fusionEditorWindow.sortedData = FusionData.FusionTableData.ToList();
                            }
                            else
                            {
                                _modalPopup.Show("Failed to import fusion csv");
                            }
                        }
                    }
                    if (ImGui.MenuItem("Import maps"))
                    {
                        var result = Dialog.FileOpen("txt");
                        if (result.IsOk)
                        {
                            Map.ImportMapFile(result.Path);
                        }
                        else
                        {
                            _modalPopup.Show("Failed to import map txt file");
                        }
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Export"))
                {
                    if (ImGui.MenuItem("Export Strings"))
                    {
                        DialogResult result = Dialog.FileSave("json");
                        if (result.IsOk)
                        {
                            string isoPath = result.Path + ".json";
                            StringEditor.ExportStringsToJSON(isoPath);
                        }
                        else
                        {
                             _modalPopup.Show("Failed to export strings");
                        }
                    }
                    if (ImGui.MenuItem("Export Card Data"))
                    {
                        DialogResult result = Dialog.FileSave("csv");
                        if (result.IsOk)
                        {
                            string isoPath = result.Path;
                            CardEditorWindow.ExportMonstersToCSV(isoPath);
                        }
                        else
                        {
                             _modalPopup.Show("Failed to export card data ");
                        }
                    }
                    if (ImGui.MenuItem("Export Fusion Data"))
                    {
                        DialogResult result = Dialog.FileSave("csv");
                        if (result.IsOk)
                        {
                            string isoPath = result.Path;
                            FusionData.ExportToCSV(isoPath);
                        }
                        else
                        {
                             _modalPopup.Show("Failed to export fusion csv");
                        }
                    }
                    if (ImGui.MenuItem("Export Maps"))
                    {
                        var result = Dialog.FileSave("txt");
                        if (result.IsOk)
                        {
                            Map.ExportMapsToFile(result.Path);
                        }
                        else
                        {
                             _modalPopup.Show("Failed to export map text file");
                        }
                    }
                    ImGui.EndMenu();
                }

                if (!dataAccess.IsIsoLoaded)
                {
                    ImGui.EndDisabled();
                }
                ImGui.EndMenu();
            }
            if (ImGui.MenuItem("Toggle Image tooltips", null, ref UserSettings.ToggleImageTooltips))
            {

            }
            ImGui.Spacing();
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Show Images when hovering over items");
                ImGui.Text("Ctrl + T");
                ImGui.EndTooltip();

            }

            ImGui.MenuItem("Performance Mode", null, ref UserSettings.performanceMode);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Caps the framerate to 30fps when the window isnt focused\nor an item isnt hovered");
                ImGui.Text("Ctrl + P");
                ImGui.EndTooltip();
            }
            ImGui.Spacing();
            if (ImGui.MenuItem("Use original strings", null, ref UserSettings.UseDefaultNames))
            {

            }
            ImGui.Spacing();
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Show the games default text for cards and characters");
                ImGui.Text("Ctrl + D");
                ImGui.EndTooltip();

            }

            ImGui.Spacing();
            if (ImGui.MenuItem("Credits"))
            {
                isCreditsOpen = true;

            }
            if (isCreditsOpen)
            {
                ImGui.OpenPopup("Credits");
            }
            if (ImGui.BeginPopupModal("Credits", ref isCreditsOpen, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.PushFont(mainFont);
                CustomImguiTypes.RenderRainbowTextPerChar_Sine("Credits:");

                if (CustomImguiTypes.RenderGradientSelectable
                    ("Batzpup: I made this", false, new GuiColour(Color.BlueViolet).value,
                        new GuiColour(Color.CornflowerBlue).value,
                        ImGuiSelectableFlags.AllowDoubleClick, 0, 7))
                {
                    OpenUrl("https://github.com/batzpup");
                }

                if (ImGui.Selectable("Blayr: Original modding tool developer", false,
                        ImGuiSelectableFlags.AllowDoubleClick))
                {

                    OpenUrl("https://github.com/Blayr");
                }
                if (ImGui.Selectable("This tool is heavily based on Blayr's original tool", false,
                        ImGuiSelectableFlags.AllowDoubleClick))
                {

                    OpenUrl("https://github.com/Blayr/DOTR-Modding-Tool");
                }

                if (ImGui.Selectable("GenericMadScientist: Rom Mapping/Documentation, Reverse Engineering, Image and Text Extraction", false,
                        ImGuiSelectableFlags.AllowDoubleClick))
                {
                    OpenUrl("https://github.com/GenericMadScientist");
                }

                if (ImGui.Selectable("Clovis: Redux, All Cards, and Redux 2 Mod Author", false, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    OpenUrl("https://www.youtube.com/@ClovissenpaiDotR");
                }

                if (ImGui.Selectable("hippochan: Original Map Editor and Starter Deck Image Fixes", false, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    OpenUrl("https://github.com/rjoken");
                }
                if (ImGui.Selectable("LordMewTwo73 for the text editing capability", false, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    OpenUrl("https://github.com/LordMewtwo73/YGO-DOTR-Text-Editor");
                }

                ImGui.TextColored(new GuiColour(Color.Cyan).value, "Code Contributors:");
                ImGui.Text("MonoCh");

                ImGui.Spacing();
                if (ImGui.Button("Close"))
                {
                    isCreditsOpen = false;
                }

                ImGui.EndPopup();
                ImGui.PopFont();
            }
            ImGui.Spacing();
            if (ImGui.MenuItem("Check for Updates"))
            {
                Task.Run(async () =>
                {
                    Disabled = true;
                    await Updater.CheckForUpdates(false);
                    Disabled = false;
                });
            }
            ImGui.Spacing();
            ImGui.EndMenuBar();
            ImGui.PopFont();

        }
        if (isSavingStrings)
        {
            _modalPopup.Show("Compressing strings", "Saving strings", null, ImGuiModalPopup.ShowType.NoButton);
        }

        DrawLeftPanel();
        ImGui.SameLine();
        ImGui.BeginChild("MainContent", new Vector2(0, 0), ImGuiChildFlags.None);

        if (reloadStrings)
        {
            StringEditor.ReloadFromISO();
            reloadStrings = false;
        }

        
        RenderMainContent();


        ImGui.EndChild();
        ImGui.Columns(0);
        if (Disabled)
        {
            ImGui.EndDisabled();
        }
        else
        {
            disabledTimer = 0;
        }
        _modalPopup.Draw(mainFont);


        ImGui.End();
        rlImGui.End();
    }

    


    void SaveStringAsync(Action onComplete = null, bool showMessage = true)
    {
        if (!isSavingStrings)
        {
            Task.Run(() =>
            {
                isSavingStrings = true;
                Disabled = true;
                SaveStrings(showMessage);
                isSavingStrings = false;
                reloadStrings = true;
                Disabled = false;
                onComplete?.Invoke();
            });
        }

    }

    void CheckForHotkeys()
    {
        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.S, ImGuiInputFlags.RouteGlobal))
        {
            SaveChanges();
        }
        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.O, ImGuiInputFlags.RouteGlobal))
        {
            OpenIso();
        }
        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.T, ImGuiInputFlags.RouteGlobal))
        {
            UserSettings.ToggleImageTooltips = !UserSettings.ToggleImageTooltips;
        }
        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.P, ImGuiInputFlags.RouteGlobal))
        {
            UserSettings.performanceMode = !UserSettings.performanceMode;
        }
        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.D, ImGuiInputFlags.RouteGlobal))
        {
            UserSettings.UseDefaultNames = !UserSettings.UseDefaultNames;
        }
        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.U, ImGuiInputFlags.RouteGlobal))
        {
            Task.Run(async () =>
            {
                Disabled = true;
                await Updater.CheckForUpdates(false);
                Disabled = false;

            });
        }
        if (ImGui.Shortcut(ImGuiKey.ModCtrl | ImGuiKey.C, ImGuiInputFlags.RouteGlobal))
        {
            isCreditsOpen = true;
        }

    }

    void DrawLeftPanel()
    {
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new GuiColour(0, 189, 0).value);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new GuiColour(0, 189, 0).value);
        //Draw stuff

        ImGui.BeginChild("LeftSidePanel", new Vector2(buttonWidthScaled + (buttonSpacingScaled * 2), 0),
            ImGuiChildFlags.Border | ImGuiChildFlags.NavFlattened | ImGuiChildFlags.AlwaysUseWindowPadding);
        ImGui.PushFont(FontManager.GetBestFitFont("fusion editor",false,FontManager.FontFamily.NotoSansJP));
        foreach (var modeButtonPair in ButtonModeTable)
        {
            if (currentMode == modeButtonPair.Key)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new GuiColour(0, 189, 0).value);
            }
            if (ImGui.Button(modeButtonPair.Value, new Vector2(buttonWidthScaled, buttonHeightRatio)))
            {
                if (currentMode == EditorContentMode.MusicEditor)
                {
                    _musicEditorWindow.StopMusic();
                }
                currentMode = modeButtonPair.Key;
            }
            if (currentMode == modeButtonPair.Key)
            {
                ImGui.PopStyleColor();
            }
            ImGui.Dummy(new Vector2(0, buttonSpacingScaled));
        }
        ImGui.PopStyleColor(2);
        ImGui.PopFont();
        ImGui.EndChild();
    }

    void RenderMainContent()
    {
        ImageHelper.DefaultImageSize = new Vector2( ImGui.GetWindowSize().X / 18f, ImGui.GetWindowSize().X / 18f);
        switch (currentMode)
        {
            case EditorContentMode.EnemyEditor:
                _enemyEditorWindow.Render();
                break;
            case EditorContentMode.CardEditor:
                _cardEditorWindow.Render();
                break;
            case EditorContentMode.FusionEditor:
                _fusionEditorWindow.Render();
                break;
            case EditorContentMode.Patches:
                _gameplayPatchesWindow.Render();
                break;
            case EditorContentMode.MusicEditor:
                _musicEditorWindow.Render();
                break;
            case EditorContentMode.StringEditor:
                _stringEditorWindow.Render();
                break;
            case EditorContentMode.Misc:
                _miscEditorWindow.Render();
                break;
            case EditorContentMode.Randomiser:
                _randomiserWindow.Render();
                break;

        }
    }


    void OpenIso()
    {
        DialogResult result;
        if (string.IsNullOrEmpty(UserSettings.LastIsoPath))
        {
            result = Dialog.FileOpen("iso");
        }
        else
        {
            result = Dialog.FileOpen("iso", UserSettings.LastIsoPath);

        }
        if (result.IsOk)
        {
            string isoPath = result.Path;
            if (result.Path.EndsWith(".iso") || result.Path.EndsWith(".ISO"))
            {
                DataAccess.Instance.OpenIso(isoPath);
                LoadDataFromIso();
                UserSettings.LastIsoPath = Path.GetDirectoryName(isoPath);
            }
            else
            {
                Console.WriteLine("Should show pop up error");
                _modalPopup.Show("Not an .iso file");
            }
        }
    }

    bool isSavingStrings = false;
    int stringCompressionProgress;

    public void SaveChanges()
    {
        if (!dataAccess.IsIsoLoaded)
        {
            _modalPopup.Show("No ISO loaded to save");
            return;
        }
        _enemyEditorWindow.DeckEditorWindow.SaveAllDecks();
        _enemyEditorWindow.MapEditorWindow.SaveAllMaps();
        _cardEditorWindow.SaveCardChanges();
        _gameplayPatchesWindow.ApplyPatches();
        _musicEditorWindow.SaveMusicChanges();
        dataAccess.SaveCardDeckLeaderAbilities(CardDeckLeaderAbilities.Bytes);
        dataAccess.SaveMonsterEnchantData(MonsterEnchantData.Bytes);
        dataAccess.SaveDeckLeaderThresholds();
        dataAccess.SaveEnemyAiData(Enemies.AiBytes);
        dataAccess.SaveEnchantData();
        _fusionEditorWindow.SaveFusionChanges();
        dataAccess.SaveEffectData(Effects.MonsterEffectBytes, Effects.MagicEffectBytes);
        UserSettings.SaveSettings();
        SaveStringAsync(printComplete, false);
        //StringEditor.ReloadFromISO();


    }

    void printComplete()
    {
        if (!_enemyEditorWindow.DeckEditorWindow.modalPopup.showErrorPopup)
        {
            _modalPopup.Show("Changes have been saved", "Save successful");
        }
    }

    public bool SaveStrings(bool showMessage = true)
    {
        Card.RebuildStringCache();
        for (int i = 0; i < Card.cardNameList.Length; i++)
        {
            StringEditor.StringTable[i + StringEditor.CardNamesOffsetStart] = Card.cardNameList[i].Edited;
        }
        stringCompressionProgress = 0;
        StringEditor.StringEncoder.CompressStrings(StringEditor.StringTable.Values.ToList(), ref stringCompressionProgress);
        StringEditor.StringEncoder.ExportToBytes();
        if (StringEditor.StringBytes.Count > DataAccess.TotalTextLength)
        {
            _modalPopup.Show("Too many bytes to save. Reduce string count or length", "Save failed");
            return false;
        }
        dataAccess.SaveStringData();
        if (showMessage)
        {
            _modalPopup.Show("Strings saved successfully", "Save success");
        }

        return true;
    }


    void LoadDataFromIso()
    {
        StringEditor.Run();
        CardConstant.LoadFromBytes(dataAccess.LoadCardConstantData());
        dataAccess.LoadEnemyAIData();
        dataAccess.LoadDecksData();
        _enemyEditorWindow.MapEditorWindow.LoadMapData();
        _enemyEditorWindow.MapEditorWindow.LoadTreasureCardData();
        _enemyEditorWindow.DeckEditorWindow.UpdateDeckData();
        _musicEditorWindow.LoadMusicFromIso();
        dataAccess.LoadMonsterEquipCardData();
        dataAccess.LoadCardDeckLeaderAbilities();
        dataAccess.LoadEffectData();
        dataAccess.LoadLeaderThresholdData();
        dataAccess.LoadFusionData();
        dataAccess.LoadEnchantData();
        Map.Initialise();
        OnIsoLoaded?.Invoke();
        StringEditor.ReloadStrings();

    }

    void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to open URL: " + ex.Message);
        }
    }

    public static void CreateCSVFromMonsterEffects(List<MonsterEffects> monsterEffectsList)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            "AttackEffectName,AttackSearchMode,MovementEffectName,MovementSearchMode,NatureEffectName,NatureSearchMode,FlipEffectName,FlipSearchMode,DestructionEffectName,DestructionSearchMode");

        foreach (var monsterEffect in monsterEffectsList)
        {
            var row = new List<string>();

            foreach (var effect in monsterEffect.Effects)
            {
                string effectData = effect.effectName;
                string searchModeData = effect.SearchModeName;

                row.Add(effectData); // Effect Name column
                row.Add(searchModeData); // Search Mode Name column
            }
            sb.AppendLine(string.Join(",", row));
        }

        File.WriteAllText("MonsterEffects.csv", sb.ToString());
        Console.WriteLine("CSV file created successfully.");
    }
}