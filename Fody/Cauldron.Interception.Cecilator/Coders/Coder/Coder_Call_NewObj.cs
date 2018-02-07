using Cauldron.Interception.Cecilator.Extensions;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    //public partial class Coder : ICallMethod<CallCoder>
    //{
    //    public CallCoder Call(Method method)
    //    {
    //        this.instructions.Append(InstructionBlock.Call(this, CodeBlocks.This, method, new object[0]));
    //        return new CallCoder(this);
    //    }

    //    public CallCoder Call(Method method, params object[] parameters)
    //    {
    //        this.instructions.Append(InstructionBlock.Call(this, CodeBlocks.This, method, parameters));
    //        return this;
    //    }

    //    public CallCoder Call(Method method, params Func<Coder, object>[] parameters)
    //    {
    //        if (parameters == null)
    //            throw new ArgumentNullException(nameof(parameters));

    //        var @params = parameters
    //            .Select(x => x(this.NewCoder()));

    //        this.instructions.Append(InstructionBlock.Call(this, CodeBlocks.This, method, @params.ToArray()));
    //        return this;
    //    }

    //    public CallCoder NewObj(Method method, params object[] parameters)
    //    {
    //        this.instructions.Append(InstructionBlock.NewObj(this, null, method, parameters));
    //        return this;
    //    }
    //}
}