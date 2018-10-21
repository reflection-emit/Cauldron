using Cauldron.ActivatorInterceptors;
using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

public static class ComponentCacheWeaver
{
    public static readonly List<BuilderType> componentTypes = new List<BuilderType>();
    public static string Name = "Activator Component Cache (Dependency Injection)";
    public static int Priority = int.MaxValue;

    [Display("Creating Component Cache and Inject Attribute")]
    public static void Implement(Builder builder)
    {
        builder.Log(LogTypes.Info, "Creating Cauldron Cache");

        var cauldron = builder.GetType("CauldronInterceptionHelper", SearchContext.Module);
        var componentAttribute = BuilderTypes.ComponentAttribute;
        var genericComponentAttribute = BuilderTypes.GenericComponentAttribute;

        // Before we start let us find all factoryextensions and add a component attribute to them
        var factoryResolverInterface = (BuilderType)BuilderTypes.IFactoryExtension;
        AddComponentAttribute(builder, builder.FindTypesByInterface(factoryResolverInterface), x => factoryResolverInterface);

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
            var factoryType =  FactoryTypeInfoWeaver.Create(component);
            componentTypes.Add(factoryType);
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
        ReplaceFactoryCreate(builder);
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
        {
            if (factoryTypeInfoGet.ParametersCount == 2)
                coder.SetValue(injectorField, x => x.Call(factoryTypeInfoGet,
                    y => coder.AssociatedMethod.DeclaringType,
                    y => y.Load(property.ReturnType.IsEnumerable ? property.ReturnType.ChildType : property.ReturnType).Call(BuilderTypes.Type.GetMethod_get_FullName()))).Insert(InsertionPosition.Beginning);
            else
                coder.SetValue(injectorField, x => x.Call(factoryTypeInfoGet,
                    y => y.Load(property.ReturnType.IsEnumerable ? property.ReturnType.ChildType : property.ReturnType).Call(BuilderTypes.Type.GetMethod_get_FullName()))).Insert(InsertionPosition.Beginning);
        }
        else if (injectAttributeValues.ContractType != null)
        {
            if (factoryTypeInfoGet.ParametersCount == 2)
                coder.SetValue(injectorField, x => x.Call(factoryTypeInfoGet,
                    y => coder.AssociatedMethod.DeclaringType,
                    y => y.Load(injectAttributeValues.ContractType.IsEnumerable ? injectAttributeValues.ContractType.ChildType : injectAttributeValues.ContractType).Call(BuilderTypes.Type.GetMethod_get_FullName()))).Insert(InsertionPosition.Beginning);
            else
                coder.SetValue(injectorField, x => x.Call(factoryTypeInfoGet,
                    y => y.Load(injectAttributeValues.ContractType.IsEnumerable ? injectAttributeValues.ContractType.ChildType : injectAttributeValues.ContractType).Call(BuilderTypes.Type.GetMethod_get_FullName()))).Insert(InsertionPosition.Beginning);
        }
        else
        {
            if (factoryTypeInfoGet.ParametersCount == 2)
                coder.SetValue(injectorField, x => x.Call(factoryTypeInfoGet, coder.AssociatedMethod.DeclaringType, injectAttributeValues.ContractName)).Insert(InsertionPosition.Beginning);
            else
                coder.SetValue(injectorField, x => x.Call(factoryTypeInfoGet, injectAttributeValues.ContractName)).Insert(InsertionPosition.Beginning);
        }
    }

    private static Field CreateInjectorField(Property property, bool isMultiple = false)
    {
        var injectorField = property.DeclaringType.CreateField(Modifiers.PrivateStatic, isMultiple ?
            BuilderTypes.IFactoryTypeInfo.BuilderType.MakeArray() :
            BuilderTypes.IFactoryTypeInfo.BuilderType, $"<{property.Name}>__injectorField");

        injectorField.CustomAttributes.AddCompilerGeneratedAttribute();
        injectorField.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);
        injectorField.CustomAttributes.AddNonSerializedAttribute();
        return injectorField;
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
                        else if (assignProperty.BackingField != null) then.Load(variable).StoreElement(assignProperty.BackingField, i);
                        else Builder.Current.Log(LogTypes.Error, $"The property '{assignProperty}' does not have a getter and a backing field.");

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
            injectAttributeValues.ForceDontCreateMany            /**/  == false /**/ &&
            property.ReturnType.IsIEnumerable()                  /**/  == true  /**/ &&
            property.ReturnType.IsIDictionary()                  /**/  == false /**/ &&
            injectAttributeValues.IsParameterless()              /**/  == true  /**/ &&
            injectAttributeValues.NoPreloading                   /**/  == false
            )
        {
            // Special case for parameterless injections - preloading stuff in .cctor
            var injectorField = CreateInjectorField(property, true);
            var localArray = then.AssociatedMethod.GetOrCreateVariable(property.ReturnType.ChildType.MakeArray());
            then.SetValue(localArray, x => x.Newarr(property.ReturnType.ChildType, injectorField));
            then.For(injectorField, (coder, item, indexer) =>
                    coder
                        .Load(localArray)
                        .StoreElement(
                            coder.NewCoder()
                            .Load<FieldCoder>(item())
                            .Call(BuilderTypes.IFactoryTypeInfo.GetMethod_CreateInstance()), indexer))
                .SetValue(property.BackingField, localArray);

            AddFactoryRebuiltHandler(injectAttributeValues, property, injectorField,
                injectAttributeValues.IsOrdered ?
                    BuilderTypes.Factory.GetMethod_GetFactoryTypeInfoManyOrdered() :
                    BuilderTypes.Factory.GetMethod_GetFactoryTypeInfoMany());
        }
        else
        if (
            injectAttributeValues.ForceDontCreateMany            /**/  == false /**/ &&
            property.ReturnType.IsIEnumerable()                  /**/  == true  /**/ &&
            property.ReturnType.IsIDictionary()                  /**/  == false /**/ &&
            injectAttributeValues.IsParameterless()              /**/  == true  /**/ &&
            injectAttributeValues.NoPreloading                   /**/  == true
            )
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
        if (
            injectAttributeValues.ForceDontCreateMany            /**/  == false /**/ &&
            property.ReturnType.IsIEnumerable()                  /**/  == true  /**/ &&
            property.ReturnType.IsIDictionary()                  /**/  == false /**/ &&
            injectAttributeValues.IsParameterless()              /**/  == false /**/ &&
            injectAttributeValues.NoPreloading                   /**/  == true
            )
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
                {
                    if (!property.DeclaringType.IsStatic && ctor.Name == ".cctor")
                        continue;

                    if (property.DeclaringType.IsStatic && ctor.Name == ".ctor")
                        continue;

                    ctor.NewCoder().SetValue(syncObject, x => x.NewObj(objectCtor)).Insert(InsertionPosition.Beginning);
                }

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

    private static void ReplaceFactoryCreate(Builder builder)
    {
        foreach (var item in FactoryCreateUsages.createMethodUsages)
        {
            Builder.Current.Log(LogTypes.Info, $"{item}");
            ((Instruction)item).Operand = Builder.Current.Import((MethodReference)BuilderTypes.Factory.GetMethod_____Create().MakeGeneric(item.GetGenericArgument(0)));
            item.HostMethod.NewCoder().Load(item.HostMethod.DeclaringType).End.Insert(InsertionAction.Before, item.Position);
        }

        foreach (var item in FactoryCreateUsages.createTypeMethodUsages)
        {
            Builder.Current.Log(LogTypes.Info, $"{item}");
            item.Replace(BuilderTypes.Factory.GetMethod_____Create(BuilderTypes.Type, BuilderTypes.Type));
            item.HostMethod.NewCoder().Load(item.HostMethod.DeclaringType).End.Insert(InsertionAction.Before, item.Position);
        }

        foreach (var item in FactoryCreateUsages.createStringMethodUsages)
        {
            Builder.Current.Log(LogTypes.Info, $"{item}");
            item.Replace(BuilderTypes.Factory.GetMethod_____Create(BuilderTypes.String, BuilderTypes.Type));
            item.HostMethod.NewCoder().Load(item.HostMethod.DeclaringType).End.Insert(InsertionAction.Before, item.Position);
        }

        foreach (var item in FactoryCreateUsages.createGenericMethodUsages)
        {
            Builder.Current.Log(LogTypes.Info, $"{item}");
            ((Instruction)item).Operand = Builder.Current.Import((MethodReference)BuilderTypes.Factory.GetMethod_____Create_Generic().MakeGeneric(item.GetGenericArgument(0)));
            item.HostMethod.NewCoder().Load(item.HostMethod.DeclaringType).End.Insert(InsertionAction.Before, item.Position);
        }

        foreach (var item in FactoryCreateUsages.createTypeTypeMethodUsages)
        {
            Builder.Current.Log(LogTypes.Info, $"{item}");
            item.Replace(BuilderTypes.Factory.GetMethod_____Create(BuilderTypes.Type, FactoryCreateUsages.objectArray, BuilderTypes.Type));
            item.HostMethod.NewCoder().Load(item.HostMethod.DeclaringType).End.Insert(InsertionAction.Before, item.Position);
        }

        foreach (var item in FactoryCreateUsages.createStringTypeMethodUsages)
        {
            Builder.Current.Log(LogTypes.Info, $"{item}");
            item.Replace(BuilderTypes.Factory.GetMethod_____Create(BuilderTypes.String, FactoryCreateUsages.objectArray, BuilderTypes.Type));
            item.HostMethod.NewCoder().Load(item.HostMethod.DeclaringType).End.Insert(InsertionAction.Before, item.Position);
        }
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
}