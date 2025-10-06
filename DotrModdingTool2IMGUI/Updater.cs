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
    public static readonly string currentVersion = "v1.2.4-beta";
    static readonly string updaterFile = "Updater.exe";
    public static string latestVersion;
    static string downloadUrl;
    static string body;
    public static Action<bool, string?> NeedsUpdate;


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
        //TODO make this cross platform
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }
        string extractPath = Path.Combine(Path.GetTempPath(), "UpdaterTemp");
        LogToFile($"Extract path: {extractPath}");
        if (!Directory.Exists(extractPath))
        {
            Directory.CreateDirectory(extractPath);
        }
        string[] filesToReplace = new[] { "Updater.deps.json", "Updater.dll", "Updater.exe", "Updater.runtimeconfig.json" };
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
                LogToFile($"No releases found.");
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
                NeedsUpdate?.Invoke(true, body);
            }
            else
            {
                if (!isStartup)
                {
                    NeedsUpdate.Invoke(false, string.Empty);
                }
                Console.WriteLine("Program is up to date");
                LogToFile("Program is up to date");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error checking for updates: {ex.Message}");
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
                LogToFile($"Download complete");
            }
            catch (Exception e)
            {
                OpenUrl(downloadUrl);
                Console.WriteLine($"Error downloading update: {e.Message}");
                LogToFile($"Error downloading update: {e.Message}");
            }

            string extractPath = Path.Combine(Path.GetTempPath(), "UpdaterTemp");
            LogToFile($"Extraction path = {extractPath}");
            if (Directory.Exists(extractPath))
            {
                LogToFile($"{extractPath} already exists deleting old temp data");
                Directory.Delete(extractPath, true);
            }
            ZipFile.ExtractToDirectory(filename, extractPath);
            string updaterPath = Path.Combine(extractPath, updaterFile);
            LogToFile($"Updater path = {updaterPath}");
            if (File.Exists(updaterPath))
            {
                LogToFile(
                    $"Starting new process with\nFilename {updaterPath}\nArguments:\n ModdingToolDirectory {AppDomain.CurrentDomain.BaseDirectory}\n Process Id: {Environment.ProcessId}");
                //THIS STARTS THE DOWNLOADED UPDATER
                Process.Start(new ProcessStartInfo() {
                    FileName = updaterPath,
                      Arguments = $"\"{AppDomain.CurrentDomain.BaseDirectory}\" {Environment.ProcessId}",
                    UseShellExecute = true
                });
                Environment.Exit(0);
            }
            else
            {
                LogToFile("Failed to find updater.exe");
                throw new Exception("Failed to find Updater.exe");
            }
        }
        catch (Exception ex)
        {
            LogToFile($"Error downloading update: {ex.Message}");
            Console.WriteLine($"Error downloading update: {ex.Message}");
        }
    }

    static void OpenUrl(string url)
    {
        try
        {
            LogToFile($"Opening url {url}");
            Process.Start(new ProcessStartInfo {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to open URL: " + ex.Message);
            LogToFile("Failed to open URL: " + ex.Message);
        }
    }
}