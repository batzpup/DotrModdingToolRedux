using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Updater;

class Program
{
    static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    static readonly string MainExeFile = IsWindows ? "DotrModdingTool2IMGUI.exe" : "DotrModdingTool2IMGUI";

    // Files belonging to the updater itself — never overwrite these while running
    static readonly string[] filesToNotReplace = new[] {
        "Updater.deps.json",
        "Updater.dll",
        IsWindows ? "Updater.exe" : "Updater",
        "Updater.runtimeconfig.json"
    };

    static string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
    static string LogFile = Path.Combine(LogDirectory, "standalone-updater.txt");

    static Program()
    {
        if (!Directory.Exists(LogDirectory))
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);
                LogToFile("Log directory created");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create log directory: {ex.Message}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }

    static void LogToFile(string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string fullMessage = $"{timestamp} [StandaloneUpdater] {message}";

        try
        {
            File.AppendAllText(LogFile, fullMessage + Environment.NewLine);
            Console.WriteLine(fullMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
            Console.WriteLine(fullMessage);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    static void LogToFile(string message, string location)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string fullMessage = $"{timestamp} [StandaloneUpdater] {message}";

        try
        {
            File.AppendAllText(location, fullMessage + Environment.NewLine);
            Console.WriteLine(fullMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
            Console.WriteLine(fullMessage);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            throw;
        }
    }

    static void Main(string[] args)
    {
        Console.Title = "DotrModdingTool Updater";

        string tempLocation = Path.Combine(Path.GetTempPath(), "DotrUpdaterStarted.txt");
        using (File.Create(tempLocation))
        {
        }

        LogToFile("Start StandAlone updater logs");
        LogToFile("Updater started", tempLocation);
        LogToFile($"Current directory: {Environment.CurrentDirectory}", tempLocation);
        LogToFile($"BaseDirectory/Updater Directory: {AppDomain.CurrentDomain.BaseDirectory}", tempLocation);
        LogToFile($"Platform: {RuntimeInformation.OSDescription}", tempLocation);

        if (args.Length < 2)
        {
            Console.WriteLine("Invalid arguments. Expected: <target_directory> <process_id>");
            LogToFile($"Expected 2 arguments, got {args.Length}", tempLocation);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        if (args.Length > 2)
        {
            Console.WriteLine("Too many arguments. Expected: <target_directory> <process_id>");
            StringBuilder argString = new StringBuilder();
            foreach (var arg in args)
                argString.AppendLine(arg);
            LogToFile($"Expected 2 arguments, got {args.Length}: {argString}", tempLocation);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        LogToFile($"Target directory: {args[0]}", tempLocation);
        LogToFile($"Main process ID: {args[1]}", tempLocation);

        string targetDir;
        int mainProcessId;

        try
        {
            targetDir = args[0];
            mainProcessId = int.Parse(args[1]);
            LogToFile($"Creating UpdaterWorks.txt in: {targetDir}", tempLocation);
            File.Create(Path.Combine(targetDir, "UpdaterWorks.txt")).Dispose();
            LogToFile($"UpdaterWorks.txt created at: {targetDir}", tempLocation);
        }
        catch (Exception e)
        {
            LogToFile($"Error with arguments: {e.Message}");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            throw;
        }

        // Move the log file into the target app's Logs directory
        string targetLogDir = Path.Combine(targetDir, "Logs");
        string targetLogFile = Path.Combine(targetLogDir, "standalone-updater.txt");
        LogToFile($"Moving log from {LogFile} to {targetLogFile}", tempLocation);

        try
        {
            if (!Directory.Exists(targetLogDir))
                Directory.CreateDirectory(targetLogDir);

            if (File.Exists(targetLogFile))
                File.Delete(targetLogFile);

            File.Move(LogFile, targetLogFile);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            throw;
        }

        LogDirectory = targetLogDir;
        LogFile = targetLogFile;

        // Wait for the main application process to exit before overwriting files
        try
        {
            LogToFile("Waiting for main application to close...");
            Process mainProcess = Process.GetProcessById(mainProcessId);
            mainProcess.WaitForExit();
        }
        catch
        {
            LogToFile("Main application already closed.");
            Console.WriteLine("Main application already closed.");
        }

        Thread.Sleep(1000);

        // Copy all updated files from the temp updater directory into the target directory,
        // skipping the updater's own files to avoid overwriting ourselves while running.
        string updaterDir = AppDomain.CurrentDomain.BaseDirectory;
        foreach (var file in Directory.GetFiles(updaterDir))
        {
            if (!filesToNotReplace.Contains(Path.GetFileName(file)))
            {
                string destFile = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
                Console.WriteLine($"Updated: {destFile}");
                LogToFile($"Updated: {file} -> {destFile}");
            }
        }
        //Update runtimes folder for OS specific dependencies, im looking at you SkisSharp and Raylib linux
        string runtimesSrc = Path.Combine(updaterDir, "runtimes");
        string runtimesDst = Path.Combine(targetDir, "runtimes");
        if (Directory.Exists(runtimesSrc))
        {
            LogToFile($"Copying runtimes directory: {runtimesSrc} -> {runtimesDst}");
            CopyDirectory(runtimesSrc, runtimesDst);
        }

        string mainExe = Path.Combine(targetDir, MainExeFile);
        if (!IsWindows && File.Exists(mainExe))
        {
            SetExecutable(mainExe);
        }


        try
        {
            File.Delete(tempLocation);
        }
        catch
        {
            LogToFile($"Failed to Delete Temp data");
        }

        LogToFile($"Launching {MainExeFile}");

        try
        {
            Process.Start(new ProcessStartInfo {
                FileName = mainExe,
                UseShellExecute = IsWindows // UseShellExecute = false is more reliable on Linux
            });
        }
        catch (Exception ex)
        {
            LogToFile($"Failed to launch main application: {ex.Message}");
            Console.WriteLine($"Failed to launch {mainExe}: {ex.Message}");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        LogToFile("Update complete.");
    }

    static void CopyDirectory(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(targetDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
            LogToFile($"Updated: {file} -> {destFile}");
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            CopyDirectory(dir, Path.Combine(targetDir, Path.GetFileName(dir)));
        }
    }

    /// <summary>
    /// Sets the executable bit on Linux/macOS.
    /// Uses File.SetUnixFileMode on .NET 7+, falls back to shelling out to chmod.
    /// </summary>
    static void SetExecutable(string filePath)
    {
        try
        {
#if NET7_0_OR_GREATER
            var mode = File.GetUnixFileMode(filePath);
            mode |= UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute;
            File.SetUnixFileMode(filePath, mode);
            LogToFile($"Set executable bit via UnixFileMode on: {filePath}");
#else
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
            LogToFile($"Failed to set executable bit on {filePath}: {ex.Message}");
        }
    }
}