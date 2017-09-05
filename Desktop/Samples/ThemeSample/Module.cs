using Cauldron.Core;
using System.IO;
using System.Reflection;

public static class Module
{
    public static void OnLoad(string[] assemblies)
    {
        Assembly.LoadFile(Path.Combine(ApplicationInfo.ApplicationPath.FullName, "Cauldron.XAML.Theme.VSDark.dll"));
    }
}