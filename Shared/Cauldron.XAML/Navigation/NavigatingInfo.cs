using System;

#if WINDOWS_UWP
using Windows.UI.Xaml.Navigation;
#else

using System.Windows.Navigation;

#endif

namespace Cauldron.XAML.Navigation
{
    /// <summary>
    /// Provides data for navigation
    /// </summary>
    public sealed class NavigatingInfo
    {
        internal NavigatingInfo(NavigationMode navigationMode, NavigationType navigationType, Type sourcePageType)
        {
            this.NavigationMode = navigationMode;
            this.NavigationType = navigationType;
            this.SourcePageType = sourcePageType;
        }

        /// <summary>
        /// Specifies whether a pending navigation should be canceled.
        /// </summary>
        public bool Cancel { get; set; } = false;

        /// <summary>
        /// Gets the value of the mode parameter from the originating Navigate call.
        /// </summary>
        public NavigationMode NavigationMode { get; private set; }

        /// <summary>
        /// Gets information about what invoked the navigation
        /// </summary>
        public NavigationType NavigationType { get; private set; }

        /// <summary>
        /// Gets the data type of the source page.
        /// </summary>
        public Type SourcePageType { get; private set; }
    }
}