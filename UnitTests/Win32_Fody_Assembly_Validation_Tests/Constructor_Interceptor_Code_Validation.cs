using Cauldron.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Win32_Fody_Assembly_Validation_Tests
{
    [TestClass]
    public sealed class Constructor_Interceptor_Code_Validation
    {
        [TestMethod]
        public void ConstructorInvokeTest()
        {
            var obj = new ConstructorInterceptorTestClass_C();
            Assert.AreEqual("Yes", obj.Title);
        }

    }

    #region Resources

    public class ConstructorInterceptorTestClass_A
    {
        [TestConstructorInterceptorA]
        public ConstructorInterceptorTestClass_A()
        {
        }

        [TestConstructorInterceptorA]
        public ConstructorInterceptorTestClass_A(string bla, int bla2, double bla3)
        {

        }
    }

    public class ConstructorInterceptorTestClass_C : IConstructorInterceptorInterface
    {
        [TestConstructorInterceptorB]
        public ConstructorInterceptorTestClass_C()
        {
        }

        public string Title { get; set; }
    }
    public class ConstructorInterceptorTestClass_B : ConstructorInterceptorTestClass_A
    {
        [TestConstructorInterceptorA]
        public ConstructorInterceptorTestClass_B(string  bla,double bla3) : base(bla, 78, bla3)
        {

        }

        [TestConstructorInterceptorA]
        public ConstructorInterceptorTestClass_B(string bla    ) : this(bla, 0.6)
        {

        }
    }

    public interface IConstructorInterceptorInterface
    {
        string Title { get; set; }
    }

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class TestConstructorInterceptorA : Attribute, IConstructorInterceptor
    {
        public void OnBeforeInitialization(Type declaringType, MethodBase methodbase, object[] values)
        {
        }

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class TestConstructorInterceptorB : Attribute, IConstructorInterceptor
    {
        public void OnBeforeInitialization(Type declaringType, MethodBase methodbase, object[] values)
        {
        }

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            if (instance is IConstructorInterceptorInterface test)
                test.Title = "Yes";
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }


    #endregion Resources
}