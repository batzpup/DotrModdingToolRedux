using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
namespace DotrModdingTool2IMGUI;

class Program
{
    public static void Main(string[] args)
    {
        CrashLogger.Register();
        try
        {
            Run();
        }
        catch (Exception ex)
        {
            CrashLogger.HandleCrash(ex);
            Environment.Exit(1);
        }
    }

    static void Run()
    {
        UserSettings.LoadSettings();
        AppDomain.CurrentDomain.DomainUnload += (sender, _) =>
        {
            Console.WriteLine("Saving settings");
            UserSettings.SaveSettings();
            Console.WriteLine("Settings saved");
        };
        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);

        /* Scaling the window on the monitor size for compatibility ;
           Raylib does not properly call window size before InitWindow. */
        Raylib.InitWindow(0, 0, "DotR Modding Tool Redux");
        int monitorWidth = Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor());
        int monitorHeight = Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor());
        int screenWidth  = monitorWidth * 2 / 3;   // 1920x1080 > 1280x720
        int screenHeight = monitorHeight * 2 / 3;
        int screenXPos = monitorWidth * 1 / 6;     // Monitor screen center
        int screenYPos = monitorHeight * 1 / 6;
        Raylib.SetWindowSize(screenWidth, screenHeight);
        Raylib.SetWindowPosition(screenXPos, screenYPos);
        // Scaling minimum size as a bonus.
        Raylib.SetWindowMinSize(screenWidth / 2, screenHeight / 2);
        
        Raylib.SetExitKey(KeyboardKey.Null);
        Image iconImage = ImageHelper.LoadImageRaylib("Images.redRoseLeader.png");
        Raylib.SetWindowIcon(iconImage);
        Raylib.UnloadImage(iconImage);
        Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor()));
        
        rlImGui.Setup(true);
        FontManager.LoadFonts();
        rlImGui.ReloadFonts();
        //Raylib.MaximizeWindow();
        GlobalImages.Instance.LoadAllImages();

        
        EditorWindow editorWindow = new EditorWindow();
        int lastFps = Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor());
        while (!Raylib.WindowShouldClose())
        {
            if (UserSettings.performanceMode)
            {
                int targetFps = Raylib.IsWindowFocused() || ImGui.IsAnyItemHovered() ? Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor()) : 30;

                if (targetFps != lastFps)
                {
                    Raylib.SetTargetFPS(targetFps);
                    lastFps = targetFps;
                }
            }
            if (Raylib.IsWindowResized() && !Raylib.IsWindowFullscreen())
            {
                screenWidth = Raylib.GetScreenWidth();
                screenHeight = Raylib.GetScreenHeight();
                Raylib.SetWindowSize(screenWidth, screenHeight);
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && (Raylib.IsKeyDown(KeyboardKey.LeftAlt) || Raylib.IsKeyDown(KeyboardKey.RightAlt)))
            {
                if (Raylib.IsWindowFullscreen())
                {
                    //Undo Full Screen mode, THEN Restore Size
                    Raylib.ToggleFullscreen();
                    Raylib.SetWindowSize(screenWidth, screenHeight);
                }
                else
                {
                    //Max Screen Size, then go Full Screen mode
                    Raylib.SetWindowSize(monitorWidth, monitorHeight);
                    Raylib.ToggleFullscreen();
                }
            }


            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);
            editorWindow.Render();
            Raylib.EndDrawing();
        }
        UserSettings.SaveSettings();
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}