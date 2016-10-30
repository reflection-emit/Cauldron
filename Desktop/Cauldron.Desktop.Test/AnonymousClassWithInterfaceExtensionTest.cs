using System;
using Cauldron.Dynamic;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace Cauldron.Test
{
    public interface IAnonymousClassWithInterfaceExtension2TestInterface : IAnonymousClassWithInterfaceExtensionTestInterface
    {
        DateTime AnyDate { get; }

        string FirstProperty { get; }

        IAnonymousClassWithInterfaceExtensionTestInterface Interface { get; set; }
    }

    public interface IAnonymousClassWithInterfaceExtensionTestInterface
    {
        double ADouble { get; }

        int AnyInt { get; }

        string TheString { get; }
    }

    public interface IAnonymousClassWithInterfaceExtensionTestInterfaceWithMethod
    {
        double ADouble { get; }

        int AnyInt { get; }

        string TheString { get; }

        string GetAnyStringMethod(int aParameter, char bParameter);
    }

    [TestClass]
    public class AnonymousClassWithInterfaceExtensionTest
    {
        [TestMethod]
        public void Complex_Type_Creation_Test()
        {
            var anonClass = new { AnyDate = DateTime.Now, FirstProperty = "TestString" };
            var newObject = anonClass.CreateObject<IAnonymousClassWithInterfaceExtension2TestInterface>();

            Assert.IsTrue(anonClass.AnyDate == newObject.AnyDate);
            Assert.IsTrue(anonClass.FirstProperty == newObject.FirstProperty);
            Assert.IsTrue(newObject.Interface == null);
        }

        [TestMethod]
        public void Simple_Type_Creation_Test()
        {
            var anonClass = new { ADouble = 9922.992, AnyInt = 666, TheString = "TestString" };
            var newObject = anonClass.CreateObject<IAnonymousClassWithInterfaceExtensionTestInterface>();

            Assert.IsTrue(anonClass.ADouble == newObject.ADouble);
            Assert.IsTrue(anonClass.AnyInt == newObject.AnyInt);
            Assert.IsTrue(anonClass.TheString == newObject.TheString);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Type_Creation_With_Method_In_Interface_Exception_Test()
        {
            var anonClass = new { ADouble = 9922.992, AnyInt = 666, TheString = "TestString" };
            var newObject = anonClass.CreateObject<IAnonymousClassWithInterfaceExtensionTestInterfaceWithMethod>();

            newObject.GetAnyStringMethod(99, 'p');
        }

        [TestMethod]
        public void Type_Creation_With_Method_In_Interface_Test()
        {
            var anonClass = new { ADouble = 9922.992, AnyInt = 666, TheString = "TestString" };
            var newObject = anonClass.CreateObject<IAnonymousClassWithInterfaceExtensionTestInterfaceWithMethod>();

            Assert.IsTrue(anonClass.ADouble == newObject.ADouble);
            Assert.IsTrue(anonClass.AnyInt == newObject.AnyInt);
            Assert.IsTrue(anonClass.TheString == newObject.TheString);
        }
    }
}