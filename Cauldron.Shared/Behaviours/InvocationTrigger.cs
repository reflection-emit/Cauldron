using Cauldron.Collections;
using System;
using System.Windows;

namespace Cauldron.Behaviours
{
    public partial class InvocationTrigger : BehaviourInvokeAwareBehaviourBase<FrameworkElement>
    {
        private ActionCollection _events;

        /// <summary>
        /// Gets a collection of actions that can be invoked by this behaviour
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                if (null == this._events)
                {
                    this._events = new ActionCollection();
                    this._events.owner = this.AssociatedObject;
                }

                return this._events;
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
        protected override void OnCopy(IBehaviour<FrameworkElement> behaviour)
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

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
                this.Actions.DisposeAll();

            base.OnDispose(disposeManaged);
        }
    }
}