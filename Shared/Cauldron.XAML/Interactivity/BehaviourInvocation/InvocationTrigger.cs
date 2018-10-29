using Cauldron.XAML.Interactivity.Actions;

#if WINDOWS_UWP

using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

#else

using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity.BehaviourInvocation
{
    /// <summary>
    /// Provides a Behaviour that can invoke <see cref="ActionBase"/> behaviours using invoke awareness event
    /// </summary>
#if WINDOWS_UWP

    [ContentProperty(Name = nameof(Actions))]
#else

    [ContentProperty(nameof(Actions))]
#endif
    public sealed partial class InvocationTrigger : BehaviourInvokeAwareBehaviourBase<FrameworkElement>
    {
        private ActionCollection _actions;

        /// <summary>
        /// Gets a collection of actions that can be invoked by this behaviour
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                if (null == this._actions)
                    this._actions = new ActionCollection(this.AssociatedObject);

                return this._actions;
            }
        }

        /// <summary>
        /// Occures if the behaviour is requested to invoke
        /// </summary>
        protected override void Invoke()
        {
            foreach (var item in this.Actions)
                item.Invoke(null);
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
        }

        /// <summary>
        /// Occures after shallow copying the behavior
        /// </summary>
        /// <param name="behaviour">The resulting behavior from <see cref="IBehaviour.Copy"/></param>
        protected override void OnCopy(IBehaviour behaviour)
        {
            var eventTrigger = behaviour as EventTrigger;

            foreach (var item in this.Actions)
                eventTrigger.Actions.Add((item as IBehaviour).Copy() as ActionBase);
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
        }
    }
}