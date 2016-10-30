using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Cauldron.XAML.Interactivity.Attached
{
    /// <summary>
    /// Provides Attached Properties to <see cref="Hyperlink"/>
    /// </summary>
    public static class HyperlinkProperties
    {
        #region Dependency Attached Property NavigateUri

        /// <summary>
        /// Identifies the NavigateUri dependency property
        /// </summary>
        public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.RegisterAttached("NavigateUri", typeof(Uri), typeof(HyperlinkProperties), new PropertyMetadata(null, OnNavigateUriChanged));

        /// <summary>
        /// Gets the value of NavigateUri
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static Uri GetNavigateUri(DependencyObject obj)
        {
            return (Uri)obj.GetValue(NavigateUriProperty);
        }

        /// <summary>
        /// Sets the value of the NavigateUri attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetNavigateUri(DependencyObject obj, Uri value)
        {
            obj.SetValue(NavigateUriProperty, value);
        }

        private static void OnNavigateUriChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var obj = dependencyObject as Hyperlink;

            if (obj == null)
                throw new NotSupportedException("This attached property can only be attached to a System.Windows.Documents.Hyperlink");

            obj.Click += (s, e) =>
            {
                var uri = args.NewValue as Uri;

                if (uri == null)
                    return;

                Process.Start(uri.ToString());
            };
        }

        #endregion Dependency Attached Property NavigateUri
    }
}