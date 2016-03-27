using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Couldron.Core
{
    /// <summary>
    /// Provides retrieving and caching of embedded images
    /// </summary>
    public static class EmbeddedImageManager
    {
        private static ConcurrentDictionary<string, BitmapImage> image = new ConcurrentDictionary<string, BitmapImage>();

        /// <summary>
        /// Returns an embedded image
        /// <para/>
        /// Returns null if the <paramref name="resourceInfoName"/> was not found
        /// </summary>
        /// <param name="resourceInfoName">The end of the fully qualified name of the embedded resource</param>
        /// <returns>The embedded image</returns>
        public static BitmapImage GetImage(string resourceInfoName)
        {
            var assemblyAndResourceInfo = AssemblyUtil.AssemblyAndResourceNamesInfo.FirstOrDefault(x => x.Filename.EndsWith(resourceInfoName, StringComparison.OrdinalIgnoreCase));

            if (assemblyAndResourceInfo == null)
                return null;

            if (image.ContainsKey(assemblyAndResourceInfo.Filename))
                return image[assemblyAndResourceInfo.Filename];

            var result = assemblyAndResourceInfo.Assembly.GetManifestResourceStream(assemblyAndResourceInfo.Filename).ToBitmapImage();
            image.TryAdd(assemblyAndResourceInfo.Filename, result);

            return result;
        }
    }
}