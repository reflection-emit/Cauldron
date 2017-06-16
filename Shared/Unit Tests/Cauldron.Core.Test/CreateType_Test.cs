using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Core.Extensions;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace Cauldron.Core.Test
{
    [TestClass]
    public class CreateInstance_Test
    {
        [TestMethod]
        public void CreateInstance_With_NullParameter()
        {
            var instance = typeof(TestClassN16).CreateInstance(null, null);
        }

        [TestMethod]
        public void Parallel_Instance_Creation()
        {
            var types = new Type[]
            {
                typeof(TestClassN1),
                typeof(TestClassN2),
                typeof(TestClassN3),
                typeof(TestClassN4),
                typeof(TestClassN5),
                typeof(TestClassN6),
                typeof(TestClassN7),
                typeof(TestClassN8),
                typeof(TestClassN9),
                typeof(TestClassN10),
                typeof(TestClassN11),
                typeof(TestClassN12),
                typeof(TestClassN13),
                typeof(TestClassN14),
                typeof(TestClassN15),
            };

            Parallel.ForEach(types, x =>
            {
                for (int i = 0; i < 1000; i++)
                    x.CreateInstance();
            });
        }

        [TestMethod]
        public void Parallel_Instance_Creation_Random()
        {
            var types = new List<Type>();

            for (int i = 1; i <= 1000; i++)
                types.Add(Type.GetType("Cauldron.Core.Test.TestClass" + i, true));

            Parallel.ForEach(types, x =>
            {
                var y1 = x.CreateInstance();
                for (int i = 0; i < 10; i++)
                    y1 = types.ToArray().RandomPick().CreateInstance();
            });
        }

        #region Test classes

        public class TestClassN1
        {
        }

        public class TestClassN10
        {
        }

        public class TestClassN11
        {
            public TestClassN11()
            {
            }

            public TestClassN11(string p)
            {
            }

            public TestClassN11(int p)
            {
            }
        }

        public class TestClassN12
        {
        }

        public class TestClassN13
        {
            public TestClassN13()
            {
            }

            public TestClassN13(string p)
            {
            }
        }

        public class TestClassN14
        {
        }

        public class TestClassN15
        {
            private TestClassN15()
            {
            }

            private TestClassN15(string p)
            {
            }
        }

        public class TestClassN16
        {
            public TestClassN16(string arg1, string arg2)
            {
            }

            public TestClassN16()
            {
            }
        }

        public class TestClassN2
        {
        }

        public class TestClassN3
        {
        }

        public class TestClassN4
        {
        }

        public class TestClassN5
        {
            public TestClassN5(string p)
            {
            }

            private TestClassN5()
            {
            }
        }

        public class TestClassN6
        {
        }

        public class TestClassN7
        {
        }

        public class TestClassN8
        {
        }

        public class TestClassN9
        {
        }

        #endregion Test classes
    }
}