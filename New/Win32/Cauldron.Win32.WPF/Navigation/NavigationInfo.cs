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
    public sealed class NavigationInfo
    {
        internal NavigationInfo(NavigationMode navigationMode, NavigationType navigationType, Type sourceViewModelType)
        {
            this.NavigationMode = navigationMode;
            this.NavigationType = navigationType;
            this.SourceViewModelType = sourceViewModelType;
        }

        /// <summary>
        /// Gets the value of the mode parameter from the originating Navigate call.
        /// </summary>
        public NavigationMode NavigationMode { get; private set; }

        /// <summary>
        /// Gets information about what invoked the navigation
        /// </summary>
        public NavigationType NavigationType { get; private set; }

        /// <summary>
        /// Gets the data type of the source view model.
        /// </summary>
        public Type SourceViewModelType { get; private set; }
    }
}