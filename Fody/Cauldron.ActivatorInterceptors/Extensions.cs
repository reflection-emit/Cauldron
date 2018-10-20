using Cauldron.Interception.Cecilator;
using System.Collections;
using static ComponentCacheWeaver;

namespace Cauldron.ActivatorInterceptors
{
    internal static class Extensions
    {
        public static bool IsIDictionary(this BuilderType builderType) => builderType.Implements(typeof(IDictionary));

        public static bool IsIEnumerable(this BuilderType builderType) =>
                    (typeof(IEnumerable)
                        .ToBuilderType()
                            .AreReferenceAssignable(builderType) || builderType.IsArray);

        public static bool IsParameterless(this InjectAttributeValues injectAttribute) =>
            (injectAttribute.Arguments == null || injectAttribute.Arguments.Length == 0);
    }
}