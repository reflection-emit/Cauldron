using System;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.Storage;
#else

using System.IO;

#endif

namespace Cauldron.Potions
{
    /// <summary>
    /// Provides functions for web operations
    /// </summary>
    public interface IWeb
    {
        /// <summary>
        /// Starts an asyncronous download operation
        /// </summary>
        /// <param name="uri">The uri of the file</param>
        /// <param name="resultFile">The file that the response will be written to.</param>
#if WINDOWS_UWP
        Task DownloadFile(Uri uri, StorageFile resultFile);
#else

        Task DownloadFile(Uri uri, FileInfo resultFile);

#endif
    }
}