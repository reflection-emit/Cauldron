using System;
using System.Net;
using System.Threading.Tasks;

#if WINDOWS_UWP

using Windows.Storage;
using System.Net.Http;
using Cauldron.Core.Extensions;

#elif ANDROID

using System.IO;

#else

using System.Net.Http;
using System.IO;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Provides functions for web operations
    /// </summary>
    public static partial class Web
    {
#if ANDROID

        /// <summary>
        /// Starts an asyncronous download operation
        /// </summary>
        /// <param name="uri">The uri of the file</param>
        /// <param name="resultFile">The file that the response will be written to.</param>
        /// <exception cref="WebException">Response status code does not indicate success</exception>
        public static async Task DownloadFile(Uri uri, FileInfo resultFile)
        {
            using (var client = new WebClientEx())
            {
                try
                {
                    await client.DownloadFileTaskAsync(uri, resultFile.FullName);
                }
                catch (System.Net.WebException e)
                {
                    var response = (System.Net.HttpWebResponse)e.Response;
                    throw new WebException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}).", response.StatusCode);
                }

                if (client.StatusCode != HttpStatusCode.OK)
                    throw new WebException($"Response status code does not indicate success: {(int)client.StatusCode} ({client.StatusCode}).", client.StatusCode);
            }
        }

        internal class WebClientEx : WebClient
        {
            private WebResponse webResponse = null;

            public HttpStatusCode StatusCode
            {
                get
                {
                    if (webResponse != null && webResponse is HttpWebResponse)
                        return (webResponse as HttpWebResponse).StatusCode;
                    else
                        return HttpStatusCode.OK;
                }
            }

            protected override WebResponse GetWebResponse(WebRequest Req, IAsyncResult ar)
            {
                webResponse = base.GetWebResponse(Req, ar);
                return webResponse;
            }
        }

#endif
    }

    /// <summary>
    /// Provides functions for web operations
    /// </summary>
    public static partial class Web
    {
#if DESKTOP || NETCORE

        /// <summary>
        /// Starts an asyncronous download operation
        /// </summary>
        /// <param name="uri">The uri of the file</param>
        /// <param name="resultFile">The file that the response will be written to.</param>
        /// <exception cref="WebException">Response status code does not indicate success</exception>
        public static async Task DownloadFile(Uri uri, FileInfo resultFile)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(resultFile.FullName, data);
                }
                else
                    throw new WebException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}).", response.StatusCode);
            }
        }

#endif
    }

    /// <summary>
    /// Provides functions for web operations
    /// </summary>
    public static partial class Web
    {
#if NETFX_CORE

        /// <summary>
        /// Starts an asyncronous download operation
        /// </summary>
        /// <param name="uri">The uri of the file</param>
        /// <param name="resultFile">The file that the response will be written to.</param>
        /// <exception cref="WebException">Response status code does not indicate success</exception>
        public static async Task DownloadFile(Uri uri, StorageFile resultFile)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsByteArrayAsync();
                    await resultFile.WriteBytesAsync(data);
                }
                else
                    throw new WebException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}).", response.StatusCode);
            }
        }

#endif
    }

    /// <summary>
    /// Represents a exception that occures during Web operations
    /// </summary>
    public sealed class WebException : Exception
    {
        internal WebException(string message, HttpStatusCode code) : base(message)
        {
            this.StatusCode = code;
        }

        /// <summary>
        /// Gets the status code of the http request
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
    }
}