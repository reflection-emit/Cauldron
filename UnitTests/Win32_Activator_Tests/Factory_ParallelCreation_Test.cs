using Cauldron;
using Cauldron.Activator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Activator_Tests
{
    [TestClass]
    public class Factory_ParallelCreation_Test
    {
        [TestMethod]
        public void Parallel_Object_Creation_Test()
        {
            var types = new List<Type>();

            for (int i = 1; i <= 1000; i++)
                types.Add(Type.GetType("Activator_Tests.TestClass" + i, true));

            Parallel.ForEach(types, x =>
            {
                var y1 = Factory.Create(x);
                for (int i = 0; i < 100; i++)
                    y1 = Factory.Create(types.ToArray().RandomPick());
            });
        }
    }
}