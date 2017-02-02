using Mono.Cecil;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    internal static class Extensions
    {
        public static bool Implements(this TypeDefinition typeDefinition, string interfaceName)
        {
            while (typeDefinition != null)
            {
                if (typeDefinition.Interfaces != null && typeDefinition.Interfaces.Any(x => x.Name == interfaceName))
                    return true;

                typeDefinition = typeDefinition.BaseType?.Resolve();
            }

            return false;
        }

        public static bool IsAttribute(this TypeDefinition typeDefinition)
        {
            typeDefinition = typeDefinition.BaseType?.Resolve();

            while (typeDefinition != null)
            {
                if (typeDefinition.FullName == "System.Attribute")
                    return true;

                typeDefinition = typeDefinition.BaseType?.Resolve();
            }

            return false;
        }
    }
}