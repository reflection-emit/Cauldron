using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;

namespace Cauldron.ActivatorInterceptors
{
    internal class FactoryTypeInfoWeaverGeneric : FactoryTypeInfoWeaverBase
    {
        internal FactoryTypeInfoWeaverGeneric(ComponentAttributeValues componentAttributeValue, BuilderType componentInfoType, Coder componentTypeCtor, BuilderType componentType, (TypeReference childType, bool isSuccessful) childType) : base(componentAttributeValue, componentInfoType, componentTypeCtor, componentType, childType)
        {
            var factoryType = FactoryTypeInfoWeaver.Create("", this.componentType, this.componentAttributeValue, @params =>
            {
                return new FactoryTypeInfoWeaverDefault(componentAttributeValue, @params.componentInfoType, @params.componentInfoType.CreateConstructor().NewCoder(), componentType, @params.childType);
            });

            foreach (var item in componentType.GenericParameters)
                factoryType.componentInfoType.GenericParameters.Add(item.Clone((TypeDefinition)componentInfoType));
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