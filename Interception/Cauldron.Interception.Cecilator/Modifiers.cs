using System;

namespace Cauldron.Interception.Cecilator
{
    [Flags]
    public enum Modifiers
    {
        Public = 1,
        Private = 2,
        Internal = 4,
        Static = 8,
        Overrrides = 16,
        PublicStatic = Public | Static,
        PrivateStatic = Private | Static,
        InternalStatic = Internal | Static
    }
}