using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
namespace Updater;

class Program
{
    static string[] filesToNotReplace = new[] { "Updater.deps.json", "Updater.dll", "Updater.exe", "Updater.runtimeconfig.json" };

    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Invalid arguments.");
            return;
        }
        string targetDir = args[0];
        int mainProcessId = int.Parse(args[1]);
        Console.WriteLine($"targetDir{targetDir}");
        Console.WriteLine($"Process ID{mainProcessId}");
        try
        {
            Process mainProcess = Process.GetProcessById(mainProcessId);
            mainProcess.WaitForExit();
        }
        catch
        {
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
            }
        }
        string mainExe = Path.Combine(targetDir, "DotrModdingTool2IMGUI.exe");
        Process.Start(new ProcessStartInfo {
            FileName = mainExe,
            UseShellExecute = true
        });
    }
}