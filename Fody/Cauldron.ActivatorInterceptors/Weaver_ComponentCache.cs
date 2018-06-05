using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

public static class Weaver_ComponentCache
{
    public const string NoIDisposableObjectExceptionText = "An object with creation policy 'Singleton' with an implemented 'IDisposable' must also implement the 'IDisposableObject' interface.";
    public const string UnknownConstructor = "There is no defined constructor that matches the passed parameters for component ";
    public static string Name = "Activator Component Cache (Dependency Injection)";
    public static int Priority = int.MaxValue;
    public static Field unknownConstructorText;
    private static int indexer = 0;

    [Display("Creating Component Cache and Inject Attribute")]
    public static void Implement(Builder builder)
    {
        builder.Log(LogTypes.Info, "Creating Cauldron Cache");

        var cauldron = builder.GetType("CauldronInterceptionHelper", SearchContext.Module);
        var componentAttribute = BuilderTypes.ComponentAttribute;
        var genericComponentAttribute = BuilderTypes.GenericComponentAttribute;
        unknownConstructorText = cauldron.CreateField(Modifiers.PublicStatic, (BuilderType)BuilderTypes.String, "UnknownConstructorText");
        unknownConstructorText.CustomAttributes.AddCompilerGeneratedAttribute();
        unknownConstructorText.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

        cauldron.CreateStaticConstructor().NewCoder()
            .SetValue(unknownConstructorText, UnknownConstructor)
            .Insert(InsertionPosition.Beginning);

        // Before we start let us find all factoryextensions and add a component attribute to them
        var factoryResolverInterface = (BuilderType)BuilderTypes.IFactoryExtension;
        AddComponentAttribute(builder, builder.FindTypesByInterface(factoryResolverInterface), x => factoryResolverInterface);
        // Also the same to all types that inherits from Factory<>
        var factoryGeneric = (BuilderType)BuilderTypes.Factory1;
        AddComponentAttribute(builder, builder.FindTypesByBaseClass(factoryGeneric), type =>
        {
            var factoryBase = type.BaseClasses.FirstOrDefault(x => x.Fullname.StartsWith("Cauldron.Activator.Factory"));

            if (factoryBase == null)
                return type;

            return factoryBase.GetGenericArgument(0);
        });

        int counter = 0;

        // Get all Components
        var componentTypes = new List<BuilderType>();
        var components = builder
            .FindTypesByAttribute(componentAttribute)
            .Concat(builder
                .CustomAttributes
                .Where(x => x.Type == genericComponentAttribute)
                .Select(x => new
                {
                    Attribute = BuilderCustomAttribute.Create(componentAttribute, x.ConstructorArguments.Skip(1)),
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
            var childType = Builder.Current.GetChildrenType((TypeReference)component.Type);

            /*
                Check for IDisposable
            */
            if (componentAttributeValue.Policy == 1 && component.Type.Implements(BuilderTypes.IDisposable) && !component.Type.Implements(BuilderTypes.IDisposableObject))
                builder.Log(LogTypes.Error, component.Type, NoIDisposableObjectExceptionText);

            var componentType = builder.CreateType("", TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "<>f__IFactoryTypeInfo_" + component.Type.Name.GetValidName() + "_" + counter++);
            componentType.AddInterface(BuilderTypes.IFactoryTypeInfo);
            componentType.CustomAttributes.AddDebuggerDisplayAttribute(component.Type.Name + " ({ContractName})");
            componentTypes.Add(componentType);

            // Create ctor
            var componentTypeCtor = componentType
               .CreateConstructor()
               .NewCoder();

            // Implement the methods
            AddCreateInstanceMethod(builder, cauldron, BuilderTypes.IFactoryTypeInfo.GetMethod_CreateInstance_1(), component, componentAttributeValue, componentType).Replace();
            AddCreateInstanceMethod(builder, cauldron, BuilderTypes.IFactoryTypeInfo.GetMethod_CreateInstance(), component, componentAttributeValue, componentType).Replace();

            // Implement the properties
            foreach (var property in BuilderTypes.IFactoryTypeInfo.BuilderType.Properties)
            {
                var propertyResult = componentType.CreateProperty(Modifiers.Public | Modifiers.Overrides, property.ReturnType, property.Name,
                    property.Setter == null ? PropertySetterCreationOption.DontCreateSetter : PropertySetterCreationOption.AlwaysCreate);

                switch (property.Name)
                {
                    case "ContractName":

                        if (string.IsNullOrEmpty(componentAttributeValue.ContractName))
                            componentTypeCtor.SetValue(propertyResult.BackingField, x =>
                                x.Load(componentAttributeValue.ContractType).Call(BuilderTypes.Type.GetMethod_get_FullName()));
                        else
                        {
                            propertyResult.BackingField.Remove();
                            propertyResult.Getter.NewCoder().Load(componentAttributeValue.ContractName).Return().Replace();
                        }
                        break;

                    case "ContractType":
                        propertyResult.BackingField.Remove();
                        if (componentAttributeValue.ContractType == null)
                            propertyResult.Getter.NewCoder().Load(value: null).Return().Replace();
                        else
                            propertyResult.Getter.NewCoder().Load(componentAttributeValue.ContractType).Return().Replace();
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

                    case "IsEnumerable":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(childType.Item2).Return().Replace();
                        break;

                    case "ChildType":
                        propertyResult.BackingField.Remove();
                        propertyResult.Getter.NewCoder().Load(childType.Item2 ? childType.Item1 : null).Return().Replace();
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
                            var instanceFieldName = $"<{component.Type}>_componentInstance";
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
        var ifactoryTypeInfo = BuilderTypes.IFactoryTypeInfo.BuilderType.Import().MakeArray();
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
                    context.SetValue(resultValue, x => x.Newarr(BuilderTypes.IFactoryTypeInfo, componentTypes.Count));

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
        var componentConstructorAttribute = BuilderTypes.ComponentConstructorAttribute;
        var componentAttribute = BuilderTypes.ComponentAttribute.BuilderType;

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
                ctor.CustomAttributes.Add(componentConstructorAttribute.BuilderType);
        }
    }

    private static Coder AddContext(Builder builder, Coder context, AttributedType component)
    {
        var componentAttributeValues = new ComponentAttributeValues(component);
        var ctors = GetComponentConstructors(component).ToArray();

        if (ctors.Length > 0 && context.AssociatedMethod.Parameters.Length > 0)
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
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull().OrOr(y => y.Load(CodeBlocks.GetParameter(0)).Call(BuilderTypes.Array.GetMethod_get_Length()).Is(0)), then =>
                        {
                            then.NewObj(component.Type.ParameterlessContructor);

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);

                            return then.Return();
                        });
                        parameterlessCtorAlreadyHandled = true;
                    }

                    context.If(@if =>
                    {
                        var resultCoder = @if.Load(CodeBlocks.GetParameter(0)).Call(BuilderTypes.Array.GetMethod_get_Length()).Is(ctorParameters.Length);
                        for (int i = 0; i < ctorParameters.Length; i++)
                            resultCoder = resultCoder.AndAnd(x => x.Load(CodeBlocks.GetParameter(0)).ArrayElement(i).Is(ctorParameters[i]));

                        return resultCoder;
                    }, @then =>
                    {
                        if (ctor.Name == ".ctor")
                        {
                            then.NewObj(ctor, CodeBlocks.GetParameter(0).ArrayElements());

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);
                        }
                        else
                        {
                            then.Call(ctor, CodeBlocks.GetParameter(0).ArrayElements());

                            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                                then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);
                        }

                        return then.Return();
                    });
                }
                else if (ctorParameters.Length == 0)
                {
                    CreateComponentParameterlessCtor(context, ctor, componentAttributeValues);
                    parameterlessCtorAlreadyHandled = true;
                }
            }

            context.ThrowNew(typeof(NotImplementedException), x =>
                x.Call(BuilderTypes.String.GetMethod_Concat(BuilderTypes.String, BuilderTypes.String), unknownConstructorText,
                    x.NewCoder().Call(BuilderTypes.IFactoryTypeInfo.GetMethod_get_ContractName())).End);
        }
        else if (ctors.Length > 0)
        {
            var ctor = ctors.FirstOrDefault(x => x.Parameters.Length == 0);
            if (ctor == null)
                context.ThrowNew(typeof(NotImplementedException), x =>
                    x.Call(BuilderTypes.String.GetMethod_Concat(BuilderTypes.String, BuilderTypes.String), unknownConstructorText,
                        x.NewCoder().Call(BuilderTypes.IFactoryTypeInfo.GetMethod_get_ContractName())).End);
            else
            {
                ctor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                CreateComponentParameterlessCtor(context, ctor, componentAttributeValues);
            }
        }
        else
        {
            context.ThrowNew(typeof(NotImplementedException), x =>
                x.Call(BuilderTypes.String.GetMethod_Concat(BuilderTypes.String, BuilderTypes.String), unknownConstructorText,
                    x.NewCoder().Call(BuilderTypes.IFactoryTypeInfo.GetMethod_get_ContractName())).End);

            builder.Log(LogTypes.Error, component.Type, $"The component '{component.Type.Fullname}' has no ComponentConstructor attribute or the constructor is not public or internal");
        }

        return context;
    }

    /*
    private static Coder AddContextParameterless(Builder builder, Coder context, AttributedType component)
    {
        var componentAttributeValues = new ComponentAttributeValues(component);
        var ctors = GetComponentConstructors(component);

        if (ctors.Length > 0)
        {
            for (int index = 0; index < ctors.Length; index++)
            {
                builder.Log(LogTypes.Info, "- " + ctors[index].Fullname);

                var ctor = ctors[index];
                if (ctor.Parameters.Length == 0)
                {
                    ctor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                    CreateComponentParameterlessCtor(context, ctor, componentAttributeValues);
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
                CreateComponentParameterlessCtor(context, component.Type.ParameterlessContructor, componentAttributeValues);
                builder.Log(LogTypes.Info, $"The component '{component.Type.Fullname}' has no ComponentConstructor attribute. A parameterless ctor was found and will be used.");
            }
        }

        context.ThrowNew(typeof(NotImplementedException), x =>
            x.Call(BuilderTypes.String.GetMethod_Concat(BuilderTypes.String, BuilderTypes.String), unknownConstructorText,
                x.NewCoder().Call(BuilderTypes.IFactoryTypeInfo.GetMethod_get_ContractName())).End);

        return context;
    }
    */

    private static Coder AddCreateInstanceMethod(Builder builder, BuilderType cauldron, Method createInstanceInterfaceMethod, AttributedType component, ComponentAttributeValues componentAttributeValue, BuilderType componentType)
    {
        var instanceFieldName = $"<{component.Type}>_componentInstance";
        return componentType.CreateMethod(Modifiers.Public | Modifiers.Overrides, createInstanceInterfaceMethod.ReturnType, createInstanceInterfaceMethod.Name, createInstanceInterfaceMethod.Parameters)
            .NewCoder()
            .Context(x =>
            {
                if (componentAttributeValue.Policy == 1)
                {
                    var instanceSyncObjectName = $"<{component.Type}>_componentInstanceSyncObject";
                    var instanceField = cauldron.GetField(instanceFieldName, false) ?? cauldron.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceFieldName);
                    var instanceFieldSyncObject = cauldron.GetField(instanceSyncObjectName, false) ?? cauldron.CreateField(Modifiers.InternalStatic, (BuilderType)BuilderTypes.Object, instanceSyncObjectName);
                    var instancedCreator = componentType.CreateMethod(Modifiers.Private, createInstanceInterfaceMethod.ReturnType, $"<{component.Type.Fullname}>__CreateInstance_{indexer++}", createInstanceInterfaceMethod.Parameters);
                    var lockTaken = instancedCreator.GetOrCreateVariable((BuilderType)BuilderTypes.Boolean);
                    instancedCreator.NewCoder()
                        .SetValue(lockTaken, false)
                        .Try(@try => @try
                            .Call(BuilderTypes.Monitor.GetMethod_Enter(), instanceFieldSyncObject, lockTaken).End
                            .If(@if => @if.Load(instanceField).IsNull(),
                                then => AddContext(builder, then, component),
                                @else => @else.Load(instanceField)))
                        .Finally(@finally => @finally.If(@if => @if.Load(lockTaken).Is(true), then => @then.Call(BuilderTypes.Monitor.GetMethod_Exit(), instanceFieldSyncObject)))
                        .EndTry()
                        .Return()
                        .Replace();
                    cauldron.CreateStaticConstructor().NewCoder().SetValue(instanceFieldSyncObject, y => y.NewObj(BuilderTypes.Object.GetMethod_ctor())).Insert(InsertionPosition.Beginning);

                    x.If(@if => @if.Load(instanceField).IsNotNull(), then => then.Load(instanceField).Return());
                    if (createInstanceInterfaceMethod.Parameters.Length == 0)
                        x.SetValue(instanceField, m => m.Call(instancedCreator));
                    else
                        x.SetValue(instanceField, m => m.Call(instancedCreator, CodeBlocks.GetParameters()));
                    // every singleton that implements the idisposable interface has also to
                    // implement the IDisposableObject interface this is because we want to know if
                    // an instance was disposed (somehow)
                    if (component.Type.Implements(BuilderTypes.IDisposable))
                    {
                        if (!component.Type.Implements(BuilderTypes.IDisposableObject))
                            builder.Log(LogTypes.Info, component.Type + " : " + NoIDisposableObjectExceptionText);
                        else
                        {
                            // Create an event handler method
                            var eventHandlerMethod =
                                componentType.GetMethod($"<IDisposableObject>_Handler", 2, false) ??
                                componentType.CreateMethod(Modifiers.Private, $"<IDisposableObject>_Handler", BuilderTypes.Object, BuilderTypes.EventArgs);
                            eventHandlerMethod.NewCoder()
                                .If(ehIf => ehIf.Load(CodeBlocks.GetParameter(0)).IsNotNull(), ehThen => ehThen.Load(CodeBlocks.GetParameter(0)).As(BuilderTypes.IDisposable).Call(BuilderTypes.IDisposable.GetMethod_Dispose()))
                                .SetValue(instanceField, null)
                                .If(@if => @if.Load(instanceField).IsNotNull(), @then => @then
                                    .Load(instanceField).As(BuilderTypes.IDisposableObject).Call(BuilderTypes.IDisposableObject.GetMethod_remove_Disposed(), o => o.NewObj(BuilderTypes.EventHandler.GetConstructor(), CodeBlocks.This, eventHandlerMethod)))
                                .Return()
                                .Replace();

                            x.Load(instanceField).As(BuilderTypes.IDisposableObject).Call(BuilderTypes.IDisposableObject.GetMethod_add_Disposed(), o => o.NewObj(BuilderTypes.EventHandler.GetConstructor(), CodeBlocks.This, eventHandlerMethod));
                        }
                    }

                    x.Load(instanceField).Return();
                }
                else
                    AddContext(builder, x, component);

                return x;
            });
    }

    private static Method AddFactoryRebuiltHandler(InjectAttributeValues injectAttributeValues, Property property, Field injectorField, Method factoryTypeInfoGet)
    {
        var declaringType = property.DeclaringType;
        var handler = declaringType.GetMethod(Modifiers.PrivateStatic, BuilderTypes.Void, $"<>__FactoryRebuiltHandler", BuilderTypes.Object, BuilderTypes.EventArgs);
        if (handler == null)
        {
            handler = declaringType.CreateMethod(Modifiers.PrivateStatic, BuilderTypes.Void, $"<>__FactoryRebuiltHandler", BuilderTypes.Object, BuilderTypes.EventArgs);
            handler.CustomAttributes.AddCompilerGeneratedAttribute();
            handler.NewCoder().Return().Replace();
            declaringType.CreateStaticConstructor().NewCoder().Call(handler, new object[] { null, null }).End.Insert(InsertionPosition.Beginning);
            declaringType.CreateStaticConstructor().NewCoder().Call(BuilderTypes.Factory.GetMethod_add_Rebuilt(), x => x.NewObj(BuilderTypes.EventHandler.GetConstructor(), CodeBlocks.This, handler)).End.Insert(InsertionPosition.End);
        }
        AssignFactoryGetFactoryInfo(handler.NewCoder(), injectAttributeValues, property, injectorField, factoryTypeInfoGet);
        return handler;
    }

    private static void AssignFactoryGetFactoryInfo(Coder coder, InjectAttributeValues injectAttributeValues, Property property, Field injectorField, Method factoryTypeInfoGet)
    {
        if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
            coder.SetValue(injectorField, x => x.Call(factoryTypeInfoGet, y => y.Load(property.ReturnType).Call(BuilderTypes.Type.GetMethod_get_FullName()))).Insert(InsertionPosition.Beginning);
        else if (injectAttributeValues.ContractType != null)
            coder.SetValue(injectorField, x => x.Call(factoryTypeInfoGet, y => y.Load(injectAttributeValues.ContractType).Call(BuilderTypes.Type.GetMethod_get_FullName()))).Insert(InsertionPosition.Beginning);
        else
            coder.SetValue(injectorField, x => x.Call(factoryTypeInfoGet, injectAttributeValues.ContractName)).Insert(InsertionPosition.Beginning);
    }

    private static void CreateComponentParameterlessCtor(Coder context, Method contructor, ComponentAttributeValues componentAttributeValues)
    {
        if (context.AssociatedMethod.Parameters.Length > 0)
            context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull().OrOr(y => y.Load(CodeBlocks.GetParameter(0)).Call(BuilderTypes.Array.GetMethod_get_Length()).Is(0)), then =>
            {
                if (contructor.Name == ".ctor")
                    then.NewObj(contructor);
                else
                    then.Call(contructor);

                if (componentAttributeValues.InvokeOnObjectCreationEvent)
                    then.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);

                return then.Return();
            });
        else
        {
            if (contructor.Name == ".ctor")
                context.NewObj(contructor);
            else
                context.Call(contructor);

            if (componentAttributeValues.InvokeOnObjectCreationEvent)
                context.Duplicate().Call(BuilderTypes.Factory.GetMethod_OnObjectCreation(), CodeBlocks.This);

            context.Return();
        }
    }

    private static Field CreateInjectorField(Property property)
    {
        var injectorField = property.DeclaringType.CreateField(Modifiers.PrivateStatic, BuilderTypes.IFactoryTypeInfo.BuilderType, $"<{property.Name}>__injectorField");
        injectorField.CustomAttributes.AddCompilerGeneratedAttribute();
        injectorField.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);
        injectorField.CustomAttributes.AddNonSerializedAttribute();
        return injectorField;
    }

    private static IEnumerable<Method> GetComponentConstructors(AttributedType component)
    {
        var methods = component.Type.GetMethods().OrderBy(x => x.Parameters.Length);

        // First all ctors with component Attribute
        foreach (var item in methods)
        {
            if (item.Name != ".ctor")
                continue;

            if (item.IsPublicOrInternal && item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                yield return item;
        }

        // Get all properties with component attribute
        foreach (var item in component.Type.GetAllProperties())
        {
            if (item.Getter == null)
                continue;

            if (item.IsStatic && item.Getter.IsPublicOrInternal && item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                yield return item.Getter;
        }

        // Then all static methods with component attribute
        foreach (var item in methods)
        {
            if (!item.IsStatic)
                continue;

            if (item.Name == ".cctor")
                continue;

            if (item.Name.StartsWith("set_"))
                continue;

            if (item.Name.StartsWith("get_"))
                continue;

            if (item.IsPublicOrInternal && item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                yield return item;
        }

        // At last all ctors without component Attribute
        foreach (var item in methods)
        {
            if (item.Name != ".ctor")
                continue;

            if (item.DeclaringType != component.Type)
                continue;

            if (item.IsPublicOrInternal && !item.CustomAttributes.HasAttribute(BuilderTypes.ComponentConstructorAttribute))
                yield return item;
        }
    }

    private static void ImplementGetterValueSet(InjectAttributeValues injectAttributeValues, Property property, Coder then)
    {
        var type = BuilderTypes.Type;
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

        var objectArray = (TypeReference)BuilderTypes.Object.BuilderType.MakeArray().Import();

        if (
            !injectAttributeValues.ForceDontCreateMany &&
            (BuilderTypes.IEnumerable.BuilderType.AreReferenceAssignable(property.ReturnType) || property.ReturnType.IsArray) &&
            !property.ReturnType.Implements(BuilderTypes.IDictionary) &&
            (injectAttributeValues.Arguments == null || injectAttributeValues.Arguments.Length == 0))
        {
            if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
                then.SetValue(property.BackingField, x => x.Call(injectAttributeValues.IsOrdered ?
                                                                    BuilderTypes.Factory.GetMethod_CreateManyOrdered(BuilderTypes.String) :
                                                                    BuilderTypes.Factory.GetMethod_CreateMany(BuilderTypes.String), y => y.Load(property.ReturnType.ChildType).Call(type.GetMethod_get_FullName())));
            else if (injectAttributeValues.ContractType != null)
                then.SetValue(property.BackingField, x => x.Call(injectAttributeValues.IsOrdered ?
                                                                    BuilderTypes.Factory.GetMethod_CreateManyOrdered(BuilderTypes.String) :
                                                                    BuilderTypes.Factory.GetMethod_CreateMany(BuilderTypes.String), y => y.Load(injectAttributeValues.ContractType).Call(type.GetMethod_get_FullName())));
            else
                then.SetValue(property.BackingField, x => x.Call(injectAttributeValues.IsOrdered ?
                                                                    BuilderTypes.Factory.GetMethod_CreateManyOrdered(BuilderTypes.String) :
                                                                    BuilderTypes.Factory.GetMethod_CreateMany(BuilderTypes.String), injectAttributeValues.ContractName));
        }
        else if (
            !injectAttributeValues.ForceDontCreateMany &&
            (BuilderTypes.IEnumerable.BuilderType.AreReferenceAssignable(property.ReturnType) || property.ReturnType.IsArray) &&
            !property.ReturnType.Implements(BuilderTypes.IDictionary))
        {
            if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
                then.SetValue(property.BackingField, x => x.Call(injectAttributeValues.IsOrdered ?
                                                                    BuilderTypes.Factory.GetMethod_CreateManyOrdered(BuilderTypes.String, objectArray) :
                                                                    BuilderTypes.Factory.GetMethod_CreateMany(BuilderTypes.String, objectArray), y => y.Load(property.ReturnType.ChildType).Call(type.GetMethod_get_FullName()), y => variable ?? null));
            else if (injectAttributeValues.ContractType != null)
                then.SetValue(property.BackingField, x => x.Call(injectAttributeValues.IsOrdered ?
                                                                    BuilderTypes.Factory.GetMethod_CreateManyOrdered(BuilderTypes.String, objectArray) :
                                                                    BuilderTypes.Factory.GetMethod_CreateMany(BuilderTypes.String, objectArray), y => y.Load(injectAttributeValues.ContractType).Call(type.GetMethod_get_FullName()), y => variable ?? null));
            else
                then.SetValue(property.BackingField, x => x.Call(injectAttributeValues.IsOrdered ?
                                                                    BuilderTypes.Factory.GetMethod_CreateManyOrdered(BuilderTypes.String, objectArray) :
                                                                    BuilderTypes.Factory.GetMethod_CreateMany(BuilderTypes.String, objectArray), injectAttributeValues.ContractName, variable ?? null));
        }
        else if (injectAttributeValues.InjectFirst && (injectAttributeValues.Arguments == null || injectAttributeValues.Arguments.Length == 0) && !injectAttributeValues.NoPreloading)
        {
            // Special case for parameterless injections - preloading stuff in .cctor
            var injectorField = CreateInjectorField(property);
            then.SetValue(property.BackingField, x => x.Load(injectorField).Call(BuilderTypes.IFactoryTypeInfo.GetMethod_CreateInstance()));
            AddFactoryRebuiltHandler(injectAttributeValues, property, injectorField, BuilderTypes.Factory.GetMethod_GetFactoryTypeInfoFirst());
        }
        else if (injectAttributeValues.InjectFirst && (injectAttributeValues.Arguments == null || injectAttributeValues.Arguments.Length == 0))
        {
            if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_CreateFirst(BuilderTypes.String), y => y.Load(property.ReturnType).Call(type.GetMethod_get_FullName())));
            else if (injectAttributeValues.ContractType != null)
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_CreateFirst(BuilderTypes.String), y => y.Load(injectAttributeValues.ContractType).Call(type.GetMethod_get_FullName())));
            else
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_CreateFirst(BuilderTypes.String), injectAttributeValues.ContractName));
        }
        else if (injectAttributeValues.InjectFirst)
        {
            if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_CreateFirst(BuilderTypes.String, objectArray), y => y.Load(property.ReturnType).Call(type.GetMethod_get_FullName()), y => variable ?? null));
            else if (injectAttributeValues.ContractType != null)
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_CreateFirst(BuilderTypes.String, objectArray), y => y.Load(injectAttributeValues.ContractType).Call(type.GetMethod_get_FullName()), y => variable ?? null));
            else
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_CreateFirst(BuilderTypes.String, objectArray), injectAttributeValues.ContractName, variable ?? null));
        }
        else if ((injectAttributeValues.Arguments == null || injectAttributeValues.Arguments.Length == 0) && !injectAttributeValues.NoPreloading)
        {
            // Special case for parameterless injections - preloading stuff in .cctor
            var injectorField = CreateInjectorField(property);
            then.SetValue(property.BackingField, x => x.Load(injectorField).Call(BuilderTypes.IFactoryTypeInfo.GetMethod_CreateInstance()));
            AddFactoryRebuiltHandler(injectAttributeValues, property, injectorField, BuilderTypes.Factory.GetMethod_GetFactoryTypeInfo());
        }
        else if ((injectAttributeValues.Arguments == null || injectAttributeValues.Arguments.Length == 0))
        {
            if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_Create(BuilderTypes.String), y => y.Load(property.ReturnType).Call(type.GetMethod_get_FullName())));
            else if (injectAttributeValues.ContractType != null)
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_Create(BuilderTypes.String), y => y.Load(injectAttributeValues.ContractType).Call(type.GetMethod_get_FullName())));
            else
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_Create(BuilderTypes.String), injectAttributeValues.ContractName).As(property.ReturnType));
        }
        else
        {
            if (string.IsNullOrEmpty(injectAttributeValues.ContractName) && injectAttributeValues.ContractType == null)
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_Create(BuilderTypes.String, objectArray), y => y.Load(property.ReturnType).Call(type.GetMethod_get_FullName()), y => variable ?? null));
            else if (injectAttributeValues.ContractType != null)
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_Create(BuilderTypes.String, objectArray), y => y.Load(injectAttributeValues.ContractType).Call(type.GetMethod_get_FullName()), y => variable ?? null));
            else
                then.SetValue(property.BackingField, x => x.Call(BuilderTypes.Factory.GetMethod_Create(BuilderTypes.String, objectArray), injectAttributeValues.ContractName, variable ?? null).As(property.ReturnType));
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
                var syncObject = property.CreateField((BuilderType)BuilderTypes.Object, $"<{property.Name}>__syncObject_injection");
                var objectCtor = BuilderTypes.Object.BuilderType.ParameterlessContructor;
                var method = property.DeclaringType.CreateMethod(property.IsStatic ? Modifiers.PrivateStatic : Modifiers.Private, $"<{property.Name}>__assigner_injection", Type.EmptyTypes);
                var lockTaken = method.GetOrCreateVariable((BuilderType)BuilderTypes.Boolean);

                foreach (var ctor in property.DeclaringType.GetRelevantConstructors())
                    ctor.NewCoder().SetValue(syncObject, x => x.NewObj(objectCtor)).Insert(InsertionPosition.Beginning);

                method.NewCoder()
                    .SetValue(lockTaken, false)
                    .Try(@try =>
                        @try.Call(BuilderTypes.Monitor.GetMethod_Enter(), syncObject, lockTaken).End
                        .If(x => x.Load(property.BackingField).IsNull(), thenInner =>
                        {
                            ImplementGetterValueSet(injectAttributeValues, property, thenInner);
                            return thenInner;
                        }))
                    .Finally(@finally =>
                    {
                        return @finally.If(x => x.Load(lockTaken).Is(true), x => x.Call(BuilderTypes.Monitor.GetMethod_Exit(), syncObject));
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
            if (builderCustomAttribute.Properties.ContainsKey("NoPreloading")) this.NoPreloading = (bool)builderCustomAttribute.Properties["NoPreloading"].Value;
        }

        public CustomAttributeArgument[] Arguments { get; }
        public string ContractName { get; }
        public BuilderType ContractType { get; }
        public bool ForceDontCreateMany { get; }
        public bool InjectFirst { get; }
        public bool IsOrdered { get; }
        public bool MakeThreadSafe { get; }
        public bool NoPreloading { get; }
    }
}