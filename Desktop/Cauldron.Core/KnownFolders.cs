using System;
using System.IO;

namespace Windows.Storage/* So that precompiler definitions are not required if classes are shared between UWP and Desktop */
{
    /// <summary>
    /// Provides access to common locations that contain user content.
    /// </summary>
    public static class KnownFolders
    {
#if DESKTOP || ANDROID
        /// <summary>
        /// Gets the Documents library. The Documents library is not intended for general use.
        /// </summary>
        public static DirectoryInfo DocumentsLibrary { get { return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)); } }

        /// <summary>
        /// Gets the Music library.
        /// </summary>
        public static DirectoryInfo MusicLibrary { get { return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)); } }

        /// <summary>
        /// Gets the Pictures library.
        /// </summary>
        public static DirectoryInfo PictureLibrary { get { return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)); } }

        /// <summary>
        /// Gets the Videos library.
        /// </summary>
        public static DirectoryInfo VideosLibrary { get { return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)); } }
#elif NETCORE
        // TODO - check Linux and Mac for environment variables
#endif
    }
}