using System;
using System.IO;
using System.Security;
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
        /// Renames a file.
        /// </summary>
        /// <param name="fileInfo">The file to rename.</param>
        /// <param name="newName">The new name of the file.</param>
        /// <returns>A new instance of <see cref="FileInfo"/> representing the renamed file.</returns>
        /// <exception cref="FileNotFoundException"><paramref name="fileInfo"/> does not exist.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="newName"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="newName"/> is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <exception cref="IOException">An I/O error occurs, such as the destination file already exists or the destination device is not ready.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException"><paramref name="newName"/> is read-only or is a directory.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="NotSupportedException"><paramref name="newName"/> contains a colon (:) in the middle of the string.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters,
        /// and file names must be less than 260 characters.
        /// </exception>
        public static FileInfo Rename(this FileInfo fileInfo, string newName)
        {
            if (!File.Exists(fileInfo.FullName))
                throw new FileNotFoundException("fileInfo does not exist.");

            if (newName == null)
                throw new ArgumentNullException(nameof(newName));

            if (newName == "")
                throw new ArgumentException(nameof(newName));

            var newFile = Path.Combine(fileInfo.DirectoryName, newName);
            fileInfo.MoveTo(newFile);

            return new FileInfo(newFile);
        }

        /// <summary>
        /// Converts a string to a <see cref="FileInfo"/>
        /// </summary>
        /// <param name="filename">
        /// The fully qualified name of the new file, or the relative file name. Do not end the path
        /// with the directory separator character.
        /// </param>
        /// <returns>An instance of <see cref="FileInfo"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="ArgumentException">
        /// The file name is empty, contains only white spaces, or contains invalid characters.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">Access to fileName is denied.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length. For
        /// example, on Windows-based platforms, paths must be less than 248 characters, and file
        /// names must be less than 260 characters.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="filename"/> contains a colon (:) in the middle of the string.
        /// </exception>
        public static FileInfo ToFileInfo(this string filename) => new FileInfo(filename);

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