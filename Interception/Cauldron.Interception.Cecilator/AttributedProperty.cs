using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class AttributedProperty : CecilatorBase, IEquatable<AttributedProperty>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal CustomAttribute customAttribute;

        internal AttributedProperty(Property property, CustomAttribute customAttribute) : base(property)
        {
            this.customAttribute = customAttribute;
            this.Property = property;
            this.Attribute = new BuilderType(property.type, customAttribute.AttributeType);
        }

        public BuilderType Attribute { get; private set; }
        public Property Property { get; private set; }

        public void Remove() => this.Property.propertyDefinition.CustomAttributes.Remove(this.customAttribute);

        #region Equitable stuff

        public static implicit operator string(AttributedProperty field) => field.ToString();

        public static bool operator !=(AttributedProperty a, AttributedProperty b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(AttributedProperty a, AttributedProperty b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is AttributedProperty)
                return this.Equals(obj as AttributedProperty);

            return false;
        }

        public bool Equals(AttributedProperty other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) ||
            (other.Property.propertyDefinition.FullName == this.Property.propertyDefinition.FullName && other.Attribute.typeDefinition.FullName == this.Attribute.typeDefinition.FullName));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Property.GetHashCode() ^ this.Attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"{this.Property} >> {this.Attribute}";

        #endregion Equitable stuff
    }
}