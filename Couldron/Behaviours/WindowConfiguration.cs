using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Provides a behaviour to configure the window that will contain the view
    /// </summary>
    [BehaviourUsage(false)]
    public sealed class WindowConfiguration : FrameworkElement, IBehaviour<FrameworkElement>
    {
        #region Dependency Property ToolbarTemplate

        /// <summary>
        /// Identifies the <see cref="ToolbarTemplate" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ToolbarTemplateProperty = DependencyProperty.Register(nameof(ToolbarTemplate), typeof(ControlTemplate), typeof(WindowConfiguration), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value containing the template for the title bar
        /// </summary>
        public ControlTemplate ToolbarTemplate
        {
            get { return (ControlTemplate)this.GetValue(ToolbarTemplateProperty); }
            set { this.SetValue(ToolbarTemplateProperty, value); }
        }

        #endregion Dependency Property ToolbarTemplate

        #region Dependency Property SizeToContent

        /// <summary>
        /// Identifies the <see cref="SizeToContent" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.Register(nameof(SizeToContent), typeof(SizeToContent), typeof(WindowConfiguration), new PropertyMetadata(SizeToContent.Manual));

        /// <summary>
        /// Gets or sets a value that indicates whether a window will automatically size itself to fit the size of its content.
        /// </summary>
        public SizeToContent SizeToContent
        {
            get { return (SizeToContent)this.GetValue(SizeToContentProperty); }
            set { this.SetValue(SizeToContentProperty, value); }
        }

        #endregion Dependency Property SizeToContent

        #region Dependency Property IsModal

        /// <summary>
        /// Identifies the <see cref="IsModal" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register(nameof(IsModal), typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that indicates that the window is modal
        /// </summary>
        public bool IsModal
        {
            get { return (bool)this.GetValue(IsModalProperty); }
            set { this.SetValue(IsModalProperty, value); }
        }

        #endregion Dependency Property IsModal

        #region Dependency Property Icon

        /// <summary>
        /// Identifies the <see cref="Icon" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(WindowConfiguration), new PropertyMetadata(null, WindowConfiguration.OnIconChanged));

        /// <summary>
        /// Occures if the <see cref="Icon"/> property has changed
        /// </summary>
        public event EventHandler IconChanged;

        /// <summary>
        /// Gets or sets a window's icon.
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var dependencyObject = d as WindowConfiguration;

            if (dependencyObject == null)
                return;

            if (dependencyObject.IconChanged != null)
                dependencyObject.IconChanged(dependencyObject, EventArgs.Empty);
        }

        #endregion Dependency Property Icon

        #region Dependency Property ResizeMode

        /// <summary>
        /// Identifies the <see cref="ResizeMode" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty = DependencyProperty.Register(nameof(ResizeMode), typeof(ResizeMode), typeof(WindowConfiguration), new PropertyMetadata(ResizeMode.CanResizeWithGrip));

        /// <summary>
        /// Gets or sets the resize mode.
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
        public static readonly DependencyProperty ShowInTaskbarProperty = DependencyProperty.Register(nameof(ShowInTaskbar), typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value that indicates whether the window has a task bar button.
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
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(WindowConfiguration), new PropertyMetadata("", WindowConfiguration.OnTitleChanged));

        /// <summary>
        /// Occures if the <see cref="Title"/> property has changed
        /// </summary>
        public event EventHandler TitleChanged;

        /// <summary>
        /// Gets or sets a window's title.
        /// </summary>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var dependencyObject = d as WindowConfiguration;

            if (dependencyObject == null || args.NewValue == null)
                return;

            if (dependencyObject.TitleChanged != null)
                dependencyObject.TitleChanged(dependencyObject, EventArgs.Empty);
        }

        #endregion Dependency Property Title

        #region Dependency Property Topmost

        /// <summary>
        /// Identifies the <see cref="Topmost" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TopmostProperty = DependencyProperty.Register(nameof(Topmost), typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that indicates whether a window appears in the topmost z-order.
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
        public static readonly DependencyProperty WindowStartupLocationProperty = DependencyProperty.Register(nameof(WindowStartupLocation), typeof(WindowStartupLocation), typeof(WindowConfiguration), new PropertyMetadata(WindowStartupLocation.CenterOwner));

        /// <summary>
        /// Gets or sets the position of the window when first shown.
        /// </summary>
        public WindowStartupLocation WindowStartupLocation
        {
            get { return (WindowStartupLocation)this.GetValue(WindowStartupLocationProperty); }
            set { this.SetValue(WindowStartupLocationProperty, value); }
        }

        #endregion Dependency Property WindowStartupLocation

        #region Dependency Property IsWindowPersistent

        /// <summary>
        /// Identifies the <see cref="IsWindowPersistent" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsWindowPersistentProperty = DependencyProperty.Register(nameof(IsWindowPersistent), typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that indicates that the window size and position are persistent
        /// </summary>
        public bool IsWindowPersistent
        {
            get { return (bool)this.GetValue(IsWindowPersistentProperty); }
            set { this.SetValue(IsWindowPersistentProperty, value); }
        }

        #endregion Dependency Property IsWindowPersistent

        #region Dependency Property WindowState

        /// <summary>
        /// Identifies the <see cref="WindowState" /> dependency property
        /// </summary>
        public static readonly DependencyProperty WindowStateProperty = DependencyProperty.Register(nameof(WindowState), typeof(WindowState), typeof(WindowConfiguration), new PropertyMetadata(WindowState.Normal));

        /// <summary>
        /// Gets or sets a value that indicates whether a window is restored, minimized, or maximized.
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
        public static readonly DependencyProperty WindowStyleProperty = DependencyProperty.Register(nameof(WindowStyle), typeof(WindowStyle), typeof(WindowConfiguration), new PropertyMetadata(WindowStyle.SingleBorderWindow));

        /// <summary>
        /// Gets or sets a window's border style.
        /// </summary>
        public WindowStyle WindowStyle
        {
            get { return (WindowStyle)this.GetValue(WindowStyleProperty); }
            set { this.SetValue(WindowStyleProperty, value); }
        }

        #endregion Dependency Property WindowStyle

        #region Dependency Property IsSplashScreen

        /// <summary>
        /// Identifies the <see cref="IsSplashScreen" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsSplashScreenProperty = DependencyProperty.Register(nameof(IsSplashScreen), typeof(bool), typeof(WindowConfiguration), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that indicates that the current window is a splash screen
        /// </summary>
        public bool IsSplashScreen
        {
            get { return (bool)this.GetValue(IsSplashScreenProperty); }
            set { this.SetValue(IsSplashScreenProperty, value); }
        }

        #endregion Dependency Property IsSplashScreen

        #region Behaviour implementation

        private FrameworkElement _associatedObject;

        /// <summary>
        /// Gets the <see cref="DependencyObject"/> to which the behavior is attached.
        /// </summary>
        public FrameworkElement AssociatedObject
        {
            get { return this._associatedObject; }
            set
            {
                if (this._associatedObject == value)
                    return;

                this._associatedObject = value;

                if (this._associatedObject == null)
                    return;

                this.SetBinding(DataContextProperty, this._associatedObject, nameof(FrameworkElement.DataContext));
            }
        }

        /// <summary>
        /// Gets a value that indicates the behaviour was assigned from a template
        /// </summary>
        public bool IsAssignedFromTemplate { get; private set; }

        /// <summary>
        /// Creates a shallow copy of the instance
        /// </summary>
        /// <returns>A copy of the behaviour</returns>
        IBehaviour IBehaviour.Copy()
        {
            var type = this.GetType();
            var behaviour = Activator.CreateInstance(type) as WindowConfiguration;

            var props = type.GetProperties().ToArray<PropertyInfo>();

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                try
                {
                    // exclude ResourceDictionaries and Styles
                    if (prop.CanWrite && prop.CanRead && prop.PropertyType != typeof(ResourceDictionary) && prop.PropertyType != typeof(Style))
                        prop.SetValue(behaviour, prop.GetValue(this));
                }
                catch
                {
                    // Happens sometimes, but it's not important if something bad happens
                }
            }

            behaviour.IsAssignedFromTemplate = true;
            return behaviour;
        }

        /// <summary>
        /// Sets the behaviour's associated object
        /// </summary>
        /// <param name="obj">The associated object</param>
        void IBehaviour.SetAssociatedObject(object obj)
        {
            if (obj == null)
                return;

            this.AssociatedObject = obj as FrameworkElement;

            if (this._associatedObject == null)
                throw new Exception(string.Format("The Type of AssociatedObject \"{0}\" does not match with T \"{1}\"", obj.GetType(), typeof(FrameworkElement)));
        }

        #region IDisposable

        /// <summary>
        /// Occures if the object has been disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets a value indicating if the object has been disposed or not
        /// </summary>
        public bool IsDisposed { get { return false; } }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Implementation not required
        }

        #endregion IDisposable

        #endregion Behaviour implementation

        /// <summary>
        /// Initializes a new instance of <see cref="WindowConfiguration"/>
        /// </summary>
        public WindowConfiguration()
        {
            this.MinHeight = 120;
            this.MinWidth = 300;
        }
    }
}