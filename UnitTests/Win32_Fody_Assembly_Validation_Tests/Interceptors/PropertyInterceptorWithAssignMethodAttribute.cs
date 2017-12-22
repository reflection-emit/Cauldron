using Cauldron.Interception;
using System;

namespace Win32_Fody_Assembly_Validation_Tests
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyInterceptorWithAssignMethodAttribute : Attribute, IPropertySetterInterceptor
    {
        [AssignMethod("On{Name}")]
        public Action superAction = null;

        public void OnException(Exception e)
        {
        }

        public void OnExit()
        {
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            this.superAction();
            return false;
        }
    }
}