using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void CreateComponentCache(Builder builder, BuilderType cauldron)
        {
            var componentAttribute = __ComponentAttribute.Instance;
            var componentConstructorAttribute = __ComponentConstructorAttribute.Type;
            var factory = __Factory.Instance;

            // Before we start let us find all factoryextensions and add a component attribute to them
            var factoryResolverInterface = __IFactoryResolver.Type;
            this.AddComponentAttribute(builder, builder.FindTypesByInterface(factoryResolverInterface), x => factoryResolverInterface.Fullname);
            // Also the same to all types that inherits from Factory<>
            var factoryGeneric = __Factory_1.Type;
            this.AddComponentAttribute(builder, builder.FindTypesByBaseClass(factoryGeneric), type =>
            {
                var factoryBase = type.BaseClasses.FirstOrDefault(x => x.Fullname.StartsWith("Cauldron.Activator.Factory"));
                if (factoryBase == null)
                    return type.Fullname;

                return factoryBase.GetGenericArgument(0).Fullname;
            });

            int counter = 0;
            var arrayAvatar = builder.GetType("System.Array").New(x => new
            {
                Length = x.GetMethod("get_Length")
            });
            var extensionAvatar = builder.GetType("Cauldron.ExtensionsReflection").New(x => new
            {
                CreateInstance = x.GetMethod("CreateInstance", 2)
            });
            var factoryCacheInterface = builder.GetType("Cauldron.Activator.IFactoryCache");
            var factoryTypeInfoInterface = builder.GetType("Cauldron.Activator.IFactoryTypeInfo");
            var createInstanceInterfaceMethod = factoryTypeInfoInterface.GetMethod("CreateInstance", 1);

            // Get all Components
            var components = builder.FindTypesByAttribute(componentAttribute.ToBuilderType);
            var componentTypes = new List<BuilderType>();

            // Create types with the components properties
            foreach (var component in components)
            {
                this.Log("Hardcoding component factory .ctor: " + component.Type.Fullname);

                var componentType = builder.CreateType("", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, $"<>f__IFactoryTypeInfo_{component.Type.Name}_{counter++}");
                var componentAttributeField = componentType.CreateField(Modifiers.Private, componentAttribute.ToBuilderType, "componentAttribute");
                componentType.AddInterface(factoryTypeInfoInterface);
                componentTypes.Add(componentType);

                // Create ctor
                componentType
                   .CreateConstructor()
                   .NewCode()
                   .Context(x =>
                   {
                       x.Load(Crumb.This).Call(builder.GetType(typeof(object)).Import().ParameterlessContructor.Import());
                       x.Assign(componentAttributeField).NewObj(component);
                   })
                   .Return()
                   .Replace();

                // Implement the methods
                componentType.CreateMethod(Modifiers.Public | Modifiers.Overrrides, createInstanceInterfaceMethod.ReturnType, createInstanceInterfaceMethod.Name, createInstanceInterfaceMethod.Parameters)
                    .NewCode()
                    .Context(x =>
                    {
                        var localVariable = x.GetReturnVariable();
                        // Find any method with a componentcontructor attribute
                        var ctors = component.Type.GetMethods(y =>
                        {
                            if (y.Name == ".cctor")
                                return true;

                            if (!y.Resolve().IsPublic && !y.Resolve().IsAssembly)
                                return false;

                            if (y.Name == ".ctor" && y.DeclaringType.FullName.GetHashCode() != component.Type.Fullname.GetHashCode() && y.DeclaringType.FullName != component.Type.Fullname)
                                return false;

                            if (y.Name.StartsWith("set_"))
                                return false;

                            return true;
                        })
                                .Where(y => y.CustomAttributes.HasAttribute(componentConstructorAttribute))
                                .Concat(
                                    component.Type.GetAllProperties().Where(y => y.CustomAttributes.HasAttribute(componentConstructorAttribute))
                                        .Select(y =>
                                        {
                                            y.CustomAttributes.Remove(componentConstructorAttribute);
                                            y.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

                                            return y.Getter;
                                        })
                                    )
                                .OrderBy(y => y.Parameters.Length)
                                .ToArray();

                        if (ctors.Length > 0)
                        {
                            bool parameterlessCtorAlreadyHandled = false;

                            for (int index = 0; index < ctors.Length; index++)
                            {
                                this.Log("- " + ctors[index].Fullname);

                                var ctor = ctors[index];
                                // add a EditorBrowsable attribute
                                ctor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                                var ctorParameters = ctor.Parameters;

                                if (ctorParameters.Length > 0)
                                {
                                    // In this case we have to find a parameterless constructor first
                                    if (component.Type.ParameterlessContructor != null && !parameterlessCtorAlreadyHandled && component.Type.ParameterlessContructor.IsPublicOrInternal)
                                    {
                                        x.Load(Crumb.GetParameter(0)).IsNull().Then(y => y.NewObj(component.Type.ParameterlessContructor).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());
                                        x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.NewObj(component.Type.ParameterlessContructor).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());
                                        parameterlessCtorAlreadyHandled = true;
                                    }

                                    var code = x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(ctorParameters.Length);

                                    for (int i = 0; i < ctorParameters.Length; i++)
                                        code.AndAnd.Load(Crumb.GetParameter(0).UnPacked(i)).Is(ctorParameters[i]);

                                    if (ctor.Name == ".ctor")
                                        code.Then(y => y.NewObj(ctor, Crumb.GetParameter(0).UnPacked()).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());
                                    else
                                        code.Then(y => y.Call(ctor, Crumb.GetParameter(0).UnPacked()).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());
                                }
                                else
                                {
                                    if (ctor.Name == ".ctor")
                                    {
                                        x.Load(Crumb.GetParameter(0)).IsNull().Then(y => y.NewObj(ctor).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());
                                        x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.NewObj(ctor).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());
                                    }
                                    else
                                    {
                                        x.Load(Crumb.GetParameter(0)).IsNull().Then(y => y.Call(ctor).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());
                                        x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.Call(ctor).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());
                                    }

                                    parameterlessCtorAlreadyHandled = true;
                                }
                            }
                        }
                        else
                        {
                            // In case we don't have constructor with ComponentConstructor Attribute,
                            // then we should look for a parameterless Ctor
                            if (component.Type.ParameterlessContructor == null)
                                this.Log(LogTypes.Error, component.Type, $"The component '{component.Type.Fullname}' has no ComponentConstructor attribute or the constructor is not public");
                            else if (component.Type.ParameterlessContructor.IsPublicOrInternal)
                            {
                                x.Load(Crumb.GetParameter(0)).IsNull().Then(y => y.NewObj(component.Type.ParameterlessContructor).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());
                                x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.NewObj(component.Type.ParameterlessContructor).Dup().Call(factory.OnObjectCreation, Crumb.This).Return());

                                this.Log($"The component '{component.Type.Fullname}' has no ComponentConstructor attribute. A parameterless ctor was found and will be used.");
                            }
                        }
                    })
                    .Context(x => x.Call(extensionAvatar.CreateInstance, component.Type, Crumb.GetParameter(0)).Dup().Call(factory.OnObjectCreation, Crumb.This).Return())
                    .Return()
                    .Replace();

                // Implement the properties
                foreach (var property in factoryTypeInfoInterface.Properties)
                {
                    var propertyResult = componentType.CreateProperty(Modifiers.Public | Modifiers.Overrrides, property.ReturnType, property.Name, true);
                    propertyResult.BackingField.Remove();

                    switch (property.Name)
                    {
                        case "ContractName":
                            propertyResult.Getter.NewCode().Call(componentAttributeField, componentAttribute.ContractName).Return().Replace();
                            break;

                        case "CreationPolicy":
                            propertyResult.Getter.NewCode().Call(componentAttributeField, componentAttribute.Policy).Return().Replace();
                            break;

                        case "Priority":
                            propertyResult.Getter.NewCode().Call(componentAttributeField, componentAttribute.Priority).Return().Replace();
                            break;

                        case "Type":
                            propertyResult.Getter.NewCode().Load(component.Type).Return().Replace();
                            break;
                    }
                }

                componentType.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                // Also remove the component attribute
                component.Attribute.Remove();
            }

            this.Log("Adding component IFactoryCache Interface");
            cauldron.AddInterface(factoryCacheInterface);
            var factoryCacheInterfaceAvatar = factoryCacheInterface.New(x => new
            {
                Components = x.GetMethod("GetComponents")
            });
            var ctorCoder = cauldron.ParameterlessContructor.NewCode();
            cauldron.CreateMethod(Modifiers.Public | Modifiers.Overrrides, factoryCacheInterfaceAvatar.Components.ReturnType, factoryCacheInterfaceAvatar.Components.Name)
                .NewCode()
                .Context(x =>
                {
                    var resultValue = x.GetReturnVariable();
                    x.Newarr(factoryTypeInfoInterface, componentTypes.Count).StoreLocal(resultValue);

                    for (int i = 0; i < componentTypes.Count; i++)
                    {
                        var field = cauldron.CreateField(Modifiers.Private, factoryTypeInfoInterface, "<FactoryType>f__" + i);
                        x.Load(resultValue);
                        x.StoreElement(factoryTypeInfoInterface, field, i);
                        // x.StoreElement(factoryTypeInfoInterface,
                        // x.NewCode().NewObj(componentTypes[i].ParameterlessContructor), i);
                        ctorCoder.Assign(field).NewObj(componentTypes[i].ParameterlessContructor);
                    }
                })
                .Return()
                .Replace();

            ctorCoder.Insert(InsertionPosition.End);
        }

        private void CreateFactoryCache(Builder builder)
        {
            this.Log($"Creating Cauldron Cache");

            var cauldron = builder.CreateType("", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "<Cauldron>");
            cauldron.CreateConstructor().NewCode()
                .Context(x => x.Call(Crumb.This, builder.GetType(typeof(object)).Import().ParameterlessContructor.Import()))
                .Return()
                .Replace();

            if (this.IsActivatorReferenced && this.IsXAML)
                AddAttributeToXAMLResources(builder);

            if (builder.TypeExists("Cauldron.Activator.Factory"))
                CreateComponentCache(builder, cauldron);

            this.ExecuteModuleAddition(builder);
            this.AddEntranceAssemblyHACK(builder);
        }
    }
}