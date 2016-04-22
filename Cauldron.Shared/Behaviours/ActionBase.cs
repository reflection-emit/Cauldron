#if NETFX_CORE
using Windows.UI.Xaml;

#else

using System.Windows;

#endif

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Represents a base class for action behaviours
    /// </summary>
    public abstract class ActionBase : Behaviour<FrameworkElement>
    {
        /// <summary>
        /// Occures when the action is invoked by an event
        /// </summary>
        /// <param name="parameter">The parameter passed by the event</param>
        public abstract void Invoke(object parameter);
    }
}