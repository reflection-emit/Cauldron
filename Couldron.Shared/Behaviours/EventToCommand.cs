using Couldron.Core;
using System.Windows.Input;

#if NETFX_CORE
using Windows.UI.Xaml;
#else

using System.Windows;

#endif

namespace Couldron.Behaviours
{
    /// <summary>
    /// Provides a behaviour that can handle events and invokes a binded command
    /// </summary>
    public class EventToCommand : Behaviour<FrameworkElement>
    {
        #region Dependency Property Command

        /// <summary>
        /// Gets or sets the <see cref="Command" /> Property
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Command" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null));

        #endregion Dependency Property Command

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
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register(nameof(EventName), typeof(string), typeof(EventToCommand), new PropertyMetadata("", EventToCommand.OnEventNameChanged));

        private static void OnEventNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var d = dependencyObject as EventToCommand;

            if (d == null)
                return;

            d.SetBinding();
        }

        #endregion Dependency Property EventName

        private DynamicEventHandler eventHandler;

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
        }

        private void SetBinding()
        {
            if (string.IsNullOrEmpty(this.EventName) || this.AssociatedObject == null)
                return;

            this.eventHandler?.Dispose();
            this.eventHandler = new DynamicEventHandler(this.AssociatedObject, this.EventName, (sender, args) =>
            {
                if (this.Command != null && this.Command.CanExecute(args))
                    this.Command.Execute(args);
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