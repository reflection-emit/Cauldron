#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else

using System.Windows;
using System.Windows.Controls;

#endif

namespace Couldron.Attached
{
    public static partial class Localized
    {
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
            var text = AssignText(dependencyObject, args.NewValue as string);

            if (dependencyObject is TextBlock)
                (dependencyObject as TextBlock).Text = text;

#if !NETFX_CORE
            else if (dependencyObject is HeaderedContentControl)
                (dependencyObject as HeaderedContentControl).Header = text;
#endif
            else if (dependencyObject is ContentControl)
                (dependencyObject as ContentControl).Content = text;
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

        #endregion Dependency Attached Property Tooltip

        private static string AssignText(DependencyObject dependencyObject, string text)
        {
            if (Factory.HasContract(typeof(ILocalizationSource)))
            {
                var localized = Factory.Create<Localization>();
                return localized[text];
            }
#if !NETFX_CORE
            else
            {
                var defaultWindows = Utils.GetStringFromModule(text);
                if (defaultWindows != null)
                    return defaultWindows;
            }
#endif

            return text;
        }
    }
}