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
            var instance = typeof(TestClass16).CreateInstance(null, null);
        }

        [TestMethod]
        public void Parallel_Instance_Creation()
        {
            var types = new Type[]
            {
                typeof(TestClass1),
                typeof(TestClass2),
                typeof(TestClass3),
                typeof(TestClass4),
                typeof(TestClass5),
                typeof(TestClass6),
                typeof(TestClass7),
                typeof(TestClass8),
                typeof(TestClass9),
                typeof(TestClass10),
                typeof(TestClass11),
                typeof(TestClass12),
                typeof(TestClass13),
                typeof(TestClass14),
                typeof(TestClass15),
            };

            Parallel.ForEach(types, x =>
            {
                for (int i = 0; i < 1000; i++)
                    x.CreateInstance();
            });
        }

        #region Test classes

        public class TestClass1
        {
        }

        public class TestClass10
        {
        }

        public class TestClass11
        {
            public TestClass11()
            {
            }

            public TestClass11(string p)
            {
            }

            public TestClass11(int p)
            {
            }
        }

        public class TestClass12
        {
        }

        public class TestClass13
        {
            public TestClass13()
            {
            }

            public TestClass13(string p)
            {
            }
        }

        public class TestClass14
        {
        }

        public class TestClass15
        {
            private TestClass15()
            {
            }

            private TestClass15(string p)
            {
            }
        }

        public class TestClass16
        {
            public TestClass16(string arg1, string arg2)
            {
            }

            public TestClass16()
            {
            }
        }

        public class TestClass2
        {
        }

        public class TestClass3
        {
        }

        public class TestClass4
        {
        }

        public class TestClass5
        {
            public TestClass5(string p)
            {
            }

            private TestClass5()
            {
            }
        }

        public class TestClass6
        {
        }

        public class TestClass7
        {
        }

        public class TestClass8
        {
        }

        public class TestClass9
        {
        }

        #endregion Test classes
    }
}