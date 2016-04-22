using System;
using System.Linq;
using System.Reflection;

namespace Cauldron.Core
{
    /// <summary>
    /// Handles dynamic event registrations
    /// </summary>
    public sealed class DynamicEventHandler : DisposableBase
    {
        private EventInfo eventInfo;
        private Delegate handler;

        /// <summary>
        /// Initializes a new instance of <see cref="DynamicEventHandler"/>
        /// </summary>
        /// <param name="associatedObject">The objects that contains the event</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="eventHandler">A delegate that handles the event</param>
        public DynamicEventHandler(object associatedObject, string eventName, Action<object, object> eventHandler)
        {
            this.EventName = eventName;
            this.AssociatedObject = associatedObject;
            this.EventHandler = eventHandler;

            this.AttachEvent();
        }

        /// <summary>
        /// Gets the <see cref="object"/> to which the event handler is attached.
        /// </summary>
        public object AssociatedObject { get; private set; }

        /// <summary>
        /// Gets the event handler that handles the event
        /// </summary>
        public Action<object, object> EventHandler { get; private set; }

        /// <summary>
        /// Gets the name of the event
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.DetachEvent();
                this.AssociatedObject = null;
                this.EventHandler = null;
            }
        }

        private void AttachEvent()
        {
            this.eventInfo = this.AssociatedObject.GetType().GetRuntimeEvent(this.EventName);

            if (eventInfo == null)
                return;

            // Get the method that gets invoked if the event is fired
            var method = this.GetType().GetRuntimeMethods().First(x => x.Name == nameof(ExecuteCommand));
            this.handler = method.CreateDelegate(eventInfo.EventHandlerType, this);
            this.eventInfo.AddEventHandler(this.AssociatedObject, this.handler);
        }

        private void DetachEvent()
        {
            if (this.eventInfo == null || this.handler == null)
                return;

            this.eventInfo.RemoveEventHandler(this.AssociatedObject, this.handler);
            this.eventInfo = null;
            this.handler = null;
        }

        private void ExecuteCommand(object sender, object args)
        {
            if (this.EventHandler != null)
                this.EventHandler(sender, args);

            // Get the handled property and set it to true
            args.GetType().GetProperty("Handled").IsNotNull(x => x.SetValue(args, true));
        }
    }
}