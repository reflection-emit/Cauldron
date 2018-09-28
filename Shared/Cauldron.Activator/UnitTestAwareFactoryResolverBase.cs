using Cauldron.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Activator
{
    /// <summary>
    /// Provides a base class that is unit test aware.
    /// </summary>
    public abstract class UnitTestAwareFactoryResolverBase : IFactoryExtension
    {
        private static string[] unitTestFramework = new string[] {
            "Microsoft.VisualStudio.QualityTools.UnitTestFramework",
            "Microsoft.VisualStudio.TestPlatform.TestFramework",
            "NUnit.Framework.TestFixtureAttribute"
        };

        private bool? _isUnitTest;

        private volatile bool isInitialized = false;

        /// <summary>
        /// Gets a value indicating that the extension is already loaded or not.
        /// </summary>
        public bool IsInitialized => this.isInitialized;

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
        /// Called during the initialization of the Factory.
        /// </summary>
        /// <param name="factoryInfoTypes">A collection of known factory types.</param>
        public void Initialize(IEnumerable<IFactoryTypeInfo> factoryInfoTypes)
        {
            if (this.isInitialized)
                return;

            this.OnInitialize(factoryInfoTypes);
            this.isInitialized = true;
        }

        /// <summary>
        /// Called during the initialization of the Factory.
        /// </summary>
        /// <param name="factoryInfoTypes">A collection of known factory types.</param>
        protected abstract void OnInitialize(IEnumerable<IFactoryTypeInfo> factoryInfoTypes);
    }
}