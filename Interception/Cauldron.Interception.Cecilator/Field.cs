using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class Field : CecilatorBase, IEquatable<Field>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FieldDefinition fieldDef;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FieldReference fieldRef;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BuilderType type;

        internal Field(BuilderType type, FieldDefinition field) : base(type)
        {
            this.fieldDef = field;
            this.fieldRef = field.CreateFieldReference();
            this.type = type;
        }

        public FieldAttributes Attributes
        {
            get { return this.fieldDef.Attributes; }
            set { this.fieldDef.Attributes = value; }
        }

        public BuilderType DeclaringType { get { return this.type; } }
        public BuilderType FieldType { get { return new BuilderType(this.type, this.fieldRef.FieldType); } }
        public bool IsPrivate { get { return this.fieldDef.IsPrivate; } }
        public bool IsPublic { get { return this.fieldDef.IsPublic; } }
        public bool IsStatic { get { return this.fieldDef.IsStatic; } }
        public string Name { get { return this.fieldDef.Name; } }

        public void Remove() => this.type.typeDefinition.Fields.Remove(this.fieldDef);

        #region Equitable stuff

        public static implicit operator string(Field field) => field.fieldRef.FullName;

        public static bool operator !=(Field a, Field b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator !=(FieldReference a, Field b) => !object.Equals(b, null) && !b.Equals(a);

        public static bool operator !=(Field a, FieldReference b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(Field a, Field b) => !object.Equals(a, null) && a.Equals(b);

        public static bool operator ==(FieldReference a, Field b) => !object.Equals(b, null) && b.Equals(a);

        public static bool operator ==(Field a, FieldReference b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is Field)
                return this.Equals(obj as Field);

            if (obj is FieldReference)
                return this.fieldDef.FullName == (obj as FieldReference).FullName;

            return false;
        }

        public bool Equals(Field other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) || (other.fieldDef.FullName == this.fieldDef.FullName));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.fieldDef.FullName.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.fieldRef.FullName;

        #endregion Equitable stuff
    }
}