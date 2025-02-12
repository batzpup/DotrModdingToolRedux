namespace DotrModdingTool2IMGUI;

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

public static class Updater
{
    static readonly string repoOwner = "batzpup";
    static readonly string repoName = "DotrModdingToolRedux";
    static readonly string currentVersion = "v1.0.2-beta";
    static readonly string updaterFile = "Updater.exe";
    static readonly string newFile = "MyApp_New.exe";

    public static async Task CheckForUpdates()
    {
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# App");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json"); // Recommended

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
            string latestVersion = latestRelease.GetProperty("tag_name").GetString();
            string downloadUrl = latestRelease.GetProperty("assets")[0].GetProperty("browser_download_url").GetString();

            Console.WriteLine($"Latest Version: {latestVersion}");
            Console.WriteLine($"Download URL: {downloadUrl}");
            if (latestVersion != currentVersion)
            {
                Console.WriteLine("Downloading new version");
                await DownloadUpdate(downloadUrl, $"DotrModdingToolRedux{latestVersion}.zip");
            }
            else
            {
                Console.WriteLine("Program is up to date");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error checking for updates: {ex.Message}");
        }
    }


    static async Task DownloadUpdate(string url, string filename)
    {
        try
        {
            using HttpClient client = new HttpClient();
            byte[] data = await client.GetByteArrayAsync(url);

            await File.WriteAllBytesAsync(filename, data);
            Console.WriteLine("Download complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading update: {ex.Message}");
            OpenUrl(url);
        }
    }

    static void ExtractUpdater()
    {
        // Extract Updater.exe from embedded resources
        using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MyNamespace.Resources.Updater.exe");
        using FileStream fileStream = new FileStream(updaterFile, FileMode.Create, FileAccess.Write);
        resourceStream.CopyTo(fileStream);
    }

    static void StartUpdater()
    {
        string currentExe = Process.GetCurrentProcess().MainModule.FileName;
        string arguments = $"\"{currentExe}\" \"{newFile}\"";

        Process.Start(new ProcessStartInfo {
            FileName = updaterFile,
            Arguments = arguments,
            UseShellExecute = false
        });

        Environment.Exit(0); // Close the app so it can be replaced
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