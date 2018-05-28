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
        Protected = 32,
        Overrides = 16,
        PublicStatic = Public | Static,
        ProtectedStatic = Protected | Static,
        PrivateStatic = Private | Static,
        InternalStatic = Internal | Static,
        Explicit = 64,
        Constant = 128,
        All = Public | Private | Internal | Static | Protected | Overrides | Explicit
    }
}