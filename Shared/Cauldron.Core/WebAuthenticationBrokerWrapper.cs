using System;
using System.Threading.Tasks;

#if WINDOWS_UWP

using Windows.Security.Authentication.Web;

#else

using System.Diagnostics;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Provides a wrapper for the UWP WebAuthenticationBroker and the Desktop Authentication handling
    /// </summary>
    public static partial class WebAuthenticationBrokerWrapper
    {
        /// <summary>
        /// Starts the asynchronous authentication operation.
        /// On Desktop the method has a timeout of 1 minute
        /// </summary>
        /// <param name="uri">The starting URI of the web service. This URI must be a secure address of https://.</param>
        /// <param name="callbackUri">The callback uri of the authentification. This will be used to verify the result value.</param>
        /// <returns>Contains the protocol data when the operation successfully completes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="uri"/> is not a secure address</exception>
        public static async Task<string> AuthenticateAsync(Uri uri, Uri callbackUri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (!uri.ToString().StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("The URI must be a secure address of https://.");

            var argument = string.Empty;
#if WINDOWS_UWP
            var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, uri, callbackUri);

            switch (webAuthenticationResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    argument = webAuthenticationResult.ResponseData.ToString();
                    break;

                default:
                    throw new Exception(webAuthenticationResult.ResponseData);
            }

#else
            var fakeTask = new Task(() => { });

            var callback = new EventHandler<WebAuthenticationBrokerCallbackEventArgs>((s, e) =>
            {
                if (e.Handled || !e.ResponseData.StartsWith(callbackUri.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    return;

                argument = e.ResponseData;
                fakeTask.Start();
            });
            WebAuthenticationBrokerWrapper.OnCallBack += callback;
            Process.Start(uri.ToString());

            await Task.WhenAny(fakeTask, Task.Delay((int)TimeSpan.FromMinutes(1).TotalMilliseconds));
            WebAuthenticationBrokerWrapper.OnCallBack -= callback;

#endif
            return argument;
        }
    }
}