using Cauldron.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Cauldron.Dynamic
{
    /// <summary>
    /// Internal wrapper for <see cref="PropertyInfo"/>
    /// </summary>
    internal sealed class PropertyPathInfo : MethodBase, _PropertyInfo
    {
        private PropertyInfo _propertyInfo;
        private List<MethodInfo> pathMethodInfo = new List<MethodInfo>();

        private PropertyPathInfo() : base()
        {
        }

        PropertyAttributes _PropertyInfo.Attributes { get { return this._propertyInfo.Attributes; } }

        public override MethodAttributes Attributes { get { return this._propertyInfo.GetMethod.Attributes; } }

        public bool CanRead { get { return this._propertyInfo.CanRead; } }

        public bool CanWrite { get { return this._propertyInfo.CanWrite; } }

        public override Type DeclaringType { get { return this._propertyInfo.DeclaringType; } }

        public MethodInfo GetMethod { get { return this._propertyInfo.GetMethod; } }

        public override MemberTypes MemberType { get { return this._propertyInfo.MemberType; } }

        public override RuntimeMethodHandle MethodHandle { get { return this._propertyInfo.GetMethod.MethodHandle; } }

        public IList<MethodInfo> Methods { get { return this.pathMethodInfo; } }

        public override string Name { get { return this._propertyInfo.Name; } }

        public Type PropertyType { get { return this._propertyInfo.PropertyType; } }

        public override Type ReflectedType { get { return this._propertyInfo.ReflectedType; } }

        public static PropertyPathInfo GetPropertyFromPath(Type declaringType, string path)
        {
            var bindingPath = path.Split('.');
            PropertyInfo propertySection = null;
            PropertyPathInfo result = new PropertyPathInfo();

            // let us follow the path and change the source accordingly
            for (int i = 0; i < bindingPath.Length; i++)
            {
                if (declaringType == null)
                    break;

                var section = bindingPath[i];

                propertySection = declaringType.GetPropertyEx(section);

                if (propertySection == null)
                    // the path is invalid...
                    return null;

                result.pathMethodInfo.Add(propertySection.GetMethod);
                declaringType = propertySection.PropertyType;
            }

            result._propertyInfo = propertySection;
            return result;
        }

        public static implicit operator MethodInfo(PropertyPathInfo info) => info._propertyInfo.GetMethod;

        public static implicit operator PropertyPathInfo(PropertyInfo propertyInfo) => new PropertyPathInfo { _propertyInfo = propertyInfo };

        public MethodInfo[] GetAccessors() => this._propertyInfo.GetAccessors();

        public MethodInfo[] GetAccessors(bool nonPublic) => this._propertyInfo.GetAccessors(nonPublic);

        public override object[] GetCustomAttributes(bool inherit) => this._propertyInfo.GetCustomAttributes(inherit);

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => this._propertyInfo.GetCustomAttributes(attributeType, inherit);

        public MethodInfo GetGetMethod() => this._propertyInfo.GetGetMethod();

        public MethodInfo GetGetMethod(bool nonPublic) => this._propertyInfo.GetGetMethod(nonPublic);

        public void GetIDsOfNames(ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId) =>
            (this._propertyInfo as _PropertyInfo).GetIDsOfNames(ref riid, rgszNames, cNames, lcid, rgDispId);

        public ParameterInfo[] GetIndexParameters() => this._propertyInfo.GetIndexParameters();

        public override MethodImplAttributes GetMethodImplementationFlags() => this._propertyInfo.GetMethod.GetMethodImplementationFlags();

        public override ParameterInfo[] GetParameters() => this._propertyInfo.GetMethod.GetParameters();

        public MethodInfo GetSetMethod() => this._propertyInfo.GetSetMethod();

        public MethodInfo GetSetMethod(bool nonPublic) => this._propertyInfo.GetSetMethod(nonPublic);

        public void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo) => (this._propertyInfo as _PropertyInfo).GetTypeInfo(iTInfo, lcid, ppTInfo);

        public void GetTypeInfoCount(out uint pcTInfo) => (this._propertyInfo as _PropertyInfo).GetTypeInfoCount(out pcTInfo);

        public object GetValue(object obj, object[] index) => this._propertyInfo.GetValue(obj, index);

        public object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) =>
            this._propertyInfo.GetValue(obj, invokeAttr, binder, index, culture);

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) =>
            this._propertyInfo.GetMethod.Invoke(obj, invokeAttr, binder, parameters, culture);

        public void Invoke(uint dispIdMember, ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr) =>
           (this._propertyInfo as _PropertyInfo).Invoke(dispIdMember, ref riid, lcid, wFlags, pDispParams, pVarResult, pExcepInfo, puArgErr);

        public override bool IsDefined(Type attributeType, bool inherit) =>
            this._propertyInfo.IsDefined(attributeType, inherit);

        public void SetValue(object obj, object value, object[] index) => this._propertyInfo.SetValue(obj, value, index);

        public void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) =>
            this._propertyInfo.SetValue(obj, value, invokeAttr, binder, index, culture);
    }
}