using System;
using System.IO;
using System.Threading.Tasks;

namespace Cauldron.Core.Extensions
{
    /// <summary>
    /// Provides usefull extension methods for the <see cref="DirectoryInfo"/> class
    /// </summary>
    public static class ExtensionsDirectoryInfo
    {
        /// <summary>
        /// Creates a new file in the current folder. This method also specifies what to do if a file with the same name already exists in the current folder.
        /// </summary>
        /// <param name="directoryInfo">The <see cref="DirectoryInfo"/> representing the current folder</param>
        /// <param name="desiredName">The name of the new file to create in the current folder.</param>
        /// <param name="options">One of the enumeration values that determines how to handle the collision if a file with the specified desiredName already exists in the current folder.</param>
        /// <returns>When this method completes, it returns a <see cref="FileInfo"/> that represents the new file.></returns>
        /// <exception cref="ArgumentNullException"><paramref name="desiredName"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="desiredName"/> is empty</exception>
        /// <exception cref="IOException">If file already exists. Only on <see cref="CreationCollisionOption.FailIfExists"/></exception>
        public static async Task<FileInfo> CreateFileAsync(this DirectoryInfo directoryInfo, string desiredName, CreationCollisionOption options)
        {
            if (desiredName == null)
                throw new ArgumentNullException(nameof(desiredName));

            if (desiredName.Length == 0)
                throw new ArgumentException($"The argument desiredName cannot be empty");

            string filename = Path.Combine(directoryInfo.FullName, desiredName);

            var task = Task.Run(() =>
             {
                 switch (options)
                 {
                     case CreationCollisionOption.ReplaceExisting:
                         File.Create(filename)?.Dispose();
                         break;

                     case CreationCollisionOption.FailIfExists:
                         if (File.Exists(filename))
                             throw new IOException($"The file '{desiredName}' already exist in the directory '{directoryInfo.FullName}'");
                         File.Create(filename)?.Dispose();
                         break;

                     case CreationCollisionOption.GenerateUniqueName:
                         File.Create(GetUniqueFilename(filename))?.Dispose();
                         break;

                     case CreationCollisionOption.OpenIfExists:
                         if (File.Exists(filename))
                             break;

                         File.Create(filename)?.Dispose();
                         break;
                 }
             });
            await task.ConfigureAwait(false);

            return new FileInfo(filename);
        }

        /// <summary>
        /// Creates a new file with the specified name in the current folder.
        /// </summary>
        /// <param name="directoryInfo">The <see cref="DirectoryInfo"/> representing the current folder</param>
        /// <param name="desiredName">The name of the new file to create in the current folder.</param>
        /// <returns>When this method completes, it returns a <see cref="FileInfo"/> that represents the new file.></returns>
        /// <exception cref="ArgumentNullException"><paramref name="desiredName"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="desiredName"/> is empty</exception>
        public static async Task<FileInfo> CreateFileAsync(this DirectoryInfo directoryInfo, string desiredName) =>
            await directoryInfo.CreateFileAsync(desiredName, CreationCollisionOption.GenerateUniqueName);

        /// <summary>
        /// Creates a new subfolder with the specified name in the current folder.
        /// </summary>
        /// <param name="directoryInfo">The <see cref="DirectoryInfo"/> representing the current folder</param>
        /// <param name="desiredName">The name of the new subfolder to create in the current folder.</param>
        /// <returns>When this method completes, it returns a <see cref="DirectoryInfo"/> that represents the new subfolder.</returns>
        public static async Task<DirectoryInfo> CreateFolderAsync(this DirectoryInfo directoryInfo, string desiredName) =>
            await directoryInfo.CreateFolderAsync(desiredName, CreationCollisionOption.GenerateUniqueName);

        /// <summary>
        /// Creates a new subfolder with the specified name in the current folder. This method
        /// also specifies what to do if a subfolder with the same name already exists in
        /// the current folder.
        /// </summary>
        /// <param name="directoryInfo">The <see cref="DirectoryInfo"/> representing the current folder</param>
        /// <param name="desiredName">The name of the new subfolder to create in the current folder.</param>
        /// <param name="options">
        /// One of the enumeration values that determines how to handle the collision if
        ///  a subfolder with the specified desiredName already exists in the current folder.
        /// </param>
        /// <returns>When this method completes, it returns a <see cref="DirectoryInfo"/> that represents the new subfolder.</returns>
        public static async Task<DirectoryInfo> CreateFolderAsync(this DirectoryInfo directoryInfo, string desiredName, CreationCollisionOption options)
        {
            if (desiredName == null)
                throw new ArgumentNullException(nameof(desiredName));

            if (desiredName.Length == 0)
                throw new ArgumentException($"The argument desiredName cannot be empty");

            string folderFullPath = Path.Combine(directoryInfo.FullName, desiredName);

            var task = Task.Run(() =>
            {
                switch (options)
                {
                    case CreationCollisionOption.ReplaceExisting:
                        Directory.CreateDirectory(folderFullPath);
                        break;

                    case CreationCollisionOption.FailIfExists:
                        if (Directory.Exists(folderFullPath))
                            throw new IOException($"The folder '{desiredName}' already exist in the directory '{directoryInfo.FullName}'");

                        Directory.CreateDirectory(folderFullPath);
                        break;

                    case CreationCollisionOption.GenerateUniqueName:
                        string name = GetUniqueDirectoryName(folderFullPath);
                        Directory.CreateDirectory(name);
                        break;

                    case CreationCollisionOption.OpenIfExists:
                        if (Directory.Exists(folderFullPath))
                            break;

                        Directory.CreateDirectory(folderFullPath);
                        break;
                }
            });
            await task.ConfigureAwait(false);

            return new DirectoryInfo(folderFullPath);
        }

        /// <summary>
        /// Gets the file with the specified name from the current folder.
        /// </summary>
        /// <param name="directoryInfo">The <see cref="DirectoryInfo"/> representing the current folder</param>
        /// <param name="name">The name (or path relative to the current folder) of the file to get.</param>
        /// <returns>When this method completes successfully, it returns a <see cref="FileInfo"/> that represents the specified file.</returns>
        public static Task<FileInfo> GetFileAsync(this DirectoryInfo directoryInfo, string name) =>
            Task.FromResult(new FileInfo(Path.Combine(directoryInfo.FullName, name)));

        /// <summary>
        /// Checks if the filename of <paramref name="path"/> exist. If the file already exists, an indexer will be added to the filename to make it unique.
        /// </summary>
        /// <param name="path">The path and filename of a file</param>
        /// <returns>A unique and valied path and filename</returns>
        private static string GetUniqueDirectoryName(string path)
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

        /// <summary>
        /// Checks if the filename of <paramref name="path"/> exist. If the file already exists, an indexer will be added to the filename to make it unique.
        /// </summary>
        /// <param name="path">The path and filename of a file</param>
        /// <returns>A unique and valied path and filename</returns>
        private static string GetUniqueFilename(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            var directoryName = directoryInfo.FullName;
            var indexer = 1;

            while (Directory.Exists(directoryName))
                directoryName = Path.Combine(directoryInfo.Parent.FullName, $"{directoryInfo.Name} ({indexer++})");

            return directoryName;
        }
    }
}