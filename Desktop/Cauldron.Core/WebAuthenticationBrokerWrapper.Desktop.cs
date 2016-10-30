using System;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides a wrapper for the UWP WebAuthenticationBroker and the Desktop Authentication handling
    /// </summary>
    public static partial class WebAuthenticationBrokerWrapper
    {
        /// <summary>
        /// Occures if the callback was invoked
        /// </summary>
        public static EventHandler<WebAuthenticationBrokerCallbackEventArgs> OnCallBack;

        /// <summary>
        /// Invokes the <see cref="OnCallBack"/> event
        /// </summary>
        /// <param name="responseData">Contains the protocol data when the operation successfully completes.</param>
        /// <returns>True if the event was handled, otherwise false</returns>
        public static bool CallBack(string responseData)
        {
            var args = new WebAuthenticationBrokerCallbackEventArgs(responseData);
            OnCallBack?.Invoke(null, args);

            return args.Handled;
        }
    }
}