using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class AttributedProperty : CecilatorBase, IEquatable<AttributedProperty>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly CustomAttribute customAttribute;

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

        public static implicit operator string(AttributedProperty value) => value.ToString();

        public static bool operator !=(AttributedProperty a, AttributedProperty b) => !(a == b);

        public static bool operator ==(AttributedProperty a, AttributedProperty b)
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

            if (obj is AttributedProperty)
                return this.Equals(obj as AttributedProperty);

            if (obj is PropertyDefinition)
                return this.Property.propertyDefinition == obj as PropertyDefinition;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(AttributedProperty other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return this.customAttribute == other.customAttribute && this.Property.propertyDefinition == other.Property.propertyDefinition;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Property.GetHashCode() ^ this.Attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"{this.Property} >> {this.Attribute}";

        #endregion Equitable stuff
    }
}