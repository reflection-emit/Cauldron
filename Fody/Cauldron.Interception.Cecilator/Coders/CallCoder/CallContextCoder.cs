namespace Cauldron.Interception.Cecilator.Coders
{
    public class CallContextCoder : ContextCoder
    {
        internal readonly Method methodToCall;
        internal readonly object[] parameters;

        internal CallContextCoder(Coder coder, Method methodToCall, object[] parameters) : base(coder, false)
        {
            this.methodToCall = methodToCall;
            this.parameters = parameters;
        }

        public CallContextCoder As(BuilderType type)
        {
            this.coder.instructions.Append(InstructionBlock.Call(this.coder, null, this.methodToCall, this.parameters));
            InstructionBlock.CastOrBoxValues(this.coder, type);
            return this;
        }

        public CallContextCoder Call(Method method, params object[] parameters)
        {
            this.coder.instructions.Append(InstructionBlock.Call(this.coder, null, this.methodToCall, this.parameters));
            return new CallContextCoder(this.coder, method, parameters);
        }

        public FieldAssignCoder Load(Field field)
        {
            this.coder.instructions.Append(InstructionBlock.Call(this.coder, null, this.methodToCall, this.parameters));
            return new FieldAssignCoder(this.coder, field);
        }

        public Coder Return()
        {
            this.coder.instructions.Append(InstructionBlock.Call(this.coder, null, this.methodToCall, this.parameters));
            return coder.Return();
        }
    }
}