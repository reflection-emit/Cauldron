using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;

namespace Cauldron.ActivatorInterceptors
{
    internal class FactoryTypeInfoWeaverGeneric : FactoryTypeInfoWeaverBase
    {
        private BuilderType factoryInfoType;

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

        protected override void OnInitialize()
        {
            var factoryType = FactoryTypeInfoWeaver.Create("", this.componentType, this.componentAttributeValue, @params =>
            {
                return new FactoryTypeInfoWeaverDefault(this.componentAttributeValue, @params.componentInfoType, @params.componentInfoType.CreateConstructor().NewCoder(), this.componentType, @params.childType);
            });

            this.factoryInfoType = factoryType.componentInfoType;

            foreach (var item in this.componentType.GenericParameters)
                this.factoryInfoType.GenericParameters.Add(item.Clone((TypeDefinition)this.componentInfoType));
        }

        protected override void OnTypeSet(Property propertyResult)
        {
            propertyResult.BackingField.Remove();
            propertyResult.Getter.NewCoder().Load(this.factoryInfoType).Return().Replace();
        }
    }
}