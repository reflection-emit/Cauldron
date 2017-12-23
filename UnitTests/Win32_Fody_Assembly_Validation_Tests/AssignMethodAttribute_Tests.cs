using Cauldron.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Win32_Fody_Assembly_Validation_Tests
{
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

        public void OnField_Action_PublicProperty() => this.fieldActionInvoked = true;

        internal void OnField_Action_InternalProperty() => this.fieldActionInvoked = true;

        protected void OnField_Action_ProtectedProperty() => this.fieldActionInvoked = true;

        private void OnField_Action_PrivateProperty() => this.fieldActionInvoked = true;

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

        [TestCleanup]
        public void ResetVariables()
        {
            this.propertyActionInvoked = false;
            this.propertyActionBaseInvoked = false;
            this.fieldActionInvoked = false;
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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Action_MethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        [AssignMethod("On{Name}")]
        public Action action = null;

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            action?.Invoke();
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Action_PropertyInterceptorAttribute : Attribute, IPropertyInterceptor
    {
        [AssignMethod("On{Name}")]
        public Action action = null;

        public void OnException(Exception e)
        {
        }

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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AssignMethod_Func_MethodInterceptorAttribute : Attribute, IMethodInterceptor
    {
        [AssignMethod("On{Name}")]
        public Func<string> action = null;

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            Debug.WriteLine(action?.Invoke());
        }

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }
    }

    #endregion MyRegion
}