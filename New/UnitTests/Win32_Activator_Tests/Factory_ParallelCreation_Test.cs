using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cauldron.Core.Extensions;
using Cauldron.Activator;

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace Win32_Activator_Tests
{
    [TestClass]
    public class Factory_ParallelCreation_Test
    {
        [TestMethod]
        public void Parallel_Object_Creation_Test()
        {
            var types = new List<Type>();

            for (int i = 1; i <= 1000; i++)
                types.Add(Type.GetType("Win32_Activator_Tests.TestClass" + i, true));

            Parallel.ForEach(types, x =>
            {
                var y1 = Factory.Create(x);
                for (int i = 0; i < 100; i++)
                    y1 = Factory.Create(types.ToArray().RandomPick());
            });
        }
    }
}