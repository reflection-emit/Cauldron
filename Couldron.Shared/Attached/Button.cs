using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Couldron.Attached
{
    /// <summary>
    /// Provides attached properties for the Button control
    /// </summary>
    public static class Button
    {
        #region Dependency Attached Property Image

        /// <summary>
        /// Identifies the <see cref="ImageProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached("Image", typeof(BitmapImage), typeof(Button), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of <see cref="ImageProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static BitmapImage GetImage(DependencyObject obj)
        {
            return (BitmapImage)obj.GetValue(ImageProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="ImageProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetImage(DependencyObject obj, BitmapImage value)
        {
            obj.SetValue(ImageProperty, value);
        }

        #endregion Dependency Attached Property Image

        #region Dependency Attached Property Symbol

        /// <summary>
        /// Identifies the <see cref="SymbolProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SymbolProperty = DependencyProperty.RegisterAttached("Symbol", typeof(int), typeof(Button), new PropertyMetadata(0));

        /// <summary>
        /// Gets the value of <see cref="SymbolProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static int GetSymbol(DependencyObject obj)
        {
            return (int)obj.GetValue(SymbolProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="SymbolProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetSymbol(DependencyObject obj, int value)
        {
            obj.SetValue(SymbolProperty, value);
        }

        #endregion Dependency Attached Property Symbol

        #region Dependency Attached Property SymbolFont

        /// <summary>
        /// Identifies the <see cref="SymbolFontProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SymbolFontProperty = DependencyProperty.RegisterAttached("SymbolFont", typeof(FontFamily), typeof(Button), new PropertyMetadata(new FontFamily("Segoe UI")));

        /// <summary>
        /// Gets the value of <see cref="SymbolFontProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static FontFamily GetSymbolFont(DependencyObject obj)
        {
            return (FontFamily)obj.GetValue(SymbolFontProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="SymbolFontProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetSymbolFont(DependencyObject obj, FontFamily value)
        {
            obj.SetValue(SymbolFontProperty, value);
        }

        #endregion Dependency Attached Property SymbolFont

        #region Dependency Attached Property SymbolSize

        /// <summary>
        /// Identifies the <see cref="SymbolSizeProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SymbolSizeProperty = DependencyProperty.RegisterAttached("SymbolSize", typeof(double), typeof(Button), new PropertyMetadata(16));

        /// <summary>
        /// Gets the value of <see cref="SymbolSizeProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetSymbolSize(DependencyObject obj)
        {
            return (double)obj.GetValue(SymbolSizeProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="SymbolSizeProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetSymbolSize(DependencyObject obj, double value)
        {
            obj.SetValue(SymbolSizeProperty, value);
        }

        #endregion Dependency Attached Property SymbolSize

        #region Dependency Attached Property SymbolAssign

        /// <summary>
        /// Identifies the <see cref="SymbolAssignProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SymbolAssignProperty = DependencyProperty.RegisterAttached("SymbolAssign", typeof(string), typeof(Button), new PropertyMetadata("", OnSymbolAssignChanged));

        /// <summary>
        /// Gets the value of <see cref="SymbolAssignProperty" />
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetSymbolAssign(DependencyObject obj)
        {
            return (string)obj.GetValue(SymbolAssignProperty);
        }

        private static void OnSymbolAssignChanged(DependencyObject dependencyProperty, DependencyPropertyChangedEventArgs args)
        {
            var d = dependencyProperty as System.Windows.Controls.Button;
        }

        /// <summary>
        /// Sets the value of the <see cref="SymbolAssignProperty" /> attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        private static void SetSymbolAssign(DependencyObject obj, string value)
        {
            obj.SetValue(SymbolAssignProperty, value);
        }

        #endregion Dependency Attached Property SymbolAssign
    }
}