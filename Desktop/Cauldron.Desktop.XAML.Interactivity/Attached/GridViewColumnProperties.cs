using System.Windows;

namespace Cauldron.XAML.Interactivity.Attached
{
    /// <summary>
    /// Provides attached properties for the <see cref="GridViewColumn"/> control
    /// </summary>
    public static class GridViewColumnProperties
    {
        #region Dependency Attached Property SortingPropertyName

        /// <summary>
        /// Identifies the SortingPropertyName dependency property
        /// </summary>
        internal static readonly DependencyProperty SortingPropertyNameProperty = DependencyProperty.RegisterAttached("SortingPropertyName", typeof(string), typeof(GridViewColumnProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of SortingPropertyName
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        internal static string GetSortingPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(SortingPropertyNameProperty);
        }

        /// <summary>
        /// Sets the value of the SortingPropertyName attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        internal static void SetSortingPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(SortingPropertyNameProperty, value);
        }

        #endregion Dependency Attached Property SortingPropertyName

        #region Dependency Attached Property Formatting

        /// <summary>
        /// Identifies the Formatting dependency property
        /// </summary>
        public static readonly DependencyProperty FormattingProperty = DependencyProperty.RegisterAttached("Formatting", typeof(GridViewColumnFormatting), typeof(GridViewColumnProperties), new PropertyMetadata(GridViewColumnFormatting.TextLeft));

        /// <summary>
        /// Gets the value of Formatting
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static GridViewColumnFormatting GetFormatting(DependencyObject obj)
        {
            return (GridViewColumnFormatting)obj.GetValue(FormattingProperty);
        }

        /// <summary>
        /// Sets the value of the Formatting attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetFormatting(DependencyObject obj, GridViewColumnFormatting value)
        {
            obj.SetValue(FormattingProperty, value);
        }

        #endregion Dependency Attached Property Formatting
    }
}