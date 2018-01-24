using Cauldron.XAML.Controls;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;

namespace Cauldron.XAML.Theme
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWPOS
    {
        public IntPtr hwnd;
        public IntPtr hwndInsertAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int flags;
    }

    /// <exclude />
    public partial class CauldronWindow : Window, IDisposable
    {
        private Border border;
        private Button closeButton;
        private IntPtr hwnd;
        private HwndSource hwndSource;
        private Image icon;
        private Button maximizeButton;
        private Button minimizeButton;
        private Button restoreButton;
        private TextBlock title;

        /// <summary>
        /// Initializes a new instance of <see cref="CauldronWindow"/>
        /// </summary>
        public CauldronWindow()
        {
            this.InitializeComponent();
            this.SourceInitialized += LightWindow_SourceInitialized;
            this.Activated += LightWindow_Activated;
            this.Deactivated += LightWindow_Deactivated;
        }

        #region Dependency Property WindowToolbarTemplate

        /// <summary>
        /// Identifies the <see cref="WindowToolbarTemplate"/> dependency property
        /// </summary>
        internal static readonly DependencyProperty WindowToolbarTemplateProperty = DependencyProperty.Register(nameof(WindowToolbarTemplate), typeof(ControlTemplate), typeof(CauldronWindow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="WindowToolbarTemplate"/> Property
        /// </summary>
        internal ControlTemplate WindowToolbarTemplate
        {
            get { return (ControlTemplate)this.GetValue(WindowToolbarTemplateProperty); }
            set { this.SetValue(WindowToolbarTemplateProperty, value); }
        }

        #endregion Dependency Property WindowToolbarTemplate

        #region Dependency Property CanGoBack

        /// <summary>
        /// Identifies the <see cref="CanGoBack"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack), typeof(bool), typeof(CauldronWindow), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="CanGoBack"/> Property
        /// </summary>
        public bool CanGoBack
        {
            get { return (bool)this.GetValue(CanGoBackProperty); }
            set { this.SetValue(CanGoBackProperty, value); }
        }

        #endregion Dependency Property CanGoBack

        /// <exclude/>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (oldContent is NavigationFrame oldNavigationFrame)
                BindingOperations.ClearBinding(this, CanGoBackProperty);

            if (newContent is NavigationFrame newNavigationFrame)
                this.SetBinding(CanGoBackProperty, newNavigationFrame, new PropertyPath("CanGoBack"), System.Windows.Data.BindingMode.OneWay);
            else
                this.CanGoBack = false;

            if (this.Content != null && this.Content is DependencyObject)
                this.WindowToolbarTemplate = WindowToolbar.GetTemplate(this.Content as DependencyObject);
            else
                this.WindowToolbarTemplate = null;
        }

        /// <exclude/>
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            this.SetWindowStateDependentEffects();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) => this.Content.As<NavigationFrame>()?.GoBack(Navigation.NavigationType.BackButton);

        private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void LightWindow_Activated(object sender, EventArgs e)
        {
            if (this.title != null)
                this.title.Foreground = Application.Current.Resources[CauldronTheme.TextBrush] as SolidColorBrush;

            if (this.icon != null)
                (this.icon.Effect as GrayscaleEffect)?.IsNotNull(x => x.DesaturationFactor = 1.0);
        }

        private void LightWindow_Deactivated(object sender, EventArgs e)
        {
            if (this.title != null)
                this.title.Foreground = Application.Current.Resources[CauldronTheme.DisabledTextBrush] as SolidColorBrush;

            if (this.icon != null)
                (this.icon.Effect as GrayscaleEffect)?.IsNotNull(x => x.DesaturationFactor = 0.0);
        }

        private void LightWindow_SourceInitialized(object sender, EventArgs e)
        {
            this.hwnd = this.GetWindowHandle();
            this.hwndSource = HwndSource.FromHwnd(this.hwnd);

            this.hwndSource.AddHook(this.WindowProc);
            this.border = this.Template.FindName("border", this) as Border;
            this.title = this.Template.FindName("title", this) as TextBlock;
            this.icon = this.Template.FindName("icon", this) as Image;

            this.minimizeButton = this.Template.FindName("MinimizeButton", this) as Button;
            this.maximizeButton = this.Template.FindName("MaximizeButton", this) as Button;
            this.restoreButton = this.Template.FindName("RestoreButton", this) as Button;
            this.closeButton = this.Template.FindName("CloseButton", this) as Button;

            this.SetWindowStateDependentEffects();

            if (this.icon != null)
                this.icon.Effect = new GrayscaleEffect { DesaturationFactor = 1.0 };

            if (WindowConfiguration.GetWindowStyle(this) == WindowStyle.None)
            {
                this.icon.IsNotNull(x => x.Visibility = Visibility.Collapsed);
                this.title.IsNotNull(x => x.Visibility = Visibility.Collapsed);
            }

            //Win32Api.AddShadow(this, hwnd);
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ResizeMode != ResizeMode.NoResize)
                this.WindowState = WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ResizeMode != ResizeMode.NoResize)
                this.WindowState = WindowState.Minimized;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void SetWindowStateDependentEffects()
        {
            this.border.IsNotNull(x =>
            {
                switch (this.WindowState)
                {
                    case WindowState.Normal:
                        x.BorderThickness = new Thickness(1);
                        x.Margin = new Thickness(0);
                        break;

                    case WindowState.Minimized:
                    case WindowState.Maximized:
                        x.BorderThickness = new Thickness(0);
                        x.Margin = new Thickness(8);
                        break;

                    default:
                        break;
                }
            });

            if (this.ResizeMode == ResizeMode.NoResize)
            {
                this.minimizeButton.Visibility = Visibility.Collapsed;
                this.maximizeButton.Visibility = Visibility.Collapsed;
                this.closeButton.Visibility = Visibility.Visible;
                this.restoreButton.Visibility = Visibility.Collapsed;
            }
            else if (this.ResizeMode == ResizeMode.CanMinimize)
            {
                this.minimizeButton.Visibility = Visibility.Visible;
                this.maximizeButton.Visibility = Visibility.Collapsed;
                this.closeButton.Visibility = Visibility.Visible;
                this.restoreButton.Visibility = Visibility.Collapsed;
            }
            else if (this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                this.minimizeButton.Visibility = Visibility.Visible;
                this.maximizeButton.Visibility = this.WindowState == WindowState.Normal ? Visibility.Visible : Visibility.Collapsed;
                this.closeButton.Visibility = Visibility.Visible;
                this.restoreButton.Visibility = this.WindowState == WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool isHandled)
        {
            if (msg == 0x0024)
            {
                MonitorInfo.WmGetMinMaxInfo(this.GetWindowHandle(), lParam);
                isHandled = true;
            }
            else if (msg == 0x0046 /* WINDOWPOSCHANGING */)
            {
                // https://stackoverflow.com/questions/1718666/how-to-enforce-minwidth-minheight-in-a-wpf-window-where-windowstyle-none

                var pos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                if ((pos.flags & 0x0002 /* SWP_NOMOVE */) != 0)
                    return IntPtr.Zero;

                var window = HwndSource.FromHwnd(hwnd).RootVisual as Window;
                if (window == null)
                    return IntPtr.Zero;

                var source = PresentationSource.FromVisual(this.window);

                var changedPos = false;
                var m11 = source.CompositionTarget.TransformToDevice.M11;
                var m22 = source.CompositionTarget.TransformToDevice.M22;
                var x = pos.cx / m11;
                var y = pos.cy / m22;

                if (this.MinWidth > x)
                {
                    pos.cx = (int)(this.MinWidth * m11);
                    changedPos = true;
                }

                if (this.MinHeight > y)
                {
                    pos.cy = (int)(this.MinHeight * m22);
                    changedPos = true;
                }

                if (!changedPos)
                    return IntPtr.Zero;

                Marshal.StructureToPtr(pos, lParam, true);
                isHandled = true;
            }

            return IntPtr.Zero;
        }

        #region Disposable Implementation

        private bool disposed = false;

        /// <summary>
        /// Destructors are used to destruct instances of classes.
        /// </summary>
        ~CauldronWindow()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occures if the object has been disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets a value indicating if the object has been disposed or not
        /// </summary>
        public bool IsDisposed { get { return this.disposed; } }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing">true if managed resources requires disposing</param>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // Note disposing has been done.
                disposed = true;

                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this.hwndSource?.RemoveHook(this.WindowProc);
                    this.hwndSource?.Dispose();
                    this.hwndSource = null;
                }

                this.Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion Disposable Implementation
    }
}