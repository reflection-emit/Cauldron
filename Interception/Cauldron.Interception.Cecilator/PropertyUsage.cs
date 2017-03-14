using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class PropertyUsage : CecilatorBase, IEquatable<PropertyUsage>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Instruction instruction;

        internal PropertyUsage(Property property, Method method, Instruction instruction) : base(property)
        {
            this.Property = property;
            this.Method = this.Method;
            this.Type = property.type;
            this.instruction = instruction;
        }

        public Method Method { get; private set; }

        public Property Property { get; private set; }

        public BuilderType Type { get; private set; }

        public PropertyUsage Replace(Property property)
        {
            if (this.Property.IsStatic != property.IsStatic || property.propertyDefinition.DeclaringType.FullName != this.Method.methodDefinition.DeclaringType.FullName)
                throw new InvalidOperationException($"Replacement property must have the same modifier and declaring type.");

            var operand = this.instruction.Operand as PropertyDefinition;
            this.instruction.Operand = operand.Name.StartsWith("set_") ? property.propertyDefinition.SetMethod : property.propertyDefinition.GetMethod;
            return new PropertyUsage(property, this.Method, this.instruction);
        }

        public FieldUsage Replace(Field field)
        {
            if (this.Property.IsStatic != field.IsStatic || field.fieldDef.DeclaringType.FullName != this.Method.methodDefinition.DeclaringType.FullName)
                throw new InvalidOperationException($"Replacement property must have the same modifier and declaring type.");

            var operand = this.instruction.Operand as MethodDefinition ?? this.instruction.Operand as MethodReference;

            if (operand.Name.StartsWith("set_"))
            {
                this.instruction.OpCode = field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld;
                this.instruction.Operand = field.fieldRef;
            }
            else
            {
                this.instruction.OpCode = field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld;
                this.instruction.Operand = field.fieldRef;
            }

            return new FieldUsage(field, this.Method, this.instruction);
        }

        #region Equitable stuff

        public static implicit operator string(PropertyUsage value) => value.ToString();

        public static bool operator !=(PropertyUsage a, PropertyUsage b) => !(a == b);

        public static bool operator ==(PropertyUsage a, PropertyUsage b)
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

            if (obj is PropertyUsage)
                return this.Equals(obj as PropertyUsage);

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(PropertyUsage other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return object.Equals(other, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Method.GetHashCode() ^ this.Property.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"IL_{this.instruction.Offset.ToString("X4")} >> {this.Method.methodDefinition.FullName} >> {this.Property.propertyDefinition.Name}";

        #endregion Equitable stuff
    }
}