using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class FieldExtensions
    {
        public static FieldAssignCoder As(this FieldAssignCoder fieldAssignCoder, BuilderType type)
        {
            fieldAssignCoder.castToType = type;
            var coder = fieldAssignCoder.coder;
            // Instance
            if (!fieldAssignCoder.target.IsStatic)
                coder.instructions.Append(coder.processor.Create(OpCodes.Ldarg_0));

            coder.instructions.Append(coder.processor.Create(OpCodes.Ldfld, fieldAssignCoder.target.fieldRef));
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

        public static Coder Return(this FieldAssignCoder fieldAssignCoder)
        {
            var coder = fieldAssignCoder.coder;
            // Instance
            if (coder.method.ReturnType.typeReference != Builder.Current.TypeSystem.Void)
                coder.instructions.Append(coder.AddParameter(coder.processor, coder.method.ReturnType.typeReference, fieldAssignCoder.target).Instructions);
            return coder.Return();
        }

        public static Coder Set(this FieldAssignCoder fieldAssignCoder, Action<Coder> valueToAssignToField)
        {
            var coder = fieldAssignCoder.coder;
            var newCoder = coder.NewCoder();
            valueToAssignToField(newCoder);

            // Instance
            if (!fieldAssignCoder.target.IsStatic)
                coder.instructions.Append(coder.processor.Create(OpCodes.Ldarg_0));
            // value to assign
            var inst = coder.AddParameter(coder.processor, fieldAssignCoder.TargetType, newCoder); /* This will take care of casting */
            coder.instructions.Append(inst.Instructions);
            // Store
            fieldAssignCoder.StoreCall();
            return coder;
        }

        public static Coder Set(this FieldAssignCoder fieldAssignCoder, object value)
        {
            var coder = fieldAssignCoder.coder;

            // Instance
            if (!fieldAssignCoder.target.IsStatic)
                coder.instructions.Append(coder.processor.Create(OpCodes.Ldarg_0));
            // value to assign
            var inst = coder.AddParameter(coder.processor, fieldAssignCoder.TargetType, value);
            coder.instructions.Append(inst.Instructions);
            // Store
            fieldAssignCoder.StoreCall();
            return coder;
        }
    }
}