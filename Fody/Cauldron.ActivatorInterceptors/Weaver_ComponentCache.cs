using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public static class Weaver_ComponentCache
{
    public static string Name = "Activator Component Cache (Dependency Injection)";
    public static int Priority = int.MaxValue;

    [Display("Creating Component Cache")]
    public static void Implement(Builder builder)
    {
        builder.Log(LogTypes.Info, "Creating Cauldron Cache");

        var cauldron = builder.GetType("CauldronInterceptionHelper", SearchContext.Module);
        var componentAttribute = __ComponentAttribute.Instance;
        var genericComponentAttribute = __GenericComponentAttribute.Instance;
        var factory = __Factory.Instance;

        // Before we start let us find all factoryextensions and add a component attribute to them
        var factoryResolverInterface = __IFactoryExtension.Type;
        AddComponentAttribute(builder, builder.FindTypesByInterface(factoryResolverInterface), x => factoryResolverInterface.Fullname);
        // Also the same to all types that inherits from Factory<>
        var factoryGeneric = __Factory_1.Type;
        AddComponentAttribute(builder, builder.FindTypesByBaseClass(factoryGeneric), type =>
        {
            var factoryBase = type.BaseClasses.FirstOrDefault(x => x.Fullname.StartsWith("Cauldron.Activator.Factory"));
            if (factoryBase == null)
                return type.Fullname;

            return factoryBase.GetGenericArgument(0).Fullname;
        });

        int counter = 0;

        var extensionAvatar = builder.GetType("Cauldron.ExtensionsReflection").With(x => new
        {
            CreateInstance = x.GetMethod("CreateInstance", 2)
        });
        //var factoryCacheInterface = builder.GetType("Cauldron.Activator.IFactoryCache");
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

        foreach (var item in builder.CustomAttributes)
            builder.Log(LogTypes.Info, $"---------------> {item.Type}");

        foreach (var component in components)
        {
            builder.Log(LogTypes.Info, "Hardcoding component factory .ctor: " + component.Type.Fullname);

            var componentType = builder.CreateType("", TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "<>f__IFactoryTypeInfo_" + component.Type.Name + "_" + counter++);
            var componentAttributeField = componentType.CreateField(Modifiers.Private, componentAttribute.ToBuilderType, "componentAttribute");
            componentType.AddInterface(factoryTypeInfoInterface);
            componentType.CustomAttributes.AddDebuggerDisplayAttribute(component.Type.Name + " ({ContractName})");
            componentTypes.Add(componentType);

            // Create ctor
            componentType
               .CreateConstructor()
               .NewCoder()
               .SetValue(componentAttributeField, x => x.NewObj(component))
               .Return()
               .Replace();

            // Implement the methods
            componentType.CreateMethod(Modifiers.Public | Modifiers.Overrides, createInstanceInterfaceMethod.ReturnType, createInstanceInterfaceMethod.Name, createInstanceInterfaceMethod.Parameters)
                .NewCoder()
                .Context(x => AddContext(builder, x, factory, component))
                .Context(x => x.Call(extensionAvatar.CreateInstance, component.Type, CodeBlocks.GetParameter(0)).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return())
                .Replace();

            // Implement the properties
            foreach (var property in factoryTypeInfoInterface.Properties)
            {
                var propertyResult = componentType.CreateProperty(Modifiers.Public | Modifiers.Overrides, property.ReturnType, property.Name, true);
                propertyResult.BackingField.Remove();

                switch (property.Name)
                {
                    case "ContractName":
                        propertyResult.Getter.NewCoder().Load(componentAttributeField).Call(componentAttribute.ContractName).Return().Replace();
                        break;

                    case "CreationPolicy":
                        propertyResult.Getter.NewCoder().Load(componentAttributeField).Call(componentAttribute.Policy).Return().Replace();
                        break;

                    case "Priority":
                        propertyResult.Getter.NewCoder().Load(componentAttributeField).Call(componentAttribute.Priority).Return().Replace();
                        break;

                    case "Type":
                        propertyResult.Getter.NewCoder().Load(component.Type).Return().Replace();
                        break;
                }
            }

            // Also remove the component attribute
            component.Attribute.Remove();
        }

        builder.Log(LogTypes.Info, "Adding component IFactoryCache Interface");

        var linqEnumerable = builder.GetType("System.Linq.Enumerable", SearchContext.AllReferencedModules);
        var ifactoryTypeInfo = factoryTypeInfoInterface.MakeArray();
        var ctorCoder = cauldron.CreateStaticConstructor().NewCoder();
        //var concat = linqEnumerable.GetMethod("Concat", 2, true).MakeGeneric(factoryTypeInfoInterface);
        //var toArray = linqEnumerable.GetMethod("ToArray", 1, true).MakeGeneric(factoryTypeInfoInterface);
        var getComponentsFromOtherAssemblies = builder.ReferencedAssemblies
            .Select(x => x.MainModule.GetType("CauldronInterceptionHelper"))
            .Where(x => x != null)
            .Select(x => x.ToBuilderType())
            .Select(x => x.GetMethod("GetComponents", false)?.Import())
            .Where(x => x != null)
            .ToArray();

        var getComponentsMethod = cauldron.CreateMethod(Modifiers.Public | Modifiers.Static, ifactoryTypeInfo, "GetComponents");
        getComponentsMethod
            .NewCoder()
                .NewCoder()
                .Context(context =>
                {
                    var resultValue = context.GetOrCreateReturnVariable();
                    context.SetValue(resultValue, x => x.Newarr(factoryTypeInfoInterface, componentTypes.Count));

                    for (int i = 0; i < componentTypes.Count; i++)
                    {
                        var field = cauldron.CreateField(Modifiers.PrivateStatic, factoryTypeInfoInterface, "<FactoryType>f__" + i);
                        context
                            .Load(resultValue)
                            .StoreElement(field, i);
                        ctorCoder.SetValue(field, x => x.NewObj(componentTypes[i].ParameterlessContructor));
                    }
                    return context;
                })
                .Return()
                .Replace();

        ctorCoder.Insert(InsertionPosition.End);

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
    }

    private static void AddComponentAttribute(Builder builder, IEnumerable<BuilderType> builderTypes, Func<BuilderType, string> contractNameDelegate = null)
    {
        var componentConstructorAttribute = builder.GetType("Cauldron.Activator.ComponentConstructorAttribute");
        var componentAttribute = builder.GetType("Cauldron.Activator.ComponentAttribute").With(x => new
        {
            Type = x,
            ContractName = x.GetMethod("get_ContractName"),
            Policy = x.GetMethod("get_Policy"),
            Priority = x.GetMethod("get_Priority")
        });

        foreach (var item in builderTypes)
        {
            if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters)
                continue;

            if (item.CustomAttributes.HasAttribute(componentAttribute.Type))
                continue;

            var contractName = (contractNameDelegate == null ? item.Fullname : contractNameDelegate(item)).Replace('/', '+');
            item.CustomAttributes.Add(componentAttribute.Type, contractName);

            // Add a component contructor attribute to all .ctors
            foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                ctor.CustomAttributes.Add(componentConstructorAttribute);
        }
    }

    private static Coder AddContext(Builder builder, Coder context, __Factory factory, AttributedType component)
    {
        var componentConstructorAttribute = __ComponentConstructorAttribute.Type;

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
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull(), then => then.NewObj(component.Type.ParameterlessContructor).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return());
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).Call(arrayAvatar.Length).Is(0), then => then.NewObj(component.Type.ParameterlessContructor).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return());
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
                            then.NewObj(ctor, CodeBlocks.GetParameter(0).ArrayElements()).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return();
                        else
                            then.Call(ctor, CodeBlocks.GetParameter(0).ArrayElements()).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return();

                        return then;
                    });
                }
                else
                {
                    if (ctor.Name == ".ctor")
                    {
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull(), then => then.NewObj(ctor).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return());
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).Call(arrayAvatar.Length).Is(0), then => then.NewObj(ctor).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return());
                    }
                    else
                    {
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull(), then => then.Call(ctor).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return());
                        context.If(x => x.Load(CodeBlocks.GetParameter(0)).Call(arrayAvatar.Length).Is(0), then => then.Call(ctor).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return());
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
                context.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull(), then => then.NewObj(component.Type.ParameterlessContructor).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return());
                context.If(x => x.Load(CodeBlocks.GetParameter(0)).Call(arrayAvatar.Length).Is(0), then => then.NewObj(component.Type.ParameterlessContructor).Duplicate().Call(factory.OnObjectCreation, CodeBlocks.This).Return());

                builder.Log(LogTypes.Info, $"The component '{component.Type.Fullname}' has no ComponentConstructor attribute. A parameterless ctor was found and will be used.");
            }
        }

        return context;
    }
}