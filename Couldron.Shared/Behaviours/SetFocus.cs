using Couldron.Core;

#if NETFX_CORE
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
#else

using System.Windows.Controls;
using System.Windows;

#endif

namespace Couldron.Behaviours
{
    /// <summary>
    /// Provides a behaviour that can set the focus of a control after <see cref="FrameworkElement.Loaded"/>
    /// </summary>
    public sealed class SetFocus : Behaviour<Control>
    {
        private CouldronDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of <see cref="SetFocus"/>
        /// </summary>
        public SetFocus()
        {
            this.dispatcher = new CouldronDispatcher();
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
        }

        /// <summary>
        /// Occures when the <see cref="Behaviour{T}.AssociatedObject"/> is loaded
        /// </summary>
        protected override void OnLoaded()
        {
#if NETFX_CORE
            this.AssociatedObject.Focus(FocusState.Programmatic);
#else

            this.AssociatedObject.Focus();
#endif
        }
    }
}