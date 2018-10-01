using Cauldron.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Reflection;

namespace UnitTests.BasicInterceptors
{
    [TestClass]
    public class AssignMethodAttribute_Method_FallBack_Tester
    {
        public int Blabla { get; set; }

        [AssignMethod_Action_PropertyInterceptor_FallBack(nameof(Blabla))]
        public string MyProperty { get; set; }

        [AssignMethod_Func_MethodInterceptor_FallBack]
        public static void FallBackWeave_Static_Test()
        {
        }

        [AssignMethod_Func_MethodInterceptor_FallBack]
        public void FallBackWeaveTest()
        {
        }

        private static string OnFallBackWeave_Static_Test(string name, object something, object anything)
        {
            return name;
        }

        private string OnFallBackWeaveTest(string name, object something, object anything)
        {
            return name;
        }
    }

    [TestClass]
    public class AssignMethodAttribute_New_Tests : AssignMethodAttributeBase_Tests
    {
        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Base_Public { get; set; }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_New_Public_Test()
        {
            var bla = this.Property_Action_Base_Public;
            Assert.IsFalse(this.propertyActionBaseInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_New_Public_Test()
        {
            this.Property_Action_Base_Public = true;
            Assert.IsFalse(this.propertyActionBaseInvoked);
        }

        public new void OnProperty_Action_Base_Public()
        {
        }

        [TestCleanup]
        public void ResetVariables()
        {
            this.propertyActionBaseInvoked = false;
        }
    }

    [TestClass]
    public class AssignMethodAttribute_Override_Tests : AssignMethodAttributeBase_Tests
    {
        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Base_Public { get; set; }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_Override_Public_Test()
        {
            var bla = this.Property_Action_Base_Public;
            Assert.IsFalse(this.propertyActionBaseInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Override_Public_Test()
        {
            this.Property_Action_Base_Public = true;
            Assert.IsFalse(this.propertyActionBaseInvoked);
        }

        public override void OnProperty_Action_Base_Public()
        {
        }

        [TestCleanup]
        public void ResetVariables()
        {
            this.propertyActionBaseInvoked = false;
        }
    }

    [TestClass]
    public class AssignMethodAttribute_Tests : AssignMethodAttributeBase_Tests
    {
        #region Property - Base Private, Protected, Internal, Public

        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Base_Internal { get; set; }

        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Base_Private { get; set; }

        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Base_Protected { get; set; }

        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Base_Public { get; set; }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_Base_Internal_Test()
        {
            var bla = this.Property_Action_Base_Internal;
            Assert.IsTrue(this.propertyActionBaseInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_Base_Private_Test()
        {
            var bla = this.Property_Action_Base_Private;
            Assert.IsFalse(this.propertyActionBaseInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_Base_Protected_Test()
        {
            var bla = this.Property_Action_Base_Protected;
            Assert.IsTrue(this.propertyActionBaseInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_Base_Public_Test()
        {
            var bla = this.Property_Action_Base_Public;
            Assert.IsTrue(this.propertyActionBaseInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Base_Internal_Test()
        {
            this.Property_Action_Base_Internal = true;
            Assert.IsTrue(this.propertyActionBaseInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Base_Private_Test()
        {
            this.Property_Action_Base_Private = true;
            Assert.IsFalse(this.propertyActionBaseInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Base_Protected_Test()
        {
            this.Property_Action_Base_Protected = true;
            Assert.IsTrue(this.propertyActionBaseInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Base_Public_Test()
        {
            this.Property_Action_Base_Public = true;
            Assert.IsTrue(this.propertyActionBaseInvoked);
        }

        #endregion Property - Base Private, Protected, Internal, Public

        #region Property - Private, Protected, Internal, public

        private bool propertyActionInvoked = false;

        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Internal { get; set; }

        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Private { get; set; }

        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Protected { get; set; }

        [AssignMethod_Action_PropertyInterceptor]
        public bool Property_Action_Public { get; set; }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_Internal_Test()
        {
            var bla = this.Property_Action_Internal;
            Assert.IsTrue(this.propertyActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_Private_Test()
        {
            var bla = this.Property_Action_Private;
            Assert.IsTrue(this.propertyActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_Protected_Test()
        {
            var bla = this.Property_Action_Protected;
            Assert.IsTrue(this.propertyActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Getter_Action_Public_Test()
        {
            var bla = this.Property_Action_Public;
            Assert.IsTrue(this.propertyActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Internal_Test()
        {
            this.Property_Action_Internal = true;
            Assert.IsTrue(this.propertyActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Private_Test()
        {
            this.Property_Action_Private = true;
            Assert.IsTrue(this.propertyActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Protected_Test()
        {
            this.Property_Action_Protected = true;
            Assert.IsTrue(this.propertyActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Public_Test()
        {
            this.Property_Action_Public = true;
            Assert.IsTrue(this.propertyActionInvoked);
        }

        public void OnProperty_Action_Public() => this.propertyActionInvoked = true;

        internal void OnProperty_Action_Internal() => this.propertyActionInvoked = true;

        protected void OnProperty_Action_Protected() => this.propertyActionInvoked = true;

        private void OnProperty_Action_Private() => this.propertyActionInvoked = true;

        #endregion Property - Private, Protected, Internal, public

        #region Property action with parameters

        private string action_With_Parameters;

        [AssignMethod_Action_WithArguments_PropertyInterceptor]
        public bool PropertyWithSetterAction { get; set; }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_With_Parameters()
        {
            this.PropertyWithSetterAction = true;
            Assert.AreEqual("Hello", this.action_With_Parameters);
        }

        private void OnPropertyWithSetterActionSet(string arg) => action_With_Parameters = arg;

        #endregion Property action with parameters

        #region Field - Private, Protected, Internal, public

        [AssignMethod_Action_PropertyInterceptor]
        private bool field_Action_Internal = false;

        [AssignMethod_Action_PropertyInterceptor]
        private bool field_Action_Private = false;

        [AssignMethod_Action_PropertyInterceptor]
        private bool field_Action_Protected = false;

        [AssignMethod_Action_PropertyInterceptor]
        private bool field_Action_Public = false;

        private bool fieldActionInvoked = false;

        [TestMethod]
        public void AssignMethod_Field_Getter_Action_Internal_Test()
        {
            var bla = this.field_Action_Internal;
            Assert.IsTrue(this.fieldActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Field_Getter_Action_Private_Test()
        {
            var bla = this.field_Action_Private;
            Assert.IsTrue(this.fieldActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Field_Getter_Action_Protected_Test()
        {
            var bla = this.field_Action_Protected;
            Assert.IsTrue(this.fieldActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Field_Getter_Action_Public_Test()
        {
            var bla = this.field_Action_Public;
            Assert.IsTrue(this.fieldActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Field_Setter_Action_Internal_Test()
        {
            this.field_Action_Internal = true;
            Assert.IsTrue(this.fieldActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Field_Setter_Action_Private_Test()
        {
            this.field_Action_Private = true;
            Assert.IsTrue(this.fieldActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Field_Setter_Action_Protected_Test()
        {
            this.field_Action_Protected = true;
            Assert.IsTrue(this.fieldActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Field_Setter_Action_Public_Test()
        {
            this.field_Action_Public = true;
            Assert.IsTrue(this.fieldActionInvoked);
        }

        public void OnField_Action_Public() => this.fieldActionInvoked = true;

        internal void OnField_Action_Internal() => this.fieldActionInvoked = true;

        protected void OnField_Action_Protected() => this.fieldActionInvoked = true;

        private void OnField_Action_Private() => this.fieldActionInvoked = true;

        #endregion Field - Private, Protected, Internal, public

        #region Method - Private, Protected, Internal, Public

        private bool methodActionInvoked = false;
        private bool methodFuncInvoked = false;

        [TestMethod]
        public void AssignMethod_Method_Action_Internal_Test()
        {
            this.Method_Action_Internal();
            Assert.IsTrue(this.methodActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Method_Action_Private_Test()
        {
            this.Method_Action_Private();
            Assert.IsTrue(this.methodActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Method_Action_Protected_Test()
        {
            this.Method_Action_Protected();
            Assert.IsTrue(this.methodActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Method_Action_Public_Test()
        {
            this.Method_Action_Public();
            Assert.IsTrue(this.methodActionInvoked);
        }

        [TestMethod]
        public void AssignMethod_Method_Func_Public_Test()
        {
            this.Method_Func_Public();
            Assert.IsTrue(this.methodFuncInvoked);
        }

        [AssignMethod_Action_MethodInterceptor]
        public void Method_Action_Internal()
        {
        }

        [AssignMethod_Action_MethodInterceptor]
        public void Method_Action_Private()
        {
        }

        [AssignMethod_Action_MethodInterceptor]
        public void Method_Action_Protected()
        {
        }

        [AssignMethod_Action_MethodInterceptor]
        public void Method_Action_Public()
        {
        }

        [AssignMethod_Func_MethodInterceptor]
        public void Method_Func_Public()
        {
        }

        public void OnMethod_Action_Public() => this.methodActionInvoked = true;

        public string OnMethod_Func_Public()
        {
            this.methodFuncInvoked = true;
            return "Tested";
        }

        internal void OnMethod_Action_Internal() => this.methodActionInvoked = true;

        protected void OnMethod_Action_Protected() => this.methodActionInvoked = true;

        private void OnMethod_Action_Private() => this.methodActionInvoked = true;

        #endregion Method - Private, Protected, Internal, Public

        #region Invoke method defined by constructor

        private bool propertyActionInvokeDefinedByContructor = false;

        [AssignMethod_Action_Constructor_PropertyInterceptor(nameof(RaiseMe))]
        public bool Toast { get; set; }

        [AssignMethod_Action_Constructor_Named_PropertyInterceptor(33, nameof(RaiseMe))]
        public bool Toast2 { get; set; }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Defined_By_Contructor_Named_Test()
        {
            this.Toast2 = true;
            Assert.IsTrue(this.propertyActionInvokeDefinedByContructor);
        }

        [TestMethod]
        public void AssignMethod_Property_Setter_Action_Defined_By_Contructor_Test()
        {
            this.Toast = true;
            Assert.IsTrue(this.propertyActionInvokeDefinedByContructor);
        }

        private void RaiseMe(string propertyName, object oldValue, object newValue)
        {
            this.propertyActionInvokeDefinedByContructor = true;
        }

        #endregion Invoke method defined by constructor

        [TestCleanup]
        public void ResetVariables()
        {
            this.propertyActionInvoked = false;
            this.propertyActionBaseInvoked = false;
            this.fieldActionInvoked = false;
            this.methodActionInvoked = false;
            this.methodFuncInvoked = false;
            this.action_With_Parameters = "";
            this.propertyActionInvokeDefinedByContructor = false;
        }
    }

    public class AssignMethodAttributeBase_Tests
    {
        protected bool propertyActionBaseInvoked = false;

        public virtual void OnProperty_Action_Base_Public() => this.propertyActionBaseInvoked = true;

        internal void OnProperty_Action_Base_Internal() => this.propertyActionBaseInvoked = true;

        protected void OnProperty_Action_Base_Protected() => this.propertyActionBaseInvoked = true;

        private void OnProperty_Action_Base_Private() => this.propertyActionBaseInvoked = true;
    }

    #region MyRegion

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Action_Constructor_Named_PropertyInterceptorAttribute : Attribute, IPropertySetterInterceptor
    {
        [AssignMethod("{CtorArgument:methodToInvokeName}")]
        public Action<string, object, object> action = null;

        public AssignMethod_Action_Constructor_Named_PropertyInterceptorAttribute(int anyParam, string methodToInvokeName)
        {
        }

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            action?.Invoke(propertyInterceptionInfo.PropertyName, oldValue, newValue);
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Action_Constructor_PropertyInterceptorAttribute : Attribute, IPropertySetterInterceptor
    {
        [AssignMethod("{CtorArgument:0}", true)]
        public Action<string, object, object> action = null;

        public AssignMethod_Action_Constructor_PropertyInterceptorAttribute(string methodToInvokeName)
        {
        }

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            action?.Invoke(propertyInterceptionInfo.PropertyName, oldValue, newValue);
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Action_MethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        [AssignMethod("On{Name}")]
        public Action action = null;

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            action?.Invoke();
        }

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Action_PropertyInterceptor_FallBackAttribute : Attribute, IPropertyInterceptor
    {
        [AssignMethod("get_{CtorArgument:propertyName}")]
        public Func<object> action = null;

        public AssignMethod_Action_PropertyInterceptor_FallBackAttribute(string propertyName)
        {
        }

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            action?.Invoke();
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            action?.Invoke();
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Action_PropertyInterceptorAttribute : Attribute, IPropertyInterceptor
    {
        [AssignMethod("On{Name}", true)]
        public Action action = null;

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            action?.Invoke();
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            action?.Invoke();
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Action_WithArguments_PropertyInterceptorAttribute : Attribute, IPropertySetterInterceptor
    {
        [AssignMethod("On{Name}Set")]
        public Action<string> action = null;

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            action?.Invoke("Hello");
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Func_MethodInterceptor_FallBackAttribute : Attribute, IMethodInterceptor
    {
        [AssignMethod("On{Name}")]
        public Func<string, object, object, object> action = null;

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            action?.Invoke(methodbase.Name, instance, values);
        }

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Func_MethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        [AssignMethod("On{Name}")]
        public Func<string> action = null;

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            action?.Invoke();
        }

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }
    }

    #endregion MyRegion
}