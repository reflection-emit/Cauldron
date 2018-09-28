using Cauldron;
using Cauldron.XAML.Interactivity;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Cauldron.XAML.Theme
{
    internal sealed class ProgressBarBehaviour : Behaviour<ProgressBar>
    {
        private Grid ellipseGrid;
        private Storyboard isIndeterminateAnimation;
        private Grid mainGrid;

        #region Dependency Property IsIndeterminate

        /// <summary>
        /// Identifies the <see cref="IsIndeterminate" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(nameof(IsIndeterminate), typeof(bool), typeof(ProgressBarBehaviour), new PropertyMetadata(false, ProgressBarBehaviour.OnIsIndeterminateChanged));

        /// <summary>
        /// Gets or sets the <see cref="IsIndeterminate" /> Property
        /// </summary>
        public bool IsIndeterminate
        {
            get { return (bool)this.GetValue(IsIndeterminateProperty); }
            set { this.SetValue(IsIndeterminateProperty, value); }
        }

        private static void OnIsIndeterminateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var d = dependencyObject as ProgressBarBehaviour;

            if (d == null)
                return;

            d.ChangeWidth();

            if ((bool)args.NewValue)
                d.isIndeterminateAnimation.Begin();
            else
                d.isIndeterminateAnimation.Stop();
        }

        #endregion Dependency Property IsIndeterminate

        protected override void OnAttach()
        {
            this.mainGrid = this.AssociatedObject.FindVisualChildByName("PART_Main") as Grid;
            this.ellipseGrid = this.AssociatedObject.FindVisualChildByName("EllipseGrid") as Grid;

            if (this.ellipseGrid == null || this.mainGrid == null)
                return;

            this.isIndeterminateAnimation = this.mainGrid.FindResource("IndeterminateAnimation") as Storyboard;
            this.SetBinding(ProgressBarBehaviour.IsIndeterminateProperty, this.AssociatedObject, new PropertyPath("IsIndeterminate"), BindingMode.OneWay);
            this.AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
            this.AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
            this.AssociatedObject.ValueChanged += AssociatedObject_ValueChanged;

            this.ChangeWidth();
            this.ChangeVisibility();
            this.ChangeValue();
        }

        protected override void OnDetach()
        {
            BindingOperations.ClearBinding(this, ProgressBarBehaviour.IsIndeterminateProperty);
            this.AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
            this.AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
            this.AssociatedObject.ValueChanged -= AssociatedObject_ValueChanged;

            this.mainGrid = null;
            this.ellipseGrid = null;
            this.isIndeterminateAnimation = null;
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => this.ChangeVisibility();

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e) => this.ChangeWidth();

        private void AssociatedObject_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => this.ChangeValue();

        private void ChangeValue()
        {
            var value = MathEx.ValueOf(this.AssociatedObject.Value, this.AssociatedObject.Maximum, this.mainGrid.ActualWidth);
            ProgressBarProperties.SetValue(this.AssociatedObject, value);
        }

        private void ChangeVisibility()
        {
            if (this.AssociatedObject.IsIndeterminate && this.AssociatedObject.Visibility == Visibility.Visible)
                this.isIndeterminateAnimation.Begin();
            else
                this.isIndeterminateAnimation.Stop();
        }

        private void ChangeWidth()
        {
            this.mainGrid.Width = this.AssociatedObject.Width;
            ProgressBarProperties.SetDiameter(this.AssociatedObject, this.AssociatedObject.ActualHeight);

            var translateAnimation = this.isIndeterminateAnimation.Children[0] as DoubleAnimation;
            translateAnimation.To = this.AssociatedObject.ActualWidth / 3;
            translateAnimation.From = -this.AssociatedObject.ActualWidth / 3;

            ProgressBarProperties.SetEllipseAnimationWellPosition(this.AssociatedObject, this.AssociatedObject.ActualWidth / 3);
            ProgressBarProperties.SetEllipseAnimationEndPosition(this.AssociatedObject, this.AssociatedObject.ActualWidth / 3 * 2);
        }
    }
}