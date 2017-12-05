using System.IO;

namespace Cauldron
{
    internal static class Utils
    {
        /// <summary>
        /// Checks if the filename of <paramref name="path"/> exist. If the file already exists, an
        /// indexer will be added to the filename to make it unique.
        /// </summary>
        /// <param name="path">The path and filename of a file</param>
        /// <returns>A unique and valied path and filename</returns>
        internal static string GetUniqueDirectoryName(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            var directoryName = directoryInfo.FullName;
            var indexer = 1;

            while (Directory.Exists(directoryName))
                directoryName = Path.Combine(directoryInfo.Parent.FullName, $"{directoryInfo.Name} ({indexer++})");

            return directoryName;
        }

        /// <summary>
        /// Checks if the filename of <paramref name="path"/> exist. If the file already exists, an
        /// indexer will be added to the filename to make it unique.
        /// </summary>
        /// <param name="path">The path and filename of a file</param>
        /// <returns>A unique and valied path and filename</returns>
        internal static string GetUniqueFilename(string path)
        {
            var extension = Path.GetExtension(path);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var filePath = Path.GetDirectoryName(path);

            // Check if the filepath exists if not... Exception
            if (!Directory.Exists(filePath))
                throw new DirectoryNotFoundException($"The path '{filePath}' does not exist.");

            var filename = path;
            var indexer = 1;

            while (File.Exists(filename))
                filename = Path.Combine(filePath, $"{filenameWithoutExtension} ({indexer++}){extension}");

            return filename;
        }
    }
}