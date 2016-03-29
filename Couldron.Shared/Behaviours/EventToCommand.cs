using Couldron.Core;
using System.Windows;
using System.Windows.Input;

namespace Couldron.Behaviours
{
    /// <summary>
    /// Provides a behaviour that can handle events and invokes a binded command
    /// </summary>
    public class EventToCommand : Behaviour<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the <see cref="Command"/> associated with a specified object.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Event"/> associated with a specified object.
        /// </summary>
        public static readonly DependencyProperty EventProperty = DependencyProperty.Register(nameof(Event), typeof(string), typeof(EventToCommand), new PropertyMetadata(""));

        private DynamicEventHandler eventHandler;

        /// <summary>
        /// Gets or sets the command to invoke
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(EventToCommand.CommandProperty); }
            set { this.SetValue(EventToCommand.CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the event name to handle
        /// </summary>
        public string Event
        {
            get { return (string)this.GetValue(EventToCommand.EventProperty); }
            set { this.SetValue(EventToCommand.EventProperty, value); }
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
        }

        /// <summary>
        /// Occures if the <see cref="FrameworkElement.DataContext"/> of <see cref="Behaviour{T}.AssociatedObject"/> has changed
        /// </summary>
        protected override void OnDataContextChanged()
        {
            base.OnDataContextChanged();

            if (string.IsNullOrEmpty(this.Event) || this.AssociatedObject == null || this.Command == null)
                return;

            this.eventHandler = new DynamicEventHandler(this.AssociatedObject, this.Event, (sender, args) =>
            {
                if (this.Command.CanExecute(args))
                {
                    this.Command.Execute(args);
                }
            });
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            this.eventHandler.DisposeAll();
        }
    }
}