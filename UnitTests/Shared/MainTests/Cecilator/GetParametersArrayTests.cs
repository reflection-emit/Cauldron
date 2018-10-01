using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests.Cecilator
{
    [TestClass]
    public class GetParametersArrayTests
    {
        [TestMethod]
        public void Overriden_Virtual_Method()
        {
            var o = new GetParametersArrayTests_InheritingClass();
            o.Test(3, "Hello");
        }

        [TestMethod]
        public void Standard_Method()
        {
            var o = new GetParametersArrayTests_Base();
            o.Test2(3, "Hello");
        }
    }

    public class GetParametersArrayTests_Base
    {
        public virtual void Test(int a, string b)
        {
        }

        [GetParametersArrayTests]
        public void Test2(int a, string b)
        {
        }
    }

    public class GetParametersArrayTests_InheritingClass : GetParametersArrayTests_Base
    {
        [GetParametersArrayTests]
        public override void Test(int a, string b) => base.Test(a, b);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GetParametersArrayTestsAttribute : Attribute
    {
        public static void Assert(object[] parameters)
        {
            if (parameters.Length != 2)
                throw new Exception("Wrong parameter length");

            var a = (int)parameters[0];

            if (a != 3)
                throw new Exception("Value of a is not 3");

            var b = parameters[1] as string;

            if (b != "Hello")
                throw new Exception("Value of b is not Hello");
        }
    }
}