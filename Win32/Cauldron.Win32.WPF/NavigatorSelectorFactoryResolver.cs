using Cauldron.Activator;
using Cauldron.Reflection;
using Cauldron.XAML.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Cauldron.XAML
{
    /// <summary>
    /// Automatically selects the correct Navigator
    /// </summary>
    public sealed class NavigatorSelectorFactoryResolver : IFactoryExtension
    {
        /// <summary>
        /// Called after Factory initialization. It is also called if <see cref="Assemblies.LoadedAssemblyChanged"/> has been executed.
        /// This will be only called one time per extension only.
        /// </summary>
        /// <param name="factoryInfoTypes">A collection of known factory types.</param>
        public void Initialize(IEnumerable<IFactoryTypeInfo> factoryInfoTypes)
        {
            Factory.Resolvers.Add(typeof(INavigator), (callingType, ambigiousTypes) =>
            {
                var app = Application.Current.As<ApplicationBase>();

                if (app != null && app.IsSinglePage)
                    return ambigiousTypes.FirstOrDefault(x => x.Type == typeof(NavigatorSinglePage));

                return ambigiousTypes.FirstOrDefault(x => x.Type == typeof(Navigator));
            });
        }
    }
}