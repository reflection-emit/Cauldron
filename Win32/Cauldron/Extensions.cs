using System.IO;
using System.Text;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods for the <see cref="DirectoryInfo"/> and <see cref="FileInfo"/> class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the short path format of the given path. e.g. C:\\Hello World converted to C:\\HelloWo~1
        /// </summary>
        /// <param name="path">The long path</param>
        /// <returns>The short path</returns>
        /// <exception cref="IOException">Unable to shortend path of a directory or a file that does not exist.</exception>
        public static string GetShortPath(this DirectoryInfo path) => GetShortPath(path.FullName);

        /// <summary>
        /// Returns the short path format of the given path. e.g. C:\\Hello World converted to C:\\HelloWo~1
        /// </summary>
        /// <param name="path">The long path</param>
        /// <returns>The short path</returns>
        /// <exception cref="IOException">Unable to shortend path of a directory or a file that does not exist.</exception>
        public static string GetShortPath(this FileInfo path) => GetShortPath(path.FullName);

        /// <summary>
        /// Determines whether a path represents a network resource.
        /// </summary>
        /// <param name="path">The path to test</param>
        /// <returns>True if the given path is a network path; otherwise false</returns>
        public static bool IsNetworkPath(this DirectoryInfo path) => UnsafeNative.PathIsNetworkPath(path.FullName);

        private static string GetShortPath(string longPath)
        {
            if (!File.Exists(longPath) && !Directory.Exists(longPath))
                throw new IOException("Unable to shortend path of a directory or a file that does not exist.: " + longPath);

            var shortPath = new StringBuilder(255);
            UnsafeNative.GetShortPathName(longPath, shortPath, 255);

            return shortPath.ToString();
        }
    }
}