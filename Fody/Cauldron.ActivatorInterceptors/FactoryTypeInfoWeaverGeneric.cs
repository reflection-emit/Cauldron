using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;

namespace Cauldron.ActivatorInterceptors
{
    internal class FactoryTypeInfoWeaverGeneric : FactoryTypeInfoWeaverBase
    {
        internal FactoryTypeInfoWeaverGeneric(ComponentAttributeValues componentAttributeValue, BuilderType componentInfoType, Coder componentTypeCtor, BuilderType componentType, (TypeReference childType, bool isSuccessful) childType) : base(componentAttributeValue, componentInfoType, componentTypeCtor, componentType, childType)
        {
        }

        protected override Coder AddCreateInstanceMethod(Method createInstanceInterfaceMethod)
        {
            return this.componentInfoType.CreateMethod(Modifiers.Public | Modifiers.Overrides, createInstanceInterfaceMethod.ReturnType, createInstanceInterfaceMethod.Name, createInstanceInterfaceMethod.Parameters)
                 .NewCoder()
                 .Load(value: null)
                 .Return();
        }
    }
}