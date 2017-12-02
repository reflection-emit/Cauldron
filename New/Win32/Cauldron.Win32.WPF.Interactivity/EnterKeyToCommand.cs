using System.Windows.Input;

#if WINDOWS_UWP

using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.System;

#else

using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Provides a behaviour that can invoke a command when the Enter key is pressed
    /// </summary>
    public sealed class EnterKeyToCommand : Behaviour<FrameworkElement>
    {
        #region Dependency Property Command

        /// <summary>
        /// Identifies the <see cref="Command" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EnterKeyToCommand), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Command" /> Property
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(MyPropertyProperty); }
            set { this.SetValue(MyPropertyProperty, value); }
        }

        #endregion Dependency Property Command

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            this.AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }

#if WINDOWS_UWP

        private void AssociatedObject_KeyDown(object sender, KeyRoutedEventArgs e)
#else

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
#endif
        {
            if (this.Command == null)
                return;

#if WINDOWS_UWP
            if (e.Key == VirtualKey.Enter)
#else
            if (e.Key == Key.Enter)
#endif
            {
                if (this.Command.CanExecute(null))
                    this.Command.Execute(null);

                e.Handled = true;
            }
        }
    }
}