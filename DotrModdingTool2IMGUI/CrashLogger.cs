using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
namespace DotrModdingTool2IMGUI;

public static class CrashLogger
{
    static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
    static int _handled;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    static extern int MessageBoxW(IntPtr hWnd, string text, string caption, uint type);

    public static void Register()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleCrash(e.ExceptionObject as Exception);
    }

    public static void HandleCrash(Exception? ex)
    {
        if (Interlocked.Exchange(ref _handled, 1) == 1)
            return;

        string? logPath = WriteLog(ex);

        try
        {
            UserSettings.SaveSettings();
        }
        catch (Exception saveEx)
        {
            Console.WriteLine($"Failed to save settings after crash: {saveEx.Message}");
        }
        string logResult = logPath != null ? $"A crash log was saved to:\n{logPath}" : "The crash log could not be saved.";
        string message = $"The tool has crashed:\n\n{ex?.GetType().Name}: {ex?.Message}\n\n {logResult}";
        ShowMessage(message);
    }

    static string? WriteLog(Exception? ex)
    {
        try
        {
            Directory.CreateDirectory(LogDirectory);
            string path = Path.Combine(LogDirectory, $"Crash_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine($"Version: {Updater.currentVersion}");
            sb.AppendLine($"OS: {RuntimeInformation.OSDescription}");
            sb.AppendLine($"Runtime: {RuntimeInformation.FrameworkDescription}");
            sb.AppendLine();
            sb.AppendLine(ex?.ToString() ?? "Unknown exception");

            File.WriteAllText(path, sb.ToString());
            Console.WriteLine(sb.ToString());
            return path;
        }
        catch (Exception logEx)
        {
            Console.WriteLine($"Failed to write crash log: {logEx.Message}");
            Console.WriteLine(ex);
            return null;
        }
    }

    static void ShowMessage(string message)
    {
        const string title = "Dotr Modding Tools - Crash";
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //MB_OK | MB_ICONERROR
                MessageBoxW(IntPtr.Zero, message, title, 0x00000010);
            }
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo("zenity") { UseShellExecute = false };
                psi.ArgumentList.Add("--error");
                psi.ArgumentList.Add($"--title={title}");
                psi.ArgumentList.Add($"--text={message}");
                Process.Start(psi)?.WaitForExit();
            }
        }
        catch (Exception)
        {
            Console.WriteLine(message);
        }
    }
}