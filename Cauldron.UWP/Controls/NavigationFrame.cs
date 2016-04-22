using Cauldron.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Cauldron.Controls
{
    /// <summary>
    /// Displays Page instances, supports navigation to new pages, and maintains a navigation history to support forward and backward navigation.
    /// </summary>
    public sealed class NavigationFrame : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NavigationFrame"/>
        /// </summary>
        public NavigationFrame()
        {
            this.BackStack = new List<PageStackEntry>();
            this.ForwardStack = new List<PageStackEntry>();
            this.SizeChanged += NavigationFrame_SizeChanged;

            this.FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe UI Light");

            ApplicationView.GetForCurrentView().VisibleBoundsChanged += NavigationFrame_VisibleBoundsChanged;
        }

        /// <summary>
        /// Occures when the <see cref="NavigationFrame.GoBack"/> was invoked
        /// </summary>
        public static event EventHandler<NavigationFrameBackRequestedEventArgs> BackRequested;

        #region Dependency Property CanGoForward

        /// <summary>
        /// Identifies the <see cref="CanGoForward" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CanGoForwardProperty = DependencyProperty.Register(nameof(CanGoForward), typeof(bool), typeof(NavigationFrame), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="CanGoForward" /> Property
        /// </summary>
        public bool CanGoForward
        {
            get { return (bool)this.GetValue(CanGoForwardProperty); }
            set { this.SetValue(CanGoForwardProperty, value); }
        }

        #endregion Dependency Property CanGoForward

        #region Dependency Property CanGoBack

        /// <summary>
        /// Identifies the <see cref="CanGoBack" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack), typeof(bool), typeof(NavigationFrame), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the <see cref="CanGoBack" /> Property
        /// </summary>
        public bool CanGoBack
        {
            get { return (bool)this.GetValue(CanGoBackProperty); }
            set { this.SetValue(CanGoBackProperty, value); }
        }

        #endregion Dependency Property CanGoBack

        #region Dependency Property MaxStackSize

        /// <summary>
        /// Identifies the <see cref="MaxStackSize" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MaxStackSizeProperty = DependencyProperty.Register(nameof(MaxStackSize), typeof(int), typeof(NavigationFrame), new PropertyMetadata(10));

        /// <summary>
        /// Gets or sets the <see cref="MaxStackSize" /> Property
        /// </summary>
        public int MaxStackSize
        {
            get { return (int)this.GetValue(MaxStackSizeProperty); }
            set { this.SetValue(MaxStackSizeProperty, value); }
        }

        #endregion Dependency Property MaxStackSize

        #region Dependency Property DefaultChildrenTransitions

        /// <summary>
        /// Identifies the <see cref="DefaultChildrenTransitions" /> dependency property
        /// </summary>
        public static readonly DependencyProperty DefaultChildrenTransitionsProperty = DependencyProperty.Register(nameof(DefaultChildrenTransitions), typeof(TransitionCollection), typeof(NavigationFrame), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="DefaultChildrenTransitions" /> Property
        /// </summary>
        public TransitionCollection DefaultChildrenTransitions
        {
            get { return (TransitionCollection)this.GetValue(DefaultChildrenTransitionsProperty); }
            set { this.SetValue(DefaultChildrenTransitionsProperty, value); }
        }

        #endregion Dependency Property DefaultChildrenTransitions

        #region Dependency Property IsBusy

        /// <summary>
        /// Identifies the <see cref="IsBusy" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(NavigationFrame), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="IsBusy" /> Property
        /// </summary>
        public bool IsBusy
        {
            get { return (bool)this.GetValue(IsBusyProperty); }
            set { this.SetValue(IsBusyProperty, value); }
        }

        #endregion Dependency Property IsBusy

        /// <summary>
        /// Gets a collection of <see cref="PageStackEntry"/> instances representing the backward navigation history of the <see cref="NavigationFrame"/>
        /// </summary>
        public List<PageStackEntry> BackStack { get; private set; }

        /// <summary>
        /// Gets a collection of <see cref="PageStackEntry"/> instances representing the forward navigation history of the <see cref="NavigationFrame"/>
        /// </summary>
        public List<PageStackEntry> ForwardStack { get; private set; }

        /// <summary>
        /// Clears the navigation history
        /// </summary>
        public void ClearStack()
        {
            this.ForwardStack.Clear();
            this.BackStack.Clear();

            this.CanGoBack = false;
            this.CanGoForward = false;
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages its own navigation history.
        /// </summary>
        /// <returns>Returns true if the going back was successful, otherwise false</returns>
        public bool GoBack()
        {
            var eventArgs = new NavigationFrameBackRequestedEventArgs();

            if (BackRequested != null)
                BackRequested(this, eventArgs);

            if (eventArgs.IsHandled)
                return true;

            if (this.BackStack.Count < 2)
                return false;

            var stackEntry = this.BackStack[this.BackStack.Count - 2];

            this.ForwardStack.Add(this.BackStack[this.BackStack.Count - 1]);
            this.BackStack.RemoveAt(this.BackStack.Count - 1);
            this.BackStack.RemoveAt(this.BackStack.Count - 1);

            Navigator.NavigateByType(stackEntry.ViewModelType, stackEntry.Parameters);

            return true;
        }

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a Frame manages its own navigation history.
        /// </summary>
        public void GoForward()
        {
            if (this.ForwardStack.Count <= 0)
                return;

            var stackEntry = this.ForwardStack[0];
            this.ForwardStack.RemoveAt(0);

            Navigator.NavigateByType(stackEntry.ViewModelType, stackEntry.Parameters);
        }

        internal bool Navigate(Type viewModelType, Type viewType, object[] args)
        {
            var page = Factory.Create(viewType) as FrameworkElement;

            if (page == null)
                return false;

            if (page.Transitions != null && page.Transitions.Count > 0)
                this.ContentTransitions = page.Transitions;
            else
                this.ContentTransitions = this.DefaultChildrenTransitions;

            this.Content.DisposeAll();

            this.BackStack.Add(new PageStackEntry(viewModelType, args));

            // remove entries from stack
            if (this.MaxStackSize > 1)
            {
                if (this.BackStack.Count > this.MaxStackSize + 1)
                    this.BackStack.RemoveAt(0);

                if (this.ForwardStack.Count > this.MaxStackSize)
                    this.ForwardStack.RemoveAt(this.ForwardStack.Count - 1);
            }

            this.CanGoBack = this.BackStack.Count > 1;
            this.CanGoForward = this.ForwardStack.Count > 0;

            this.Content = page;

            return true;
        }

        internal bool NavigateWithoutHistory(Type viewType)
        {
            var page = Factory.Create(viewType) as FrameworkElement;

            if (page == null)
                return false;

            if (this.ContentTransitions != null)
                this.ContentTransitions.Clear();

            this.Content.DisposeAll();
            this.Content = page;

            return true;
        }

        internal void NavigateWithoutView(Type viewModelType, object[] args)
        {
            this.ContentTransitions = this.DefaultChildrenTransitions;
            this.Content.DisposeAll();
            this.BackStack.Add(new PageStackEntry(viewModelType, args));

            // remove entries from stack
            if (this.MaxStackSize > 1)
            {
                if (this.BackStack.Count > this.MaxStackSize + 1)
                    this.BackStack.RemoveAt(0);

                if (this.ForwardStack.Count > this.MaxStackSize)
                    this.ForwardStack.RemoveAt(this.ForwardStack.Count - 1);
            }

            this.CanGoBack = this.BackStack.Count > 1;
            this.CanGoForward = this.ForwardStack.Count > 0;

            // null this content... we navigate with the navigator
            this.Content = null;
        }

        private void NavigationFrame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var sizableContent = this.DataContext as IWindowViewModel;

            if (sizableContent != null)
                sizableContent.SizeChanged(e.NewSize.Width, e.NewSize.Height);
        }

        private void NavigationFrame_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            var visible = ApplicationView.GetForCurrentView().VisibleBounds;

            var isOpen = (visible.Height != Window.Current.Bounds.Height || visible.Width != Window.Current.Bounds.Width);

            if (isOpen)
                this.Margin = new Thickness(
                    visible.Left,
                    visible.Top,
                    Window.Current.Bounds.Width - visible.Width - visible.Left,
                    Window.Current.Bounds.Height - visible.Height - visible.Top);
            else
                this.Margin = new Thickness(0);
        }
    }

    /// <summary>
    /// Represents an entry in the <see cref="NavigationFrame.BackStack"/> or <see cref="NavigationFrame.ForwardStack"/> of a <see cref="NavigationFrame"/>.
    /// </summary>
    public sealed class PageStackEntry
    {
        internal PageStackEntry(Type viewModelType, object[] parameters)
        {
            this.ViewModelType = viewModelType;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the navigation parameter associated with this navigation entry.
        /// </summary>
        public object[] Parameters { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the View model
        /// </summary>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Returns the <see cref="Type.Name"/> of <see cref="ViewModelType"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ViewModelType.Name;
        }
    }
}