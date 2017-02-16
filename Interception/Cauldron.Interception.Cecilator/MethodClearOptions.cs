using System;

namespace Cauldron.Interception.Cecilator
{
    [Flags]
    public enum MethodClearOptions
    {
        Body = 1,
        LocalVariables = 2,
        All = Body | LocalVariables
    }
}