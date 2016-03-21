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
            this.AssociatedObject.Focus();
        }
    }
}