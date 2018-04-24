using System;
using System.IO;
using System.Runtime.CompilerServices;

var path = Args[0];

if (!File.Exists(path))
    return;

var content = File.ReadAllText(path);
var directory = Path.GetDirectoryName(GetCurrentFileName());
var projectDir = Path.GetDirectoryName(path);

if (content.IndexOf("Cauldron.Activator") >= 0)
{
    File.Copy(Path.Combine(directory, @"..\Fody\Cauldron.Interception.Fody\Scripts\activator-cauldron.fody.cauldron"), Path.Combine(projectDir, "activator-cauldron.fody.cauldron"), true);
}

if (content.IndexOf("Cauldron.Win32.WPF") >= 0)
{
    File.Copy(Path.Combine(directory, @"..\Fody\Cauldron.Interception.Fody\Scripts\wpf-baml-init.cauldron.fody.cauldron"), Path.Combine(projectDir, "wpf-baml-init.cauldron.fody.cauldron"), true);
    File.Copy(Path.Combine(directory, @"..\Fody\Cauldron.Interception.Fody\Scripts\wpf-interception.cauldron.fody.cauldron"), Path.Combine(projectDir, "wpf-interception.cauldron.fody.cauldron"), true);
}

private static string GetCurrentFileName([CallerFilePath] string fileName = null) => fileName;