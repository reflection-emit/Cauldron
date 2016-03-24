using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Couldron
{
    /// <summary>
    /// Provides usefull extension methods extending <see cref="FrameworkElement"/>
    /// </summary>
    public static class ExtensionsFrameworkElement
    {
        public static FrameworkElement FindElementWithName(this DependencyObject element, string name)
        {
            if (element == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child != null && child.Name == name)
                    return child;
            }

            var parent = VisualTreeHelper.GetParent(element);

            if (parent == null)
                return null;

            return FindElementWithName(parent, name);
        }

        public static IEnumerable<FrameworkElement> FindVisualChildren<T>(this DependencyObject element)
        {
            List<FrameworkElement> elements = new List<FrameworkElement>();
            GetVisualChildren<T>((FrameworkElement)element, elements);
            return elements;
        }

        /// <summary>
        /// Attaches a binding to this element, based on the provided source property name as a path qualification to the data source.
        /// </summary>
        /// <param name="frameworkElement">The <see cref="FrameworkElement"/> that is extended</param>
        /// <param name="dp">Identifies the destination property where the binding should be established</param>
        /// <param name="source">The object to use as the binding source.</param>
        /// <param name="propertyPath">The path to the binding source property.</param>
        public static void SetBinding(this FrameworkElement frameworkElement, DependencyProperty dp, object source, string propertyPath)
        {
            Binding binding = new Binding();
            binding.Mode = BindingMode.OneWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = source;
            binding.Path = new PropertyPath(propertyPath);

            frameworkElement.SetBinding(dp, binding);
        }

        /// <summary>
        /// Attaches a binding to this element, based on the provided source property name as a path qualification to the data source.
        /// </summary>
        /// <param name="frameworkElement">The <see cref="FrameworkElement"/> that is extended</param>
        /// <param name="dp">Identifies the destination property where the binding should be established</param>
        /// <param name="source">The object to use as the binding source.</param>
        /// <param name="propertyPath">The path to the binding source property.</param>
        /// <param name="bindingMode">A value that indicates the direction of the data flow in the binding.</param>
        public static void SetBinding(this FrameworkElement frameworkElement, DependencyProperty dp, object source, string propertyPath, BindingMode bindingMode)
        {
            Binding binding = new Binding();
            binding.Mode = bindingMode;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = source;
            binding.Path = new PropertyPath(propertyPath);

            frameworkElement.SetBinding(dp, binding);
        }

        /// <summary>
        /// Attaches a binding to this element, based on the provided source property name as a path qualification to the data source.
        /// </summary>
        /// <param name="frameworkElement">The <see cref="FrameworkElement"/> that is extended</param>
        /// <param name="dp">Identifies the destination property where the binding should be established</param>
        /// <param name="source">The object to use as the binding source.</param>
        /// <param name="propertyPath">The path to the binding source property.</param>
        /// <param name="bindingMode">A value that indicates the direction of the data flow in the binding.</param>
        /// <param name="valueConverterName">The converter name to use</param>
        public static void SetBinding(this FrameworkElement frameworkElement, DependencyProperty dp, object source, string propertyPath, BindingMode bindingMode, string valueConverterName)
        {
            Binding binding = new Binding();
            binding.Mode = bindingMode;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = source;
            binding.Path = new PropertyPath(propertyPath);
            binding.Converter = Application.Current.Resources[valueConverterName] as IValueConverter;

            frameworkElement.SetBinding(dp, binding);
        }

        private static void GetVisualChildren<T>(FrameworkElement element, List<FrameworkElement> list)
        {
            if (element != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                    if (child != null && child is T)
                        list.Add(child);

                    if (child != null)
                        GetVisualChildren<T>(child, list);
                }
            }
        }
    }
}