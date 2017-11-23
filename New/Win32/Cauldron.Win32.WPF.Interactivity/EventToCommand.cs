using System.Reflection;
using System.Windows.Input;
using Cauldron;

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
    /// Provides a behaviour that can handle events and invokes a binded command
    /// </summary>
    public class EventToCommand : Behaviour<FrameworkElement>
    {
        #region Dependency Property Command

        /// <summary>
        /// Identifies the <see cref="Command" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Command" /> Property
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        #endregion Dependency Property Command

        #region Dependency Property EventName

        /// <summary>
        /// Identifies the <see cref="EventName" /> dependency property
        /// </summary>
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register(nameof(EventName), typeof(string), typeof(EventToCommand), new PropertyMetadata("", EventToCommand.OnEventNameChanged));

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
            var d = dependencyObject as EventToCommand;

            if (d == null)
                return;

            d.SetBinding();
        }

        #endregion Dependency Property EventName

        #region Dependency Property ArgumentConverter

        /// <summary>
        /// Identifies the <see cref="ArgumentConverter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ArgumentConverterProperty = DependencyProperty.Register(nameof(ArgumentConverter), typeof(IValueConverter), typeof(EventToCommand), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="ArgumentConverter" /> Property
        /// </summary>
        public IValueConverter ArgumentConverter
        {
            get { return (IValueConverter)this.GetValue(ArgumentConverterProperty); }
            set { this.SetValue(ArgumentConverterProperty, value); }
        }

        #endregion Dependency Property ArgumentConverter

        private DynamicEventHandler eventHandler;

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach() => this.SetBinding();

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach() => this.eventHandler.TryDispose();

        private void SetBinding()
        {
            if (string.IsNullOrEmpty(this.EventName) || this.AssociatedObject == null)
                return;

            this.eventHandler?.Dispose();
            this.eventHandler = new DynamicEventHandler(this.AssociatedObject, this.EventName, (sender, args) =>
            {
                if (this.ArgumentConverter == null && this.Command != null && this.Command.CanExecute(args))
                    this.Command.Execute(args);
                else
                {
                    var targetType = this.Command.GetType().GetGenericArguments()[0];
                    var convertedArg = this.ArgumentConverter.Convert(args, targetType, null, null);

                    if (this.Command != null && this.Command.CanExecute(convertedArg))
                        this.Command.Execute(convertedArg);
                }
            });
        }
    }
}