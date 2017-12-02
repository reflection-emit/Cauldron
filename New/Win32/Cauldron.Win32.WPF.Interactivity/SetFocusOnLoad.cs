using Cauldron.Core.Threading;
using Cauldron.Activator;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#else

using System.Windows.Controls;
using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Provides a behaviour that can set the focus of a control after <see cref="FrameworkElement.Loaded"/>
    /// </summary>
    public sealed class SetFocusOnLoad : Behaviour<Control>
    {
        [Inject]
        private IDispatcher dispatcher = null;

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected async override void OnAttach()
        {
#if WINDOWS_UWP
            await this.dispatcher.RunAsync(DispatcherPriority.Low, () => this.AssociatedObject.Focus(FocusState.Programmatic));
#else
            await this.dispatcher.RunAsync(DispatcherPriority.Low, () => this.AssociatedObject.Focus());
#endif
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
        }
    }
}