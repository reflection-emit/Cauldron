using System;

namespace Cauldron.XAML.Navigation
{
    /// <summary>
    /// Provides data for the back request event
    /// </summary>
    public sealed class CauldronBackRequestedEventArgs : EventArgs
    {
        internal CauldronBackRequestedEventArgs()
        {
            this.IsHandled = false;
        }

        /// <summary>
        /// Gets or sets a value that indicated that the request was handled
        /// </summary>
        public bool IsHandled { get; set; }
    }
}