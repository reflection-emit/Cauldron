using Cauldron.Collections;
using Cauldron.Core;

#if NETFX_CORE
using Windows.UI.Xaml;
#else

using System.Windows;

#endif

namespace Cauldron.Behaviours
{
    public sealed partial class EventTrigger : Behaviour<FrameworkElement>
    {
        #region Dependency Property EventName

        /// <summary>
        /// Gets or sets the <see cref="EventName" /> Property
        /// </summary>
        public string EventName
        {
            get { return (string)this.GetValue(EventNameProperty); }
            set { this.SetValue(EventNameProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="EventName" /> dependency property
        /// </summary>
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register(nameof(EventName), typeof(string), typeof(EventTrigger), new PropertyMetadata("", EventTrigger.OnEventNameChanged));

        private static void OnEventNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var d = dependencyObject as EventTrigger;

            if (d == null)
                return;

            d.SetEventHandler();
        }

        #endregion Dependency Property EventName

        private ActionCollection _events;
        private DynamicEventHandler eventHandler;

        /// <summary>
        /// Gets a collection of actions that can be invoked by this behaviour
        /// </summary>
        public ActionCollection Events
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

        private void SetEventHandler()
        {
            if (this.Events != null)
                this.Events.owner = this.AssociatedObject;

            if (string.IsNullOrEmpty(this.EventName) || this.AssociatedObject == null)
                return;

            this.eventHandler?.Dispose();
            this.eventHandler = new DynamicEventHandler(this.AssociatedObject, this.EventName, (sender, args) =>
            {
                foreach (var item in this.Events)
                    item.Invoke(args);
            });
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

            foreach (var item in this.Events)
                eventTrigger.Events.Add((item as IBehaviour).Copy() as ActionBase);
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            this.eventHandler?.Dispose();
        }
    }
}