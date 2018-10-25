using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;
using System.ComponentModel;

namespace Cauldron.ActivatorInterceptors
{
    internal static class FactoryTypeInfoWeaver
    {
        public const string UnknownConstructor = "There is no defined constructor that matches the passed parameters for component ";
        public static readonly BuilderType cauldronInterceptionHelper;
        public static readonly Field unknownConstructorText;

        /// <summary>
        /// Used to make implementations unique even with the same name
        /// </summary>
        private static volatile int counter = 0;

        static FactoryTypeInfoWeaver()
        {
            cauldronInterceptionHelper = Builder.Current.GetType("CauldronInterceptionHelper", SearchContext.Module);

            unknownConstructorText = cauldronInterceptionHelper.CreateField(Modifiers.PublicStatic, (BuilderType)BuilderTypes.String, "UnknownConstructorText");
            unknownConstructorText.CustomAttributes.AddCompilerGeneratedAttribute();
            unknownConstructorText.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

            cauldronInterceptionHelper
                .CreateStaticConstructor()
                .NewCoder()
                .SetValue(unknownConstructorText, UnknownConstructor)
                .Insert(InsertionPosition.Beginning);
        }

        public static FactoryTypeInfoWeaverBase Create(AttributedType component)
        {
            var builder = Builder.Current;

            builder.Log(LogTypes.Info, "Hardcoding component factory .ctor: " + component.Type.Fullname);

            var componentAttributeValue = new ComponentAttributeValues(component);
            var childType = Builder.Current.GetChildrenType((TypeReference)component.Type);

            /*
                Check for IDisposable
            */
            if (componentAttributeValue.Policy == 1 && component.Type.Implements(BuilderTypes.IDisposable) && !component.Type.Implements(BuilderTypes.IDisposableObject))
                builder.Log(LogTypes.Error, component.Type, FactoryTypeInfoWeaverBase.NoIDisposableObjectExceptionText);

            var componentInfoType = builder.CreateType("", TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "<>f__IFactoryTypeInfo_" + component.Type.Name.GetValidName() + "_" + counter++);
            componentInfoType.AddInterface(BuilderTypes.IFactoryTypeInfo);
            componentInfoType.CustomAttributes.AddDebuggerDisplayAttribute(component.Type.Name + " ({ContractName})");

            if (component.Type.HasGenericArguments)
                componentInfoType.ToGenericInstance(component.Type.GenericArguments);

            FactoryTypeInfoWeaverBase result;

            if (component.Type.IsGenericType)
                result = new FactoryTypeInfoWeaverGeneric(componentAttributeValue, componentInfoType, componentInfoType.CreateConstructor().NewCoder(), component.Type, childType);
            else
                result = new FactoryTypeInfoWeaverDefault(componentAttributeValue, componentInfoType, componentInfoType.CreateConstructor().NewCoder(), component.Type, childType);

            ImplementProperties(result);

            result.componentTypeCtor.Return().Replace();
            // Also remove the component attribute
            component.Attribute.Remove();

            return result;
        }

        private static void ImplementProperties(FactoryTypeInfoWeaverBase factoryTypeInfoWeaver)
        {
            foreach (var property in BuilderTypes.IFactoryTypeInfo.BuilderType.Properties)
            {
                var propertyResult = factoryTypeInfoWeaver.componentInfoType.CreateProperty(Modifiers.Public | Modifiers.Overrides, property.ReturnType, property.Name,
                    property.Setter == null ? PropertySetterCreationOption.DontCreateSetter : PropertySetterCreationOption.AlwaysCreate);

                switch (property.Name)
                {
                    case "ContractName":

                        if (string.IsNullOrEmpty(factoryTypeInfoWeaver.componentAttributeValue.ContractName))
                            factoryTypeInfoWeaver.componentTypeCtor.SetValue(propertyResult.BackingField, x =>
                                x.Load(factoryTypeInfoWeaver.componentAttributeValue.ContractType).Call(BuilderTypes.Type.GetMethod_get_FullName()));
                        else
                        {
                            propertyResult.BackingField.Remove();
                            propertyResult.Getter.NewCoder().Load(factoryTypeInfoWeaver.componentAttributeValue.ContractName).Return().Replace();
                        }
                        break;

                    case "ContractType":
                        propertyResult.BackingField.Remove();
                        if (factoryTypeInfoWeaver.componentAttributeValue.ContractType == null)
                            propertyResult.Getter.NewCoder().Load(value: null).Return().Replace();
                        else
                            propertyResult.Getter.NewCoder().Load(factoryTypeInfoWeaver.componentAttributeValue.ContractType).Return().Replace();
                        break;

                    case "CreationPolicy":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(factoryTypeInfoWeaver.componentAttributeValue.Policy).Return().Replace();
                        break;

                    case "Priority":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(factoryTypeInfoWeaver.componentAttributeValue.Priority).Return().Replace();
                        break;

                    case "Type":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(factoryTypeInfoWeaver.componentType).Return().Replace();
                        break;

                    case "IsEnumerable":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(factoryTypeInfoWeaver.isIEnumerable).Return().Replace();
                        break;

                    case "ChildType":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(factoryTypeInfoWeaver.isIEnumerable ? factoryTypeInfoWeaver.childType : null).Return().Replace();
                        break;

                    case "Instance":
                        propertyResult.BackingField.Remove();
                        if (factoryTypeInfoWeaver.componentAttributeValue.Policy == 0)
                        {
                            propertyResult.Getter.NewCoder().Load(value: null).Return().Replace();
                            propertyResult.Setter.NewCoder().Return().Replace();
                        }
                        else
                        {
                            var instanceFieldName = $"<{factoryTypeInfoWeaver.componentType}>_componentInstance";
                            var instanceField = cauldronInterceptionHelper.GetField(instanceFieldName, false) ?? cauldronInterceptionHelper.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceFieldName);
                            propertyResult.Getter.NewCoder().Load(instanceField).Return().Replace();
                            propertyResult.Setter.NewCoder().SetValue(instanceField, CodeBlocks.GetParameter(0)).Return().Replace();
                        }
                        break;
                }
            }
        }
    }
}