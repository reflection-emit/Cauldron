using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class FieldExtensions
    {
        public static FieldAssignCoder As(this FieldAssignCoder fieldAssignCoder, BuilderType type)
        {
            var coder = fieldAssignCoder.coder;

            var lastInstruction = coder.instructions.LastOrDefault();
            if (lastInstruction.IsCallOrNew())
            {
                var lastType = (lastInstruction.Operand as MethodReference)?.ReturnType;

                if (lastType != null && lastType.FullName == type.Fullname)
                    return fieldAssignCoder;

                if (lastType != null && lastType.IsPrimitive)
                {
                    var paramResult = new ParamResult
                    {
                        Type = lastType
                    };

                    coder.processor.CastOrBoxValues(type.typeReference, paramResult, type.typeDefinition);
                    coder.instructions.Append(paramResult.Instructions);
                    return fieldAssignCoder;
                }
            }
            // Add parameter, field and variable if required later on

            coder.instructions.Append(coder.processor.Create(OpCodes.Isinst, Builder.Current.Import(type.typeReference)));
            return fieldAssignCoder;
        }

        public static FieldAssignCoder Call(this FieldAssignCoder coder, Method method, params object[] parameters)
        {
            coder.coder.CallInternal(coder.target, method, OpCodes.Call, parameters);
            return coder;
        }

        public static FieldAssignCoder Load(this Coder coder, Field field) => new FieldAssignCoder(coder, field);

        public static FieldAssignCoder Load(this FieldAssignCoder fieldAssignCoder, Field field)
        {
            var coder = fieldAssignCoder.coder;
            // Instance
            coder.instructions.Append(coder.AddParameter(coder.processor, null, fieldAssignCoder.target).Instructions);
            return new FieldAssignCoder(coder, field);
        }

        public static Coder Set(this FieldAssignCoder fieldAssignCoder, Action<Coder> valueToAssignToField)
        {
            var coder = fieldAssignCoder.coder;
            var newCoder = coder.NewCoder();
            valueToAssignToField(newCoder);

            // value to assign
            var inst = coder.AddParameter(coder.processor, fieldAssignCoder.TargetType, newCoder); /* This will take care of casting */
            coder.instructions.Append(inst.Instructions);
            // Instance
            coder.instructions.Append(coder.AddParameter(coder.processor, null, fieldAssignCoder.target).Instructions);
            // Store
            fieldAssignCoder.StoreCall();
            return new Coder(coder.method, coder.instructions);
        }

        public static Coder Set(this FieldAssignCoder fieldAssignCoder, object value)
        {
            var coder = fieldAssignCoder.coder;

            // value to assign
            var inst = coder.AddParameter(coder.processor, fieldAssignCoder.TargetType, value);
            coder.instructions.Append(inst.Instructions);
            // Instance
            coder.instructions.Append(coder.AddParameter(coder.processor, null, fieldAssignCoder.target).Instructions);
            // Store
            fieldAssignCoder.StoreCall();
            return new Coder(coder.method, coder.instructions);
        }
    }
}