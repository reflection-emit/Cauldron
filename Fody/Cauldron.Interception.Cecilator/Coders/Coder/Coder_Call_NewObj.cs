using Cauldron.Interception.Cecilator.Extensions;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class Coder : ICallMethod<Coder>
    {
        public Coder Call(Method method)
        {
            this.instructions.Append(InstructionBlock.Call(this.instructions, CodeBlocks.This, method, new object[0]));
            return this;
        }

        public Coder Call(Method method, params object[] parameters)
        {
            this.instructions.Append(InstructionBlock.Call(this.instructions, CodeBlocks.This, method, parameters));
            return this;
        }

        public Coder Call(Method method, params Func<Coder, object>[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var @params = parameters
                .Select(x => x(this.NewCoder()));

            this.instructions.Append(InstructionBlock.Call(this.instructions, CodeBlocks.This, method, @params.ToArray()));
            return this;
        }

        public Coder NewObj(Method method, params object[] parameters)
        {
            this.instructions.Append(InstructionBlock.NewObj(this.instructions, null, method, parameters));
            return this;
        }
    }
}