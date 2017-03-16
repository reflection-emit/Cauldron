using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public sealed class AttributedField : CecilatorBase, IEquatable<AttributedField>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly CustomAttribute customAttribute;

        internal AttributedField(Field field, CustomAttribute customAttribute) : base(field)
        {
            this.customAttribute = customAttribute;
            this.Field = field;
            this.Attribute = new BuilderCustomAttribute(field.type, field.fieldDef, customAttribute);
        }

        public BuilderCustomAttribute Attribute { get; private set; }

        public Field Field { get; private set; }

        public void Remove() => this.Field.fieldDef.CustomAttributes.Remove(this.customAttribute);

        #region Equitable stuff

        public static implicit operator string(AttributedField value) => value.ToString();

        public static bool operator !=(AttributedField a, AttributedField b) => !(a == b);

        public static bool operator ==(AttributedField a, AttributedField b)
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

            if (obj is AttributedField)
                return this.Equals(obj as AttributedField);

            if (obj is FieldDefinition)
                return this.Field.fieldDef == obj as FieldDefinition;

            if (obj is FieldReference)
                return this.Field.fieldRef == obj as FieldReference;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(AttributedField other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return this.customAttribute == other.customAttribute && this.Field.fieldRef == other.Field.fieldRef;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Field.GetHashCode() ^ this.Attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"{this.Field} >> {this.Attribute}";

        #endregion Equitable stuff
    }
}