using Cauldron.Activator;
using Cauldron.Core.Extensions;
using Cauldron.XAML.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Cauldron.XAML
{
    public sealed class NavigatorSelectorFactoryExtension : IFactoryExtension
    {
        /// <summary>
        /// Gets a value that indicates that this extension is able to resolve <see cref="AmbiguousMatchException"/>
        /// </summary>
        public bool CanHandleAmbiguousMatch
        {
            get { return true; }
        }

        /// <summary>
        /// Returns true if a <see cref="Type"/> can be modify arguments passed to <see cref="IFactoryExtension.ModifyArgument(ParameterInfo[], object[])"/> with this <see cref="IFactoryExtension"/> implementation
        /// </summary>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        /// <returns>True if can be manipulated</returns>
        public bool CanModifyArguments(MethodBase method, Type objectType) => false;

        /// <summary>
        /// Modifies the arguments defined by <paramref name="arguments"/> and returns the modified array
        /// </summary>
        /// <param name="argumentTypes">The parameter info of the constructor</param>
        /// <param name="arguments">The arguments used to create an object</param>
        /// <returns>A modified array of arguments</returns>
        public object[] ModifyArgument(ParameterInfo[] argumentTypes, object[] arguments)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Occures when an object is created
        /// </summary>
        /// <param name="context">The object instance</param>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        public void OnCreateObject(object context, Type objectType)
        {
        }

        /// <summary>
        /// Occures when <see cref="Factory"/> is initialized
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the object created</param>
        public void OnInitialize(Type type)
        {
        }

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