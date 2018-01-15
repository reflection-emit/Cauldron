using Cauldron.Interception.Cecilator;
using System.Linq;

namespace Cauldron.Interception.Fody.Extensions
{
    internal static class BuilderExtensions
    {
        public static Modifiers GetPrivate(this Modifiers value) => value.HasFlag(Modifiers.Static) ? Modifiers.PrivateStatic : Modifiers.Private;
    }
}