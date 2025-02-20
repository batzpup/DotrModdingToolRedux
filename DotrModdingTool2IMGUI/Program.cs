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
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.MaximizedWindow);

        Raylib.InitWindow(1920, 1080, "Dotr Modding Tools");
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