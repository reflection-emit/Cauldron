using System;
using System.IO;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides access to common locations that contain user content.
    /// </summary>
    public static class KnownFolders
    {
        /// <summary>
        /// Gets the Documents library. The Documents library is not intended for general use.
        /// </summary>
        public static DirectoryInfo DocumentsLibrary => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        /// <summary>
        /// Gets the Music library.
        /// </summary>
        public static DirectoryInfo MusicLibrary => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));

        /// <summary>
        /// Gets the Pictures library.
        /// </summary>
        public static DirectoryInfo PictureLibrary => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

        /// <summary>
        /// Gets the Videos library.
        /// </summary>
        public static DirectoryInfo VideosLibrary => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
    }
}