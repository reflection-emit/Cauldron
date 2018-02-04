using Cauldron.Interception.Cecilator.Extensions;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public class LocalVariableContextCoder : ContextCoder, ICallMethod<CallContextCoder>
    {
        internal readonly LocalVariable target;

        internal LocalVariableContextCoder(Coder coder, LocalVariable variable) : base(coder, true) => this.target = variable;

        internal LocalVariableContextCoder(Coder coder, bool autoAddThisInstance, LocalVariable variable) : base(coder, autoAddThisInstance) => this.target = variable;

        public CallContextCoder Call(Method method) => this.Call(method, new object[0]);

        public CallContextCoder Call(Method method, params object[] parameters)
        {
            if (this.target != null)
                this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, null, this.target));

            return new CallContextCoder(this.coder, method, parameters);
        }

        public CallContextCoder Call(Method method, params Func<Coder, object>[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var @params = parameters
                .Select(x => x(this.coder.NewCoder()));

            return this.Call(method, parameters);
        }

        public FieldAssignCoder Load(Field field)
        {
            if (this.target != null)
                this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, null, this.target));

            return new FieldAssignCoder(coder, false, field);
        }

        public Coder Return()
        {
            if (this.target != null && this.coder.instructions.associatedMethod.ReturnType != BuilderType.Void)
                this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, this.coder.instructions.associatedMethod.ReturnType, this.target));

            return coder.Return();
        }
    }
}