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

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Provides a Behaviour that can invoke <see cref="ActionBase"/> behaviours.
    /// <para/>
    /// The <see cref="EventTrigger"/> is triggered by an event of the associated <see cref="FrameworkElement"/>
    /// </summary>
#if WINDOWS_UWP

    [ContentProperty(Name = nameof(Actions))]
#else

    [ContentProperty(nameof(Actions))]
#endif
    public sealed class EventTrigger : Behaviour<FrameworkElement>, IBehaviour
    {
        #region Dependency Property EventName

        /// <summary>
        /// Identifies the <see cref="EventName" /> dependency property
        /// </summary>
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register(nameof(EventName), typeof(string), typeof(EventTrigger), new PropertyMetadata("", EventTrigger.OnEventNameChanged));

        /// <summary>
        /// Gets or sets the <see cref="EventName" /> Property
        /// </summary>
        public string EventName
        {
            get { return (string)this.GetValue(EventNameProperty); }
            set { this.SetValue(EventNameProperty, value); }
        }

        private static void OnEventNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var d = dependencyObject as EventTrigger;

            if (d == null)
                return;

            d.SetEventHandler();
        }

        #endregion Dependency Property EventName

        private ActionCollection _Actions;
        private DynamicEventHandler eventHandler;

        /// <summary>
        /// Gets a collection of actions that can be invoked by this behaviour
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                if (null == this._Actions)
                    this._Actions = new ActionCollection(this.AssociatedObject);

                return this._Actions;
            }
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach() => this.SetEventHandler();

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
        protected override void OnDetach() => this.eventHandler?.Dispose();

        private void SetEventHandler()
        {
            if (string.IsNullOrEmpty(this.EventName) || this.AssociatedObject == null)
                return;

            this.eventHandler?.Dispose();
            this.eventHandler = new DynamicEventHandler(this.AssociatedObject, this.EventName, (sender, args) =>
            {
                foreach (var item in this.Actions)
                    item.Invoke(args);
            });
        }
    }
}