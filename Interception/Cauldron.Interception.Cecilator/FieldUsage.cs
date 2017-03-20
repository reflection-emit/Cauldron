using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class FieldUsage : CecilatorBase, IEquatable<FieldUsage>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Instruction instruction;

        internal FieldUsage(Field field, Method method, Instruction instruction) : base(field)
        {
            this.Field = field;
            this.Method = method;
            this.Type = field.type;
            this.instruction = instruction;
        }

        public Field Field { get; private set; }

        public bool IsBeforeBaseCall
        {
            get
            {
                var instructions = this.Method.methodDefinition.Body.Instructions;
                for (int i = 0; i < instructions.Count; i++)
                {
                    if (instructions[i].OpCode == OpCodes.Call && (instructions[i].Operand as MethodReference).Name == ".ctor")
                    {
                        if (this.instruction.Offset < instructions[i].Offset)
                            return true;

                        break;
                    }
                }

                return false;
            }
        }

        public bool IsLoadField
        {
            get { return this.instruction.OpCode == OpCodes.Ldfld || this.instruction.OpCode == OpCodes.Ldsfld; }
        }

        public bool IsStoreField
        {
            get { return this.instruction.OpCode == OpCodes.Stfld || this.instruction.OpCode == OpCodes.Stsfld; }
        }

        public Method Method { get; private set; }

        public BuilderType Type { get; private set; }

        public FieldUsage Replace(Field field)
        {
            if (field.fieldDef.DeclaringType.FullName != this.Method.methodDefinition.DeclaringType.FullName)
                throw new InvalidOperationException($"Replacement property must be declared in the same type. Field: {field.fieldDef.DeclaringType.FullName} - Method: {this.Method.methodDefinition.DeclaringType.FullName}");

            if (this.Field.IsStatic != field.IsStatic)
                throw new InvalidOperationException($"Replacement property must have the same modifier.");

            this.instruction.Operand = field.fieldRef;
            return new FieldUsage(field, this.Method, this.instruction);
        }

        public PropertyUsage Replace(Property property)
        {
            this.LogInfo("Replacing - " + this.Method);

            if (property.propertyDefinition.DeclaringType.FullName != this.Method.methodDefinition.DeclaringType.FullName)
                throw new InvalidOperationException($"Replacement property must be declared in the same type. Property: {property.propertyDefinition.DeclaringType.FullName} - Method: {this.Method.methodDefinition.DeclaringType.FullName}");

            if (this.Field.IsStatic != property.IsStatic)
                throw new InvalidOperationException($"Replacement property must have the same modifier.");

            if (this.instruction.OpCode == OpCodes.Ldfld || this.instruction.OpCode == OpCodes.Ldsfld)
            {
                this.instruction.OpCode = property.Getter.IsAbstract ? OpCodes.Callvirt : OpCodes.Call;
                this.instruction.Operand = property.Getter.methodReference.ResolveMethod(property.type.typeReference);
            }
            else if (this.instruction.OpCode == OpCodes.Stfld || this.instruction.OpCode == OpCodes.Stsfld)
            {
                this.instruction.OpCode = property.Setter.IsAbstract ? OpCodes.Callvirt : OpCodes.Call;
                this.instruction.Operand = property.Setter.methodReference.ResolveMethod(property.type.typeReference);
            }
            else if (this.instruction.OpCode == OpCodes.Ldflda || this.instruction.OpCode == OpCodes.Ldsflda)
            {
                // We leave this alone for this version... We have to find out how to deal with this first
                this.LogWarning($"OpCodes.Ldflda or OpCodes.Ldsflda found in {this.Method.Name}. This will not be replaced. Affected field: {this.Field}");
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