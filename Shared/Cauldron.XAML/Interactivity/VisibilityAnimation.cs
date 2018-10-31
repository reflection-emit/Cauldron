using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Provides a <see cref="Behaviour{T}"/> that starts a <see cref="Storyboard"/> on <see cref="UIElement.Visibility"/> change.
    /// </summary>
    public sealed class VisibilityAnimation : Behaviour<FrameworkElement>
    {
        /// <summary>
        /// Identifies the <see cref="Storyboard" /> dependency property
        /// </summary>
        public static readonly DependencyProperty StoryboardInProperty = DependencyProperty.Register(nameof(StoryboardIn), typeof(Storyboard), typeof(VisibilityAnimation), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Storyboard" /> dependency property
        /// </summary>
        public static readonly DependencyProperty StoryboardOutProperty = DependencyProperty.Register(nameof(StoryboardOut), typeof(Storyboard), typeof(VisibilityAnimation), new PropertyMetadata(null, OnStoryboardOutChanged));

        /// <summary>
        /// Identifies the <see cref="VisibilityAnimation.Visibility"/> dependency property.
        /// </summary>
        public static new readonly DependencyProperty VisibilityProperty =    DependencyProperty.Register(nameof(Visibility), typeof(Visibility), typeof(VisibilityAnimation), new PropertyMetadata(Visibility.Visible, OnVisibilityChanged));

        /// <summary>
        /// Gets or sets the during of the fade-in animation
        /// </summary>
        public double DurationIn { get; set; } = 210;

        /// <summary>
        /// Gets or sets the during of the fade-out animation
        /// </summary>
        public double DurationOut { get; set; } = 230;

        /// <summary>
        /// Gets or sets the <see cref="Storyboard" /> Property. This is a dependency property.
        /// </summary>
        public Storyboard StoryboardIn
        {
            get => this.GetValue(StoryboardInProperty) as Storyboard;
            set => this.SetValue(StoryboardInProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="Storyboard" /> Property. This is a dependency property.
        /// </summary>
        public Storyboard StoryboardOut
        {
            get => this.GetValue(StoryboardOutProperty) as Storyboard;
            set => this.SetValue(StoryboardOutProperty, value);
        }

        /// <summary>
        /// Gets or sets the user interface (UI) visibility of this element. This is a dependency property.
        /// </summary>
        public new Visibility Visibility
        {
            get => (Visibility)this.GetValue(VisibilityProperty);
            set => this.SetValue(VisibilityProperty, value);
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
            this.StoryboardIn = new Storyboard();
            this.StoryboardOut = new Storyboard();

            Storyboard.SetTarget(this.StoryboardIn, this.AssociatedObject);
            Storyboard.SetTargetProperty(this.StoryboardIn, new PropertyPath(FrameworkElement.OpacityProperty));

            Storyboard.SetTarget(this.StoryboardOut, this.AssociatedObject);
            Storyboard.SetTargetProperty(this.StoryboardOut, new PropertyPath(FrameworkElement.OpacityProperty));

            var opacityAnimationIn = new DoubleAnimation(this.AssociatedObject.Opacity, new Duration(TimeSpan.FromMilliseconds(this.DurationIn)))
            {
                FillBehavior = FillBehavior.HoldEnd
            };
            this.StoryboardIn.Children.Add(opacityAnimationIn);
            this.StoryboardIn.AutoReverse = false;

            var opacityAnimationOut = new DoubleAnimation(0.0, new Duration(TimeSpan.FromMilliseconds(this.DurationIn)))
            {
                FillBehavior = FillBehavior.HoldEnd
            };
            this.StoryboardOut.Children.Add(opacityAnimationOut);
            this.StoryboardOut.AutoReverse = false;
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            if (this.StoryboardOut != null)
                this.StoryboardOut.Completed += this.StoryboardOut_Completed;
        }

        private static void OnStoryboardOutChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (Comparer.Equals(args.NewValue, args.OldValue))
                return;

            if (d is VisibilityAnimation effect)
            {
                effect.StoryboardOut.Completed += effect.StoryboardOut_Completed;

                if (args.OldValue is Storyboard storyboard)
                    storyboard.Completed -= effect.StoryboardOut_Completed;
            }
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is VisibilityAnimation visibilityAnimation)
            {
                if (visibilityAnimation.StoryboardIn == null || visibilityAnimation.StoryboardOut == null)

                    if (visibilityAnimation.Visibility == visibilityAnimation.AssociatedObject.Visibility)
                        return;

                visibilityAnimation.StoryboardIn.Stop();
                visibilityAnimation.StoryboardOut.Stop();

                if (visibilityAnimation.Visibility == Visibility.Visible && visibilityAnimation.AssociatedObject.Visibility != Visibility.Visible)
                {
                    visibilityAnimation.AssociatedObject.Opacity = 0.0;
                    visibilityAnimation.AssociatedObject.Visibility = Visibility.Visible;
                    visibilityAnimation.StoryboardIn.Begin();
                    return;
                }

                if (visibilityAnimation.Visibility != Visibility.Visible && visibilityAnimation.AssociatedObject.Visibility == Visibility.Visible)
                    visibilityAnimation.StoryboardOut.Begin();
            }
        }

        private void StoryboardOut_Completed(object sender, EventArgs e) => this.AssociatedObject.Visibility = this.Visibility;
    }
}