using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCoder
    {
        /// <summary>
        /// Calls a instanced or static <see cref="Method"/> that exists in the declaring type.
        /// </summary>
        /// <param name="coder"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public BooleanExpressionCallCoder Call(Method method, params object[] parameters)
        {
            if (method.ReturnType.typeDefinition == Builder.Current.TypeSystem.Void)
                throw new InvalidOperationException("Void method are not supported by this call.");

            return new BooleanExpressionCallCoder(this.coder, this.jumpTarget, CodeBlocks.This, method, parameters);
        }
    }
}