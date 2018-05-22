using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class AttributedType : CecilatorBase, IEquatable<AttributedType>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly CustomAttribute customAttribute;

        public AttributedType(BuilderType type, BuilderCustomAttribute builderCustomAttribute) : base(type)
        {
            this.customAttribute = builderCustomAttribute.attribute;
            this.Type = type;
            this.Attribute = builderCustomAttribute;
        }

        internal AttributedType(BuilderType type, CustomAttribute customAttribute) : base(type)
        {
            this.customAttribute = customAttribute;
            this.Type = type;
            this.Attribute = new BuilderCustomAttribute(type.Builder, type.typeDefinition, customAttribute);
        }

        public BuilderCustomAttribute Attribute { get; private set; }

        public BuilderType Type { get; private set; }

        public void Remove() => this.Type.typeDefinition.CustomAttributes.Remove(this.customAttribute);

        #region Equitable stuff

        public static implicit operator string(AttributedType value) => value.ToString();

        public static bool operator !=(AttributedType a, AttributedType b) => !(a == b);

        public static bool operator ==(AttributedType a, AttributedType b)
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

            if (obj is AttributedType)
                return this.Equals(obj as AttributedType);

            if (obj is PropertyDefinition)
                return this.Type.typeDefinition == obj as TypeDefinition;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(AttributedType other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return this.customAttribute == other.customAttribute && this.Type.typeDefinition == other.Type.typeDefinition;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Type.GetHashCode() ^ this.Attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"{this.Type} >> {this.Attribute}";

        #endregion Equitable stuff
    }
}