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
    public static readonly string currentVersion = "v1.1.8-beta";
    static readonly string updaterFile = "Updater.exe";
    public static string latestVersion;
    static string downloadUrl;
    static string body;
    public static Action<bool,string?> NeedsUpdate;

    public static async Task CheckForUpdates(bool isStartup = false)
    {
        //TODO make this cross platform
        if( !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
            
        }
            
        string extractPath = Path.Combine(Path.GetTempPath(), "UpdaterTemp");
        if (Directory.Exists(extractPath))
        {
            string[] filesToReplace = new[] { "Updater.deps.json", "Updater.dll", "Updater.exe", "Updater.runtimeconfig.json" };
            foreach (var file in Directory.GetFiles(extractPath))
            {
                string destFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(file));

                if (filesToReplace.All(fileName => Path.GetFileName(file) == fileName))
                {
                    File.Copy(file, destFile, true);
                    Console.WriteLine($"Updated: {destFile}");
                }
            }
            Directory.Delete(extractPath, true);
        }
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
                Console.WriteLine("No releases found.");
                return;
            }

            var latestRelease = releases.First();
            latestVersion = latestRelease.GetProperty("tag_name").GetString();
            downloadUrl = latestRelease.GetProperty("assets")[0].GetProperty("browser_download_url").GetString();
            body = latestRelease.GetProperty("body").GetString();

            Console.WriteLine($"Latest Version: {latestVersion}");
            Console.WriteLine($"Download URL: {downloadUrl}");
            if (latestVersion != currentVersion)
            {
                
                Console.WriteLine("Update Available");
                NeedsUpdate?.Invoke(true,body);
            }
            else
            {
                if (!isStartup)
                {
                    NeedsUpdate.Invoke(false,string.Empty);
                }
                Console.WriteLine("Program is up to date");
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
        try
        {
            using HttpClient client = new HttpClient();
            byte[] data = await client.GetByteArrayAsync(downloadUrl);

            try
            {
                await File.WriteAllBytesAsync(filename, data);
                Console.WriteLine("Download complete!");
            }
            catch (Exception e)
            {
                OpenUrl(downloadUrl);
                Console.WriteLine($"Error downloading update: {e.Message}");
            }

            string extractPath = Path.Combine(Path.GetTempPath(), "UpdaterTemp");
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }
            ZipFile.ExtractToDirectory(filename, extractPath);
            string updaterPath = Path.Combine(extractPath, updaterFile);
            if (File.Exists(updaterPath))
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                Process.Start(new ProcessStartInfo() {
                    FileName = updaterPath,
                    Arguments = $"{AppDomain.CurrentDomain.BaseDirectory} {Environment.ProcessId}",
                    UseShellExecute = false
                });
                Environment.Exit(0);
            }
            else
            {
                throw new Exception("Failed to find Updater.exe");
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading update: {ex.Message}");
        }
    }

    static void OpenUrl(string url)
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