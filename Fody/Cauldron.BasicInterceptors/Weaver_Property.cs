using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static class Weaver_Property
{
    public static string Name = "Property Interceptors";

    public static int Priority = 1;

    static Weaver_Property()
    {
        PropertyInterceptingAttributes = Builder.Current.FindAttributesByInterfaces(
        "Cauldron.Interception.IPropertyInterceptor",
        "Cauldron.Interception.IPropertyGetterInterceptor",
        "Cauldron.Interception.IPropertySetterInterceptor");
    }

    internal static IEnumerable<BuilderType> PropertyInterceptingAttributes { get; private set; }

    [Display("Type-Wide Property Interception")]
    public static void ImplementTypeWidePropertyInterception(Builder builder)
    {
        if (!PropertyInterceptingAttributes.Any())
            return;

        var types = builder
            .FindTypesByAttributes(PropertyInterceptingAttributes)
            .GroupBy(x => x.Type)
            .Select(x => new
            {
                x.Key,
                Item = x.ToArray()
            })
            .ToArray();

        foreach (var type in types)
        {
            builder.Log(LogTypes.Info, $"Implementing interceptors in type {type.Key.Fullname}");

            foreach (var property in type.Key.Properties)
            {
                for (int i = 0; i < type.Item.Length; i++)
                    property.CustomAttributes.Copy(type.Item[i].Attribute);
            }

            for (int i = 0; i < type.Item.Length; i++)
                type.Item[i].Remove();
        }
    }

    [Display("Property Interception")]
    public static void InterceptProperties(Builder builder)
    {
        if (!PropertyInterceptingAttributes.Any())
            return;

        var propertyInterceptionInfo = __PropertyInterceptionInfo.Instance;

        var properties = builder
            .FindPropertiesByAttributes(PropertyInterceptingAttributes)
            .GroupBy(x => x.Property)
            .Select(x => new PropertyBuilderInfo(x.Key, x.Select(y => new PropertyBuilderInfoItem(y, y.Property,
                     y.Attribute.Type.Implements(__IPropertyGetterInterceptor.Type.Fullname) ? __IPropertyGetterInterceptor.Instance : null,
                     y.Attribute.Type.Implements(__IPropertySetterInterceptor.Type.Fullname) ? __IPropertySetterInterceptor.Instance : null,
                     y.Attribute.Type.Implements(__IPropertyInterceptorInitialize.Type.Fullname) ? __IPropertyInterceptorInitialize.Instance : null))))
            .ToArray();

        foreach (var member in properties)
        {
            builder.Log(LogTypes.Info, $"Implementing property interceptors: {member.Property.DeclaringType.Name.PadRight(40, ' ')} {member.Property.Name} {member.Property.ReturnType.Name}");

            if (!member.HasGetterInterception && !member.HasSetterInterception && !member.HasInitializer)
                continue;

            var propertyField = member.Property.CreateField(__PropertyInterceptionInfo.Type, $"<{member.Property.Name}>p__propertyInfo");
            propertyField.CustomAttributes.AddNonSerializedAttribute();

            Method GetOrCreatePropertyValueComparerDelegate()
            {
                if (!member.HasComparer)
                    return null;

                var methodName = $"<{member.Property.ReturnType.Fullname.Replace('.', '_').Replace('`', '_')}>__comparerMethod";
                var originType = member.Property.OriginType;

                if (originType.GetMethod(methodName, 2, false) is Method result)
                    return result;

                return originType.CreateMethod(
                    member.Property.Modifiers.GetPrivate(),
                    builder.GetType(typeof(bool)),
                    methodName,
                    builder.GetType(typeof(object)), builder.GetType(typeof(object)));
            }

            var actionObjectCtor = builder.Import(typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
            var propertySetter = member.Property.Setter == null ?
                null :
                member.Property.OriginType.CreateMethod(member.Property.Modifiers.GetPrivate(), $"<{member.Property.Name}>m__setterMethod", builder.GetType(typeof(object)));
            var propertyValueComparer = GetOrCreatePropertyValueComparerDelegate();

            if (propertySetter != null)
                CreatePropertySetterDelegate(builder, member, propertySetter);

            if (propertyValueComparer != null)
                CreateEqualityComparerDelegate(builder, propertyValueComparer, member.Property.ReturnType);

            var indexer = 0;
            var interceptorFields = member.InterceptorInfos.ToDictionary(x => x.Attribute.Identification,
                x =>
                {
                    var field = member.Property.OriginType.CreateField(x.Property.Modifiers.GetPrivate(), x.Attribute.Attribute.Type,
                           $"<{x.Property.Name}>_attrib{indexer++}_{x.Attribute.Identification}");

                    field.CustomAttributes.AddNonSerializedAttribute();
                    return field;
                });

            if (member.HasInitializer)
                AddPropertyInitializeInterception(builder, propertyInterceptionInfo, member, propertyField, actionObjectCtor, propertySetter, propertyValueComparer, interceptorFields);

            if (member.HasGetterInterception && member.Property.Getter != null)
                AddPropertyGetterInterception(builder, propertyInterceptionInfo, member, propertyField, actionObjectCtor, propertySetter, propertyValueComparer, interceptorFields);

            if (member.HasSetterInterception && member.Property.Setter != null)
                AddPropertySetterInterception(builder, propertyInterceptionInfo, member, propertyField, actionObjectCtor, propertySetter, propertyValueComparer, interceptorFields);

            // Do this at the end to ensure that syncroot init is always on the top
            if (member.RequiresSyncRootField)
            {
                if (member.SyncRoot.IsStatic)
                    member.Property.OriginType.CreateStaticConstructor().NewCoder()
                        .SetValue(member.SyncRoot, x => x.NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor))
                        .Insert(InsertionPosition.Beginning);
                else
                    foreach (var ctors in member.Property.OriginType.GetRelevantConstructors().Where(x => x.Name == ".ctor"))
                        ctors.NewCoder().SetValue(member.SyncRoot, x => x.NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor)).Insert(InsertionPosition.Beginning);
            }

            // Also remove the compilergenerated attribute
            member.Property.Getter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
            member.Property.Setter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
        }
    }

    private static void AddPropertyGetterInterception(
        Builder builder,
        __PropertyInterceptionInfo propertyInterceptionInfo,
        PropertyBuilderInfo member,
        Field propertyField,
        Method actionObjectCtor,
        Method propertySetter,
        Method propertyComparer,
        Dictionary<string, Field> interceptorFields)
    {
        var syncRoot = new __ISyncRoot();
        var propertyInterceptorComparer = new __IPropertyInterceptorComparer();
        var legalGetterInterceptors = member.InterceptorInfos.Where(x => x.InterfaceGetter != null).ToArray();
        var propertyInterceptorFunc = propertyInterceptorComparer.GetAreEqual.ReturnType.GetMethod(".ctor", true, new Type[] { typeof(object), typeof(IntPtr) })
                    .MakeGeneric(propertyInterceptorComparer.GetAreEqual.ReturnType.GenericArguments().ToArray())
                    .Import();

        member.Property.Getter
            .NewCoder()
                .Context(context =>
                {
                    if (member.HasInitializer)
                        return context;

                    for (int i = 0; i < legalGetterInterceptors.Length; i++)
                    {
                        var item = legalGetterInterceptors[i];
                        var field = interceptorFields[item.Attribute.Identification];

                        context.If(x => x.Load(field).IsNull(), then =>
                        {
                            then.SetValue(field, x => x.NewObj(item.Attribute));

                            if (item.HasSyncRootInterface)
                                then.Load(field).As(__ISyncRoot.Type.Import()).Call(syncRoot.SyncRoot, member.SyncRoot);

                            if (item.HasComparer)
                                context.Load(field).As(__IPropertyInterceptorComparer.Type.Import())
                                    .Call(propertyInterceptorComparer.SetAreEqual,
                                        x => x.NewObj(propertyInterceptorFunc, propertyComparer));

                            ModuleWeaver.ImplementAssignMethodAttribute(builder, legalGetterInterceptors[i].AssignMethodAttributeInfos, field, item.Attribute.Attribute.Type, then);
                            return then;
                        });
                        item.Attribute.Remove();
                    }

                    return context.If(x => x.Load(propertyField).IsNull(), then =>
                          then.SetValue(propertyField, x =>
                              x.NewObj(propertyInterceptionInfo.Ctor,
                                  member.Property.Getter,
                                  member.Property.Setter,
                                  member.Property.Name,
                                  member.Property.ReturnType,
                                  CodeBlocks.This,
                                  member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                  propertySetter == null ? null : then.NewCoder().NewObj(actionObjectCtor, propertySetter.ThisOrNull(), propertySetter))));
                })
                .Try(@try =>
                {
                    if (member.Property.BackingField == null)
                    {
                        var returnValue = @try.GetOrCreateReturnVariable();
                        @try.SetValue(returnValue, x => x.OriginalBody(true));

                        for (int i = 0; i < legalGetterInterceptors.Length; i++)
                        {
                            var item = legalGetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            @try.Load(field).As(item.InterfaceGetter.ToBuilderType).Call(item.InterfaceGetter.OnGet, propertyField, returnValue);
                        }

                        @try.Load(returnValue).Return();
                    }
                    else
                    {
                        for (int i = 0; i < legalGetterInterceptors.Length; i++)
                        {
                            var item = legalGetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            @try.Load(field).As(item.InterfaceGetter.ToBuilderType).Call(item.InterfaceGetter.OnGet, propertyField, member.Property.BackingField);
                        }

                        @try.OriginalBody();
                    }

                    return @try;
                })
                .Catch(typeof(Exception), (ex, e) =>
                {
                    return ex.If(x => x.Or(legalGetterInterceptors, (coder, y, i) => coder.Load(interceptorFields[legalGetterInterceptors[i].Attribute.Identification])
                          .As(legalGetterInterceptors[i].InterfaceGetter.ToBuilderType)
                          .Call(legalGetterInterceptors[i].InterfaceGetter.OnException, e())).Is(true),
                          then => ex.NewCoder().Rethrow())
                      .DefaultValue()
                      .Return();
                })
                .Finally(x =>
                {
                    for (int i = 0; i < legalGetterInterceptors.Length; i++)
                    {
                        var item = legalGetterInterceptors[i];
                        var field = interceptorFields[item.Attribute.Identification];
                        x.Load(field).As(legalGetterInterceptors[i].InterfaceGetter.ToBuilderType).Call(legalGetterInterceptors[i].InterfaceGetter.OnExit);
                    }

                    return x;
                })
                .EndTry()
                .Return()
            .Replace();
    }

    private static void AddPropertyInitializeInterception(
        Builder builder,
        __PropertyInterceptionInfo propertyInterceptionInfo,
        PropertyBuilderInfo member,
        Field propertyField,
        Method actionObjectCtor,
        Method propertySetter,
        Method propertyComparer,
        Dictionary<string, Field> interceptorFields)
    {
        var declaringType = member.Property.OriginType;
        var syncRoot = new __ISyncRoot();
        var propertyInterceptorComparer = new __IPropertyInterceptorComparer();
        var legalInitInterceptors = member.InterceptorInfos.Where(x => x.InterfaceInitializer != null).ToArray();
        var relevantCtors = member.Property.IsStatic ? new Method[] { declaringType.StaticConstructor } : declaringType.GetRelevantConstructors().Where(x => x.Name != ".cctor");
        var propertyInterceptorFunc = propertyInterceptorComparer.GetAreEqual.ReturnType.GetMethod(".ctor", true, new Type[] { typeof(object), typeof(IntPtr) })
                    .MakeGeneric(propertyInterceptorComparer.GetAreEqual.ReturnType.GenericArguments().ToArray())
                    .Import();

        foreach (var ctor in relevantCtors)
        {
            ctor.NewCoder()
                .Context(context =>
                {
                    for (int i = 0; i < legalInitInterceptors.Length; i++)
                    {
                        var item = legalInitInterceptors[i];
                        var field = interceptorFields[item.Attribute.Identification];

                        context.SetValue(field, x => x.NewObj(item.Attribute));

                        if (item.HasSyncRootInterface)
                            context.Load(field).As(__ISyncRoot.Type.Import()).Call(syncRoot.SyncRoot, member.SyncRoot);

                        if (item.HasComparer)
                            context.Load(field).As(__IPropertyInterceptorComparer.Type.Import())
                                .Call(propertyInterceptorComparer.SetAreEqual, x => x.NewObj(propertyInterceptorFunc, propertyComparer));

                        ModuleWeaver.ImplementAssignMethodAttribute(builder, legalInitInterceptors[i].AssignMethodAttributeInfos, field, item.Attribute.Attribute.Type, context);
                    }

                    context.SetValue(propertyField, x =>
                            x.NewObj(propertyInterceptionInfo.Ctor,
                                member.Property.Getter,
                                member.Property.Setter,
                                member.Property.Name,
                                member.Property.ReturnType,
                                CodeBlocks.This,
                                member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                propertySetter == null ? null : x.NewCoder().NewObj(actionObjectCtor, propertySetter.ThisOrNull(), propertySetter)));

                    for (int i = 0; i < legalInitInterceptors.Length; i++)
                    {
                        var item = legalInitInterceptors[i];
                        var field = interceptorFields[item.Attribute.Identification];
                        context.Load(field).As(item.InterfaceInitializer.ToBuilderType)
                            .Call(item.InterfaceInitializer.OnInitialize, propertyField, member.Property.BackingField);
                    }

                    return context;
                })
                .Insert(InsertionPosition.Beginning);
        }
    }

    private static void AddPropertySetterInterception(
        Builder builder,
        __PropertyInterceptionInfo propertyInterceptionInfo,
        PropertyBuilderInfo member,
        Field propertyField,
        Method actionObjectCtor,
        Method propertySetter,
        Method propertyComparer,
        Dictionary<string, Field> interceptorFields)
    {
        var syncRoot = new __ISyncRoot();
        var propertyInterceptorComparer = new __IPropertyInterceptorComparer();
        var legalSetterInterceptors = member.InterceptorInfos.Where(x => x.InterfaceSetter != null).ToArray();
        var propertyInterceptorFunc = propertyInterceptorComparer.GetAreEqual.ReturnType.GetMethod(".ctor", true, new Type[] { typeof(object), typeof(IntPtr) })
                .MakeGeneric(propertyInterceptorComparer.GetAreEqual.ReturnType.GenericArguments().ToArray())
                .Import();

        member.Property.Setter
            .NewCoder()
                .Context(context =>
                {
                    if (member.HasInitializer)
                        return context;

                    for (int i = 0; i < legalSetterInterceptors.Length; i++)
                    {
                        var item = legalSetterInterceptors[i];
                        var field = interceptorFields[item.Attribute.Identification];
                        context.If(x => x.Load(field).IsNull(), then =>
                         {
                             then.SetValue(field, x => x.NewObj(item.Attribute));

                             if (item.HasSyncRootInterface)
                                 then.Load(field).As(syncRoot.ToBuilderType.Import()).Call(syncRoot.SyncRoot, member.SyncRoot);

                             if (item.HasComparer)
                                 then.Load(field).As(__IPropertyInterceptorComparer.Type.Import())
                                     .Call(propertyInterceptorComparer.SetAreEqual,
                                         x => x.NewObj(propertyInterceptorFunc, propertyComparer));

                             ModuleWeaver.ImplementAssignMethodAttribute(builder, legalSetterInterceptors[i].AssignMethodAttributeInfos, field, item.Attribute.Attribute.Type, then);

                             return then;
                         });
                        item.Attribute.Remove();
                    }

                    return context.If(x => x.Load(propertyField).IsNull(), then =>
                         then.SetValue(propertyField, x =>
                             x.NewObj(propertyInterceptionInfo.Ctor,
                                 member.Property.Getter,
                                 member.Property.Setter,
                                 member.Property.Name,
                                 member.Property.ReturnType,
                                 CodeBlocks.This,
                                 member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                 propertySetter == null ? null : x.NewCoder().NewObj(actionObjectCtor, propertySetter.ThisOrNull(), propertySetter))));
                })
                .Try(@try =>
                {
                    if (member.Property.BackingField == null)
                    {
                        // If we don't have a backing field, we will try getting the value from the
                        // getter itself... But in this case we have to watch out that we don't accidentally code a
                        // StackOverFlow
                        var oldvalue = member.Property.Getter == null ? null : member.Property.Setter.GetOrCreateVariable(member.Property.ReturnType);

                        if (oldvalue != null)
                        {
                            var getter = member.Property.Getter.Copy();
                            @try.SetValue(oldvalue, y => y.Call(getter));
                        }

                        for (int i = 0; i < legalSetterInterceptors.Length; i++)
                        {
                            var item = legalSetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            @try.If(x =>
                               x.Load(field)
                                   .As(legalSetterInterceptors[i].InterfaceSetter.ToBuilderType)
                                   .Call(item.InterfaceSetter.OnSet, propertyField, oldvalue, CodeBlocks.GetParameter(0))
                                   .Is(false), then => then.OriginalBody(true));
                        }
                    }
                    else
                    {
                        @try.If(x => x.And(legalSetterInterceptors,
                            (coder, item, i) => coder.Load(interceptorFields[item.Attribute.Identification])
                                    .As(legalSetterInterceptors[i].InterfaceSetter.ToBuilderType)
                                    .Call(item.InterfaceSetter.OnSet, propertyField, member.Property.BackingField, CodeBlocks.GetParameter(0)))
                                        .Is(false),
                                    then => then.OriginalBody());
                    }

                    return @try;
                })
                .Catch(typeof(Exception), (ex, e) =>
                {
                    return ex.If(x => x.Or(legalSetterInterceptors,
                        (coder, y, i) => coder.Load(interceptorFields[legalSetterInterceptors[i].Attribute.Identification])
                            .As(legalSetterInterceptors[i].InterfaceSetter.ToBuilderType)
                            .Call(legalSetterInterceptors[i].InterfaceSetter.OnException, e()))
                                .Is(true),
                            then => ex.NewCoder().Rethrow())
                    .DefaultValue()
                    .Return();
                })
                .Finally(x =>
                {
                    for (int i = 0; i < legalSetterInterceptors.Length; i++)
                    {
                        var item = legalSetterInterceptors[i];
                        var field = interceptorFields[item.Attribute.Identification];
                        x.Load(field).As(legalSetterInterceptors[i].InterfaceSetter.ToBuilderType).Call(legalSetterInterceptors[i].InterfaceSetter.OnExit);
                    }

                    return x;
                })
                .EndTry()
                .Return()
            .Replace();
    }

    private static void CreateEqualityComparerDelegate(Builder builder, Method equalityComparerMethod, BuilderType propertyType)
    {
        var coder = equalityComparerMethod.NewCoder();

        if (propertyType.IsValueType || propertyType.IsPrimitive)
            coder.If(x => x.Load(CodeBlocks.GetParameter(0)).Is(CodeBlocks.GetParameter(1)), then => then.Return());
        else
        {
            var methodReferenceEqual = builder.GetType("System.Object").GetMethod("ReferenceEquals", false, "System.Object", "System.Object").Import();
            coder.If(x => x.Call(methodReferenceEqual, CodeBlocks.GetParameter(0), CodeBlocks.GetParameter(1)).Is(true), then => then.Load(true).Return());
            coder.If(x => x.Load(CodeBlocks.GetParameter(0)).Is(CodeBlocks.GetParameter(1)), then => then.Return());
        }

        coder.Replace();
    }

    private static void CreatePropertySetterDelegate(Builder builder, PropertyBuilderInfo member, Method propertySetter)
    {
        // If we don't have a backing field and we don't have a setter and getter
        // don't bother creating a setter delegate
        if (member.Property.BackingField == null && propertySetter == null)
            return;

        if (member.Property.BackingField == null && member.Property.Getter != null && member.Property.Setter != null)
        {
            var getter = member.Property.Getter.Copy();
            var setter = member.Property.Setter.Copy();

            CreateSetterDelegate(builder, propertySetter, member.Property.ReturnType, member.Property);
        }
        else if (member.Property.BackingField != null && !member.Property.BackingField.FieldType.IsGenericType)
        {
            CreateSetterDelegate(builder, propertySetter, member.Property.BackingField.FieldType, member.Property.BackingField);
        }
        else if (member.Property.BackingField == null && member.Property.Setter != null)
        {
            var methodSetter = member.Property.Setter.Copy();
            propertySetter.NewCoder().Call(methodSetter, CodeBlocks.GetParameter(0)).Return().Replace();
        }
        else if (member.Property.BackingField == null && member.Property.Getter != null)
        {
            // This shouldn't be a thing
        }
        else
            propertySetter.NewCoder().SetValue(member.Property.BackingField, CodeBlocks.GetParameter(0)).Return().Replace();
    }

    private static void CreateSetterDelegate(Builder builder, Method setterDelegateMethod, BuilderType propertyType, object value)
    {
        var extensions = new __Extensions();
        var iList = new __IList();
        var setterCode = setterDelegateMethod.NewCoder();

        T CodeMe<T>(Func<Field, T> fieldCode, Func<Property, T> propertyCode) where T : class
        {
            switch (value)
            {
                case Field field: return fieldCode(field);
                case Property property: return propertyCode(property);
                default: return null;
            }
        }

        if (propertyType.ParameterlessContructor != null && propertyType.ParameterlessContructor.IsPublic)
            CodeMe(
                field => setterCode.If(x => x.Load(field).IsNull(), then => then.SetValue(field, x => x.NewObj(propertyType.ParameterlessContructor))),
                property => setterCode.If(x => x.Call(property.Getter).IsNull(), then => then.Call(property.Setter, x => x.NewObj(propertyType.ParameterlessContructor))));

        // Only this if the property implements idisposable
        if (propertyType.Implements(typeof(IDisposable)))
            CodeMe(
                field => setterCode.Call(extensions.TryDisposeInternal, x => x.Load(field)),
                property => setterCode.Call(extensions.TryDisposeInternal, x => x.Call(property.Getter)));

        setterCode.If(x => x.Load(CodeBlocks.GetParameter(0)).IsNull(), then =>
        {
            // Just clear if its clearable
            if (propertyType.Implements(__IList.Type.Fullname))
                CodeMe(
                    field => setterCode.Load(field).Call(iList.Clear).Return(),
                    property => setterCode.Call(property.Getter).Call(iList.Clear).Return());
            // Otherwise if the property is not a value type and nullable
            else if (!propertyType.IsValueType || propertyType.IsNullable || propertyType.IsArray)
                CodeMe<CoderBase>(
                    field => setterCode.SetValue(field, null),
                    property => setterCode.Call(property.Setter, null));
            else // otherwise... throw an exception
                then.ThrowNew(typeof(NotSupportedException), "Value types does not accept null values.");

            return then;
        });

        if (propertyType.IsArray)
            setterCode.If(x => x.Load(CodeBlocks.GetParameter(0)).Is(typeof(IEnumerable)), then =>
               CodeMe(
                   field => then.SetValue(field, CodeBlocks.GetParameter(0)).Return(),
                   property => then.Call(property.Setter, CodeBlocks.GetParameter(0)).Return()))
               .ThrowNew(typeof(NotSupportedException), "Value does not inherits from IEnumerable");
        else if (propertyType.Implements(__IList.Type.Fullname) && propertyType.ParameterlessContructor != null)
        {
            var addRange = propertyType.GetMethod("AddRange", 1, false);
            if (addRange == null)
            {
                var add = propertyType.GetMethod("Add", 1);
                var array = setterDelegateMethod.GetOrCreateVariable(propertyType.ChildType.MakeArray());
                setterCode.SetValue(array, CodeBlocks.GetParameter(0));
                setterCode.For(array, (x, item) => CodeMe(
                    field => x.Load(field).Call(add, item),
                    property => x.Call(property.Getter).Call(add, item)));
                if (!add.ReturnType.IsVoid)
                    setterCode.Pop();
            }
            else
                CodeMe(
                    field => setterCode.Load(field).Call(addRange, CodeBlocks.GetParameter(0)),
                    property => setterCode.Call(property.Getter).Call(addRange, CodeBlocks.GetParameter(0)));
        }
        else if (propertyType.IsEnum)
        {
            // Enums requires special threatment
            setterCode.If(x => x.Load(CodeBlocks.GetParameter(0)).Is(typeof(string)),
                then =>
                {
                    var stringVariable = setterDelegateMethod.GetOrCreateVariable(typeof(string));
                    then.SetValue(stringVariable, CodeBlocks.GetParameter(0));
                    CodeMe( // Cecilator automagically implements a convertion for this
                        field => then.SetValue(field, stringVariable).Return(),
                        property => then.Call(property.Setter, stringVariable).Return());
                    return then;
                });

            CodeMe<CoderBase>(
                field => setterCode.SetValue(field, CodeBlocks.GetParameter(0)),
                property => setterCode.Call(property.Setter, CodeBlocks.GetParameter(0)));
        }
        else
            CodeMe<CoderBase>(
                field => setterCode.SetValue(field, CodeBlocks.GetParameter(0)),
                property => setterCode.Call(property.Setter, CodeBlocks.GetParameter(0)));

        setterCode.Return().Replace();
    }
}