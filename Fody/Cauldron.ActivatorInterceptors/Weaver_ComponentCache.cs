using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

public static class Weaver_ComponentCache
{
    public const string NoIDisposableObjectExceptionText = "An object with creation policy 'Singleton' with an implemented 'IDisposable' must also implement the 'IDisposableObject' interface.";
    public static string Name = "Activator Component Cache (Dependency Injection)";
    public static int Priority = int.MaxValue;

    [Display("Creating Component Cache and Inject Attribute")]
    public static void Implement(Builder builder)
    {
        builder.Log(LogTypes.Info, "Creating Cauldron Cache");

        var cauldron = builder.GetType("CauldronInterceptionHelper", SearchContext.Module);
        var componentAttribute = __ComponentAttribute.Instance;
        var genericComponentAttribute = __GenericComponentAttribute.Instance;
        var factory = __Factory.Instance;
        var noIDisposableObjectExceptionText = cauldron.CreateField(Modifiers.PublicStatic, (BuilderType)BuilderTypes.String, "NoIDisposableObjectExceptionText");
        noIDisposableObjectExceptionText.CustomAttributes.AddCompilerGeneratedAttribute();
        noIDisposableObjectExceptionText.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

        cauldron.CreateStaticConstructor().NewCoder()
            .SetValue(noIDisposableObjectExceptionText, NoIDisposableObjectExceptionText)
            .Insert(InsertionPosition.Beginning);

        // Before we start let us find all factoryextensions and add a component attribute to them
        var factoryResolverInterface = __IFactoryExtension.Type;
        AddComponentAttribute(builder, builder.FindTypesByInterface(factoryResolverInterface), x => factoryResolverInterface);
        // Also the same to all types that inherits from Factory<>
        var factoryGeneric = __Factory_1.Type;
        AddComponentAttribute(builder, builder.FindTypesByBaseClass(factoryGeneric), type =>
        {
            var factoryBase = type.BaseClasses.FirstOrDefault(x => x.Fullname.StartsWith("Cauldron.Activator.Factory"));

            if (factoryBase == null)
                return type;

            return factoryBase.GetGenericArgument(0);
        });

        int counter = 0;

        var extensionAvatar = builder.GetType("Cauldron.ExtensionsReflection").With(x => new
        {
            CreateInstance = x.GetMethod("CreateInstance", 2)
        });

        var factoryTypeInfoInterface = builder.GetType("Cauldron.Activator.IFactoryTypeInfo");
        var createInstanceInterfaceMethod = factoryTypeInfoInterface.GetMethod("CreateInstance", 1);

        // Get all Components
        var componentTypes = new List<BuilderType>();
        var components = builder
            .FindTypesByAttribute(componentAttribute.ToBuilderType)
            .Concat(builder
                .CustomAttributes
                .Where(x => x.Type == genericComponentAttribute.ToBuilderType)
                .Select(x => new
                {
                    Attribute = BuilderCustomAttribute.Create(componentAttribute.ToBuilderType, x.ConstructorArguments.Skip(1)),
                    Type = (x.ConstructorArguments[0].Value as TypeReference).ToBuilderType()
                })
                .Where(x =>
                {
                    if (x.Type.IsAbstract)
                    {
                        builder.Log(LogTypes.Error, $"The GenericComponentAttribute does not support abstract types.");
                        return false;
                    }

                    if (x.Type.IsInterface)
                    {
                        builder.Log(LogTypes.Error, $"The GenericComponentAttribute does not support interfaces.");
                        return false;
                    }

                    if (x.Type.IsValueType)
                    {
                        builder.Log(LogTypes.Error, $"The GenericComponentAttribute does not support value types.");
                        return false;
                    }

                    return true;
                })
                .Select(x => new AttributedType(x.Type, x.Attribute)));

        foreach (var component in components)
        {
            builder.Log(LogTypes.Info, "Hardcoding component factory .ctor: " + component.Type.Fullname);

            var componentAttributeValue = new ComponentAttributeValues(component);

            /*
                Check for IDisposable
            */
            if (componentAttributeValue.Policy == 1 && component.Type.Implements(BuilderTypes.IDisposable) && !component.Type.Implements(BuilderTypes2.IDisposableObject))
                builder.Log(LogTypes.Error, component.Type, NoIDisposableObjectExceptionText);

            var instanceFieldName = $"<{component.Type}>_componentInstance";
            var componentType = builder.CreateType("", TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "<>f__IFactoryTypeInfo_" + component.Type.Name.GetValidName() + "_" + counter++);
            componentType.AddInterface(factoryTypeInfoInterface);
            componentType.CustomAttributes.AddDebuggerDisplayAttribute(component.Type.Name + " ({ContractName})");
            componentTypes.Add(componentType);

            // Create ctor
            var componentTypeCtor = componentType
               .CreateConstructor()
               .NewCoder();

            // Implement the methods
            componentType.CreateMethod(Modifiers.Public | Modifiers.Overrides, createInstanceInterfaceMethod.ReturnType, createInstanceInterfaceMethod.Name, createInstanceInterfaceMethod.Parameters)
                .NewCoder()
                .Context(x =>
                {
                    if (componentAttributeValue.Policy == 1)
                    {
                        var instancedCreator = componentType.CreateMethod(Modifiers.Private, createInstanceInterfaceMethod.ReturnType, $"<{component.Type.Fullname}>__CreateInstance", createInstanceInterfaceMethod.Parameters);
                        instancedCreator.NewCoder().Context(c => AddContext(builder, c, factory, component)).Replace();

                        var instanceField = cauldron.GetField(instanceFieldName, false) ?? cauldron.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceFieldName);
                        x.If(@if => @if.Load(instanceField).IsNotNull(), then => then.Load(instanceField).Return());
                        x.SetValue(instanceField, m => m.Call(instancedCreator, CodeBlocks.GetParameters()));
                        // every singleton that implements the idisposable interface has also to
                        // implement the IDisposableObject interface this is because we want to know if
                        // an instance was disposed (somehow)
                        x.If(@if => @if.Load(instanceField).Is(BuilderTypes.IDisposable), then =>
                        {
                            then.If(ifInner => ifInner.Load(instanceField).IsNot(BuilderTypes2.IDisposableObject),
                                then2 => then2.ThrowNew(BuilderTypes.NotSupportedException.GetConstructor_String(), noIDisposableObjectExceptionText));

                            // Create an event handler method
                            var eventHandlerMethod = componentType.CreateMethod(Modifiers.Private, $"<IDisposableObject>_Handler", BuilderTypes.Object, BuilderTypes.EventArgs);
                            eventHandlerMethod.NewCoder()
                                .If(ehIf => ehIf.Load(CodeBlocks.GetParameter(0)).IsNotNull(), ehThen => ehThen.Load(CodeBlocks.GetParameter(0)).As(BuilderTypes.IDisposable).Call(BuilderTypes.IDisposable.GetMethod_Dispose()))
                                .SetValue(instanceField, null)
                                .Return()
                                .Replace();

                            then.Load(instanceField).As(BuilderTypes2.IDisposableObject).Call(BuilderTypes2.IDisposableObject.GetMethod_add_Disposed(BuilderTypes.EventHandler), o => o.NewObj(BuilderTypes.EventHandler.GetConstructor(), CodeBlocks.This, eventHandlerMethod));

                            return then;
                        });

                        x.Load(instanceField).Return();
                    }
                    else
                        AddContext(builder, x, factory, component);

                    return x;
                })
                .Replace();

            // Implement the properties
            foreach (var property in factoryTypeInfoInterface.Properties)
            {
                var propertyResult = componentType.CreateProperty(Modifiers.Public | Modifiers.Overrides, property.ReturnType, property.Name,
                    property.Setter == null ? PropertySetterCreationOption.DontCreateSetter : PropertySetterCreationOption.AlwaysCreate);

                switch (property.Name)
                {
                    case "ContractName":

                        if (string.IsNullOrEmpty(componentAttributeValue.ContractName))
                            componentTypeCtor.SetValue(propertyResult.BackingField, x => x.Load(componentAttributeValue.ContractType).Call(__Type.Instance.FullName));
                        else
                        {
                            propertyResult.BackingField.Remove();
                            propertyResult.Getter.NewCoder().Load(componentAttributeValue.ContractName).Return().Replace();
                        }
                        break;

                    case "CreationPolicy":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(componentAttributeValue.Policy).Return().Replace();
                        break;

                    case "Priority":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(componentAttributeValue.Priority).Return().Replace();
                        break;

                    case "Type":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(component.Type).Return().Replace();
                        break;

                    case "Instance":
                        propertyResult.BackingField.Remove();
                        if (componentAttributeValue.Policy == 0)
                        {
                            propertyResult.Getter.NewCoder().Load(value: null).Return().Replace();
                            propertyResult.Setter.NewCoder().Return().Replace();
                        }
                        else
                        {
                            var instanceField = cauldron.GetField(instanceFieldName, false) ?? cauldron.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceFieldName);
                            propertyResult.Getter.NewCoder().Load(instanceField).Return().Replace();
                            propertyResult.Setter.NewCoder().SetValue(instanceField, CodeBlocks.GetParameter(0)).Return().Replace();
                        }
                        break;
                }
            }

            componentTypeCtor.Return().Replace();
            // Also remove the component attribute
            component.Attribute.Remove();
        }

        builder.Log(LogTypes.Info, "Adding component IFactoryCache Interface");

        var linqEnumerable = builder.GetType("System.Linq.Enumerable", SearchContext.AllReferencedModules);
        var ifactoryTypeInfo = factoryTypeInfoInterface.MakeArray();
        var getComponentsFromOtherAssemblies = builder.ReferencedAssemblies
            .Select(x => x.MainModule.GetType("CauldronInterceptionHelper"))
            .Where(x => x != null)
            .Select(x => x.ToBuilderType())
            .Select(x => x.GetMethod("GetComponents", false)?.Import())
            .Where(x => x != null)
            .ToArray();

        var getComponentsMethod = cauldron.CreateMethod(Modifiers.Public | Modifiers.Static, ifactoryTypeInfo, "GetComponents");
        var componentsField = cauldron.CreateField(Modifiers.PrivateStatic, ifactoryTypeInfo, "<FactoryType>f__components");
        getComponentsMethod
            .NewCoder()
                .NewCoder()
                .Context(context =>
                {
                    context.If(x => x.Load(componentsField).IsNotNull(), then => then.Load(componentsField).Return());

                    var resultValue = context.GetOrCreateReturnVariable();
                    context.SetValue(resultValue, x => x.Newarr(factoryTypeInfoInterface, componentTypes.Count));

                    for (int i = 0; i < componentTypes.Count; i++)
                    {
                        context
                            .Load(resultValue)
                            .StoreElement(context.NewCoder().NewObj(componentTypes[i].ParameterlessContructor), i);
                    }

                    context.SetValue(componentsField, resultValue);
                    return context.Load(resultValue).Return();
                })
                .Replace();

        var voidMain = builder.GetMain();
        if (voidMain != null)
        {
            voidMain.NewCoder()
                .Context(context =>
                {
                    context.Call(builder.GetType("Cauldron.Activator.FactoryCore").GetMethod("SetComponents", 1, true), x => x.Call(getComponentsMethod));
                    if (getComponentsFromOtherAssemblies.Length > 0)

                        foreach (var item in getComponentsFromOtherAssemblies)
                            context.Call(builder.GetType("Cauldron.Activator.FactoryCore").GetMethod("SetComponents", 1, true), x => x.Call(item));

                    return context;
                })
                .End
                .Insert(InsertionPosition.Beginning);
        }

        var injectAttribute = new BuilderType[] { builder.GetType("Cauldron.Activator.InjectAttribute") };
        var properties = builder.FindPropertiesByAttributes(injectAttribute).ToArray();
        var fields = builder.FindFieldsByAttributes(injectAttribute).ToArray();

        ImplementInjectField(builder, fields);
        ImplementInjectProperties(builder, properties);
    }

    private static void AddComponentAttribute(Builder builder, IEnumerable<BuilderType> builderTypes, Func<BuilderType, BuilderType> contractNameDelegate = null)
    {
        var componentConstructorAttribute = __ComponentConstructorAttribute.Instance;
        var componentAttribute = __ComponentAttribute.Instance.ToBuilderType;

        foreach (var item in builderTypes)
        {
            if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters)
                continue;

            if (item.CustomAttributes.HasAttribute(componentAttribute))
                continue;

            var contractType = contractNameDelegate == null ? item : contractNameDelegate(item);
            item.CustomAttributes.Add(componentAttribute, contractType);

            // Add a component contructor attribute to all .ctors
            foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                ctor.CustomAttributes.Add(componentConstructorAttribute.ToBuilderType);
        }
    }

    private static Coder AddContext(Builder builder, Coder context, __Factory factory, AttributedType component)
    {
        var componentConstructorAttribute = __ComponentConstructorAttribute.Type;
        var componentAttributeValues = new ComponentAttributeValues(component);

        // Find any method with a componentcontructor attribute
        var ctors = component.Type.GetMethods(y =>
        {
            if (y.Name == ".cctor")
                return false;

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

        var arrayAvatar = __Array.Instance;

        if (ctors.Length > 0)
        {
            var parameterlessCtorAlreadyHandled = false;

            for (int index = 0; index < ctors.Length; index++)
            {
                builder.Log(LogTypes.Info, "- " + ctors[index].Fullname);

                var ctor = ctors[index];
                // add a EditorBrowsable attribute
                ctor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                var ctorParameters = ctor.Parameters;

                if (ctorParameters.Length > 0)
                {
                    // In this case we have to find a parameterless constructor first
                    if (component.Type.ParameterlessContructor != null && !parameterlessCtorAlreadyHandled && component.Type.ParameterlessContructor.IsPublicOrInternal)
                    {
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull(), then =>
                        {
                            then.NewObj(component.Type.ParameterlessContructor);

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);

                            return then.Return();
                        });
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).Call(arrayAvatar.Length).Is(0), then =>
                        {
                            then.NewObj(component.Type.ParameterlessContructor);

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);

                            return then.Return();
                        });
                        parameterlessCtorAlreadyHandled = true;
                    }

                    context.If(@if =>
                    {
                        var resultCoder = @if.Load(CodeBlocks.GetParameter(0)).Call(arrayAvatar.Length).Is(ctorParameters.Length);
                        for (int i = 0; i < ctorParameters.Length; i++)
                            resultCoder = resultCoder.AndAnd(x => x.Load(CodeBlocks.GetParameter(0)).ArrayElement(i).Is(ctorParameters[i]));

                        return resultCoder;
                    }, @then =>
                    {
                        if (ctor.Name == ".ctor")
                        {
                            then.NewObj(ctor, CodeBlocks.GetParameter(0).ArrayElements());

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);
                        }
                        else
                        {
                            then.Call(ctor, CodeBlocks.GetParameter(0).ArrayElements());

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);
                        }

                        return then.Return();
                    });
                }
                else
                {
                    if (ctor.Name == ".ctor")
                    {
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull(), then =>
                        {
                            then.NewObj(ctor);

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);
                            return then.Return();
                        });
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).Call(arrayAvatar.Length).Is(0), then =>
                        {
                            then.NewObj(ctor);

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);
                            return then.Return();
                        });
                    }
                    else
                    {
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull(), then =>
                        {
                            then.Call(ctor);

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);
                            return then.Return();
                        });
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).Call(arrayAvatar.Length).Is(0), then =>
                        {
                            then.Call(ctor);

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);
                            return then.Return();
                        });
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
                builder.Log(LogTypes.Error, component.Type, $"The component '{component.Type.Fullname}' has no ComponentConstructor attribute or the constructor is not public");
            else if (component.Type.ParameterlessContructor.IsPublicOrInternal)
            {
                context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull(), then =>
                {
                    then.NewObj(component.Type.ParameterlessContructor);

                    if (componentAttributeValues.InvokeOnObjectCreationEvent)
                        then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);
                    return then.Return();
                });
                context.If(x => x.Load(CodeBlocks.GetParameter(0)).Call(arrayAvatar.Length).Is(0), then =>
                {
                    then.NewObj(component.Type.ParameterlessContructor);

                    if (componentAttributeValues.InvokeOnObjectCreationEvent)
                        then.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);
                    return then.Return();
                });

                builder.Log(LogTypes.Info, $"The component '{component.Type.Fullname}' has no ComponentConstructor attribute. A parameterless ctor was found and will be used.");
            }
        }

        context.Call(__ExtensionsReflection.Instance.CreateInstance, component.Type, CodeBlocks.GetParameter(0));
        if (componentAttributeValues.InvokeOnObjectCreationEvent)
            context.Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This);

        return context.Return();
    }

    private static void ImplementGetterValueSet(InjectAttributeValues injectAttributeValues, Property property, Coder then)
    {
        var type = __Type.Instance;
        var factory = __Factory.Instance;
        LocalVariable variable = null;

        if (injectAttributeValues.Arguments != null && injectAttributeValues.Arguments.Length > 0)
        {
            variable = property.Getter.GetOrCreateVariable(BuilderTypes.Object.BuilderType.MakeArray());
            then.SetValue(variable, x => x.Newarr(BuilderTypes.Object, injectAttributeValues.Arguments.Length));

            for (int i = 0; i < injectAttributeValues.Arguments.Length; i++)
            {
                var arg = injectAttributeValues.Arguments[i];
                var customAttributeArgument = ((CustomAttributeArgument)arg.Value).Value;

                if (customAttributeArgument is string value)
                {
                    if (value == "[this]")
                    {
                        then.Load(variable).StoreElement(CodeBlocks.This, i);
                        continue;
                    }

                    if (value.StartsWith("[property]"))
                    {
                        var assignProperty = property.DeclaringType.GetProperty(value.Substring("[property]".Length).Trim());

                        if (assignProperty.Getter != null) then.Load(variable).StoreElement(then.NewCoder().Call(assignProperty.Getter), i);
                        else if (assignProperty.BackingField != null)
                            then.Load(variable).StoreElement(property.DeclaringType.GetField(assignProperty.BackingField), i);
                        else
                            Builder.Current.Log(LogTypes.Error, $"The property '{assignProperty}' does not have a getter and a backing field.");

                        continue;
                    }

                    if (value.StartsWith("[field]"))
                    {
                        then.Load(variable).StoreElement(property.DeclaringType.GetField(value.Substring("[field]".Length).Trim()), i);
                        continue;
                    }
                }

                then.Load(variable).StoreElement(customAttributeArgument, i);
            }
        }

        if (!injectAttributeValues.ForceDontCreateMany && (BuilderTypes.IEnumerable.BuilderType.AreReferenceAssignable(property.ReturnType) || property.ReturnType.IsArray) && !property.ReturnType.Implements(BuilderTypes.IDictionary))
        {
            if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
                then.SetValue(property.BackingField, x => x.Call(injectAttributeValues.IsOrdered ? factory.CreateManyOrdered : factory.CreateMany, y => y.Load(property.ReturnType.ChildType).Call(type.FullName), y => variable ?? null));
            else if (injectAttributeValues.ContractType != null)
                then.SetValue(property.BackingField, x => x.Call(injectAttributeValues.IsOrdered ? factory.CreateManyOrdered : factory.CreateMany, y => y.Load(injectAttributeValues.ContractType).Call(type.FullName), y => variable ?? null));
            else
                then.SetValue(property.BackingField, x => x.Call(injectAttributeValues.IsOrdered ? factory.CreateManyOrdered : factory.CreateMany, injectAttributeValues.ContractName, variable ?? null));
        }
        else if (injectAttributeValues.InjectFirst)
        {
            if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
                then.SetValue(property.BackingField, x => x.Call(factory.CreateFirst, y => y.Load(property.ReturnType).Call(type.FullName), y => variable ?? null));
            else if (injectAttributeValues.ContractType != null)
                then.SetValue(property.BackingField, x => x.Call(factory.CreateFirst, y => y.Load(injectAttributeValues.ContractType).Call(type.FullName), y => variable ?? null));
            else
                then.SetValue(property.BackingField, x => x.Call(factory.CreateFirst, injectAttributeValues.ContractName, variable ?? null));
        }
        else
        {
            if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
                then.SetValue(property.BackingField, x => x.Call(factory.Create, y => y.Load(property.ReturnType).Call(type.FullName), y => variable ?? null));
            else if (injectAttributeValues.ContractType != null)
                then.SetValue(property.BackingField, x => x.Call(factory.Create, y => y.Load(injectAttributeValues.ContractType).Call(type.FullName), y => variable ?? null));
            else
                then.SetValue(property.BackingField, x => x.Call(factory.Create, injectAttributeValues.ContractName, variable ?? null).As(property.ReturnType));
        }
    }

    private static void ImplementInjectField(Builder builder, AttributedField[] fields)
    {
        foreach (var member in fields)
        {
            builder.Log(LogTypes.Info, $"Implementing field interceptors: {member.Field.DeclaringType.Name.PadRight(40, ' ')} {member.Field.Name}");

            if (!member.Field.Modifiers.HasFlag(Modifiers.Private))
            {
                builder.Log(LogTypes.Error, $"Injection to non-private fields is not supported: {member.Field.DeclaringType.Name.PadRight(40, ' ')} {member.Field.Name}");
                continue;
            }

            if (member.Field.FieldType.IsValueType)
            {
                builder.Log(LogTypes.Error, $"Really? You want to inject to a value-type? Are you drunk? This is definitely not supported: {member.Field.DeclaringType.Name.PadRight(40, ' ')} {member.Field.Name}");
                continue;
            }

            var injectAttributeData = new InjectAttributeValues(member.Attribute);

            var type = member.Field.OriginType;
            var usage = member.Field.FindUsages().ToArray();
            var property = type.CreateProperty(member.Field, PropertySetterCreationOption.DontCreateSetter);

            property.CustomAttributes.AddCompilerGeneratedAttribute();
            property.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);
            property.CustomAttributes.AddNonSerializedAttribute();

            ImplementPropertyGetter(injectAttributeData, property);

            foreach (var item in usage)
                if (item.Field.IsStatic || !item.IsBeforeBaseCall)
                    item.Replace(property);

            member.Remove();
        }
    }

    private static void ImplementInjectProperties(Builder builder, AttributedProperty[] properties)
    {
        foreach (var member in properties)
        {
            builder.Log(LogTypes.Info, $"Implementing property interceptors: {member.Property.DeclaringType.Name.PadRight(40, ' ')} {member.Property.Name}");

            if (member.Property.Getter == null)
            {
                builder.Log(LogTypes.Error, $"Injection to properties without a getter is not supported: {member.Property.DeclaringType.Name.PadRight(40, ' ')} {member.Property.Name}");
                continue;
            }

            if (member.Property.ReturnType.IsValueType)
            {
                builder.Log(LogTypes.Error, $"Really? You want to inject to a value-type? Are you drunk? This is definitely not supported: {member.Property.DeclaringType.Name.PadRight(40, ' ')} {member.Property.Name}");
                continue;
            }

            if (member.Property.BackingField == null)
            {
                builder.Log(LogTypes.Error, $"Properties without backing fields are not supported: {member.Property.DeclaringType.Name.PadRight(40, ' ')} {member.Property.Name}");
                continue;
            }

            if (member.Property.IsAbstract)
                continue;

            var injectAttributeData = new InjectAttributeValues(member.Attribute);
            ImplementPropertyGetter(injectAttributeData, member.Property);
            member.Remove();
        }
    }

    private static void ImplementPropertyGetter(InjectAttributeValues injectAttributeValues, Property property)
    {
        var coder = property.Getter.NewCoder();
        property.BackingField.ReadOnly = false;

        coder.If(x => x.Load(property.BackingField).IsNull(), then =>
        {
            if (injectAttributeValues.MakeThreadSafe)
            {
                var monitor = __Monitor.Instance;
                var syncObject = property.CreateField((BuilderType)BuilderTypes.Object, $"<{property.Name}>__syncObject_injection");
                var objectCtor = BuilderTypes.Object.BuilderType.ParameterlessContructor;
                var method = property.DeclaringType.CreateMethod(property.IsStatic ? Modifiers.PrivateStatic : Modifiers.Private, $"<{property.Name}>__assigner_injection", Type.EmptyTypes);
                var lockTaken = method.GetOrCreateVariable((BuilderType)BuilderTypes.Boolean);

                foreach (var ctor in property.DeclaringType.GetRelevantConstructors())
                    ctor.NewCoder().SetValue(syncObject, x => x.NewObj(objectCtor)).Insert(InsertionPosition.Beginning);

                method.NewCoder()
                    .SetValue(lockTaken, false)
                    .Try(@try =>
                        @try.Call(monitor.Enter, syncObject, lockTaken).End
                        .If(x => x.Load(property.BackingField).IsNull(), thenInner =>
                        {
                            ImplementGetterValueSet(injectAttributeValues, property, thenInner);
                            return thenInner;
                        }))
                    .Finally(@finally =>
                    {
                        return @finally.If(x => x.Load(lockTaken).Is(true), x => x.Call(monitor.Exit, syncObject));
                    })
                    .EndTry()
                    .Return()
                    .Replace();

                then.Call(method);
            }
            else
                ImplementGetterValueSet(injectAttributeValues, property, then);

            return then;
        }).Insert(InsertionPosition.Beginning);
    }

    private class ComponentAttributeValues
    {
        public ComponentAttributeValues(AttributedType attributedType)
        {
            if (attributedType.Attribute.Properties.ContainsKey("InvokeOnObjectCreationEvent")) this.InvokeOnObjectCreationEvent = (bool)attributedType.Attribute.Properties["InvokeOnObjectCreationEvent"].Value;
            foreach (var item in attributedType.Attribute.ConstructorArguments)
            {
                switch (item.Type.FullName)
                {
                    case "System.String":
                        this.ContractName = item.Value as string;
                        break;

                    case "System.Type":
                        this.ContractType = (item.Value as TypeReference)?.ToBuilderType() ?? item.Value as BuilderType ?? Builder.Current.Import(item.Value as Type)?.ToBuilderType();
                        break;

                    case "System.UInt32":
                        this.Priority = (uint)item.Value;
                        break;

                    case "Cauldron.Activator.FactoryCreationPolicy":
                        this.Policy = (int)item.Value;
                        break;
                }
            }
        }

        public string ContractName { get; }
        public BuilderType ContractType { get; }
        public bool InvokeOnObjectCreationEvent { get; }
        public int Policy { get; }
        public uint Priority { get; }
    }

    private class InjectAttributeValues
    {
        public InjectAttributeValues(BuilderCustomAttribute builderCustomAttribute)
        {
            if (builderCustomAttribute.ConstructorArguments != null && builderCustomAttribute.ConstructorArguments.Length > 0)
                this.Arguments = builderCustomAttribute.ConstructorArguments[0].Value as CustomAttributeArgument[];

            if (builderCustomAttribute.Properties.ContainsKey("ContractType")) this.ContractType = (builderCustomAttribute.Properties["ContractType"].Value as TypeReference)?.ToBuilderType();
            if (builderCustomAttribute.Properties.ContainsKey("ContractName")) this.ContractName = builderCustomAttribute.Properties["ContractName"].Value as string;
            if (builderCustomAttribute.Properties.ContainsKey("InjectFirst")) this.InjectFirst = (bool)builderCustomAttribute.Properties["InjectFirst"].Value;
            if (builderCustomAttribute.Properties.ContainsKey("IsOrdered")) this.IsOrdered = (bool)builderCustomAttribute.Properties["IsOrdered"].Value;
            if (builderCustomAttribute.Properties.ContainsKey("MakeThreadSafe")) this.MakeThreadSafe = (bool)builderCustomAttribute.Properties["MakeThreadSafe"].Value;
            if (builderCustomAttribute.Properties.ContainsKey("ForceDontCreateMany")) this.ForceDontCreateMany = (bool)builderCustomAttribute.Properties["ForceDontCreateMany"].Value;
        }

        public CustomAttributeArgument[] Arguments { get; }
        public string ContractName { get; }
        public BuilderType ContractType { get; }
        public bool ForceDontCreateMany { get; }
        public bool InjectFirst { get; }
        public bool IsOrdered { get; }
        public bool MakeThreadSafe { get; }
    }
}