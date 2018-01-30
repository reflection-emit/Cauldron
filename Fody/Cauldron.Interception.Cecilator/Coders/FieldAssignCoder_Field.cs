using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class FieldAssignCoder
    {
        public FieldAssignCoder As(BuilderType type)
        {
            this.castToType = type;
            var coder = this.coder;
            // Instance
            if (!this.target.IsStatic)
                coder.instructions.Append(coder.processor.Create(OpCodes.Ldarg_0));

            coder.instructions.Append(coder.processor.Create(OpCodes.Ldfld, this.target.fieldRef));
            return this;
        }

        public FieldAssignCoder Call(Method method, params object[] parameters)
        {
            this.coder.CallInternal(this.target, method, OpCodes.Call, parameters);
            return this;
        }

        public FieldAssignCoder Load(Field field)
        {
            var coder = this.coder;
            // Instance
            coder.instructions.Append(coder.AddParameter(coder.processor, null, this.target).Instructions);
            return new FieldAssignCoder(coder, field);
        }

        public Coder Return()
        {
            var coder = this.coder;
            // Instance
            if (coder.method.ReturnType.typeReference != Builder.Current.TypeSystem.Void)
                coder.instructions.Append(coder.AddParameter(coder.processor, coder.method.ReturnType.typeReference, this.target).Instructions);
            return coder.Return();
        }

        public Coder Set(object value)
        {
            var coder = this.coder;

            // Instance
            if (!this.target.IsStatic)
                coder.instructions.Append(coder.processor.Create(OpCodes.Ldarg_0));
            // value to assign
            var inst = coder.AddParameter(coder.processor, this.TargetType, value);
            coder.instructions.Append(inst.Instructions);
            // Store
            this.StoreCall();
            return coder;
        }

        public Coder Set(Action<Coder> valueToAssignToField)
        {
            var coder = this.coder;
            var newCoder = coder.NewCoder();
            valueToAssignToField(newCoder);

            // Instance
            if (!this.target.IsStatic)
                coder.instructions.Append(coder.processor.Create(OpCodes.Ldarg_0));
            // value to assign
            var inst = coder.AddParameter(coder.processor, this.TargetType, newCoder.ToCodeBlock()); /* This will take care of casting */
            coder.instructions.Append(inst.Instructions);
            // Store
            this.StoreCall();
            return coder;
        }
    }
}