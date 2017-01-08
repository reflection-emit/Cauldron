using System;
using System.Threading.Tasks;

#if WINDOWS_UWP

using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

#else

using System.Net;
using System.IO;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Provides functions for web operations
    /// </summary>
    public static class Web
    {
        /// <summary>
        /// Starts an asyncronous download operation
        /// </summary>
        /// <param name="uri">The uri of the file</param>
        /// <param name="resultFile">The file that the response will be written to.</param>
#if WINDOWS_UWP

        public static async Task DownloadFile(Uri uri, StorageFile resultFile)
#else

        public static async Task DownloadFile(Uri uri, FileInfo resultFile)
#endif
        {
            try
            {
#if WINDOWS_UWP
                var downloader = new BackgroundDownloader();
                var download = downloader.CreateDownload(uri, resultFile);
                await download.StartAsync();
#else

                var client = new WebClient();
                await client.DownloadFileTaskAsync(uri, resultFile.FullName);
#endif
            }
            catch
            {
                throw;
            }
        }
    }
}