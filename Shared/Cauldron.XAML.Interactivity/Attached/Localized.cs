using Cauldron.Activator;
using Cauldron.Localization;
using System.Reflection;

#if WINDOWS_UWP

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

#else

using System.Windows;
using System.Windows.Controls;

#endif

namespace Cauldron.XAML.Interactivity.Attached
{
    /// <summary>
    /// Provides Attached Properties to enable localization in controls.
    /// <para/>
    /// Existing text are overridden.
    /// <para />
    /// Supported controls: <see cref="FrameworkElement.ToolTip"/> <see cref="TextBlock.Text"/>, <see cref="ContentControl.Content"/>
    /// </summary>
    public static class Localized
    {
        private static string AssignText(DependencyObject dependencyObject, string text)
        {
            if (Factory.HasContract(typeof(ILocalizationSource)))
            {
                var localized = Factory.Create<Locale>();
                return localized[text];
            }

            return text;
        }

        #region Dependency Attached Property Text

        /// <summary>
        /// Identifies the Text dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Localized), new PropertyMetadata("", OnTextChanged));

        /// <summary>
        /// Gets the value of Text
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        /// <summary>
        /// Sets the value of the Text attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var newValue = args.NewValue as string;

            if (newValue == null)
                newValue = string.Empty;

            var text = newValue.StartsWith("<Inline>") && newValue.EndsWith("</Inline>") ? newValue : AssignText(dependencyObject, newValue);

            if (dependencyObject is TextBlock)
            {
                var textBlock = dependencyObject as TextBlock;

                if (text.StartsWith("<Inline>") && text.EndsWith("</Inline>"))
                {
                    textBlock.Inlines.Clear();
                    textBlock.Inlines.Add(XAMLHelper.ParseToInline(text));
                }
                else
                    textBlock.Text = text;
            }
#if WINDOWS_UWP
            else if (dependencyObject is TextBox)
            {
                var element = dependencyObject as TextBox;
                element.Header = text;
            }
            else if (dependencyObject is ComboBox)
            {
                var element = dependencyObject as ComboBox;
                element.Header = text;
            }
            else if (dependencyObject is ButtonBase)
                (dependencyObject as ButtonBase).Content = text;
#else
            else if (dependencyObject is HeaderedContentControl)
                (dependencyObject as HeaderedContentControl).Header = text;
            else if (dependencyObject is HeaderedItemsControl)
                (dependencyObject as HeaderedItemsControl).Header = text;
            else if (dependencyObject is GridViewColumn)
                (dependencyObject as GridViewColumn).Header = text;

#endif
            else if (dependencyObject is ContentControl)
                (dependencyObject as ContentControl).Content = text;
            else
            {
                // We just try to get a title property - this will not work in UWP Native
                var title = dependencyObject.GetType().GetProperty("Title", BindingFlags.Public | BindingFlags.Instance);
                if (title != null)
                    title.SetValue(dependencyObject, text);
            }
        }

        #endregion Dependency Attached Property Text

        #region Dependency Attached Property Tooltip

        /// <summary>
        /// Identifies the <see cref="TooltipProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TooltipProperty = DependencyProperty.RegisterAttached("Tooltip", typeof(string), typeof(Localized), new PropertyMetadata("", OnTooltipChanged));

        /// <summary>
        /// Gets the value of <see cref="TooltipProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetTooltip(DependencyObject obj)
        {
            return (string)obj.GetValue(TooltipProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="TooltipProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetTooltip(DependencyObject obj, string value)
        {
            obj.SetValue(TooltipProperty, value);
        }

        private static void OnTooltipChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var text = AssignText(dependencyObject, args.NewValue as string);
            var frameworkElement = dependencyObject as FrameworkElement;

            if (frameworkElement == null)
                return;

            if (text.StartsWith("<Inline>") && text.EndsWith("</Inline>"))
            {
                var textBlock = new TextBlock();
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(XAMLHelper.ParseToInline(text));
                textBlock.VerticalAlignment = VerticalAlignment.Stretch;
                textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.TextTrimming = TextTrimming.None;
                textBlock.TextAlignment = TextAlignment.Justify;
                textBlock.MaxWidth = 350;

#if WINDOWS_UWP
                ToolTipService.SetToolTip(frameworkElement, textBlock);
#else
                frameworkElement.ToolTip = textBlock;
#endif
            }
            else

#if WINDOWS_UWP
                ToolTipService.SetToolTip(frameworkElement, text);
#else
                frameworkElement.ToolTip = text;
#endif
        }

        #endregion Dependency Attached Property Tooltip
    }
}