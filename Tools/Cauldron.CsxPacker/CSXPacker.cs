using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using VSLangProj80;

namespace Cauldron.CsxPacker
{
    [ComVisible(true)]
    [Guid("7BC64680-F71D-4F15-908D-C668884B0B54")]
    [CodeGeneratorRegistration(typeof(CSXPacker), "CSX to CSXGZ Packer", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(CSXPacker), "CSX to CSXGZ Packer", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(CSXPacker), "CSX to CSXGZ Packer", vsContextGuids.vsContextGuidVJSProject, GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(CSXPacker))]
    public class CSXPacker : IVsSingleFileGenerator
    {
        private string solutionPath;
        private OutputWindowPane windowPane;

        public CSXPacker()
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            this.solutionPath = Path.GetDirectoryName(dte.Solution.FullName);
            var window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            var outputWindow = window.Object as OutputWindow;
            this.windowPane = outputWindow.OutputWindowPanes.Add("CSXPacker");
            this.windowPane.Clear();
        }

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".cauldron";
            return VSConstants.S_OK;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            try
            {
                var csc = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET\\Framework"), "csc.exe", SearchOption.AllDirectories)
                    .Select(x => new { Path = x, Version = new Version(Path.GetFileName(Path.GetDirectoryName(x)).Substring(1)) })
                    .MaxVersion(x => x.Version);

                if (csc == null)
                {
                    pcbOutput = 0;
                    return VSConstants.S_FALSE;
                }

                var code =
                    "using Cauldron.Interception.Cecilator;          " +
                    "using Cauldron.Interception.Fody;               " +
                    "using Cauldron.Interception.Fody.HelperTypes;   " +
                    "using System.Collections;                       " +
                    "using System.Collections.Generic;               " +
                    "using System.ComponentModel;                    " +
                    "using System.Diagnostics;                       " +
                    "using System.Collections.Concurrent;            " +
                    "using System;                                   " +
                    "using System.IO                                 " +
                    "using System.Threading.Tasks;                   " +
                    "using System.Linq;                              " +
                    "using Reflection = System.Reflection;           " +
                    "using System.Runtime.CompilerServices;          " +
                    "using System.Runtime.InteropServices;           " +
                    "using Mono.Cecil;                               " +
                    "public sealed class Interceptor { " +
                        bstrInputFileContents +
                    "}";

                var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                var tempCode = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(wszInputFilePath) + ".cs");
                var output = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(wszInputFilePath) + ".dll");
                Directory.CreateDirectory(tempDirectory);
                File.WriteAllText(tempCode, code);

                var references = new List<string>();
                var toolsPath = Path.Combine(this.solutionPath, "Tools\\Cauldron.Interception.Fody.99.9.9");

                if (Directory.Exists(toolsPath))
                {
                    references.Add(Path.Combine(toolsPath, "Mono.Cecil.dll"));
                    references.Add(Path.Combine(toolsPath, "Cauldron.Interception.Fody.dll"));
                    references.Add(Path.Combine(toolsPath, "Cauldron.Interception.Cecilator.dll"));
                }
                else
                {
                    var packages = Path.Combine(this.solutionPath, "packages");
                    var cauldronInterception = GetFolderWithHighestVersion(packages, "Cauldron.Interception.Fody");

                    references.Add(Path.Combine(cauldronInterception, "Mono.Cecil.dll"));
                    references.Add(Path.Combine(cauldronInterception, "Cauldron.Interception.Fody.dll"));
                    references.Add(Path.Combine(cauldronInterception, "Cauldron.Interception.Cecilator.dll"));
                }

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = csc.Path,
                    Arguments = $"/t:library /out:\"{output}\" /optimize+  \"{tempCode}\" /r:{string.Join(",", references.Select(x => "\"" + x + "\""))}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = tempDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = new System.Diagnostics.Process
                {
                    StartInfo = processStartInfo
                };

                this.windowPane.OutputString(processStartInfo.FileName + "\r\n");
                this.windowPane.OutputString(processStartInfo.Arguments + "\r\n\r\n");

                process.OutputDataReceived += (s, e) => { this.windowPane.OutputString("\r\n" + e.Data); };
                process.ErrorDataReceived += (s, e) => { this.windowPane.OutputString("\r\n" + e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                var content = File.ReadAllBytes(output);
                pcbOutput = (uint)content.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(content.Length);
                Marshal.Copy(content, 0, rgbOutputFileContents[0], content.Length);

                Directory.Delete(tempDirectory, true);

                return VSConstants.S_OK;
            }
            catch (Exception)
            {
                pcbOutput = 0;
                return VSConstants.S_FALSE;
            }
        }

        private string GetFolderWithHighestVersion(string path, string name) =>
                    Directory.GetDirectories(path, name + "*", SearchOption.TopDirectoryOnly)
                        .Select(x =>
                        {
                            var splitted = Path.GetFileName(x).Split('.');
                            var version = new Version(
                                Convert.ToInt32(splitted[splitted.Length - 3]),
                                Convert.ToInt32(splitted[splitted.Length - 2]),
                                Convert.ToInt32(splitted[splitted.Length - 1].Split('-')[0]), 0);
                            return new { Path = x, Version = version };
                        })
                        .MaxVersion(x => x.Version)
                        .Path;
    }

    internal static class Extensions
    {
        public static TSource MaxVersion<TSource>(this IEnumerable<TSource> source, Func<TSource, Version> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (selector == null)
                throw new ArgumentNullException("selector");

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");

                var max = sourceIterator.Current;
                var maxKey = selector(max);

                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);

                    if (candidateProjected > maxKey)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }

                return max;
            }
        }
    }
}