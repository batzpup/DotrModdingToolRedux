using System.Diagnostics;
using System.Text;
namespace Updater;

class Program
{
    static string[] filesToNotReplace = new[] { "Updater.deps.json", "Updater.dll", "Updater.exe", "Updater.runtimeconfig.json" };

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
        string tempLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DotrUpdaterStarted.txt");
        using (File.Create(tempLocation))
        {
        }
        LogToFile("Start StandAlone updater logs");
        LogToFile("Updater started", tempLocation);
        LogToFile($"Current directory: {Environment.CurrentDirectory}", tempLocation);
        LogToFile($"BaseDirectory/Updater Directory: {AppDomain.CurrentDomain.BaseDirectory}", tempLocation);
        LogToFile($"Target directory: {args[0]}", tempLocation);
        LogToFile($"Main process ID: {args[1]}", tempLocation);
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
            {
                argString.AppendLine(arg);
            }
            LogToFile($"Expected 2 arguments, got {args.Length} {argString} ", tempLocation);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        string targetDir;
        int mainProcessId;
        try
        {
            targetDir = args[0];
            mainProcessId = int.Parse(args[1]);
            LogToFile($"Creating file in: {args[0]}", tempLocation);
            File.Create(Path.Combine(targetDir, "UpdaterWorks.txt"));
            LogToFile($"UpdaterWorks.txt created at : {args[0]}", tempLocation);
        }
        catch (Exception e)
        {
            LogToFile($"Error with arguments: {e.Message}");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            throw;
        }
        LogToFile($"$moving log from{LogFile} to {Path.Combine(Path.Combine(targetDir, "Logs"), "standalone-updater.txt")}", tempLocation);
        try
        {
            string logsFile = Path.Combine(Path.Combine(targetDir, "Logs"), "standalone-updater.txt");
            if (File.Exists(logsFile))
            {
                File.Delete(logsFile);
            }
            File.Move(LogFile, logsFile);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            throw;
        }
        LogDirectory = Path.Combine(targetDir, "Logs");
        LogFile = Path.Combine(LogDirectory, "standalone-updater.txt");

        try
        {
            LogToFile($"waiting for modding tool to close");
            Process mainProcess = Process.GetProcessById(mainProcessId);
            mainProcess.WaitForExit();

        }
        catch
        {
            LogToFile("Main application already closed.");
            Console.WriteLine("Main application already closed.");
        }
        Thread.Sleep(1000);
        //THIS IS IN TEMP DIRECTORY
        string updaterDir = AppDomain.CurrentDomain.BaseDirectory;
        foreach (var file in Directory.GetFiles(updaterDir))
        {
            if (!filesToNotReplace.Contains(Path.GetFileName(file)))
            {
                string destFile = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
                Console.WriteLine($"Updated: {destFile}");
                LogToFile($"Updated: {file} saved to  {destFile}");
            }
        }
        string mainExe = Path.Combine(targetDir, "DotrModdingTool2IMGUI.exe");
        LogToFile($"Starting DotrModdingTool2IMGUI.exe ");
        Process.Start(new ProcessStartInfo {
            FileName = mainExe,
            UseShellExecute = true
        });
        LogToFile($"update complete ");
    }
}