using Cauldron.Activator;
using Windows.UI.Xaml;

namespace Cauldron.XAML.Interactivity
{
    public sealed class HeaderBehaviourHelper
    {
        internal HeaderBehaviourHelper()
        {
        }

        public object Content { get; internal set; }

        [CloneIgnore]
        public FrameworkElement Object { get; internal set; }
    }
}