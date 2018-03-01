using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class BooleanExpressionCoder :
        BooleanExpressionCoderBase<BooleanExpressionCoder, BooleanExpressionCoder>,
        ICallMethod<BooleanExpressionCallCoder>,
        IFieldOperations<BooleanExpressionFieldCoder>,
        IVariableOperations<BooleanExpressionVariableCoder>,
        IArgOperationsExtended<BooleanExpressionArgCoder>,
        ICasting<BooleanExpressionCoder>,
        INewObj<BooleanExpressionCallCoder>
    {
        internal BooleanExpressionCoder(InstructionBlock instructionBlock, (Instruction beginning, Instruction ending)? jumpTargets) : base(instructionBlock, jumpTargets)
        {
        }

        internal BooleanExpressionCoder(InstructionBlock instructionBlock) : base(instructionBlock)
        {
        }

        public override BooleanExpressionCoder End => new BooleanExpressionCoder(this);

        public static implicit operator InstructionBlock(BooleanExpressionCoder coder) => coder.instructions;

        #region Call Methods

        public BooleanExpressionCallCoder Call(Method method)
        {
            if (method.ReturnType == BuilderType.Void)
                throw new InvalidOperationException("Void method are not supported by this call.");

            this.InternalCall(CodeBlocks.This, method);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        public BooleanExpressionCallCoder Call(Method method, params Func<Coder, object>[] parameters)
        {
            if (method.ReturnType == BuilderType.Void)
                throw new InvalidOperationException("Void method are not supported by this call.");

            this.InternalCall(CodeBlocks.This, method, this.CreateParameters(parameters));
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        /// <summary>
        /// Calls a instanced or static <see cref="Method"/> that exists in the declaring type.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public BooleanExpressionCallCoder Call(Method method, params object[] parameters)
        {
            if (method.ReturnType == BuilderType.Void)
                throw new InvalidOperationException("Void method are not supported by this call.");

            this.InternalCall(CodeBlocks.This, method, parameters);
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        #endregion Call Methods

        #region NewObj Methods

        public BooleanExpressionCallCoder NewObj(Method method) => this.NewObj(method, new object[0]);

        public BooleanExpressionCallCoder NewObj(Method method, params object[] parameters)
        {
            this.instructions.Append(InstructionBlock.NewObj(this.instructions, method, parameters));
            return new BooleanExpressionCallCoder(this, method.ReturnType);
        }

        public BooleanExpressionCallCoder NewObj(AttributedMethod attributedMethod)
        {
            this.NewObj(attributedMethod.customAttribute);
            return new BooleanExpressionCallCoder(this, attributedMethod.Attribute.Type);
        }

        public BooleanExpressionCallCoder NewObj(AttributedProperty attributedProperty)
        {
            this.NewObj(attributedProperty.customAttribute);
            return new BooleanExpressionCallCoder(this, attributedProperty.Attribute.Type);
        }

        public BooleanExpressionCallCoder NewObj(Method method, params Func<Coder, object>[] parameters) => this.NewObj(method, this.CreateParameters(parameters));

        #endregion NewObj Methods

        #region Arg Operations

        public BooleanExpressionArgCoder Load(ParametersCodeBlock arg)
        {
            if (arg.IsAllParameters)
                throw new NotSupportedException("This kind of parameter is not supported by Load.");

            var argInfo = arg.GetTargetType(this.instructions.associatedMethod);
            this.instructions.Append(InstructionBlock.CreateCode(this, null, arg));
            return new BooleanExpressionArgCoder(this, argInfo.Item1);
        }

        public Coder SetValue(ParametersCodeBlock arg, object value)
        {
            if (arg.IsAllParameters)
                throw new NotSupportedException("Setting value to all parameters at once is not supported");

            var argInfo = arg.GetTargetType(this.instructions.associatedMethod);
            this.instructions.Append(InstructionBlock.CreateCode(this, argInfo.Item1, value));
            this.instructions.Emit(OpCodes.Starg, argInfo.Item3);
            return new Coder(this);
        }

        public Coder SetValue(ParametersCodeBlock arg, Func<Coder, object> value) => this.SetValue(arg, value(this.NewCoder()));

        #endregion Arg Operations

        #region Field Operations

        public BooleanExpressionFieldCoder Load(Field field)
        {
            InstructionBlock.CreateCodeForFieldReference(this, field.FieldType, field, true);
            return new BooleanExpressionFieldCoder(this, field.FieldType);
        }

        public BooleanExpressionFieldCoder Load(Func<BuilderType, Field> field) => Load(field(this.instructions.associatedMethod.type));

        #endregion Field Operations

        #region Local Variable Operations

        public BooleanExpressionVariableCoder Load(LocalVariable variable)
        {
            InstructionBlock.CreateCodeForVariableDefinition(this, variable.Type, variable);
            return new BooleanExpressionVariableCoder(this, variable.Type);
        }

        public BooleanExpressionVariableCoder Load(Func<Method, LocalVariable> variable) => Load(variable(this.instructions.associatedMethod));

        #endregion Local Variable Operations

        #region Casting Operations

        public BooleanExpressionCoder As(BuilderType type)
        {
            if (this.instructions.associatedMethod.IsStatic)
                throw new NotSupportedException("This is not supported in static methods.");

            this.instructions.Emit(OpCodes.Ldarg_0);
            InstructionBlock.CastOrBoxValues(this, type);
            return new BooleanExpressionCoder(this, this.jumpTargets);
        }

        #endregion Casting Operations

        public BooleanExpressionCoder And<T>(T[] collection, Func<BooleanExpressionCoder, T, int, object> code)
        {
            if (collection == null || collection.Length == 0)
                return this;

            code(this, collection[0], 0);
            InstructionBlock.CastOrBoxValues(this.instructions, BuilderType.Boolean);

            for (int i = 1; i < collection.Length; i++)
            {
                code(this, collection[i], i);
                InstructionBlock.CastOrBoxValues(this.instructions, BuilderType.Boolean);
                this.instructions.Emit(OpCodes.And);
            }

            return this;
        }

        public BooleanExpressionCoder Or<T>(T[] collection, Func<BooleanExpressionCoder, T, int, object> code)
        {
            if (collection == null || collection.Length == 0)
                return this;

            code(this, collection[0], 0);
            InstructionBlock.CastOrBoxValues(this.instructions, BuilderType.Boolean);

            for (int i = 1; i < collection.Length; i++)
            {
                code(this, collection[i], i);
                InstructionBlock.CastOrBoxValues(this.instructions, BuilderType.Boolean);
                this.instructions.Emit(OpCodes.Or);
            }

            return this;
        }
    }
}