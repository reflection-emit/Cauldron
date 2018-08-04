using System;
using System.Threading.Tasks;

#if WINDOWS_UWP

using Windows.Storage;
using System.Net.Http;
using Cauldron;

#else

using System.Net.Http;
using System.IO;

#endif

namespace Cauldron.Net
{
    /// <summary>
    /// Provides functions for web operations
    /// </summary>
    public static partial class Web
    {
        /// <summary>
        /// Starts an asyncronous download operation
        /// </summary>
        /// <param name="uri">The uri of the file</param>
        /// <param name="resultFile">The file that the response will be written to.</param>
        /// <exception cref="WebException">Response status code does not indicate success</exception>
        public static async Task DownloadFileAsync(Uri uri,
#if !NETFX_CORE

            FileInfo resultFile)
#else
            StorageFile resultFile)
#endif
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsByteArrayAsync();
#if NETSTANDARD2_0
                    File.WriteAllBytes(resultFile.FullName, data);
#elif NETFX_CORE
                    await resultFile.WriteBytesAsync(data);

#endif
                }
                else
                    throw new WebException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}).", response.StatusCode);
            }
        }
    }
}