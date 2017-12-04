using System.Windows;
using System.Windows.Controls;

namespace Cauldron.XAML
{
    /// <summary>
    /// Provides a way to choose a <see cref="DataTemplate"/> based on the data object and the data-bound element.
    /// </summary>
    public class CauldronTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="DataTemplate"/> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>Returns a <see cref="DataTemplate"/> or null. The default value is null.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return base.SelectTemplate(item, container);

            var defaultDataTemplateKey = "View_" + item.GetType().Name;
            var specializedDataTemplateKey = defaultDataTemplateKey;

            if (Application.Current.Resources.Contains(specializedDataTemplateKey))
                return Application.Current.Resources[specializedDataTemplateKey] as DataTemplate;

            specializedDataTemplateKey = defaultDataTemplateKey;
            var orientation = MonitorInfo.GetCurrentOrientation();

            if (orientation == ViewOrientation.Landscape)
                specializedDataTemplateKey += "_Landscape";
            else if (orientation == ViewOrientation.Portrait)
                specializedDataTemplateKey += "_Portrait";

            // Example: x:Key="View_MainViewModel_Landscape"

            if (Application.Current.Resources.Contains(specializedDataTemplateKey))
                return Application.Current.Resources[specializedDataTemplateKey] as DataTemplate;

            if (Application.Current.Resources.Contains(defaultDataTemplateKey))
                return Application.Current.Resources[defaultDataTemplateKey] as DataTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}