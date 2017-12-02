using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods for the <see cref="DirectoryInfo"/> class
    /// </summary>
    public static class ExtensionsDirectoryInfo
    {
        /// <summary>
        /// Combines a <see cref="DirectoryInfo"/> and a string to a path
        /// </summary>
        /// <param name="directory">The first path to combine</param>
        /// <param name="path">The second path to combine</param>
        /// <returns>The combine path as <see cref="DirectoryInfo"/></returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="directory"/> or <paramref name="path"/> contains one or more of the
        /// invalid characters defined in <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="directory"/> or <paramref name="path"/> is null.
        /// </exception>
        public static DirectoryInfo Combine(this DirectoryInfo directory, string path) => new DirectoryInfo(Path.Combine(directory?.FullName, path));

        /// <summary>
        /// Combines a <see cref="DirectoryInfo"/> and a string to a path
        /// </summary>
        /// <param name="directory">The first path to combine</param>
        /// <param name="path1">The second path to combine</param>
        /// <param name="path2">The third path to combine</param>
        /// <returns>The combine path as <see cref="DirectoryInfo"/></returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="directory"/> or <paramref name="path1"/> or <paramref name="path2"/>
        /// contains one or more of the invalid characters defined in <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="directory"/> or <paramref name="path1"/> or <paramref name="path2"/> is null.
        /// </exception>
        public static DirectoryInfo Combine(this DirectoryInfo directory, string path1, string path2) => new DirectoryInfo(Path.Combine(directory.FullName, path1, path2));

        /// <summary>
        /// Combines a <see cref="DirectoryInfo"/> and a string to a path
        /// </summary>
        /// <param name="directory">The first path to combine</param>
        /// <param name="path1">The second path to combine</param>
        /// <param name="path2">The third path to combine</param>
        /// <param name="path3">The fourth path to combine</param>
        /// <returns>The combine path as <see cref="DirectoryInfo"/></returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="directory"/> or <paramref name="path1"/> or <paramref name="path2"/> or
        /// <paramref name="path3"/> contains one or more of the invalid characters defined in <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="directory"/> or <paramref name="path1"/> or <paramref name="path2"/> or
        /// <paramref name="path3"/> is null.
        /// </exception>
        public static DirectoryInfo Combine(this DirectoryInfo directory, string path1, string path2, string path3) => new DirectoryInfo(Path.Combine(directory.FullName, path1, path2, path3));

        /// <summary>
        /// Combines a <see cref="DirectoryInfo"/> and a string to a path
        /// </summary>
        /// <param name="directory">The first path to combine</param>
        /// <param name="path1">The second path to combine</param>
        /// <param name="path2">The third path to combine</param>
        /// <param name="path3">The fourth path to combine</param>
        /// <param name="path4">The fourth path to combine</param>
        /// <returns>The combine path as <see cref="DirectoryInfo"/></returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="directory"/> or <paramref name="path1"/> or <paramref name="path2"/> or
        /// <paramref name="path3"/> or <paramref name="path4"/> contains one or more of the invalid
        /// characters defined in <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="directory"/> or <paramref name="path1"/> or <paramref name="path2"/> or
        /// <paramref name="path3"/> or <paramref name="path4"/> is null.
        /// </exception>
        public static DirectoryInfo Combine(this DirectoryInfo directory, string path1, string path2, string path3, string path4) => new DirectoryInfo(Path.Combine(directory.FullName, path1, path2, path3, path4));

        /// <summary>
        /// Creates a copy of the file in the specified folder and renames the copy.
        /// </summary>
        /// <param name="source">The file to be copied</param>
        /// <param name="destinationFolder">
        /// The destination folder where the copy of the file is created.
        /// </param>
        /// <param name="desiredNewName">
        /// The new name for the copy of the file created in the <paramref name="destinationFolder"/>
        /// </param>
        /// <returns>
        /// When this method completes, it returns a <see cref="FileInfo"/> that represents the copy
        /// of the file created in the <paramref name="destinationFolder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="desiredNewName"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="desiredNewName"/> is empty</exception>
        public static async Task<FileInfo> CopyAsync(this FileInfo source, DirectoryInfo destinationFolder, string desiredNewName) =>
          await source.CopyAsync(destinationFolder, desiredNewName, NameCollisionOption.GenerateUniqueName);

        /// <summary>
        /// Creates a copy of the file in the specified folder.
        /// </summary>
        /// <param name="source">The file to be copied</param>
        /// <param name="destinationFolder">
        /// The destination folder where the copy of the file is created.
        /// </param>
        /// <returns>
        /// When this method completes, it returns a <see cref="FileInfo"/> that represents the copy
        /// of the file created in the <paramref name="destinationFolder"/>.
        /// </returns>
        /// <exception cref="IOException">If file already exists. Only on <see cref="NameCollisionOption.FailIfExists"/></exception>
        public static async Task<FileInfo> CopyAsync(this FileInfo source, DirectoryInfo destinationFolder) =>
          await source.CopyAsync(destinationFolder, source.Name, NameCollisionOption.GenerateUniqueName);

        /// <summary>
        /// Creates a copy of the file in the specified folder and renames the copy. This method also
        /// specifies what to do if a file with the same name already exists in the destination folder.
        /// </summary>
        /// <param name="source">The file to be copied</param>
        /// <param name="destinationFolder">
        /// The destination folder where the copy of the file is created.
        /// </param>
        /// <param name="desiredNewName">
        /// The new name for the copy of the file created in the <paramref name="destinationFolder"/>.
        /// </param>
        /// <param name="option">
        /// One of the enumeration values that determines how to handle the collision if a file with
        /// the specified <paramref name="desiredNewName"/> already exists in the destination folder.
        /// </param>
        /// <returns>
        /// When this method completes, it returns a <see cref="FileInfo"/> that represents the copy
        /// of the file created in the <paramref name="destinationFolder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="desiredNewName"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="desiredNewName"/> is empty</exception>
        /// <exception cref="IOException">If file already exists. Only on <see cref="NameCollisionOption.FailIfExists"/></exception>
        public static async Task<FileInfo> CopyAsync(this FileInfo source, DirectoryInfo destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            if (desiredNewName == null)
                throw new ArgumentNullException(nameof(desiredNewName));

            if (desiredNewName.Length == 0)
                throw new ArgumentException($"The argument desiredNewName cannot be empty");

            var filename = Path.Combine(destinationFolder.FullName, desiredNewName);

            var task = Task.Run(() =>
            {
                switch (option)
                {
                    case NameCollisionOption.ReplaceExisting:
                        File.Copy(source.FullName, filename, true);
                        break;

                    case NameCollisionOption.FailIfExists:
                        if (File.Exists(filename))
                            throw new IOException($"The file '{desiredNewName}' already exist in the directory '{destinationFolder.FullName}'");
                        File.Copy(source.FullName, filename, true);
                        break;

                    case NameCollisionOption.GenerateUniqueName:
                        File.Copy(source.FullName, GetUniqueFilename(filename), true);
                        break;
                }
            });
            await task.ConfigureAwait(false);

            return new FileInfo(filename);
        }

        /// <summary>
        /// Creates a new file in the current folder. This method also specifies what to do if a file
        /// with the same name already exists in the current folder.
        /// </summary>
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> representing the current folder
        /// </param>
        /// <param name="desiredName">The name of the new file to create in the current folder.</param>
        /// <param name="options">
        /// One of the enumeration values that determines how to handle the collision if a file with
        /// the specified desiredName already exists in the current folder.
        /// </param>
        /// <returns>
        /// When this method completes, it returns a <see cref="FileInfo"/> that represents the new file.&gt;
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="desiredName"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="desiredName"/> is empty</exception>
        /// <exception cref="IOException">If file already exists. Only on <see cref="CreationCollisionOption.FailIfExists"/></exception>
        public static async Task<FileInfo> CreateFileAsync(this DirectoryInfo directoryInfo, string desiredName, CreationCollisionOption options)
        {
            if (desiredName == null)
                throw new ArgumentNullException(nameof(desiredName));

            if (desiredName.Length == 0)
                throw new ArgumentException($"The argument desiredName cannot be empty");

            var filename = Path.Combine(directoryInfo.FullName, desiredName);

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
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> representing the current folder
        /// </param>
        /// <param name="desiredName">The name of the new file to create in the current folder.</param>
        /// <returns>
        /// When this method completes, it returns a <see cref="FileInfo"/> that represents the new file.&gt;
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="desiredName"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="desiredName"/> is empty</exception>
        public static async Task<FileInfo> CreateFileAsync(this DirectoryInfo directoryInfo, string desiredName) =>
            await directoryInfo.CreateFileAsync(desiredName, CreationCollisionOption.GenerateUniqueName);

        /// <summary>
        /// Creates a new subfolder with the specified name in the current folder.
        /// </summary>
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> representing the current folder
        /// </param>
        /// <param name="desiredName">The name of the new subfolder to create in the current folder.</param>
        /// <returns>
        /// When this method completes, it returns a <see cref="DirectoryInfo"/> that represents the
        /// new subfolder.
        /// </returns>
        public static async Task<DirectoryInfo> CreateFolderAsync(this DirectoryInfo directoryInfo, string desiredName) =>
            await directoryInfo.CreateFolderAsync(desiredName, CreationCollisionOption.GenerateUniqueName);

        /// <summary>
        /// Creates a new subfolder with the specified name in the current folder. This method also
        /// specifies what to do if a subfolder with the same name already exists in the current folder.
        /// </summary>
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> representing the current folder
        /// </param>
        /// <param name="desiredName">The name of the new subfolder to create in the current folder.</param>
        /// <param name="options">
        /// One of the enumeration values that determines how to handle the collision if a subfolder
        /// with the specified desiredName already exists in the current folder.
        /// </param>
        /// <returns>
        /// When this method completes, it returns a <see cref="DirectoryInfo"/> that represents the
        /// new subfolder.
        /// </returns>
        public static async Task<DirectoryInfo> CreateFolderAsync(this DirectoryInfo directoryInfo, string desiredName, CreationCollisionOption options)
        {
            if (desiredName == null)
                throw new ArgumentNullException(nameof(desiredName));

            if (desiredName.Length == 0)
                throw new ArgumentException($"The argument desiredName cannot be empty");

            var folderFullPath = Path.Combine(directoryInfo.FullName, desiredName);

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
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> representing the current folder
        /// </param>
        /// <param name="name">
        /// The name (or path relative to the current folder) of the file to get.
        /// </param>
        /// <returns>
        /// When this method completes successfully, it returns a <see cref="FileInfo"/> that
        /// represents the specified file.
        /// </returns>
        public static Task<FileInfo> GetFileAsync(this DirectoryInfo directoryInfo, string name) =>
            Task.FromResult(new FileInfo(Path.Combine(directoryInfo.FullName, name)));

        /// <summary>
        /// Converts a string to a <see cref="DirectoryInfo"/>
        /// </summary>
        /// <param name="path">
        /// A string specifying the <paramref name="path"/> on which to create the <see cref="DirectoryInfo"/>.
        /// </param>
        /// <returns>An instance of <see cref="DirectoryInfo"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path"/> contains invalid characters such as ", &lt;, &gt;, or |.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length. For
        /// example, on Windows-based platforms, paths must be less than 248 characters, and file
        /// names must be less than 260 characters. The specified path, file name, or both are too long.
        /// </exception>
        public static DirectoryInfo ToDirectoryInfo(this string path) => new DirectoryInfo(path);

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
        /// Checks if the filename of <paramref name="path"/> exist. If the file already exists, an
        /// indexer will be added to the filename to make it unique.
        /// </summary>
        /// <param name="path">The path and filename of a file</param>
        /// <returns>A unique and valied path and filename</returns>
        private static string GetUniqueDirectoryName(string path)
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
        private static string GetUniqueFilename(string path)
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