using Cauldron.Core.Reflection;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class Module
{
    public static void ModuleLoad(string[] assemblyNames)
    {
        var theme = ApplicationInfo.ApplicationPath.GetFiles("*.Theme.*.dll").FirstOrDefault();
        if (theme != null && File.Exists(theme.FullName))
            Assembly.LoadFile(theme.FullName);
    }
}