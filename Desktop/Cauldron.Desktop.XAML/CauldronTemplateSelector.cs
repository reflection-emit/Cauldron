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

            var itemName = "View_" + item.GetType().Name;

            if (Application.Current.Resources.Contains(itemName))
                return Application.Current.Resources[itemName] as DataTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}