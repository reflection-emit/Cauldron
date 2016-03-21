using Couldron.Core;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Couldron.Behaviours
{
    /// <summary>
    /// Provides a behaviour that can set the focus of a control after <see cref="FrameworkElement.Loaded"/>
    /// </summary>
    public sealed class SetFocusBehaviour : Behaviour<Control>
    {
        private CouldronDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of <see cref="SetFocusBehaviour"/>
        /// </summary>
        public SetFocusBehaviour()
        {
            this.dispatcher = new CouldronDispatcher();
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
            base.OnAttach();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.OnDetach();
        }

        private async void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                // Wait a short while before setting the focus... Let the GUI do its thing first
                await Task.Delay(200);
                await this.dispatcher.RunAsync(CouldronDispatcherPriority.Low, () => this.AssociatedObject.Focus());
            });
        }
    }
}