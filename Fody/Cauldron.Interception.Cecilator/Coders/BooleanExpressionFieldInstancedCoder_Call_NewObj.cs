namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionFieldInstancedCoder
    {
        /// <summary>
        /// Calls a instanced or static <see cref="Method"/> that exists in the loaded field.
        /// </summary>
        /// <param name="coder"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public new BooleanExpressionCallCoder Call(Method method, params object[] parameters)
            => new BooleanExpressionCallCoder(this.coder, this.jumpTarget, this.target, method, parameters);
    }
}