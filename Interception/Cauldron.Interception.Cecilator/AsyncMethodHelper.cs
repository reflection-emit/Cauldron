using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class AsyncMethodHelper : CecilatorBase
    {
        private readonly Method method;

        internal AsyncMethodHelper(Method method) : base(method)
        {
            this.method = method;
        }

        public Position GetAsyncTaskMethodBuilderInitialization()
        {
            var result = this.method.methodDefinition.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Newobj && (x.Operand as MethodDefinition ?? x.Operand as MethodReference).Name == ".ctor");

            if (result == null)
                return new Position(this.method, this.method.methodDefinition.Body.Instructions[0]);

            return new Position(this.method, result.Next);
        }
    }
}