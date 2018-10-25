using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;

namespace Cauldron.ActivatorInterceptors
{
    internal class FactoryTypeInfoWeaverDefault : FactoryTypeInfoWeaverBase
    {
        internal FactoryTypeInfoWeaverDefault(ComponentAttributeValues componentAttributeValue, BuilderType componentInfoType, Coder componentTypeCtor, BuilderType componentType, (TypeReference childType, bool isSuccessful) childType) : base(componentAttributeValue, componentInfoType, componentTypeCtor, componentType, childType)
        {
        }
    }
}