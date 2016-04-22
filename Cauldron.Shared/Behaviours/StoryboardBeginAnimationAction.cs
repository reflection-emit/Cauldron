#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;
#else

using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

#endif

using System;

namespace Cauldron.Behaviours
{
#if NETFX_CORE
    /// <summary>
    /// Represents an action that can invoke <see cref="Storyboard.Begin"/>
    /// </summary>
    [ContentProperty(Name = "Storyboard")]
#else

    /// <summary>
    /// Represents an action that can invoke <see cref="Storyboard.Begin()"/>
    /// </summary>
    [ContentProperty("Storyboard")]
#endif
    public class StoryboardBeginAnimationAction : ActionBase
    {
        #region Dependency Property Storyboard

        /// <summary>
        /// Identifies the <see cref="Storyboard" /> dependency property
        /// </summary>
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register(nameof(Storyboard), typeof(Storyboard), typeof(StoryboardBeginAnimationAction), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Storyboard" /> Property
        /// </summary>
        public Storyboard Storyboard
        {
            get { return (Storyboard)this.GetValue(StoryboardProperty); }
            set { this.SetValue(StoryboardProperty, value); }
        }

        #endregion Dependency Property Storyboard

        /// <summary>
        /// Occures when the action is invoked by an event
        /// </summary>
        /// <param name="parameter">The parameter passed by the event</param>
        public override void Invoke(object parameter)
        {
            if (this.Storyboard == null)
                return;

            this.Storyboard.Begin();
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
        }
    }
}