#if WINDOWS_UWP
using Windows.UI.Xaml;

#else

using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity.Actions
{
    /// <summary>
    /// Represents a collection of <see cref="ActionBase"/>
    /// </summary>
    public sealed class ActionCollection : BehaviourCollection<ActionBase>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ActionCollection"/>
        /// </summary>
        /// <param name="owner"></param>
        public ActionCollection(DependencyObject owner) : base(owner)
        {
        }
    }
}