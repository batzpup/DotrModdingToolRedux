using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
namespace DotrModdingTool2IMGUI;

class Program
{
    public static void Main(string[] args)
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
        int screenWidth = 1280;
        int screenHeight = 720;

        Raylib.InitWindow(screenWidth, screenHeight, "Dotr Modding Tools");
        Raylib.SetExitKey(KeyboardKey.Null);
        Image iconImage = ImageHelper.LoadImageRaylib("Images.redRoseLeader.png");
        Raylib.SetWindowIcon(iconImage);
        Raylib.UnloadImage(iconImage);
        Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor()));
        rlImGui.Setup(true);
        Raylib.MaximizeWindow();
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
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && (Raylib.IsKeyDown(KeyboardKey.LeftAlt) || Raylib.IsKeyDown(KeyboardKey.RightAlt)))
            {
                int display = Raylib.GetCurrentMonitor();
                if (Raylib.IsWindowFullscreen())
                {
                    Raylib.SetWindowSize(screenWidth, screenHeight);
                }
                else
                {
                    Raylib.SetWindowSize(Raylib.GetMonitorWidth(display), Raylib.GetMonitorHeight(display));
                }
                Raylib.ToggleFullscreen();
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