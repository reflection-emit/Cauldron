using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Couldron
{
    /// <summary>
    /// Provides a way to choose a <see cref="DataTemplate"/> based on the data object and the data-bound element.
    /// </summary>
    public class CouldronTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="DataTemplate"/> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>Returns a <see cref="DataTemplate"/> or null. The default value is null.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item == null)
                return base.SelectTemplateCore(item, container);

            var itemName = "View_" + item.GetType().Name;

            if (Application.Current.Resources.ContainsKey(itemName))
                return Application.Current.Resources[itemName] as DataTemplate;

            return base.SelectTemplateCore(item, container);
        }
    }
}