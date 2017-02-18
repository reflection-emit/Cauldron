using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class Field : CecilatorBase, IEquatable<Field>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal FieldDefinition fieldDef;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal FieldReference fieldRef;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal BuilderType type;

        internal Field(BuilderType type, FieldDefinition field) : base(type)
        {
            this.fieldDef = field;
            this.fieldRef = field.CreateFieldReference();
            this.type = type;
        }

        public BuilderType DeclaringType { get { return this.type; } }
        public BuilderType FieldType { get { return new BuilderType(this.type, this.fieldRef.FieldType); } }
        public bool IsPrivate { get { return this.fieldDef.IsPrivate; } }
        public bool IsPublic { get { return this.fieldDef.IsPublic; } }
        public bool IsStatic { get { return this.fieldDef.IsStatic; } }
        public string Name { get { return this.fieldDef.Name; } }

        public IEnumerable<FieldUsage> FindUsages()
        {
            var result = this.fieldDef
                .DeclaringType
                .Methods
                .SelectMany(x => this.GetFieldUsage(x));

            if (!this.IsPrivate)
                return result.Concat(this.type.Builder.GetTypes().SelectMany(x => x.Resolve().Methods).SelectMany(x => this.GetFieldUsage(x)));

            return result;
        }

        public void Remove()
        {
            if (this.FindUsages().Any())
                throw new FieldInUseException("The field cannot be removed. It is in use.");

            this.type.typeDefinition.Fields.Remove(this.fieldDef);
        }

        private IEnumerable<FieldUsage> GetFieldUsage(MethodDefinition method)
        {
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                var instruction = method.Body.Instructions[i];
                if (instruction.OpCode == OpCodes.Ldsfld ||
                    instruction.OpCode == OpCodes.Ldflda ||
                    instruction.OpCode == OpCodes.Ldsflda ||
                    instruction.OpCode == OpCodes.Ldfld ||
                    instruction.OpCode == OpCodes.Stsfld ||
                    instruction.OpCode == OpCodes.Stfld ||
                    instruction.Operand is FieldDefinition ||
                    instruction.Operand is FieldReference)
                    yield return new FieldUsage(this, method, instruction);
            }
        }

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