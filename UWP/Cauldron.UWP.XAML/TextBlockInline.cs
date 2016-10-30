#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else

using System.Windows;
using System.Windows.Controls;

#endif

namespace Cauldron.XAML
{
    internal static class TextBlockInline
    {
        #region Dependency Attached Property TextBlockInline

        /// <summary>
        /// Identifies the TextBlockInline dependency property
        /// </summary>
        public static readonly DependencyProperty TextBlockInlineProperty = DependencyProperty.RegisterAttached("TextBlockInline", typeof(string), typeof(TextBlockInline), new PropertyMetadata(null, OnTextBlockInlineChanged));

        /// <summary>
        /// Gets the value of TextBlockInline
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetTextBlockInline(DependencyObject obj)
        {
            return (string)obj.GetValue(TextBlockInlineProperty);
        }

        /// <summary>
        /// Sets the value of the TextBlockInline attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetTextBlockInline(DependencyObject obj, string value)
        {
            obj.SetValue(TextBlockInlineProperty, value);
        }

        private static void OnTextBlockInlineChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBlock = dependencyObject as TextBlock;

            if (textBlock == null)
                return;

            var value = args.NewValue as string;

            if (value == null)
                return;

            if (value.StartsWith("<Inline>") && value.EndsWith("</Inline>"))
            {
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(XAMLHelper.ParseToInline(value));
            }
            else
                textBlock.Text = value;
        }

        #endregion Dependency Attached Property TextBlockInline
    }
}