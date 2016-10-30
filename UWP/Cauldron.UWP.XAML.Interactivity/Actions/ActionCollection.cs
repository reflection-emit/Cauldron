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
        public ActionCollection(DependencyObject owner) : base(owner)
        {
        }
    }
}