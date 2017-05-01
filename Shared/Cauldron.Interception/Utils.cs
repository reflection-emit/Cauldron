using System.ComponentModel;
using System.Reflection;

#if DESKTOP

using System.IO;

#endif

namespace Cauldron.Interception
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Utils
    {
#if DESKTOP

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void TryLoadAssembly(Assembly assembly, string assemblyName)
        {
            var path = Path.Combine(Path.GetDirectoryName(assembly.Location), assemblyName);

            //if (File.Exists(path))
            //    Assembly.LoadFile(path);

            string location = Assembly.GetAssembly(typeof(Utils)).Location;
        }

#endif
    }
}