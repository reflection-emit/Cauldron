using System;
using System.IO;
using System.Threading.Tasks;

namespace Cauldron.Core.Extensions
{
    /// <summary>
    /// Provides usefull extension methods for the <see cref="FileInfo"/> class
    /// </summary>
    public static class ExtensionsFileInfo
    {
        /// <summary>
        /// Deletes the current file.
        /// </summary>
        /// <param name="file">The file to delete</param>
        /// <returns>No object or value is returned by this method when it completes.</returns>
        public static Task DeleteAsync(this FileInfo file)
        {
            file.Delete();
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the timestamp of the last time the file was modified. (Wrapper for <see
        /// cref="FileSystemInfo.LastAccessTime"/> to match with UWP)
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>The timestamp.</returns>
        public static Task<DateTime> GetDateModifiedAsync(this FileInfo file) => Task.FromResult(file.LastAccessTime);
    }
}