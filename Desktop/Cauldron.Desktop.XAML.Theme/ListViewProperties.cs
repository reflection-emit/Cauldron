using System.Collections;
using System.Windows;
using System.Windows.Input;

namespace Cauldron.XAML.Theme
{
    /// <summary>
    /// Provides attached properties for the <see cref="ListView"/>s, that can be used to extend the functionalities of the control
    /// </summary>
    public static class ListViewProperties
    {
        #region Dependency Attached Property Header

        /// <summary>
        /// Identifies the Header dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(string), typeof(ListViewProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of Header
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetHeader(DependencyObject obj)
        {
            return (string)obj.GetValue(HeaderProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetHeader(DependencyObject obj, string value)
        {
            obj.SetValue(HeaderProperty, value);
        }

        #endregion Dependency Attached Property Header

        #region Dependency Attached Property SelectedItems

        /// <summary>
        /// Identifies the SelectedItems dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached("SelectedItems", typeof(IEnumerable), typeof(ListViewProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of SelectedItems
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static IEnumerable GetSelectedItems(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(SelectedItemsProperty);
        }

        /// <summary>
        /// Sets the value of the SelectedItems attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetSelectedItems(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        #endregion Dependency Attached Property SelectedItems

        #region Dependency Attached Property Command

        /// <summary>
        /// Identifies the Command dependency property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ListViewProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of Command
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the value of the Command attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        #endregion Dependency Attached Property Command
    }
}