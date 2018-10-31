using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;
using System;
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
            var result = Create(component.Type, new ComponentAttributeValues(component));
            component.Attribute.Remove();
            return result;
        }

        public static FactoryTypeInfoWeaverBase Create(BuilderType componentType, ComponentAttributeValues componentAttributeValue) =>
            Create("", componentType, componentAttributeValue, @params =>
            {
                if (@params.componentType.HasGenericParameters)
                    return new FactoryTypeInfoWeaverGeneric(componentAttributeValue, @params.componentInfoType, @params.componentInfoType.CreateConstructor().NewCoder(), componentType, @params.childType);
                else
                    return new FactoryTypeInfoWeaverDefault(componentAttributeValue, @params.componentInfoType, @params.componentInfoType.CreateConstructor().NewCoder(), componentType, @params.childType);
            });

        public static FactoryTypeInfoWeaverBase Create(
            string @namespace,
            BuilderType componentType,
            ComponentAttributeValues componentAttributeValue,
            Func<(ComponentAttributeValues componentAttributeValue, BuilderType componentInfoType, BuilderType componentType, (TypeReference childType, bool isSuccessful) childType), FactoryTypeInfoWeaverBase> typeInfoWeaver)
        {
            var builder = Builder.Current;

            builder.Log(LogTypes.Info, "Hardcoding component factory .ctor: " + componentType.Fullname);

            /*
                Check for IDisposable
            */
            if (componentAttributeValue.Policy == 1 && componentType.Implements(BuilderTypes.IDisposable) && !componentType.Implements(BuilderTypes.IDisposableObject))
                builder.Log(LogTypes.Error, componentType, FactoryTypeInfoWeaverBase.NoIDisposableObjectExceptionText);

            var componentInfoType = builder.CreateType(@namespace, TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "<>f__IFactoryTypeInfo_" + componentType.Name.GetValidName() + "_" + counter++);
            componentInfoType.AddInterface(BuilderTypes.IFactoryTypeInfo);
            componentInfoType.CustomAttributes.AddDebuggerDisplayAttribute(componentType.Name + " ({ContractName})");

            var childType = Builder.Current.GetChildrenType((TypeReference)componentType);
            var result = typeInfoWeaver(( componentAttributeValue, componentInfoType, componentType, childType ));

            //ImplementProperties(result);

            result.componentTypeCtor.Return().Replace();
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