using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var interception = GetProjects("Interception");
            var uwp = GetProjects("UWP");
            var netcore = GetProjects("NetCore");
            var desktop = GetProjects("Desktop");

            BuildProjects(interception);
            BuildProjects(uwp.Concat(netcore).Concat(desktop).Where(x => x.Contains("Interception")));
            BuildProjects(uwp.Concat(netcore).Concat(desktop).Where(x => !x.Contains("Interception")));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Creating NuGet Packages");

            foreach (var item in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "*.nuspec", SearchOption.AllDirectories))
                BuildNuGetPackage(item, args[0]);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private static void BuildProjects(IEnumerable<string> paths)
    {
        foreach (var project in paths)
            BuildProject(project);
    }

    private static void BuildNuGetPackage(string path, string version)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Compiling " + Path.GetFileName(path));

        var filename = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "nuget.exe"));

        var startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = false;
        startInfo.WorkingDirectory = filename.Directory.FullName;
        startInfo.FileName = filename.FullName;
        startInfo.Arguments = string.Format("pack \"{0}\" -OutputDir {1} -version {2}", path, Path.Combine(filename.Directory.FullName, "Packages"), version);
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;

        var process = Process.Start(startInfo);
        var error = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception(error);
    }

    private static void BuildProject(string path)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Compiling " + Path.GetFileName(path));

        var filename = new FileInfo(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.com");

        var startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = false;
        startInfo.WorkingDirectory = filename.Directory.FullName;
        startInfo.FileName = filename.FullName;
        startInfo.Arguments = string.Format("\"{0}\" /Rebuild Release", path);
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;

        var process = Process.Start(startInfo);
        var error = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception(error);
    }

    private static IEnumerable<string> GetProjects(string subFolder)
    {
        var path = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)), subFolder);
        return Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)
            .Where(x => !(Path.GetFileNameWithoutExtension(x).EndsWith(".Test") || x.Contains("\\Samples\\")));
    }
}