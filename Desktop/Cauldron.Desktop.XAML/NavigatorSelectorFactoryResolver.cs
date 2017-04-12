using Cauldron.Activator;
using Cauldron.Core.Extensions;
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
    public sealed class NavigatorSelectorFactoryResolver : IFactoryResolver
    {
        /// <summary>
        /// Occures if multiple Types with the same <paramref name="contractName"/> was found.
        /// <para/>
        /// Should return null if <paramref name="ambiguousTypes"/> collection does not contain the required <see cref="Type"/>
        /// </summary>
        /// <param name="ambiguousTypes">A collection of Types that with the same <paramref name="contractName"/></param>
        /// <param name="contractName">The contract name of the implementations</param>
        /// <returns>The selected <see cref="Type"/></returns>
        public Type SelectAmbiguousMatch(IEnumerable<Type> ambiguousTypes, string contractName)
        {
            if (contractName == typeof(INavigator).FullName && ambiguousTypes.Count() == 2)
            {
                var app = Application.Current.As<ApplicationBase>();

                if (app != null && app.IsSinglePage)
                    return typeof(NavigatorSinglePage);

                return typeof(Navigator);
            }

            return null;
        }
    }
}