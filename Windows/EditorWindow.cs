﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using NativeFileDialogSharp;
using Raylib_cs;
using rlImGui_cs;

namespace DotrModdingTool2IMGUI;

public class EditorWindow
{
    EditorContentMode currentMode = EditorContentMode.EnemyEditor;

    ImGuiIOPtr io = ImGui.GetIO();
    ImFontPtr largerfont;
    ImFontPtr menuBarFont = Fonts.LoadCustomFont("SpaceMonoRegular-JRrmm.ttf", 24);

    public static Vector2 AspectRatio
    {
        get { return ImGui.GetWindowSize() / new Vector2(2560, 1440f); }
    }

    float buttonWidthScaled = 200f;
    float buttonHeightRatio = 100f;
    float buttonSpacingScaled = 10f;

    ImColor backgroundColour;
    EnemyEditorWindow _enemyEditorWindow;
    CardEditorWindow _cardEditorWindow;
    GameplayPatchesWindow _gameplayPatchesWindow;
    FusionEditorWindow _fusionEditorWindow;

    MusicEditorWindow _musicEditorWindow;

    bool isCreditsOpen = false;

    //Data Access
    DataAccess dataAccess = DataAccess.Instance;
    ImGuiModalPopup _modalPopup = new ImGuiModalPopup();
    public static Action OnIsoLoaded;


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
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

        largerfont = io.Fonts.AddFontDefault();
        largerfont.ConfigData.SizePixels = 19;
        ImGui.GetIO().Fonts.Build();
        rlImGui.ReloadFonts();
        _enemyEditorWindow = new EnemyEditorWindow(Fonts.LoadCustomFont(pixelSize: 26), Fonts.LoadCustomFont(pixelSize: 22));
        _cardEditorWindow = new CardEditorWindow();
        _gameplayPatchesWindow = new GameplayPatchesWindow();
        _musicEditorWindow = new MusicEditorWindow();
        _fusionEditorWindow = new FusionEditorWindow();
        _enemyEditorWindow.DeckEditorWindow.ViewCardInEditor += ViewCardInEditor;
    }

    void ViewCardInEditor(int index)
    {
        currentMode = EditorContentMode.CardEditor;
        _cardEditorWindow.SetCurrentCardIndex(index);
    }

    public Dictionary<EditorContentMode, string> ButtonModeTable = new Dictionary<EditorContentMode, string>() {
        { EditorContentMode.EnemyEditor, "Enemy Editor" },
        { EditorContentMode.CardEditor, "Card Editor" },
        { EditorContentMode.FusionEditor, "Fusion Editor" },
        { EditorContentMode.MechanicsEditor, "Mechanics Editor" },
        { EditorContentMode.MusicEditor, "Music Editor" }
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
        ImGui.PopStyleVar(2);
        buttonWidthScaled = Math.Max(200f * AspectRatio.X, 100);
        buttonHeightRatio = Math.Max(100f * AspectRatio.Y, 50);
        buttonSpacingScaled = Math.Max(10f * AspectRatio.X, 5);
        ImGui.Text($"FPS: {ImGui.GetIO().Framerate.ToString()}");

        ImGui.PushFont(menuBarFont);
        if (ImGui.BeginMenuBar())
        {
            ImGui.SetNextItemShortcut(ImGuiKey.ModCtrl | ImGuiKey.O);
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
            ImGui.SetNextItemShortcut(ImGuiKey.ModCtrl | ImGuiKey.S);
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
            ImGui.SetNextItemShortcut(ImGuiKey.ModCtrl | ImGuiKey.T);
            if (ImGui.MenuItem("Toggle Image tooltips", null, ref GlobalImgui.ShowImageHighlight))
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

            ImGui.SetNextItemShortcut(ImGuiKey.ModCtrl | ImGuiKey.P);
            ImGui.MenuItem("Performance Mode", null, ref Program.peformanceMode);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Caps the framerate to 30fps when the window isnt focused\nor an item isnt hovered");
                ImGui.Text("Ctrl + P");
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
                ImGui.PushFont(Fonts.MonoSpace);
                ImGui.Text("Credits: \n");

                if (ImGui.Selectable("Batzpup: I made this", false, ImGuiSelectableFlags.AllowDoubleClick))
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

                ImGui.Spacing();
                if (ImGui.Button("Close"))
                {
                    isCreditsOpen = false; // Close the modal
                }

                ImGui.EndPopup();
                ImGui.PopFont();
            }
          //  if (ImGui.MenuItem("Check for Updates"))
          //  {
          //      Task.Run(async () =>await Updater.CheckForUpdates());
          //  }
            

            ImGui.EndMenuBar();

            ImGui.PopFont();

        }


        DrawLeftPanel();
        ImGui.SameLine();
        ImGui.BeginChild("MainContent", new Vector2(0, 0), ImGuiChildFlags.None);
        RenderMainContent();
        ImGui.EndChild();
        ImGui.Columns(0);
        _modalPopup.Draw(largerfont);

        ImGui.End();

        rlImGui.End();
    }

    void DrawLeftPanel()
    {
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new GuiColour(0, 189, 0).value);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new GuiColour(0, 189, 0).value);
        //Draw stuff

        ImGui.BeginChild("LeftSidePanel", new Vector2(buttonWidthScaled + (buttonSpacingScaled * 2), 0),
            ImGuiChildFlags.Border | ImGuiChildFlags.NavFlattened);
        ImGui.PushFont(menuBarFont);
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
            case EditorContentMode.MechanicsEditor:
                _gameplayPatchesWindow.Render();
                break;
            case EditorContentMode.MusicEditor:
                _musicEditorWindow.Render();
                break;
        }
    }

    void EncodeStrings()
    {
        StringEncoder.Run();
    }

    void OpenIso()
    {
        DialogResult result = Dialog.FileOpen("iso");
        if (result.IsOk)
        {
            string isoPath = result.Path;
            if (result.Path.EndsWith(".iso"))
            {
                DataAccess.Instance.OpenIso(isoPath);
                LoadDataFromIso();
            }
            else
            {
                Console.WriteLine("Should show pop up error");
                _modalPopup.Show("Not an .iso file");
            }
        }
    }

    public void ChangeMode(EditorContentMode mode)
    {
        currentMode = mode;
    }
    void SaveChanges()
    {
        _enemyEditorWindow.DeckEditorWindow.SaveAllDecks();
        _enemyEditorWindow.MapEditorWindow.SaveAllMaps();
        _cardEditorWindow.SaveCardChanges();
        _gameplayPatchesWindow.ApplyPatches();
        _musicEditorWindow.SaveMusicChanges();
        dataAccess.SaveCardDeckLeaderAbilities(CardDeckLeaderAbilities.Bytes);
        dataAccess.SaveMonsterEnchantData(MonsterEnchantData.Bytes);
        dataAccess.SaveDeckLeaderThresholds();
        dataAccess.SaveEnemyAiData(Enemies.AiBytes);
        _fusionEditorWindow.SaveFusionChanges();
        dataAccess.SaveEffectData(Effects.MonsterEffectBytes,Effects.MagicEffectBytes);
        if(!_enemyEditorWindow.DeckEditorWindow.modalPopup.showErrorPopup)
        {
            _modalPopup.Show("Changes have been saved");
        }
    }

    void LoadDataFromIso()
    {
        CardConstant.LoadFromBytes(dataAccess.LoadCardConstantData());
        _enemyEditorWindow.MapEditorWindow.LoadMapData();
        _enemyEditorWindow.MapEditorWindow.LoadTreasureCardData();
        _enemyEditorWindow.DeckEditorWindow.LoadDeckLists();

        _musicEditorWindow.LoadMusicFromIso();
        dataAccess.LoadMonsterEquipCardData();
        dataAccess.LoadCardDeckLeaderAbilities();
        dataAccess.LoadEffectData();
        dataAccess.LoadLeaderThresholdData();
        dataAccess.LoadFusionData();
        dataAccess.LoadEnemyAIData();

        StringDecoder.Run();
        OnIsoLoaded?.Invoke();
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
}