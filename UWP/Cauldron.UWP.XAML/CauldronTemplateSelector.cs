using Cauldron.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cauldron.XAML
{
    /// <summary>
    /// Provides a way to choose a <see cref="DataTemplate"/> based on the data object and the data-bound element.
    /// </summary>
    public class CauldronTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// When implemented by a derived class, returns a specific DataTemplate for a given item or container.
        /// </summary>
        /// <param name="item">The item to return a template for.</param>
        /// <param name="container">The parent container for the templated item.</param>
        /// <returns>The template to use for the given item and/or container.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item == null)
                return base.SelectTemplateCore(item, container);

            var defaultDataTemplateKey = "View_" + item.GetType().Name;
            var specializedDataTemplateKey = defaultDataTemplateKey;

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == DeviceFamilies.DeviceFamily_WindowsMobile)
                specializedDataTemplateKey += "_Mobile";
            else if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == DeviceFamilies.DeviceFamily_WindowsDesktop)
                specializedDataTemplateKey += "_Desktop";
            else if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == DeviceFamilies.DeviceFamily_WindowsXbox)
                specializedDataTemplateKey += "_Xbox";
            else if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == DeviceFamilies.DeviceFamily_WindowsIoT)
                specializedDataTemplateKey += "_IoT";

            // Example: x:Key="View_MainViewModel_Mobile"

            if (Application.Current.Resources.ContainsKey(specializedDataTemplateKey))
                return Application.Current.Resources[specializedDataTemplateKey] as DataTemplate;

            var currentView = ApplicationView.GetForCurrentView();

            if (currentView.Orientation == ApplicationViewOrientation.Landscape)
                specializedDataTemplateKey += "_Landscape";
            else if (currentView.Orientation == ApplicationViewOrientation.Portrait)
                specializedDataTemplateKey += "_Portrait";

            // Example: x:Key="View_MainViewModel_Mobile_Landscape"

            if (Application.Current.Resources.ContainsKey(specializedDataTemplateKey))
                return Application.Current.Resources[specializedDataTemplateKey] as DataTemplate;

            specializedDataTemplateKey = defaultDataTemplateKey;

            if (currentView.Orientation == ApplicationViewOrientation.Landscape)
                specializedDataTemplateKey += "_Landscape";
            else if (currentView.Orientation == ApplicationViewOrientation.Portrait)
                specializedDataTemplateKey += "_Portrait";

            // Example: x:Key="View_MainViewModel_Landscape"

            if (Application.Current.Resources.ContainsKey(specializedDataTemplateKey))
                return Application.Current.Resources[specializedDataTemplateKey] as DataTemplate;

            if (Application.Current.Resources.ContainsKey(defaultDataTemplateKey))
                return Application.Current.Resources[defaultDataTemplateKey] as DataTemplate;

            return base.SelectTemplateCore(item, container);
        }
    }
}