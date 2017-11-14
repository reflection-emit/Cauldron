using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CauldronBuilder
{
    internal static class Program
    {
        private static CauldronBuilderData data;

        public static void Main(string[] args)
        {
            var startingLocation = new DirectoryInfo(args == null || args.Length == 0 ? Path.GetDirectoryName(Path.GetFileName(typeof(Program).Assembly.Location)) : args[0]);
            data = CauldronBuilderData.GetConfig(startingLocation);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Version {data.CurrentPackageVersion} / {data.CurrentAssemblyVersion}");
            Console.WriteLine($"Path {startingLocation.FullName}");
            Console.ResetColor();

            try
            {
                var packages = new DirectoryInfo(Path.Combine(startingLocation.FullName, "Nuget\\Packages"));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Do you want to build the projects? Y/N");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");

                    var allProjectFiles = Directory.GetFiles(startingLocation.FullName, "*.csproj", SearchOption.AllDirectories)
                        .Select(x => new FileInfo(x))
                        .Where(x => !x.Name.Contains(".Test") && x.Name.StartsWith("Cauldron"))
                        .ToArray();
                    var solutionPath = new FileInfo(Path.Combine(startingLocation.FullName, "Cauldron.sln"));

                    foreach (var project in allProjectFiles)
                        ChangeVersion(project);

                    for (int i = 0; i < allProjectFiles.Length; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Compiling [{i + 1}/{allProjectFiles.Length}] " + allProjectFiles[i].Name);
                        BuildProject(solutionPath, allProjectFiles[i]);
                    }

                    // Clean the package directory
                    foreach (var file in Directory.GetFiles(packages.FullName, "*.nupkg"))
                        File.Delete(file);

                    // Move all packages to the Package directory
                    foreach (var nuget in Directory.GetFiles(startingLocation.FullName, "*.nupkg", SearchOption.AllDirectories)
                        .Where(x => x.Contains("\\Release\\")))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Moving " + Path.GetFileName(nuget));
                        File.Move(nuget, Path.Combine(startingLocation.FullName, "Nuget\\Packages", Path.GetFileName(nuget)));
                    }

                    data.IncrementAndSave();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Do you want to build the Nuget packages? Y/N");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    // Build nugets of non NetStandard2.0 projects
                    foreach (var nuget in Directory.GetFiles(Path.Combine(startingLocation.FullName, "Nuget"), "*.nuspec").Select(x => new FileInfo(x)))
                        BuildNuGetPackage(nuget.FullName, packages.FullName);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Do you want to upload the packages? Y/N");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");

                    foreach (var package in Directory.GetFiles(packages.FullName, "*.nupkg", SearchOption.TopDirectoryOnly))
                        UploadNugetPackage(package);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.GetStackTrace());
                Console.WriteLine("\r\n\r\nCancelled");
                Console.Read();
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void BuildNuGetPackage(string path, string targetDirectory)
        {
            var filename = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "nuget.exe"));

            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetDirectoryName(path);
            startInfo.FileName = filename.FullName;
            startInfo.Arguments = string.Format("pack \"{0}\" -OutputDir Packages -version {2}", path, targetDirectory,
                data.IsBeta ? data.CurrentPackageVersion + "-beta" : data.CurrentPackageVersion);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Compiling " + startInfo.Arguments);

            var process = Process.Start(startInfo);
            var error = process.StandardError.ReadToEnd();
            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            Console.WriteLine(output);

            if (process.ExitCode != 0)
                throw new Exception(error);
        }

        private static void BuildProject(FileInfo solutionPath, FileInfo path)
        {
            var filename = new FileInfo(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe");

            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = path.DirectoryName;
            startInfo.FileName = filename.FullName;
            startInfo.Arguments = string.Format("\"{1}\" /target:Clean;Rebuild /p:Configuration=Release", solutionPath.FullName, path.FullName);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            var process = Process.Start(startInfo);
            var error = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception(error);
        }

        private static void ChangeVersion(FileInfo path)
        {
            var projectFileBody = File.ReadAllText(path.FullName);
            if (projectFileBody.StartsWith(@"<Project Sdk=""Microsoft.NET.Sdk"">")) // This is a NetStandard project
            {
                var packageVersion = projectFileBody.EnclosedIn("<Version>", "</Version>");
                if (packageVersion != null)
                    projectFileBody = projectFileBody.Replace(packageVersion, $"<Version>{(data.IsBeta ? data.CurrentPackageVersion + "-beta" : data.CurrentPackageVersion)}</Version>");

                var assemblyVersion = projectFileBody.EnclosedIn("<AssemblyVersion>", "</AssemblyVersion>");
                if (assemblyVersion != null)
                    projectFileBody = projectFileBody.Replace(assemblyVersion, $"<AssemblyVersion>{data.CurrentAssemblyVersion}</AssemblyVersion>");
                else
                    Insert(ref projectFileBody, $"<AssemblyVersion>{data.CurrentAssemblyVersion}</AssemblyVersion>\r\n");

                var fileVersion = projectFileBody.EnclosedIn("<FileVersion>", "</FileVersion>");
                if (fileVersion != null)
                    projectFileBody = projectFileBody.Replace(fileVersion, $"<FileVersion>{data.CurrentAssemblyVersion}</FileVersion>");
                else
                    Insert(ref projectFileBody, $"<FileVersion>{data.CurrentAssemblyVersion}</FileVersion>\r\n");

                File.WriteAllText(path.FullName, projectFileBody);
            }
            else
            {
                // Lets look for AssemblyInfo.cs
                var assemblyInfo = Path.Combine(path.Directory.FullName, "Properties\\AssemblyInfo.cs");
                if (!File.Exists(assemblyInfo))
                    return;

                var assemblyInfoBody = File.ReadAllText(assemblyInfo);

                var assemblyVersion = assemblyInfoBody.EnclosedIn("[assembly: AssemblyVersion(\"", "\")]");
                if (assemblyVersion != null)
                    assemblyInfoBody = assemblyInfoBody.Replace(assemblyVersion, $"[assembly: AssemblyVersion(\"{data.CurrentAssemblyVersion}\")]");

                var fileVersion = assemblyInfoBody.EnclosedIn("[assembly: AssemblyFileVersion(\"", "\")]");
                if (fileVersion != null)
                    assemblyInfoBody = assemblyInfoBody.Replace(fileVersion, $"[assembly: AssemblyFileVersion(\"{data.CurrentAssemblyVersion}\")]");

                File.WriteAllText(assemblyInfo, assemblyInfoBody);
            }
        }

        private static string EnclosedIn(this string target, string start, string end)
        {
            if (string.IsNullOrEmpty(target))
                return null;

            int startPos = target.IndexOf(start);

            if (startPos < 0)
                return null;

            int endPos = target.IndexOf(end, startPos + 1);

            if (endPos <= startPos)
                return null;

            return target.Substring(startPos, endPos - startPos + end.Length);
        }

        private static string GetStackTrace(this Exception e)
        {
            var sb = new StringBuilder();
            var ex = e;

            do
            {
                sb.AppendLine("Exception Type: " + ex.GetType().Name);
                sb.AppendLine("Source: " + ex.Source);
                sb.AppendLine(ex.Message);
                sb.AppendLine("------------------------");
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine("------------------------");

                ex = ex.InnerException;
            } while (ex != null);

            return sb.ToString();
        }

        private static void Insert(ref string body, string data)
        {
            int position = body.IndexOf("</PropertyGroup>");

            if (position < 0)
                return;

            body = body.Insert(position, data);
        }

        private static void UploadNugetPackage(string path)
        {
            var filename = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "nuget.exe"));

            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetDirectoryName(path);
            startInfo.FileName = filename.FullName;
            startInfo.Arguments = string.Format("push \"{0}\" -Source https://www.nuget.org/api/v2/package", path);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Uploading " + startInfo.Arguments);
            Console.ResetColor();

            var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(output);

            if (process.ExitCode != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ResetColor();
            }
        }
    }
}