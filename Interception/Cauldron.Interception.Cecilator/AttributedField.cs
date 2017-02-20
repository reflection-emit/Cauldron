using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public sealed class AttributedField : CecilatorBase, IEquatable<AttributedField>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal CustomAttribute customAttribute;

        internal AttributedField(Field field, CustomAttribute customAttribute) : base(field)
        {
            this.customAttribute = customAttribute;
            this.Field = field;
            this.Attribute = new BuilderType(field.type, customAttribute.AttributeType);
        }

        public BuilderType Attribute { get; private set; }
        public Field Field { get; private set; }

        #region Equitable stuff

        public static implicit operator string(AttributedField field) => field.ToString();

        public static bool operator !=(AttributedField a, AttributedField b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(AttributedField a, AttributedField b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is AttributedField)
                return this.Equals(obj as AttributedField);

            return false;
        }

        public bool Equals(AttributedField other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) || (other.Field.fieldDef.FullName == this.Field.fieldDef.FullName && other.Attribute.typeDefinition.FullName == this.Attribute.typeDefinition.FullName));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Field.GetHashCode() ^ this.Attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"{this.Field} >> {this.Attribute}";

        #endregion Equitable stuff
    }
}