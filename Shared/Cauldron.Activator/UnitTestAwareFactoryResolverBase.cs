using Cauldron.Core.Reflection;
using System;
using System.Linq;

namespace Cauldron.Activator
{
    /// <summary>
    /// Provides a base class that is unit test aware.
    /// </summary>
    public abstract class UnitTestAwareFactoryResolverBase : IFactoryResolver
    {
        private static string[] unitTestFramework = new string[] {
            "Microsoft.VisualStudio.QualityTools.UnitTestFramework",
            "Microsoft.VisualStudio.TestPlatform.TestFramework",
            "NUnit.Framework.TestFixtureAttribute"
        };

        private bool? _isUnitTest;

        /// <summary>
        /// Gets a value that indicates if the code is running in a unit test.
        /// </summary>
        protected bool IsUnitTest
        {
            get
            {
                if (!this._isUnitTest.HasValue)
                    this._isUnitTest = Assemblies.Known
                        .Any(x => unitTestFramework.Contains(x.GetName().Name));

                return this._isUnitTest.Value;
            }
        }

        /// <summary>
        /// Occures if multiple Types with the same <paramref name="contractName"/> was found.
        /// <para/>
        /// Should return null if <paramref name="ambiguousTypes"/> collection does not contain the required <see cref="Type"/>
        /// </summary>
        /// <param name="ambiguousTypes">A collection of Types that with the same <paramref name="contractName"/></param>
        /// <param name="contractName">The contract name of the implementations</param>
        /// <returns>The selected <see cref="Type"/></returns>
        public IFactoryTypeInfo SelectAmbiguousMatch(IFactoryTypeInfo[] ambiguousTypes, string contractName)
        {
            if (!this.IsApplicable(contractName))
                return null;

            return this.OnSelectAmbiguousMatch(ambiguousTypes, contractName);
        }

        /// <summary>
        /// Checks if this resolver implementation is applicable to the contract
        /// </summary>
        /// <param name="contractName">The contract name of the implementations</param>
        /// <returns>Returns true if applicable; otherwise false.</returns>
        protected abstract bool IsApplicable(string contractName);

        /// <summary>
        /// Occures if multiple Types with the same <paramref name="contractName"/> was found.
        /// <para/>
        /// Should return null if <paramref name="ambiguousTypes"/> collection does not contain the required <see cref="Type"/>
        /// </summary>
        /// <param name="ambiguousTypes">A collection of Types that with the same <paramref name="contractName"/></param>
        /// <param name="contractName">The contract name of the implementations</param>
        /// <returns>The selected <see cref="Type"/></returns>
        protected abstract IFactoryTypeInfo OnSelectAmbiguousMatch(IFactoryTypeInfo[] ambiguousTypes, string contractName);
    }
}