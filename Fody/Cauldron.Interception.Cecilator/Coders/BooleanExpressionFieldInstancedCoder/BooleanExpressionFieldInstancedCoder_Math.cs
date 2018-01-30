using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionFieldInstancedCoder
    {
        public new BooleanExpressionFieldInstancedCoder Negate()
        {
            this.negate = true;
            return this;
        }
    }
}