using Cauldron.XAML.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Cauldron.XAML.Interactivity.Actions
{
    /// <summary>
    /// Represents an action that can invoke <see cref="Storyboard.Begin()"/>
    /// </summary>
    [ContentProperty("Storyboard")]
    public class StoryboardBeginAnimationAction : ActionBase
    {
        /// <summary>
        /// Identifies the <see cref="StoryboardKey" /> dependency property
        /// </summary>
        public static readonly DependencyProperty StoryboardKeyProperty = DependencyProperty.Register(nameof(StoryboardKey), typeof(string), typeof(StoryboardBeginAnimationAction), new PropertyMetadata(null, OnStoryboardKeyChanged));

        /// <summary>
        /// Identifies the <see cref="Storyboard" /> dependency property
        /// </summary>
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register(nameof(Storyboard), typeof(Storyboard), typeof(StoryboardBeginAnimationAction), new PropertyMetadata(null, OnStoryboardChanged));

        private readonly CallOnce methodHandler;

        /// <summary>
        /// Initialized a new Instance of <see cref="StoryboardBeginAnimationAction"/>
        /// </summary>
        public StoryboardBeginAnimationAction() => this.methodHandler = CallOnce.Create(() => this.Storyboard?.Begin(this.AssociatedObject, true));

        /// <summary>
        /// Gets or sets the <see cref="Storyboard" /> Property
        /// </summary>
        public Storyboard Storyboard
        {
            get => this.GetValue(StoryboardProperty) as Storyboard;
            set => this.SetValue(StoryboardProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="StoryboardKey" /> Property
        /// </summary>
        public string StoryboardKey
        {
            get => (string)this.GetValue(StoryboardKeyProperty);
            set => this.SetValue(StoryboardKeyProperty, value);
        }

        /// <summary>
        /// Occures when the action is invoked by an event
        /// </summary>
        /// <param name="parameter">The parameter passed by the event</param>
        public override void Invoke(object parameter)
        {
            if (this.Storyboard == null || this.AssociatedObject == null)
                return;

            var type = parameter?.GetType();

            this.Storyboard.Stop(this.AssociatedObject);

            if (type != null && type == typeof(bool) && (bool)parameter)
                this.methodHandler.Invoke();
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
            if (this.StoryboardKey == null)
                return;

            this.Storyboard = this.AssociatedObject.Resources[this.StoryboardKey] as Storyboard;
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            if (this.Storyboard == null)
                return;

            this.Storyboard.Stop(this.AssociatedObject);
        }

        private static void OnStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is StoryboardBeginAnimationAction action && args.NewValue is Storyboard value && value.IsSealed)
                action.Storyboard = value.Clone();
        }

        private static void OnStoryboardKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is StoryboardBeginAnimationAction action && args.NewValue is string value)
                action.OnAttach();
        }
    }
}