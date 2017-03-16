using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class BuilderCustomAttribute : CecilatorBase, IEquatable<BuilderCustomAttribute>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly CustomAttribute attribute;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly FieldDefinition fieldDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MethodDefinition methodDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TypeDefinition typeDefinition;

        internal BuilderCustomAttribute(BuilderType type, FieldDefinition fieldDefinition, CustomAttribute attribute) : base(type)
        {
            this.fieldDefinition = fieldDefinition;
            this.attribute = attribute;
            this.Type = new BuilderType(type, attribute.AttributeType);
        }

        internal BuilderCustomAttribute(BuilderType type, MethodDefinition methodDefinition, CustomAttribute attribute) : base(type)
        {
            this.methodDefinition = methodDefinition;
            this.attribute = attribute;
            this.Type = new BuilderType(type, attribute.AttributeType);
        }

        internal BuilderCustomAttribute(BuilderType type, TypeDefinition typeDefinition, CustomAttribute attribute) : base(type)
        {
            this.typeDefinition = typeDefinition;
            this.attribute = attribute;
            this.Type = new BuilderType(type, attribute.AttributeType);
        }

        public string Fullname { get { return this.attribute.AttributeType.FullName; } }

        public BuilderType Type { get; private set; }

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

        public void Remove()
        {
            if (this.methodDefinition != null)
                this.methodDefinition.CustomAttributes.Remove(this.attribute);

            if (this.typeDefinition != null)
                this.typeDefinition.CustomAttributes.Remove(this.attribute);

            if (this.fieldDefinition != null)
                this.fieldDefinition.CustomAttributes.Remove(this.attribute);
        }

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