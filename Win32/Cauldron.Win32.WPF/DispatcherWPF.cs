using Cauldron.Activator;
using Cauldron.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Cauldron.XAML
{
    /// <exclude />
    [Component(typeof(IDispatcher), FactoryCreationPolicy.Singleton, 10)]
    public sealed class DispatcherWPF : DispatcherEx
    {
        /// <exclude />
        [ComponentConstructor]
        public DispatcherWPF()
        {
        }

        /// <exclude />
        protected override Dispatcher OnCreateDispatcher() => (Application.Current as Application).Dispatcher;
    }
}