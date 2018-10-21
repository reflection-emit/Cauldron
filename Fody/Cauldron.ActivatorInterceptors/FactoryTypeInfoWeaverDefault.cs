using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;

namespace Cauldron.ActivatorInterceptors
{
    internal class FactoryTypeInfoWeaverDefault : FactoryTypeInfoWeaverBase
    {
        internal FactoryTypeInfoWeaverDefault(ComponentAttributeValues componentAttributeValue, BuilderType componentType, Coder componentTypeCtor, (TypeReference childType, bool isSuccessful) childType) :
            base(componentAttributeValue, componentType, componentTypeCtor, childType)
        {
        }
    }
}