using System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Cauldron.XAML.Interactivity.Attached
{
    /// <summary>
    /// An attach property that can assign a cursor to a <see cref="FrameworkElement"/>
    /// </summary>
    public static class CustomCursor
    {
        #region Dependency Attached Property ResourceIndex

        /// <summary>
        /// Identifies the ResourceIndex dependency property
        /// <para/>
        /// See https://blogs.msdn.microsoft.com/devfish/2012/08/01/customcursors-in-windows-8-csharp-metro-applications/ for more information about custom cursors in UWP.
        /// </summary>
        public static readonly DependencyProperty ResourceIndexProperty = DependencyProperty.RegisterAttached("ResourceIndex", typeof(uint), typeof(CustomCursor), new PropertyMetadata(uint.MaxValue, OnResourceIndexChanged));

        /// <summary>
        /// Gets the value of ResourceIndex
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static uint GetResourceIndex(DependencyObject obj)
        {
            return (uint)obj.GetValue(ResourceIndexProperty);
        }

        /// <summary>
        /// Sets the value of the ResourceIndex attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetResourceIndex(DependencyObject obj, uint value)
        {
            obj.SetValue(ResourceIndexProperty, value);
        }

        private static void OnResourceIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var value = args.NewValue as string;
            var frameWorkElement = dependencyObject as FrameworkElement;

            if (frameWorkElement == null || value == null)
                return;
        }

        #endregion Dependency Attached Property ResourceIndex

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
            var value = (uint)args.NewValue;
            var frameWorkElement = dependencyObject as FrameworkElement;

            if (frameWorkElement == null)
                throw new NotSupportedException($"This attached property can only be attached to a {typeof(FrameworkElement).FullName}");

            if (value == uint.MaxValue)
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                return;
            }

            frameWorkElement.PointerEntered += (s, e) =>
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(Windows.UI.Core.CoreCursorType.Custom, value);
            };

            frameWorkElement.PointerCanceled += (s, e) =>
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
            };

            frameWorkElement.PointerExited += (s, e) =>
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
            };
        }

        #endregion Dependency Attached Property ResourceKey
    }
}