using Cauldron.Interception.Cecilator;
using System.Linq;

internal static class Extensions
{
    public static Modifiers GetPrivate(this Modifiers value) => value.HasFlag(Modifiers.Static) ? Modifiers.PrivateStatic : Modifiers.Private;
}