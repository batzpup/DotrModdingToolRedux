using System.IO.Compression;
using System.Runtime.InteropServices;

namespace DotrModdingTool2IMGUI;

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public static class Updater
{
    static readonly string repoOwner = "batzpup";
    static readonly string repoName = "DotrModdingToolRedux";
    public static readonly string currentVersion = "v1.2.7-beta";

    // Platform-aware executable names
    static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    static readonly string UpdaterFile = IsWindows ? "Updater.exe" : "Updater";
    static readonly string MainExeFile = IsWindows ? "DotrModdingTool2IMGUI.exe" : "DotrModdingTool2IMGUI";

    public static string latestVersion;
    static string downloadUrl;
    static string body;
    public static Action<bool, string?, bool> NeedsUpdate;

    static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
    static readonly string LogFile = Path.Combine(LogDirectory, "ModdingUpdaterCSLog.txt");

    static Updater()
    {
        if (!Directory.Exists(LogDirectory))
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);
                LogToFile("Updater initialized - Log directory created");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create log directory: {ex.Message}");
            }
        }
    }

    static void LogToFile(string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string fullMessage = $"{timestamp} [Updater] {message}";

        try
        {
            File.AppendAllText(LogFile, fullMessage + Environment.NewLine);
            Console.WriteLine(fullMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
            Console.WriteLine(fullMessage);
        }
    }

    public static async Task CheckForUpdates(bool isStartup = false)
    {
        string extractPath = Path.Combine(Path.GetTempPath(), "UpdaterTemp");
        LogToFile($"Extract path: {extractPath}");

        if (!Directory.Exists(extractPath))
        {
            Directory.CreateDirectory(extractPath);
        }

        // Replace updater files if a previous update extracted them
        string[] filesToReplace = new[]
        {
            "Updater.deps.json",
            "Updater.dll",
            UpdaterFile,
            "Updater.runtimeconfig.json"
        };

        foreach (var file in Directory.GetFiles(extractPath))
        {
            if (filesToReplace.Contains(Path.GetFileName(file)))
            {
                string destFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(file));
                File.Copy(file, destFile, true);
                LogToFile($"Updated: {destFile}");
                Console.WriteLine($"Updated: {destFile}");
            }
        }

        Directory.Delete(extractPath, true);

        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# App");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");

            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases";
            string response = await client.GetStringAsync(url);

            using JsonDocument json = JsonDocument.Parse(response);
            var releases = json.RootElement.EnumerateArray();

            if (!releases.Any())
            {
                LogToFile("No releases found.");
                Console.WriteLine("No releases found.");
                return;
            }

            var latestRelease = releases.First();
            latestVersion = latestRelease.GetProperty("tag_name").GetString();
            downloadUrl = latestRelease.GetProperty("assets")[0].GetProperty("browser_download_url").GetString();
            body = latestRelease.GetProperty("body").GetString();

            Console.WriteLine($"Latest Version: {latestVersion}");
            Console.WriteLine($"Download URL: {downloadUrl}");
            LogToFile($"Latest Version: {latestVersion}");
            LogToFile($"Download URL: {downloadUrl}");

            if (latestVersion != currentVersion)
            {
                Console.WriteLine("Update Available");
                LogToFile("Update Available");
                NeedsUpdate?.Invoke(true, body, isStartup);
            }
            else
            {
                NeedsUpdate?.Invoke(false, string.Empty, isStartup);
                Console.WriteLine("Program is up to date");
                LogToFile("Program is up to date");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error checking for updates: {ex.Message}");
            LogToFile($"Error checking for updates: {ex.Message}");
        }
    }

    public static async Task DownloadUpdate()
    {
        Console.WriteLine("Downloading new version");
        string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"DotrModdingToolRedux{latestVersion}.zip");
        LogToFile($"Downloading version {latestVersion} to {filename}");

        try
        {
            using HttpClient client = new HttpClient();
            byte[] data = await client.GetByteArrayAsync(downloadUrl);

            try
            {
                LogToFile($"Downloading from {downloadUrl}");
                await File.WriteAllBytesAsync(filename, data);
                Console.WriteLine("Download complete!");
                LogToFile("Download complete");
            }
            catch (Exception e)
            {
                OpenUrl(downloadUrl);
                Console.WriteLine($"Error saving download: {e.Message}");
                LogToFile($"Error saving download: {e.Message}");
                return;
            }

            string extractPath = Path.Combine(Path.GetTempPath(), "UpdaterTemp");
            LogToFile($"Extraction path = {extractPath}");

            if (Directory.Exists(extractPath))
            {
                LogToFile($"{extractPath} already exists, deleting old temp data");
                Directory.Delete(extractPath, true);
            }

            ZipFile.ExtractToDirectory(filename, extractPath);

            string updaterPath = Path.Combine(extractPath, UpdaterFile);
            LogToFile($"Updater path = {updaterPath}");

            if (File.Exists(updaterPath))
            {
                // On Linux/macOS, zip archives don't preserve Unix execute permissions,
                // so we must set them manually before launching.
                if (!IsWindows)
                {
                    SetExecutable(updaterPath);
                }

                string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
                LogToFile($"Starting updater: {updaterPath} \"{baseDir}\" {Environment.ProcessId}");

                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterPath,
                    Arguments = $"\"{baseDir}\" {Environment.ProcessId}",
                    UseShellExecute = IsWindows // UseShellExecute can be unreliable on Linux
                });

                Environment.Exit(0);
            }
            else
            {
                LogToFile("Failed to find updater executable");
                throw new Exception($"Failed to find {UpdaterFile}");
            }
        }
        catch (Exception ex)
        {
            LogToFile($"Error downloading update: {ex.Message}");
            Console.WriteLine($"Error downloading update: {ex.Message}");
        }
    }


    static void SetExecutable(string filePath)
    {
        try
        {
            // .NET 7+ approach — no external process needed
#if NET7_0_OR_GREATER
            var mode = File.GetUnixFileMode(filePath);
            mode |= UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute;
            File.SetUnixFileMode(filePath, mode);
            LogToFile($"Set executable bit via UnixFileMode on: {filePath}");
#else
            // Fallback: shell out to chmod
            Process.Start(new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = $"+x \"{filePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            })?.WaitForExit();
            LogToFile($"Set executable bit via chmod on: {filePath}");
#endif
        }
        catch (Exception ex)
        {
            LogToFile($"Failed to set executable bit: {ex.Message}");
        }
    }

    static void OpenUrl(string url)
    {
        try
        {
            LogToFile($"Opening URL: {url}");

            ProcessStartInfo psi;
            if (IsWindows)
            {
                psi = new ProcessStartInfo { FileName = url, UseShellExecute = true };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                psi = new ProcessStartInfo { FileName = "open", Arguments = url, UseShellExecute = false };
            }
            else
            {
                // Linux — xdg-open is the standard cross-desktop launcher
                psi = new ProcessStartInfo { FileName = "xdg-open", Arguments = url, UseShellExecute = false };
            }

            Process.Start(psi);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to open URL: " + ex.Message);
            LogToFile("Failed to open URL: " + ex.Message);
        }
    }
}