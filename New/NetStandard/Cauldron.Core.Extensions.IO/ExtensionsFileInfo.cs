using System;
using System.IO;
using System.Threading.Tasks;

namespace Cauldron
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

        /// <summary>
        /// Checks if the filename exist. If the file already exists, an indexer will be added to the filename to make it unique.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns>A unique and valid path and filename.</returns>
        public static FileInfo GetUniqueFilename(this FileInfo file) => new FileInfo(Utils.GetUniqueFilename(file.FullName));

        /// <summary>
        /// Waits for a file to be accessable. The default waiting period is 1.5s.
        /// </summary>
        /// <param name="fileInfo">The path of the file</param>
        /// <returns>true if successful, otherwise false</returns>
        public static async Task<bool> WaitUntilFileIsAccessableAsync(this FileInfo fileInfo) =>
            await WaitUntilFileIsAccessableAsync(fileInfo, 3, TimeSpan.FromSeconds(1.5));

        /// <summary>
        /// Waits for a file to be accessable. The default waiting period is 1.5s.
        /// </summary>
        /// <param name="fileInfo">The path of the file</param>
        /// <param name="tries">The total count of attempts to access the file</param>
        /// <returns>true if successful, otherwise false</returns>
        public static async Task<bool> WaitUntilFileIsAccessableAsync(this FileInfo fileInfo, uint tries) =>
            await WaitUntilFileIsAccessableAsync(fileInfo, tries, TimeSpan.FromSeconds(1.5));

        /// <summary>
        /// Waits for a file to be accessable.
        /// </summary>
        /// <param name="fileInfo">The path of the file</param>
        /// <param name="tries">The total count of attempts to access the file</param>
        /// <param name="timeBetweenTries">The amount of time to wait between attempts</param>
        /// <returns>true if successful, otherwise false</returns>
        public static async Task<bool> WaitUntilFileIsAccessableAsync(this FileInfo fileInfo, uint tries, TimeSpan timeBetweenTries)
        {
            var tryCounter = 0;

            do
            {
                try
                {
                    using (var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                    }

                    break;
                }
                catch (IOException)
                {
                }

                tryCounter++;

                await Task.Delay(timeBetweenTries);
            } while (tryCounter < tries);

            return tryCounter < tries;
        }
    }
}