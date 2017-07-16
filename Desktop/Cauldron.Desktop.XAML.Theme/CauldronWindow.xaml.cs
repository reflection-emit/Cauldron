using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.XAML.Controls;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Cauldron.XAML.Theme
{
    public partial class CauldronWindow : IDisposable
    {
        private Border border;
        private Button closeButton;
        private IntPtr hwnd;
        private HwndSource hwndSource;
        private Image icon;
        private Button maximizeButton;
        private Button minimizeButton;
        private Button restoreButton;
        private Grid sizer;
        private TextBlock title;
        private Rectangle titleRect;
        private Thumb titleThumb;

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

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            var oldNavigationFrame = oldContent as NavigationFrame;
            var newNavigationFrame = newContent as NavigationFrame;

            if (oldNavigationFrame != null)
                BindingOperations.ClearBinding(this, CanGoBackProperty);

            if (newNavigationFrame != null)
                this.SetBinding(CanGoBackProperty, newNavigationFrame, new PropertyPath("CanGoBack"), System.Windows.Data.BindingMode.OneWay);
            else
                this.CanGoBack = false;

            if (this.Content != null && this.Content is DependencyObject)
                this.WindowToolbarTemplate = WindowToolbar.GetTemplate(this.Content as DependencyObject);
            else
                this.WindowToolbarTemplate = null;
        }

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
            this.titleThumb = this.Template.FindName("TitleThumb", this) as Thumb;
            this.sizer = this.Template.FindName("sizer", this) as Grid;
            this.titleRect = this.Template.FindName("TitleRect", this) as Rectangle;

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
                this.titleThumb.IsNotNull(x => x.Visibility = Visibility.Collapsed);
                this.title.IsNotNull(x => x.Visibility = Visibility.Collapsed);
                this.titleRect.IsNotNull(x => Grid.SetRowSpan(this, 3));
            }
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

        private void Resizer_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.BeginInit();
            sender.As<Thumb>().IsNotNull(x =>
            {
                if (x.VerticalAlignment == VerticalAlignment.Bottom)
                    this.Height = Mathc.Clamp(this.Height + e.VerticalChange, this.MinHeight, this.MaxHeight);

                if (x.HorizontalAlignment == HorizontalAlignment.Right)
                    this.Width = Mathc.Clamp(this.Width + e.HorizontalChange, this.MinWidth, this.MaxWidth);

                if (x.VerticalAlignment == VerticalAlignment.Top)
                {
                    var height = this.Height;
                    this.Height = Mathc.Clamp(this.Height - e.VerticalChange, this.MinHeight, this.MaxHeight);

                    if (this.Height != height)
                        this.Top += e.VerticalChange;
                }

                if (x.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    var width = this.Width;
                    this.Width = Mathc.Clamp(this.Width - e.HorizontalChange, this.MinWidth, this.MaxWidth);

                    if (this.Width != width)
                        this.Left += e.HorizontalChange;
                }
            });
            this.EndInit();
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
                        break;

                    case WindowState.Minimized:
                    case WindowState.Maximized:
                        x.BorderThickness = new Thickness(0);
                        break;

                    default:
                        break;
                }
            });

            this.titleThumb.IsNotNull(x => x.IsHitTestVisible = this.WindowState == WindowState.Maximized);
            this.sizer.IsNotNull(x => x.Visibility = this.WindowState == WindowState.Maximized || this.ResizeMode == ResizeMode.NoResize ? Visibility.Collapsed : Visibility.Visible);

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

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1 && this.WindowState == WindowState.Normal)
                this.DragMove();
            else if (e.ClickCount == 2 && this.WindowState == WindowState.Normal && this.ResizeMode != ResizeMode.NoResize)
                this.WindowState = WindowState.Maximized;
        }

        private void TitleThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized || (Math.Abs(e.HorizontalChange) < 3 && Math.Abs(e.VerticalChange) < 3) || this.ResizeMode == ResizeMode.NoResize)
                return;

            var mouse = Mouse.GetPosition(this);
            var oldWidth = this.ActualWidth;
            var mouseOnScreen = new Func<Point>(() =>
            {
                var position = Win32Api.GetMousePosition();
                var source = PresentationSource.FromVisual(window);
                return new Point(position.X / source.CompositionTarget.TransformToDevice.M11, position.Y / source.CompositionTarget.TransformToDevice.M22);
            })();

            var currentLeft = mouse.X + 5;

            this.WindowState = WindowState.Normal;

            if (currentLeft > this.ActualWidth / 2 && currentLeft < oldWidth / 2)
                this.Left = mouseOnScreen.X - (this.ActualWidth / 2);
            else if (currentLeft > this.ActualWidth / 2 && currentLeft > oldWidth / 2)
                this.Left = mouseOnScreen.X - (this.ActualWidth - (oldWidth - currentLeft));
            else
                this.Left = mouseOnScreen.X - currentLeft;

            this.Top = mouseOnScreen.Y - mouse.Y - 5;

            var lParam = (int)(uint)mouse.X | ((int)mouse.Y << 16);

            var hwnd = this.GetWindowHandle();
            Win32Api.SendMessage(hwnd, WindowsMessages.LBUTTONUP, (IntPtr)0x2 /* HT_CAPTION */, (IntPtr)lParam);
            Win32Api.SendMessage(hwnd, WindowsMessages.SYSCOMMAND, (IntPtr)0xf012 /* SC_MOUSEMOVE */, IntPtr.Zero);
        }

        private void TitleThumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.ResizeMode != ResizeMode.NoResize)
                this.WindowState = WindowState.Normal;
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool isHandled)
        {
            if (msg == 0x0024)
            {
                MonitorInfo.WmGetMinMaxInfo(this.GetWindowHandle(), lParam);
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

                if (this.Disposed != null)
                    this.Disposed(this, EventArgs.Empty);
            }
        }

        #endregion Disposable Implementation
    }
}