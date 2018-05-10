using Cauldron.Core.Reflection;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Cauldron.XAML
{
    /// <summary>
    /// Provides attached properties used to configure the <see cref="Window"/> that will contain the view
    /// </summary>
    public static class WindowConfiguration
    {
        #region Dependency Attached Property SizeToContent

        /// <summary>
        /// Identifies the SizeToContent dependency property
        /// </summary>
        public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.RegisterAttached("SizeToContent", typeof(SizeToContent), typeof(WindowConfiguration), new PropertyMetadata(SizeToContent.Manual));

        /// <summary>
        /// Gets the value of SizeToContent
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static SizeToContent GetSizeToContent(DependencyObject obj)
        {
            return (SizeToContent)obj.GetValue(SizeToContentProperty);
        }

        /// <summary>
        /// Sets the value of the SizeToContent attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetSizeToContent(DependencyObject obj, SizeToContent value)
        {
            obj.SetValue(SizeToContentProperty, value);
        }

        #endregion Dependency Attached Property SizeToContent

        #region Dependency Attached Property IsModal

        /// <summary>
        /// Identifies the IsModal dependency property
        /// </summary>
        public static readonly DependencyProperty IsModalProperty = DependencyProperty.RegisterAttached("IsModal", typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(false));

        /// <summary>
        /// Gets the value of IsModal
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetIsModal(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsModalProperty);
        }

        /// <summary>
        /// Sets the value of the IsModal attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetIsModal(DependencyObject obj, bool value)
        {
            obj.SetValue(IsModalProperty, value);
        }

        #endregion Dependency Attached Property IsModal

        #region Dependency Attached Property IconKey

        /// <summary>
        /// Identifies the IconKey dependency property
        /// </summary>
        public static readonly DependencyProperty IconKeyProperty = DependencyProperty.RegisterAttached("IconKey", typeof(string), typeof(WindowConfiguration), new PropertyMetadata(null, OnIconKeyChanged));

        /// <summary>
        /// Gets the value of IconKey
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetIconKey(DependencyObject obj)
        {
            return (string)obj.GetValue(IconKeyProperty);
        }

        /// <summary>
        /// Sets the value of the IconKey attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetIconKey(DependencyObject obj, string value)
        {
            obj.SetValue(IconKeyProperty, value);
        }

        private static async void OnIconKeyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var name = args.NewValue as string;

            if (string.IsNullOrEmpty(name))
            {
                SetIcon(dependencyObject, null);
                return;
            }

            var data = Assemblies.GetManifestResource(name);
            SetIcon(dependencyObject, await data.ToBitmapImageAsync());
        }

        #endregion Dependency Attached Property IconKey

        #region Dependency Attached Property Icon

        /// <summary>
        /// Identifies the Icon dependency property
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(ImageSource), typeof(WindowConfiguration), new PropertyMetadata(null, OnIconChanged));

        /// <summary>
        /// Gets the value of Icon
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static ImageSource GetIcon(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(IconProperty);
        }

        /// <summary>
        /// Sets the value of the Icon attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetIcon(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(IconProperty, value);
        }

        private static async void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var value = args.NewValue as ImageSource;

            if (value == null)
                SetIcon(d, await UnsafeNative.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location).ToBitmapImageAsync());
        }

        #endregion Dependency Attached Property Icon

        #region Dependency Attached Property ResizeMode

        /// <summary>
        /// Identifies the ResizeMode dependency property
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty = DependencyProperty.RegisterAttached("ResizeMode", typeof(ResizeMode), typeof(WindowConfiguration), new PropertyMetadata(ResizeMode.CanResizeWithGrip));

        /// <summary>
        /// Gets the value of ResizeMode
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static ResizeMode GetResizeMode(DependencyObject obj) => (ResizeMode)(obj?.GetValue(ResizeModeProperty) ?? ResizeMode.CanResizeWithGrip);

        /// <summary>
        /// Sets the value of the ResizeMode attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetResizeMode(DependencyObject obj, ResizeMode value) => obj.SetValue(ResizeModeProperty, value);

        #endregion Dependency Attached Property ResizeMode

        #region Dependency Attached Property ShowInTaskbar

        /// <summary>
        /// Identifies the ShowInTaskbar dependency property
        /// </summary>
        public static readonly DependencyProperty ShowInTaskbarProperty = DependencyProperty.RegisterAttached("ShowInTaskbar", typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(true));

        /// <summary>
        /// Gets the value of ShowInTaskbar
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetShowInTaskbar(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowInTaskbarProperty);
        }

        /// <summary>
        /// Sets the value of the ShowInTaskbar attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetShowInTaskbar(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowInTaskbarProperty, value);
        }

        #endregion Dependency Attached Property ShowInTaskbar

        #region Dependency Attached Property Title

        /// <summary>
        /// Identifies the Title dependency property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(string), typeof(WindowConfiguration), new PropertyMetadata(ApplicationInfo.ApplicationName));

        /// <summary>
        /// Gets the value of Title
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetTitle(DependencyObject obj)
        {
            return (string)obj.GetValue(TitleProperty);
        }

        /// <summary>
        /// Sets the value of the Title attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetTitle(DependencyObject obj, string value)
        {
            obj.SetValue(TitleProperty, value);
        }

        #endregion Dependency Attached Property Title

        #region Dependency Attached Property Topmost

        /// <summary>
        /// Identifies the Topmost dependency property
        /// </summary>
        public static readonly DependencyProperty TopmostProperty = DependencyProperty.RegisterAttached("Topmost", typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(false));

        /// <summary>
        /// Gets the value of Topmost
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetTopmost(DependencyObject obj)
        {
            return (bool)obj.GetValue(TopmostProperty);
        }

        /// <summary>
        /// Sets the value of the Topmost attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetTopmost(DependencyObject obj, bool value)
        {
            obj.SetValue(TopmostProperty, value);
        }

        #endregion Dependency Attached Property Topmost

        #region Dependency Attached Property WindowStartupLocation

        /// <summary>
        /// Identifies the WindowStartupLocation dependency property
        /// </summary>
        public static readonly DependencyProperty WindowStartupLocationProperty = DependencyProperty.RegisterAttached("WindowStartupLocation", typeof(WindowStartupLocation), typeof(WindowConfiguration), new PropertyMetadata(WindowStartupLocation.CenterOwner));

        /// <summary>
        /// Gets the value of WindowStartupLocation
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static WindowStartupLocation GetWindowStartupLocation(DependencyObject obj)
        {
            return (WindowStartupLocation)obj.GetValue(WindowStartupLocationProperty);
        }

        /// <summary>
        /// Sets the value of the WindowStartupLocation attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetWindowStartupLocation(DependencyObject obj, WindowStartupLocation value)
        {
            obj.SetValue(WindowStartupLocationProperty, value);
        }

        #endregion Dependency Attached Property WindowStartupLocation

        #region Dependency Attached Property IsWindowPersistent

        /// <summary>
        /// Identifies the IsWindowPersistent dependency property
        /// </summary>
        public static readonly DependencyProperty IsWindowPersistentProperty = DependencyProperty.RegisterAttached("IsWindowPersistent", typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(false));

        /// <summary>
        /// Gets the value of IsWindowPersistent
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetIsWindowPersistent(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWindowPersistentProperty);
        }

        /// <summary>
        /// Sets the value of the IsWindowPersistent attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetIsWindowPersistent(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWindowPersistentProperty, value);
        }

        #endregion Dependency Attached Property IsWindowPersistent

        #region Dependency Attached Property WindowState

        /// <summary>
        /// Identifies the WindowState dependency property
        /// </summary>
        public static readonly DependencyProperty WindowStateProperty = DependencyProperty.RegisterAttached("WindowState", typeof(WindowState), typeof(WindowConfiguration), new PropertyMetadata(WindowState.Normal));

        /// <summary>
        /// Gets the value of WindowState
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static WindowState GetWindowState(DependencyObject obj)
        {
            return (WindowState)obj.GetValue(WindowStateProperty);
        }

        /// <summary>
        /// Sets the value of the WindowState attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetWindowState(DependencyObject obj, WindowState value)
        {
            obj.SetValue(WindowStateProperty, value);
        }

        #endregion Dependency Attached Property WindowState

        #region Dependency Attached Property HasOwner

        /// <summary>
        /// Identifies the HasOwner dependency property
        /// </summary>
        public static readonly DependencyProperty HasOwnerProperty = DependencyProperty.RegisterAttached("HasOwner", typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(true));

        /// <summary>
        /// Gets the value of HasOwner
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static bool GetHasOwner(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasOwnerProperty);
        }

        /// <summary>
        /// Sets the value of the HasOwner attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetHasOwner(DependencyObject obj, bool value)
        {
            obj.SetValue(HasOwnerProperty, value);
        }

        #endregion Dependency Attached Property HasOwner

        #region Dependency Attached Property MinWidth

        /// <summary>
        /// Identifies the MinWidth dependency property
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.RegisterAttached("MinWidth", typeof(double), typeof(WindowConfiguration), new PropertyMetadata(300.0));

        /// <summary>
        /// Gets the value of MinWidth
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetMinWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(MinWidthProperty);
        }

        /// <summary>
        /// Sets the value of the MinWidth attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetMinWidth(DependencyObject obj, double value)
        {
            obj.SetValue(MinWidthProperty, value);
        }

        #endregion Dependency Attached Property MinWidth

        #region Dependency Attached Property MinHeight

        /// <summary>
        /// Identifies the MinHeight dependency property
        /// </summary>
        public static readonly DependencyProperty MinHeightProperty = DependencyProperty.RegisterAttached("MinHeight", typeof(double), typeof(WindowConfiguration), new PropertyMetadata(120.0));

        /// <summary>
        /// Gets the value of MinHeight
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetMinHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(MinHeightProperty);
        }

        /// <summary>
        /// Sets the value of the MinHeight attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetMinHeight(DependencyObject obj, double value)
        {
            obj.SetValue(MinHeightProperty, value);
        }

        #endregion Dependency Attached Property MinHeight

        #region Dependency Attached Property Height

        /// <summary>
        /// Identifies the Height dependency property
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached("Height", typeof(double), typeof(WindowConfiguration), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets the value of Height
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(HeightProperty);
        }

        /// <summary>
        /// Sets the value of the Height attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetHeight(DependencyObject obj, double value)
        {
            obj.SetValue(HeightProperty, value);
        }

        #endregion Dependency Attached Property Height

        #region Dependency Attached Property Width

        /// <summary>
        /// Identifies the Width dependency property
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("Width", typeof(double), typeof(WindowConfiguration), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets the value of Width
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(WidthProperty);
        }

        /// <summary>
        /// Sets the value of the Width attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetWidth(DependencyObject obj, double value)
        {
            obj.SetValue(WidthProperty, value);
        }

        #endregion Dependency Attached Property Width

        #region Dependency Attached Property WindowStyle

        /// <summary>
        /// Identifies the WindowStyle dependency property
        /// </summary>
        public static readonly DependencyProperty WindowStyleProperty = DependencyProperty.RegisterAttached("WindowStyle", typeof(WindowStyle), typeof(WindowConfiguration), new PropertyMetadata(WindowStyle.SingleBorderWindow));

        /// <summary>
        /// Gets the value of WindowStyle
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static WindowStyle GetWindowStyle(DependencyObject obj)
        {
            return (WindowStyle)obj.GetValue(WindowStyleProperty);
        }

        /// <summary>
        /// Sets the value of the WindowStyle attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetWindowStyle(DependencyObject obj, WindowStyle value)
        {
            obj.SetValue(WindowStyleProperty, value);
        }

        #endregion Dependency Attached Property WindowStyle

        #region Dependency Attached Property MaxHeight

        /// <summary>
        /// Identifies the MaxHeight dependency property
        /// </summary>
        public static readonly DependencyProperty MaxHeightProperty = DependencyProperty.RegisterAttached("MaxHeight", typeof(double), typeof(WindowConfiguration), new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Gets the value of MaxHeight
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetMaxHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(MaxHeightProperty);
        }

        /// <summary>
        /// Sets the value of the MaxHeight attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetMaxHeight(DependencyObject obj, double value)
        {
            obj.SetValue(MaxHeightProperty, value);
        }

        #endregion Dependency Attached Property MaxHeight

        #region Dependency Attached Property MaxWidth

        /// <summary>
        /// Identifies the MaxWidth dependency property
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.RegisterAttached("MaxWidth", typeof(double), typeof(WindowConfiguration), new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Gets the value of MaxWidth
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetMaxWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(MaxWidthProperty);
        }

        /// <summary>
        /// Sets the value of the MaxWidth attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetMaxWidth(DependencyObject obj, double value)
        {
            obj.SetValue(MaxWidthProperty, value);
        }

        #endregion Dependency Attached Property MaxWidth
    }
}