using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Cauldron.XAML.Resources
{
    internal sealed partial class MessageDialogView : UserControl
    {
        #region Dependency Property EnterCommand

        /// <summary>
        /// Identifies the <see cref="EnterCommand" /> dependency property
        /// </summary>
        public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.Register(nameof(EnterCommand), typeof(ICommand), typeof(MessageDialogView), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="EnterCommand" /> Property
        /// </summary>
        public ICommand EnterCommand
        {
            get { return (ICommand)this.GetValue(EnterCommandProperty); }
            set { this.SetValue(EnterCommandProperty, value); }
        }

        #endregion Dependency Property EnterCommand

        #region Dependency Property CancelCommand

        /// <summary>
        /// Identifies the <see cref="CancelCommand" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(MessageDialogView), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="CancelCommand" /> Property
        /// </summary>
        public ICommand CancelCommand
        {
            get { return (ICommand)this.GetValue(CancelCommandProperty); }
            set { this.SetValue(CancelCommandProperty, value); }
        }

        #endregion Dependency Property CancelCommand

        public MessageDialogView()
        {
            this.InitializeComponent();
            this.SetBinding(EnterCommandProperty, new Binding { Path = new PropertyPath("EnterCommand"), Source = this.DataContext });
            this.SetBinding(CancelCommandProperty, new Binding { Path = new PropertyPath("CancelCommand"), Source = this.DataContext });

            this.Unloaded += MessageDialogView_Unloaded;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Enter)
                this.EnterCommand?.Execute(null);
            else if (args.VirtualKey == VirtualKey.Escape)
                this.CancelCommand?.Execute(null);
        }

        private void MessageDialogView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= MessageDialogView_Unloaded;
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
        }
    }
}