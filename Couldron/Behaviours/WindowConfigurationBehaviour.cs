using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Couldron.Behaviours
{
    /// <summary>
    /// Provides a behaviour to configure the window that will contain the view
    /// </summary>
    [BehaviourUsage(false)]
    public sealed class WindowConfigurationBehaviour : Behaviour<FrameworkElement>
    {
        #region Dependency Property IsDialog

        /// <summary>
        /// Identifies the <see cref="IsDialog" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsDialogProperty = DependencyProperty.Register(nameof(IsDialog), typeof(bool), typeof(WindowConfigurationBehaviour), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="IsDialog" /> Property
        /// </summary>
        public bool IsDialog
        {
            get { return (bool)this.GetValue(IsDialogProperty); }
            set { this.SetValue(IsDialogProperty, value); }
        }

        #endregion Dependency Property IsDialog

        #region Dependency Property Template

        /// <summary>
        /// Identifies the <see cref="Template" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TemplateProperty = DependencyProperty.Register(nameof(Template), typeof(ControlTemplate), typeof(WindowConfigurationBehaviour), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Template" /> Property
        /// </summary>
        public ControlTemplate Template
        {
            get { return (ControlTemplate)this.GetValue(TemplateProperty); }
            set { this.SetValue(TemplateProperty, value); }
        }

        #endregion Dependency Property Template

        #region Dependency Property Width

        /// <summary>
        /// Identifies the <see cref="Width" /> dependency property
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(nameof(Width), typeof(double), typeof(WindowConfigurationBehaviour), new PropertyMetadata(800.0));

        /// <summary>
        /// Gets or sets the <see cref="Width" /> Property
        /// </summary>
        public double Width
        {
            get { return (double)this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }

        #endregion Dependency Property Width

        #region Dependency Property Height

        /// <summary>
        /// Identifies the <see cref="Height" /> dependency property
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(nameof(Height), typeof(double), typeof(WindowConfigurationBehaviour), new PropertyMetadata(600.0));

        /// <summary>
        /// Gets or sets the <see cref="Height" /> Property
        /// </summary>
        public double Height
        {
            get { return (double)this.GetValue(HeightProperty); }
            set { this.SetValue(HeightProperty, value); }
        }

        #endregion Dependency Property Height

        #region Dependency Property MinimumWidth

        /// <summary>
        /// Identifies the <see cref="MinimumWidth" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MinimumWidthProperty = DependencyProperty.Register(nameof(MinimumWidth), typeof(double), typeof(WindowConfigurationBehaviour), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the <see cref="MinimumWidth" /> Property
        /// </summary>
        public double MinimumWidth
        {
            get { return (double)this.GetValue(MinimumWidthProperty); }
            set { this.SetValue(MinimumWidthProperty, value); }
        }

        #endregion Dependency Property MinimumWidth

        #region Dependency Property MinimumHeight

        /// <summary>
        /// Identifies the <see cref="MinimumHeight" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MinimumHeightProperty = DependencyProperty.Register(nameof(MinimumHeight), typeof(double), typeof(WindowConfigurationBehaviour), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the <see cref="MinimumHeight" /> Property
        /// </summary>
        public double MinimumHeight
        {
            get { return (double)this.GetValue(MinimumHeightProperty); }
            set { this.SetValue(MinimumHeightProperty, value); }
        }

        #endregion Dependency Property MinimumHeight

        #region Dependency Property MaximumWidth

        /// <summary>
        /// Identifies the <see cref="MaximumWidth" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MaximumWidthProperty = DependencyProperty.Register(nameof(MaximumWidth), typeof(double), typeof(WindowConfigurationBehaviour), new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Gets or sets the <see cref="MaximumWidth" /> Property
        /// </summary>
        public double MaximumWidth
        {
            get { return (double)this.GetValue(MaximumWidthProperty); }
            set { this.SetValue(MaximumWidthProperty, value); }
        }

        #endregion Dependency Property MaximumWidth

        #region Dependency Property MaximumHeight

        /// <summary>
        /// Identifies the <see cref="MaximumHeight" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MaximumHeightProperty = DependencyProperty.Register(nameof(MaximumHeight), typeof(double), typeof(WindowConfigurationBehaviour), new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Gets or sets the <see cref="MaximumHeight" /> Property
        /// </summary>
        public double MaximumHeight
        {
            get { return (double)this.GetValue(MaximumHeightProperty); }
            set { this.SetValue(MaximumHeightProperty, value); }
        }

        #endregion Dependency Property MaximumHeight

        #region Dependency Property Icon

        /// <summary>
        /// Identifies the <see cref="Icon" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(WindowConfigurationBehaviour), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Icon" /> Property
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        #endregion Dependency Property Icon

        #region Dependency Property ResizeMode

        /// <summary>
        /// Identifies the <see cref="ResizeMode" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty = DependencyProperty.Register(nameof(ResizeMode), typeof(ResizeMode), typeof(WindowConfigurationBehaviour), new PropertyMetadata(ResizeMode.CanResizeWithGrip));

        /// <summary>
        /// Gets or sets the <see cref="ResizeMode" /> Property
        /// </summary>
        public ResizeMode ResizeMode
        {
            get { return (ResizeMode)this.GetValue(ResizeModeProperty); }
            set { this.SetValue(ResizeModeProperty, value); }
        }

        #endregion Dependency Property ResizeMode

        #region Dependency Property ShowInTaskbar

        /// <summary>
        /// Identifies the <see cref="ShowInTaskbar" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ShowInTaskbarProperty = DependencyProperty.Register(nameof(ShowInTaskbar), typeof(bool), typeof(WindowConfigurationBehaviour), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the <see cref="ShowInTaskbar" /> Property
        /// </summary>
        public bool ShowInTaskbar
        {
            get { return (bool)this.GetValue(ShowInTaskbarProperty); }
            set { this.SetValue(ShowInTaskbarProperty, value); }
        }

        #endregion Dependency Property ShowInTaskbar

        #region Dependency Property Title

        /// <summary>
        /// Identifies the <see cref="Title" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(WindowConfigurationBehaviour), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="Title" /> Property
        /// </summary>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        #endregion Dependency Property Title

        #region Dependency Property Topmost

        /// <summary>
        /// Identifies the <see cref="Topmost" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TopmostProperty = DependencyProperty.Register(nameof(Topmost), typeof(bool), typeof(WindowConfigurationBehaviour), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="Topmost" /> Property
        /// </summary>
        public bool Topmost
        {
            get { return (bool)this.GetValue(TopmostProperty); }
            set { this.SetValue(TopmostProperty, value); }
        }

        #endregion Dependency Property Topmost

        #region Dependency Property WindowStartupLocation

        /// <summary>
        /// Identifies the <see cref="WindowStartupLocation" /> dependency property
        /// </summary>
        public static readonly DependencyProperty WindowStartupLocationProperty = DependencyProperty.Register(nameof(WindowStartupLocation), typeof(WindowStartupLocation), typeof(WindowConfigurationBehaviour), new PropertyMetadata(WindowStartupLocation.CenterScreen));

        /// <summary>
        /// Gets or sets the <see cref="WindowStartupLocation" /> Property
        /// </summary>
        public WindowStartupLocation WindowStartupLocation
        {
            get { return (WindowStartupLocation)this.GetValue(WindowStartupLocationProperty); }
            set { this.SetValue(WindowStartupLocationProperty, value); }
        }

        #endregion Dependency Property WindowStartupLocation

        #region Dependency Property WindowState

        /// <summary>
        /// Identifies the <see cref="WindowState" /> dependency property
        /// </summary>
        public static readonly DependencyProperty WindowStateProperty = DependencyProperty.Register(nameof(WindowState), typeof(WindowState), typeof(WindowConfigurationBehaviour), new PropertyMetadata(WindowState.Normal));

        /// <summary>
        /// Gets or sets the <see cref="WindowState" /> Property
        /// </summary>
        public WindowState WindowState
        {
            get { return (WindowState)this.GetValue(WindowStateProperty); }
            set { this.SetValue(WindowStateProperty, value); }
        }

        #endregion Dependency Property WindowState

        #region Dependency Property WindowStyle

        /// <summary>
        /// Identifies the <see cref="WindowStyle" /> dependency property
        /// </summary>
        public static readonly DependencyProperty WindowStyleProperty = DependencyProperty.Register(nameof(WindowStyle), typeof(WindowStyle), typeof(WindowConfigurationBehaviour), new PropertyMetadata(WindowStyle.SingleBorderWindow));

        /// <summary>
        /// Gets or sets the <see cref="WindowStyle" /> Property
        /// </summary>
        public WindowStyle WindowStyle
        {
            get { return (WindowStyle)this.GetValue(WindowStyleProperty); }
            set { this.SetValue(WindowStyleProperty, value); }
        }

        #endregion Dependency Property WindowStyle
    }
}