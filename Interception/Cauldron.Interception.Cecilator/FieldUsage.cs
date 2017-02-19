using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class FieldUsage : CecilatorBase, IEquatable<FieldUsage>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Instruction instruction;

        internal FieldUsage(Field field, MethodDefinition method, Instruction instruction) : base(field)
        {
            this.Field = field;
            this.Method = new Method(field.type, method);
            this.Type = field.type;
            this.instruction = instruction;
        }

        internal FieldUsage(Field field, Method method, Instruction instruction) : base(field)
        {
            this.Field = field;
            this.Method = this.Method;
            this.Type = field.type;
            this.instruction = instruction;
        }

        public Field Field { get; private set; }
        public Method Method { get; private set; }
        public BuilderType Type { get; private set; }

        public FieldUsage Replace(Field field)
        {
            if (this.Field.IsStatic == field.IsStatic && field.fieldDef.DeclaringType.FullName == this.Method.methodDefinition.DeclaringType.FullName)
            {
                this.instruction.Operand = field.fieldRef;
                return new FieldUsage(this.Field, this.Method, this.instruction);
            }

            throw new InvalidOperationException($"Replacement field must have the same modifier and declaring type.");
        }

        #region Equitable stuff

        public static implicit operator string(FieldUsage usage) => usage.ToString();

        public static bool operator !=(FieldUsage a, FieldUsage b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(FieldUsage a, FieldUsage b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is FieldUsage)
                return this.Equals(obj as FieldUsage);

            return false;
        }

        public bool Equals(FieldUsage other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) || (other.ToString() == this.ToString()));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Method.ToString().GetHashCode() ^ this.Field.ToString().GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"IL_{this.instruction.Offset.ToString("X4")} >> {this.Method.methodDefinition.FullName} >> {this.Field.fieldDef.Name}";

        #endregion Equitable stuff
    }
}