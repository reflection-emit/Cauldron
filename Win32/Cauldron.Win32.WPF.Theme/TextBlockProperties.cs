using System;
using System.Windows;
using System.Windows.Controls;

namespace Cauldron.XAML.Theme
{
    /// <summary>
    /// Provides attached properties for the <see cref="Button"/> control
    /// </summary>
    public static class TextBlockProperties
    {
        #region Dependency Attached Property Inline

        /// <summary>
        /// Identifies the Inline dependency property
        /// </summary>
        public static readonly DependencyProperty InlineProperty = DependencyProperty.RegisterAttached("Inline", typeof(string), typeof(TextBlockProperties), new PropertyMetadata(null, OnInlineChanged));

        /// <summary>
        /// Gets the value of Inline
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetInline(DependencyObject obj)
        {
            return (string)obj.GetValue(InlineProperty);
        }

        /// <summary>
        /// Sets the value of the Inline attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetInline(DependencyObject obj, string value)
        {
            obj.SetValue(InlineProperty, value);
        }

        private static void OnInlineChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBlock = dependencyObject as TextBlock;
            var value = args.NewValue as string;

            if (textBlock == null)
                throw new NotSupportedException($"This attached property can only be attached to a {typeof(TextBlock).FullName}");

            textBlock.SetInlinedText(value);
        }

        #endregion Dependency Attached Property Inline
    }
}