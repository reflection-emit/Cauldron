using System.Threading.Tasks;
using System.Text;

#if WINDOWS_UWP

using Windows.Storage;
using Windows.Storage.Streams;
#elif NETCORE

using System.Runtime.InteropServices;
using System.IO;

#else

using System.IO;

#endif

namespace Cauldron
{
    /// <summary>
    /// Provides helper methods for reading, writing and checking files in Windows Desktop applications and Universal Windows Plattform.
    /// </summary>
    public static class ExtensionsFileIO
    {
#if WINDOWS_UWP

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="folder">The folder where the file resides</param>
        /// <param name="filename">The filename of the file to check.</param>
        /// <returns>
        /// When this method completes, it returns true if the file exists, otherwise false.
        /// If the caller does not have sufficient
        /// permissions to read the specified file, no exception is thrown and the method
        /// returns false regardless of the existence of path.
        /// </returns>
        public static async Task<bool> ExistsAsync(this StorageFolder folder, string filename) =>
            await folder.TryGetItemAsync(filename) != null;

        /// <summary>
        /// Reads the contents of the specified file and returns a byte array.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <returns>When this method completes, it returns an array of bytes that represents the contents of the file.</returns>
        public static async Task<byte[]> ReadBytesAsync(this IStorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                return await stream.ToBytesAsync();
            }
        }

        /// <summary>
        /// Reads the contents of the specified file and returns text.
        /// </summary>
        /// <param name="file">The file that the byte is written to.</param>
        /// <returns>When this method completes successfully, it returns the contents of the file as a text string.</returns>
        public static async Task<string> ReadTextAsync(this IStorageFile file) =>
            await FileIO.ReadTextAsync(file);

        /// <summary>
        /// Reads the contents of the specified file and returns text.
        /// </summary>
        /// <param name="folder">The folder where the file resides</param>
        /// <param name="filename">The name of the file to read.</param>
        /// <returns>When this method completes successfully, it returns the contents of the file as a text string.</returns>
        public static async Task<string> ReadTextAsync(this StorageFolder folder, string filename)
        {
            try
            {
                var file = await folder.TryGetItemAsync(filename) as StorageFile;

                if (file == null)
                    return null;

                return await FileIO.ReadTextAsync(file);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Writes an array of bytes of data to the specified file.
        /// </summary>
        /// <param name="file">The file that the byte is written to.</param>
        /// <param name="content">The array of bytes to write.</param>
        /// <returns>No object or value is returned when this method completes.</returns>
        public static async Task WriteBytesAsync(this IStorageFile file, byte[] content) =>
            await FileIO.WriteBytesAsync(file, content);

        /// <summary>
        /// Writes text to the specified file.
        /// </summary>
        /// <param name="file">The file that the text is written to.</param>
        /// <param name="content">The text to write.</param>
        /// <returns>No object or value is returned when this method completes.</returns>
        public static async Task WriteTextAsync(this IStorageFile file, string content) =>
            await FileIO.WriteTextAsync(file, content);

#else

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="folder">The folder where the file resides</param>
        /// <param name="filename">The filename of the file to check.</param>
        /// <returns>
        /// When this method completes, it returns true if the file exists, otherwise false.
        /// If the caller does not have sufficient
        /// permissions to read the specified file, no exception is thrown and the method
        /// returns false regardless of the existence of path.
        /// </returns>
        public static Task<bool> ExistsAsync(this DirectoryInfo folder, string filename) =>
            Task.FromResult(File.Exists(Path.Combine(folder.FullName, filename)));

        /// <summary>
        /// Reads the contents of the specified file and returns a byte array.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <returns>When this method completes, it returns an array of bytes that represents the contents of the file.</returns>
        public static async Task<byte[]> ReadBytesAsync(this FileInfo file)
        {
            byte[] result = null;
            var task = Task.Run(() => result = File.ReadAllBytes(file.FullName));
            await task.ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Reads the contents of the specified file and returns text.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <returns>When this method completes successfully, it returns the contents of the file as a text string.</returns>
        public static Task<string> ReadTextAsync(this FileInfo file) => Task.FromResult(File.ReadAllText(file.FullName));

        /// <summary>
        /// Reads the contents of the specified file and returns text.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <returns>When this method completes successfully, it returns the contents of the file as a text string.</returns>
        public static Task<string> ReadTextAsync(this FileInfo file, Encoding encoding) => Task.FromResult(File.ReadAllText(file.FullName, encoding));

        /// <summary>
        /// Reads the contents of the specified file and returns text.
        /// </summary>
        /// <param name="folder">The folder where the file resides</param>
        /// <param name="filename">The name of the file to read.</param>
        /// <returns>When this method completes successfully, it returns the contents of the file as a text string.</returns>
        public static Task<string> ReadTextAsync(this DirectoryInfo folder, string filename)
        {
#if NETCORE

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return folder.ReadTextAsync(filename, Encoding.GetEncoding("ISO-8859-1"));
            else
                return folder.ReadTextAsync(filename, Encoding.UTF8);

#else

            return folder.ReadTextAsync(filename, Encoding.Default);
#endif
        }

        /// <summary>
        /// Reads the contents of the specified file and returns text.
        /// </summary>
        /// <param name="folder">The folder where the file resides</param>
        /// <param name="filename">The name of the file to read.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <returns>When this method completes successfully, it returns the contents of the file as a text string.</returns>
        public static Task<string> ReadTextAsync(this DirectoryInfo folder, string filename, Encoding encoding)
        {
            try
            {
                var file = Path.Combine(folder.FullName, filename);

                if (!File.Exists(file))
                    return null;

                return Task.FromResult(File.ReadAllText(file, encoding));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Writes an array of bytes of data to the specified file.
        /// </summary>
        /// <param name="file">The file that the byte is written to.</param>
        /// <param name="content">The array of bytes to write.</param>
        /// <returns>No object or value is returned when this method completes.</returns>
        public static async Task WriteBytesAsync(this FileInfo file, byte[] content)
        {
            var task = Task.Run(() =>
             {
                 using (var stream = file.OpenWrite())
                 {
                     stream.Write(content, 0, content.Length);
                     stream.Flush();
                 }
             });
            await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="content">The string to write to the file.</param>
        /// <returns>No object or value is returned when this method completes.</returns>
        public static async Task WriteTextAsync(this FileInfo file, string content)
        {
#if NETCORE

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                await file.WriteTextAsync(content, Encoding.GetEncoding("ISO-8859-1"));
            else
                await file.WriteTextAsync(content, Encoding.UTF8);

#else

            await file.WriteTextAsync(content, Encoding.Default);
#endif
        }

        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <param name="content">The string to write to the file.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <returns>No object or value is returned when this method completes.</returns>
        public static async Task WriteTextAsync(this FileInfo file, string content, Encoding encoding)
        {
            var task = Task.Run(() => File.WriteAllText(file.FullName, content, encoding));
            await task.ConfigureAwait(false);
        }

#endif
    }
}