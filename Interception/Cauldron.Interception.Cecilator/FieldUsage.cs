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
        private readonly Instruction instruction;

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
            if (this.Field.IsStatic != field.IsStatic || field.fieldDef.DeclaringType.FullName != this.Method.methodDefinition.DeclaringType.FullName)
                throw new InvalidOperationException($"Replacement field must have the same modifier and declaring type.");

            this.instruction.Operand = field.fieldRef;
            return new FieldUsage(field, this.Method, this.instruction);
        }

        public PropertyUsage Replace(Property property)
        {
            if (this.Field.IsStatic != property.IsStatic || property.propertyDefinition.DeclaringType.FullName != this.Method.methodDefinition.DeclaringType.FullName)
                throw new InvalidOperationException($"Replacement property must have the same modifier and declaring type.");

            if (this.instruction.OpCode == OpCodes.Ldfld || this.instruction.OpCode == OpCodes.Ldsfld || this.instruction.OpCode == OpCodes.Ldsflda || this.instruction.OpCode == OpCodes.Ldflda)
            {
                this.instruction.OpCode = OpCodes.Callvirt;
                this.instruction.Operand = property.Getter.methodReference;
            }
            else if (this.instruction.OpCode == OpCodes.Stfld || this.instruction.OpCode == OpCodes.Stsfld)
            {
                this.instruction.OpCode = OpCodes.Callvirt;
                this.instruction.Operand = property.Setter.methodReference;
            }

            return new PropertyUsage(property, this.Method, this.instruction);
        }

        #region Equitable stuff

        public static implicit operator string(FieldUsage value) => value.ToString();

        public static bool operator !=(FieldUsage a, FieldUsage b) => !(a == b);

        public static bool operator ==(FieldUsage a, FieldUsage b)
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

            if (obj is FieldUsage)
                return this.Equals(obj as FieldUsage);

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(FieldUsage other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return object.Equals(other, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Method.GetHashCode() ^ this.Field.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"IL_{this.instruction.Offset.ToString("X4")} >> {this.Method.methodDefinition.FullName} >> {this.Field.fieldDef.Name}";

        #endregion Equitable stuff
    }
}