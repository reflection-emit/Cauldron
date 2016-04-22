using Cauldron;
using Cauldron.Behaviours;
using Cauldron.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Cauldron.Themes.VisualStudio.Behaviours
{
    internal sealed class TabItemBehaviour : Behaviour<TabItem>
    {
        private Border border;
        private Button closeButton;
        private TextBlock header;

        #region Dependency Property MouseOverBrush

        /// <summary>
        /// Identifies the <see cref="MouseOverBrush" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(nameof(MouseOverBrush), typeof(Brush), typeof(TabItemBehaviour), new PropertyMetadata(null, TabItemBehaviour.OnMouseOverBrushChanged));

        private Brush _mouseOverBrush;

        /// <summary>
        /// Gets or sets the <see cref="MouseOverBrush" /> Property
        /// </summary>
        public Brush MouseOverBrush
        {
            get { return (Brush)this.GetValue(MouseOverBrushProperty); }
            set { this.SetValue(MouseOverBrushProperty, value); }
        }

        private static void OnMouseOverBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var castedDependencyObject = d as TabItemBehaviour;

            if (castedDependencyObject == null)
                return;

            args.NewValue.CastTo<SolidColorBrush>().IsNotNull(x => castedDependencyObject._mouseOverBrush = x.ChangeAlpha(120));
        }

        #endregion Dependency Property MouseOverBrush

        protected override void OnAttach()
        {
            this.AssociatedObject.GotFocus += AssociatedObject_GotFocus;
            this.AssociatedObject.LostFocus += AssociatedObject_LostFocus;
            this.AssociatedObject.SourceUpdated += AssociatedObject_SourceUpdated;
            this.AssociatedObject.IsEnabledChanged += AssociatedObject_IsEnabledChanged;
            this.AssociatedObject.IsKeyboardFocusedChanged += AssociatedObject_IsKeyboardFocusedChanged;
            this.AssociatedObject.LayoutUpdated += AssociatedObject_LayoutUpdated;
            this.AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            this.AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;

            this.SetBinding(MouseOverBrushProperty, new Binding
            {
                Source = this.AssociatedObject,
                Path = new PropertyPath(nameof(TabItem.Background)),
                Mode = BindingMode.OneWay
            });

            // Set the Header binding
            var parent = this.AssociatedObject.FindVisualParent<TabControl>();

            if (parent != null)
                this.AssociatedObject.SetBinding(TabItemHeader.HeaderProperty, this.AssociatedObject.DataContext, parent.DisplayMemberPath);

            this.UpdateColours();
        }

        protected override void OnDetach()
        {
            this.AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
            this.AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
            this.AssociatedObject.SourceUpdated -= AssociatedObject_SourceUpdated;
            this.AssociatedObject.IsEnabledChanged -= AssociatedObject_IsEnabledChanged;
            this.AssociatedObject.IsKeyboardFocusedChanged -= AssociatedObject_IsKeyboardFocusedChanged;
            this.AssociatedObject.LayoutUpdated -= AssociatedObject_LayoutUpdated;
            this.AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
            this.AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }

        private void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.UpdateColours();
        }

        private void AssociatedObject_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateColours();
        }

        private void AssociatedObject_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateColours();
        }

        private void AssociatedObject_LayoutUpdated(object sender, System.EventArgs e)
        {
            this.UpdateColours();
        }

        private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
        {
            this.UpdateColours();
        }

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.UpdateColours();
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.UpdateColours();
        }

        private void AssociatedObject_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            this.UpdateColours();
        }

        private void UpdateColours()
        {
            if (this.AssociatedObject.Template == null)
                return;

            if (this.border == null)
                this.border = this.AssociatedObject.Template.FindName("border", this.AssociatedObject) as Border;

            if (this.header == null)
                this.header = this.AssociatedObject.Template.FindName("header", this.AssociatedObject) as TextBlock;

            if (this.closeButton == null)
            {
                this.closeButton = this.AssociatedObject.Template.FindName("closeButton", this.AssociatedObject) as Button;
                this.closeButton.IsNotNull(x => x.Click += (s, e) =>
                {
                    this.AssociatedObject.IsSelected = true;
                    (this.AssociatedObject.DataContext as IClose).IsNotNull(o => o.Close());
                });
            }

            if (this.border == null || this.header == null || this.closeButton == null)
                return;

            if (this.AssociatedObject.IsEnabled && this.AssociatedObject.IsMouseOver && !this.AssociatedObject.IsSelected)
            {
                this.border.IsNotNull(x => x.Background = this._mouseOverBrush);
                this.header.IsNotNull(x => x.Foreground = Application.Current.Resources["ThemeHoveredTextBrush"] as SolidColorBrush);
                this.closeButton.IsNotNull(x => x.Visibility = this.AssociatedObject.DataContext is IClose ? Visibility.Visible : Visibility.Hidden);
                return;
            }

            if (this.AssociatedObject.IsSelected && this.AssociatedObject.IsEnabled)
            {
                this.border.IsNotNull(x => x.Background = this.AssociatedObject.Background);
                this.header.IsNotNull(x => x.Foreground = Application.Current.Resources["ThemeHoveredTextBrush"] as SolidColorBrush);
                this.closeButton.IsNotNull(x => x.Visibility = this.AssociatedObject.DataContext is IClose ? Visibility.Visible : Visibility.Hidden);
            }
            else if (!this.AssociatedObject.IsSelected && this.AssociatedObject.IsEnabled)
            {
                this.border.IsNotNull(x => x.Background = Brushes.Transparent);
                this.header.IsNotNull(x => x.Foreground = Application.Current.Resources["ThemeTextBrush"] as SolidColorBrush);
                this.closeButton.IsNotNull(x => x.Visibility = Visibility.Hidden);
            }
            else if (this.AssociatedObject.IsSelected && !this.AssociatedObject.IsEnabled)
            {
                this.border.IsNotNull(x => x.Background = Application.Current.Resources["ThemeDisabledBackgroundBrush"] as SolidColorBrush);
                this.header.IsNotNull(x => x.Foreground = Application.Current.Resources["ThemeDisabledTextBrush"] as SolidColorBrush);
                this.closeButton.IsNotNull(x => x.Visibility = Visibility.Hidden);
            }
            else if (!this.AssociatedObject.IsSelected && !this.AssociatedObject.IsEnabled)
            {
                this.border.IsNotNull(x => x.Background = Brushes.Transparent);
                this.header.IsNotNull(x => x.Foreground = Application.Current.Resources["ThemeDisabledTextBrush"] as SolidColorBrush);
                this.closeButton.IsNotNull(x => x.Visibility = Visibility.Hidden);
            }
        }
    }
}