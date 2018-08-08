using Cauldron.Extensions;
using Cauldron.XAML.Interactivity;
using Cauldron.XAML.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Cauldron.XAML.Theme
{
    internal class TabItemBehaviour : Behaviour<TabItem>
    {
        private Border border;
        private Button closeButton;
        private TextBlock header;

        #region Dependency Property MouseOverBrush

        /// <summary>
        /// Identifies the <see cref="MouseOverBrush"/> dependency property
        /// </summary>
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(nameof(MouseOverBrush), typeof(Brush), typeof(TabItemBehaviour), new PropertyMetadata(null, TabItemBehaviour.OnMouseOverBrushChanged));

        private Brush _mouseOverBrush;

        /// <summary>
        /// Gets or sets the <see cref="MouseOverBrush"/> Property
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

            args.NewValue.As<SolidColorBrush>().IsNotNull(x => castedDependencyObject._mouseOverBrush = new SolidColorBrush(new Color { A = 120, R = x.Color.R, G = x.Color.G, B = x.Color.B }));
        }

        #endregion Dependency Property MouseOverBrush

        private int lastUpdateIndex = 0;

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

            BindingOperations.SetBinding(this, MouseOverBrushProperty, new Binding
            {
                Source = this.AssociatedObject,
                Path = new PropertyPath(nameof(TabItem.Background)),
                Mode = BindingMode.OneWay
            });

            // Set the Header binding
            var parent = this.AssociatedObject.FindVisualParent<TabControl>();

            if (parent != null && !string.IsNullOrEmpty(parent.DisplayMemberPath))
                this.AssociatedObject.SetBinding(TabItemProperties.HeaderProperty, this.AssociatedObject.DataContext, new PropertyPath(parent.DisplayMemberPath), BindingMode.OneWay);
            else
                this.AssociatedObject.SetBinding(TabItemProperties.HeaderProperty, this.AssociatedObject, new PropertyPath(nameof(TabItem.Header)), BindingMode.OneWay);

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

        private void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e) => this.UpdateColours();

        private void AssociatedObject_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) => this.UpdateColours();

        private void AssociatedObject_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e) => this.UpdateColours();

        private void AssociatedObject_LayoutUpdated(object sender, System.EventArgs e) => this.UpdateColours();

        private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e) => this.UpdateColours();

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => this.UpdateColours();

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => this.UpdateColours();

        private void AssociatedObject_SourceUpdated(object sender, DataTransferEventArgs e) => this.UpdateColours();

        private void UpdateColours()
        {
            if (this.AssociatedObject.Template == null)
                return;

            var updateIndex = 0;

            if (this.AssociatedObject.IsEnabled && this.AssociatedObject.IsMouseOver && !this.AssociatedObject.IsSelected)
                updateIndex = 1;
            else if (this.AssociatedObject.IsSelected && this.AssociatedObject.IsEnabled)
                updateIndex = 2;
            else if (!this.AssociatedObject.IsSelected && this.AssociatedObject.IsEnabled)
                updateIndex = 3;
            else if (this.AssociatedObject.IsSelected && !this.AssociatedObject.IsEnabled)
                updateIndex = 4;
            else if (!this.AssociatedObject.IsSelected && !this.AssociatedObject.IsEnabled)
                updateIndex = 5;

            if (updateIndex == this.lastUpdateIndex)
                return;

            this.lastUpdateIndex = updateIndex;

            if (this.border == null)
                this.border = this.AssociatedObject.Template.FindName("border", this.AssociatedObject) as Border;

            if (this.header == null)
                this.header = this.AssociatedObject.Template.FindName("header", this.AssociatedObject) as TextBlock;

            if (this.closeButton == null)
            {
                this.closeButton = this.AssociatedObject.Template.FindName("closeButton", this.AssociatedObject) as Button;
                this.closeButton.IsNotNull(x =>
                {
                    x.Click += (s, e) =>
                    {
                        this.AssociatedObject.IsSelected = true;
                        (this.AssociatedObject.DataContext as ICloseAwareViewModel).IsNotNull(o => o.Close());
                    };
                    this.AssociatedObject.InputBindings.Add(
                        new InputBinding(
                            new RelayCommand(() => (this.AssociatedObject.DataContext as ICloseAwareViewModel).IsNotNull(o => o.Close())), new KeyGesture(Key.Delete)));
                });
            }

            if (this.border == null || this.header == null || this.closeButton == null)
                return;

            if (this.AssociatedObject.IsEnabled && this.AssociatedObject.IsMouseOver && !this.AssociatedObject.IsSelected)
            {
                this.border.IsNotNull(x => x.Background = this._mouseOverBrush);
                this.header.IsNotNull(x => x.Foreground = Application.Current.Resources[CauldronTheme.HoveredTextBrush] as SolidColorBrush);
                this.closeButton.IsNotNull(x => x.Visibility = this.AssociatedObject.DataContext is ICloseAwareViewModel ? Visibility.Visible : Visibility.Hidden);
                return;
            }

            if (this.AssociatedObject.IsSelected && this.AssociatedObject.IsEnabled)
            {
                this.border.IsNotNull(x => x.Background = this.AssociatedObject.Background);
                var color = this.border.Background.As<SolidColorBrush>().Color;
                this.header.IsNotNull(x => x.Foreground = (384 - color.R - color.G - color.B) > 0 ?
                    Application.Current.Resources[CauldronTheme.HoveredTextBrush] as SolidColorBrush :
                    Application.Current.Resources[CauldronTheme.DarkBackgroundBrush] as SolidColorBrush);
                this.closeButton.IsNotNull(x =>
                {
                    x.Visibility = this.AssociatedObject.DataContext is ICloseAwareViewModel ? Visibility.Visible : Visibility.Hidden;
                    x.Foreground = (384 - color.R - color.G - color.B) > 0 ?
                        Application.Current.Resources[CauldronTheme.HoveredTextBrush] as SolidColorBrush :
                        Application.Current.Resources[CauldronTheme.DarkBackgroundBrush] as SolidColorBrush;
                });
            }
            else if (!this.AssociatedObject.IsSelected && this.AssociatedObject.IsEnabled)
            {
                this.border.IsNotNull(x => x.Background = Brushes.Transparent);
                this.header.IsNotNull(x => x.Foreground = Application.Current.Resources[CauldronTheme.TextBrush] as SolidColorBrush);
                this.closeButton.IsNotNull(x => x.Visibility = Visibility.Hidden);
            }
            else if (this.AssociatedObject.IsSelected && !this.AssociatedObject.IsEnabled)
            {
                this.border.IsNotNull(x => x.Background = Application.Current.Resources[CauldronTheme.DisabledBackgroundBrush] as SolidColorBrush);
                this.header.IsNotNull(x => x.Foreground = Application.Current.Resources[CauldronTheme.DisabledTextBrush] as SolidColorBrush);
                this.closeButton.IsNotNull(x => x.Visibility = Visibility.Hidden);
            }
            else if (!this.AssociatedObject.IsSelected && !this.AssociatedObject.IsEnabled)
            {
                this.border.IsNotNull(x => x.Background = Brushes.Transparent);
                this.header.IsNotNull(x => x.Foreground = Application.Current.Resources[CauldronTheme.DisabledTextBrush] as SolidColorBrush);
                this.closeButton.IsNotNull(x => x.Visibility = Visibility.Hidden);
            }
        }
    }
}