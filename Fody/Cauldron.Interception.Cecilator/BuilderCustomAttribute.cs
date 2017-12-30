using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class BuilderCustomAttribute : CecilatorBase, IEquatable<BuilderCustomAttribute>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly CustomAttribute attribute;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ICustomAttributeProvider customAttributeProvider;

        internal BuilderCustomAttribute(Builder builder, ICustomAttributeProvider customAttributeProvider, CustomAttribute attribute) : base(builder)
        {
            this.customAttributeProvider = customAttributeProvider;
            this.attribute = attribute;
            this.Type = new BuilderType(builder, attribute.AttributeType);
        }

        public CustomAttributeArgument[] ConstructorArguments => this.attribute.HasConstructorArguments ?
            this.attribute.ConstructorArguments.ToArray() :
            new CustomAttributeArgument[0];

        public string Fullname => this.attribute.AttributeType.FullName;

        public BuilderType Type { get; private set; }

        public CustomAttributeArgument GetConstructorArgument(int parameterIndex) => this.attribute.ConstructorArguments[parameterIndex];

        public CustomAttributeArgument GetConstructorArgument(string parameterName)
        {
            var parameter = this.attribute.Constructor.Parameters.FirstOrDefault(x => x.Name == parameterName);
            if (parameter == null)
                return default(CustomAttributeArgument);

            return this.attribute.ConstructorArguments[parameter.Index];
        }

        public BuilderType GetConstructorArgumentType(int index) => new BuilderType(this.Type, this.attribute.Constructor.Parameters[index].ParameterType);

        public void MoveTo(Property property)
        {
            this.Remove();
            property.propertyDefinition.CustomAttributes.Add(attribute);
        }

        public void MoveTo(Field field)
        {
            this.Remove();
            field.fieldDef.CustomAttributes.Add(attribute);
        }

        public void MoveTo(BuilderType type)
        {
            this.Remove();
            type.typeDefinition.CustomAttributes.Add(attribute);
        }

        public void MoveTo(Method method)
        {
            this.Remove();
            method.methodDefinition.CustomAttributes.Add(attribute);
        }

        public void Remove() => this.customAttributeProvider?.CustomAttributes.Remove(this.attribute);

        #region Equitable stuff

        public static implicit operator string(BuilderCustomAttribute value) => value.ToString();

        public static bool operator !=(BuilderCustomAttribute a, BuilderCustomAttribute b) => !(a == b);

        public static bool operator ==(BuilderCustomAttribute a, BuilderCustomAttribute b)
        {
            if (object.Equals(a, null) && object.Equals(b, null))
                return true;

            if (object.Equals(a, null))
                return false;

            return a.Equals(b);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is BuilderCustomAttribute)
                return this.Equals(obj as BuilderCustomAttribute);

            if (obj is CustomAttribute)
                return this.attribute == obj as CustomAttribute;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(BuilderCustomAttribute other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return this.attribute == other.attribute;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.attribute.AttributeType.FullName;

        #endregion Equitable stuff
    }
}