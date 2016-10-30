using System;

namespace Cauldron.Core
{
    /// <summary>
    /// Contains data for the the <see cref="WebAuthenticationBrokerWrapper.OnCallBack"/>
    /// </summary>
    public sealed class WebAuthenticationBrokerCallbackEventArgs : EventArgs
    {
        internal WebAuthenticationBrokerCallbackEventArgs(string responseData)
        {
            this.ResponseData = responseData;
        }

        /// <summary>
        /// Gets or sets a value that marks the event as handled. A true value for Handled prevents most handlers along the event route from handling the same event again.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Contains the protocol data when the operation successfully completes.
        /// </summary>
        public string ResponseData { get; private set; }
    }
}