using Cauldron.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// An attach property that can assign a cursor to a <see cref="FrameworkElement"/>
    /// </summary>
    public static class CustomCursor
    {
        #region Dependency Attached Property ResourceKey

        /// <summary>
        /// Identifies the ResourceKey dependency property
        /// </summary>
        public static readonly DependencyProperty ResourceKeyProperty = DependencyProperty.RegisterAttached("ResourceKey", typeof(string), typeof(CustomCursor), new PropertyMetadata(null, OnResourceKeyChanged));

        /// <summary>
        /// Gets the value of ResourceKey
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetResourceKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ResourceKeyProperty);
        }

        /// <summary>
        /// Sets the value of the ResourceKey attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetResourceKey(DependencyObject obj, string value)
        {
            obj.SetValue(ResourceKeyProperty, value);
        }

        private static void OnResourceKeyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var value = args.NewValue as string;
            var frameWorkElement = dependencyObject as FrameworkElement;

            if (frameWorkElement == null)
                throw new NotSupportedException($"This attached property can only be attached to a {typeof(FrameworkElement).FullName}");

            if (value == null)
            {
                frameWorkElement.Cursor = Cursors.Arrow;
                return;
            }

            var assemblyAndResourceInfo = Assemblies.AssemblyAndResourceNamesInfo.FirstOrDefault(x => x.Filename.EndsWith(value, StringComparison.OrdinalIgnoreCase));

            if (assemblyAndResourceInfo == null)
                return;

            var result = assemblyAndResourceInfo.Assembly.GetManifestResourceStream(assemblyAndResourceInfo.Filename);
            result.Position = 0;

            frameWorkElement.Cursor = new Cursor(result);
        }

        #endregion Dependency Attached Property ResourceKey
    }
}