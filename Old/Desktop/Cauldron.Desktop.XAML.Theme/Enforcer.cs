using Cauldron.Core.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Cauldron.XAML.Theme
{
    internal static class Enforcer
    {
        #region Dependency Attached Property ForcedResource

        /// <summary>
        /// Identifies the ForcedResource dependency property
        /// </summary>
        public static readonly DependencyProperty ForcedResourceProperty = DependencyProperty.RegisterAttached("ForcedResource", typeof(ResourceDictionary), typeof(Enforcer), new PropertyMetadata(null, OnForcedResourceChanged));

        /// <summary>
        /// Gets the value of ForcedResource
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static ResourceDictionary GetForcedResource(DependencyObject obj)
        {
            return (ResourceDictionary)obj.GetValue(ForcedResourceProperty);
        }

        /// <summary>
        /// Sets the value of the ForcedResource attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetForcedResource(DependencyObject obj, ResourceDictionary value)
        {
            obj.SetValue(ForcedResourceProperty, value);
        }

        private static void OnForcedResourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var frameworkElement = dependencyObject.As<FrameworkElement>();

            if (frameworkElement == null)
                return;

            frameworkElement.Resources = args.NewValue as ResourceDictionary;
        }

        #endregion Dependency Attached Property ForcedResource

        #region Dependency Attached Property ForegroundColorAutoSelect

        /// <summary>
        /// Identifies the ForegroundColorAutoSelect dependency property
        /// </summary>
        public static readonly DependencyProperty ForegroundColorAutoSelectProperty = DependencyProperty.RegisterAttached("ForegroundColorAutoSelect", typeof(object), typeof(Enforcer), new PropertyMetadata(null, OnForegroundColorAutoSelectChanged));

        /// <summary>
        /// Gets the value of ForegroundColorAutoSelect
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static object GetForegroundColorAutoSelect(DependencyObject obj)
        {
            return (object)obj.GetValue(ForegroundColorAutoSelectProperty);
        }

        /// <summary>
        /// Sets the value of the ForegroundColorAutoSelect attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetForegroundColorAutoSelect(DependencyObject obj, object value)
        {
            obj.SetValue(ForegroundColorAutoSelectProperty, value);
        }

        private static void OnForegroundColorAutoSelectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var color = (Color)Application.Current.Resources[CauldronTheme.AccentColor];
            var foreground = Application.Current.Resources[CauldronTheme.HoveredTextBrush] as SolidColorBrush;

            if (!(384 - color.R - color.G - color.B > 0))
                foreground = Application.Current.Resources[CauldronTheme.DarkBackgroundBrush] as SolidColorBrush;

            if (dependencyObject is TextBlock) (dependencyObject as TextBlock).Foreground = foreground;
            else if (dependencyObject is ButtonBase) (dependencyObject as ButtonBase).Foreground = foreground;
            else if (dependencyObject is TextBox) (dependencyObject as TextBox).Foreground = foreground;
            else if (dependencyObject is Control) (dependencyObject as Control).Foreground = foreground;
        }

        #endregion Dependency Attached Property ForegroundColorAutoSelect
    }
}