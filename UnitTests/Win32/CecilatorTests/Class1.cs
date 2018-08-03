using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CecilatorTests
{
    internal class Class1
    {
        public void If_Primitiv_Is_NullableArgv()
        {
            this.If_Primitiv_Is_NullableArgv_toast(22);
        }

        private void If_Primitiv_Is_NullableArgv_toast(int? num)
        {
            int num2 = 22;
            if (object.Equals(num2, num))
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }
    }
}