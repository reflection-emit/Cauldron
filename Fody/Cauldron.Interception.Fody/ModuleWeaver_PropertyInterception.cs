using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.Extensions;
using Cauldron.Interception.Fody.HelperTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private static void AddPropertyGetterInterception(Builder builder, __PropertyInterceptionInfo propertyInterceptionInfo, PropertyBuilderInfo member, Field propertyField, Method actionObjectCtor, Method propertySetter, Dictionary<string, Field> interceptorFields)
        {
            var syncRoot = new __ISyncRoot(builder);
            var legalGetterInterceptors = member.InterceptorInfos.Where(x => x.InterfaceGetter != null).ToArray();
            member.Property.Getter
                .NewCode()
                    .Context(x =>
                    {
                        if (member.HasInitializer)
                            return;

                        for (int i = 0; i < legalGetterInterceptors.Length; i++)
                        {
                            var item = legalGetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).IsNull().Then(y =>
                            {
                                y.Assign(field).NewObj(item.Attribute);
                                if (item.HasSyncRootInterface)
                                    y.Load(field).As(syncRoot.Type.Import()).Call(syncRoot.SyncRoot, member.SyncRoot);
                            });
                            item.Attribute.Remove();
                        }

                        x.Load(propertyField).IsNull().Then(y =>
                            y.Assign(propertyField)
                                .NewObj(propertyInterceptionInfo.Ctor,
                                    member.Property.Getter,
                                    member.Property.Setter,
                                    member.Property.Name,
                                    member.Property.ReturnType,
                                    Crumb.This,
                                    member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                    y.NewCode().NewObj(actionObjectCtor, propertySetter)));
                    })
                    .Try(x =>
                    {
                        for (int i = 0; i < legalGetterInterceptors.Length; i++)
                        {
                            var item = legalGetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(item.InterfaceGetter.Type).Call(item.InterfaceGetter.OnGet, propertyField, member.Property.BackingField);
                        }

                        x.OriginalBody();
                    })
                    .Catch(typeof(Exception), x =>
                    {
                        for (int i = 0; i < legalGetterInterceptors.Length; i++)
                        {
                            var item = legalGetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(legalGetterInterceptors[i].InterfaceGetter.Type).Call(legalGetterInterceptors[i].InterfaceGetter.OnException, x.Exception);
                        }

                        x.Rethrow();
                    })
                    .Finally(x =>
                    {
                        for (int i = 0; i < legalGetterInterceptors.Length; i++)
                        {
                            var item = legalGetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(legalGetterInterceptors[i].InterfaceGetter.Type).Call(legalGetterInterceptors[i].InterfaceGetter.OnExit);
                        }
                    })
                    .EndTry()
                    .Return()
                .Replace();
        }

        private static void AddPropertyInitializeInterception(Builder builder, __PropertyInterceptionInfo propertyInterceptionInfo, PropertyBuilderInfo member, Field propertyField, Method actionObjectCtor, Method propertySetter, Dictionary<string, Field> interceptorFields)
        {
            var declaringType = member.Property.DeclaringType;
            var syncRoot = new __ISyncRoot(builder);
            var legalInitInterceptors = member.InterceptorInfos.Where(x => x.InterfaceInitializer != null).ToArray();
            var relevantCtors = member.Property.IsStatic ? new Method[] { declaringType.StaticConstructor } : declaringType.GetRelevantConstructors().Where(x => x.Name != ".cctor");

            foreach (var ctor in relevantCtors)
            {
                ctor.NewCode()
                    .Context(x =>
                    {
                        for (int i = 0; i < legalInitInterceptors.Length; i++)
                        {
                            var item = legalInitInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];

                            x.Assign(field).NewObj(item.Attribute);
                            if (item.HasSyncRootInterface)
                                x.Load(field).As(syncRoot.Type.Import()).Call(syncRoot.SyncRoot, member.SyncRoot);
                        }

                        x.Assign(propertyField)
                                .NewObj(propertyInterceptionInfo.Ctor,
                                    member.Property.Getter,
                                    member.Property.Setter,
                                    member.Property.Name,
                                    member.Property.ReturnType,
                                    Crumb.This,
                                    member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                    x.NewCode().NewObj(actionObjectCtor, propertySetter));

                        for (int i = 0; i < legalInitInterceptors.Length; i++)
                        {
                            var item = legalInitInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Call(field, item.InterfaceInitializer.OnInitialize, propertyField, member.Property.BackingField);
                        }
                    })
                    .Insert(InsertionPosition.Beginning);
            }
        }

        private static void AddPropertySetterInterception(Builder builder, __PropertyInterceptionInfo propertyInterceptionInfo, PropertyBuilderInfo member, Field propertyField, Method actionObjectCtor, Method propertySetter, Dictionary<string, Field> interceptorFields)
        {
            var syncRoot = new __ISyncRoot(builder);
            var legalSetterInterceptors = member.InterceptorInfos.Where(x => x.InterfaceSetter != null).ToArray();
            member.Property.Setter
                .NewCode()
                    .Context(x =>
                    {
                        if (member.HasInitializer)
                            return;

                        for (int i = 0; i < legalSetterInterceptors.Length; i++)
                        {
                            var item = legalSetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).IsNull().Then(y =>
                            {
                                y.Assign(field).NewObj(item.Attribute);

                                if (item.HasSyncRootInterface)
                                    y.Load(field).As(syncRoot.Type.Import()).Call(syncRoot.SyncRoot, member.SyncRoot);
                            });
                            item.Attribute.Remove();
                        }

                        x.Load(propertyField).IsNull().Then(y =>
                            y.Assign(propertyField)
                                .NewObj(propertyInterceptionInfo.Ctor,
                                    member.Property.Getter,
                                    member.Property.Setter,
                                    member.Property.Name,
                                    member.Property.ReturnType,
                                    Crumb.This,
                                    member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                    y.NewCode().NewObj(actionObjectCtor, propertySetter)));
                    })
                    .Try(x =>
                    {
                        for (int i = 0; i < legalSetterInterceptors.Length; i++)
                        {
                            var item = legalSetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(legalSetterInterceptors[i].InterfaceSetter.Type).Call(item.InterfaceSetter.OnSet, propertyField, member.Property.BackingField, Crumb.GetParameter(0));

                            //x.IsFalse().Then(y => y.Assign(member.Property.BackingField).Set(member.Property.Setter.NewCode().GetParameter(0)));
                            x.IsFalse().Then(y => y.OriginalBody());
                        }
                    })
                    .Catch(typeof(Exception), x =>
                    {
                        for (int i = 0; i < legalSetterInterceptors.Length; i++)
                        {
                            var item = legalSetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(legalSetterInterceptors[i].InterfaceSetter.Type).Call(legalSetterInterceptors[i].InterfaceSetter.OnException, x.Exception);
                        }

                        x.Rethrow();
                    })
                    .Finally(x =>
                    {
                        for (int i = 0; i < legalSetterInterceptors.Length; i++)
                        {
                            var item = legalSetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(legalSetterInterceptors[i].InterfaceSetter.Type).Call(legalSetterInterceptors[i].InterfaceSetter.OnExit);
                        }
                    })
                    .EndTry()
                    .Return()
                .Replace();
        }

        private static void CreatePropertySetterDelegate(Builder builder, PropertyBuilderInfo member, Method propertySetter)
        {
            var iList = new __IList(builder);
            var setterCode = propertySetter.NewCode();

            if (!member.Property.BackingField.FieldType.IsGenericType)
            {
                var extensions = new __Extensions(builder);

                if (member.Property.BackingField.FieldType.ParameterlessContructor != null && member.Property.BackingField.FieldType.ParameterlessContructor.IsPublic)
                    setterCode.Load(member.Property.BackingField).IsNull().Then(y =>
                        y.Assign(member.Property.BackingField).Set(propertySetter.NewCode()
                            .NewObj(member.Property.BackingField.FieldType.ParameterlessContructor)));

                // Only this if the property implements idisposable
                if (member.Property.BackingField.FieldType.Implements(typeof(IDisposable)))
                    setterCode.Call(extensions.TryDisposeInternal, member.Property.BackingField);

                setterCode.Load(Crumb.GetParameter(0)).IsNull().Then(x =>
                {
                    // Just clear if its clearable
                    if (member.Property.BackingField.FieldType.Implements(iList.Type.Fullname))
                        x.Load(member.Property.BackingField).Callvirt(iList.Clear).Return();
                    // Otherwise if the property is not a value type and nullable
                    else if (!member.Property.BackingField.FieldType.IsValueType || member.Property.BackingField.FieldType.IsNullable || member.Property.BackingField.FieldType.IsArray)
                        x.Assign(member.Property.BackingField).Set(null).Return();
                    else // otherwise... throw an exception
                        x.ThrowNew(typeof(NotSupportedException), "Value types does not accept null values.");
                });

                if (member.Property.BackingField.FieldType.IsArray)
                    setterCode.Load(Crumb.GetParameter(0)).Is(typeof(IEnumerable))
                        .Then(x => x.Assign(member.Property.BackingField).Set(Crumb.GetParameter(0)).Return())
                        .ThrowNew(typeof(NotSupportedException), "Value does not inherits from IEnumerable");
                else if (member.Property.BackingField.FieldType.Implements(iList.Type.Fullname) && member.Property.BackingField.FieldType.ParameterlessContructor != null)
                {
                    var addRange = member.Property.BackingField.FieldType.GetMethod("AddRange", 1, false);
                    if (addRange == null)
                    {
                        var add = member.Property.BackingField.FieldType.GetMethod("Add", 1);
                        var array = setterCode.CreateVariable(member.Property.BackingField.FieldType.ChildType.MakeArray());
                        setterCode.Assign(array).Set(Crumb.GetParameter(0));
                        setterCode.For(array, (x, item) => x.Load(member.Property.BackingField).Callvirt(add, item));
                        if (!add.ReturnType.IsVoid)
                            setterCode.Pop();
                    }
                    else
                        setterCode.Load(member.Property.BackingField).Callvirt(addRange, Crumb.GetParameter(0));
                }
                else if (member.Property.BackingField.FieldType.IsEnum)
                {
                    // Enums requires special threatment
                    setterCode.Load(Crumb.GetParameter(0)).Is(typeof(string)).Then(x =>
                    {
                        var stringVariable = setterCode.CreateVariable(typeof(string));
                        setterCode.Assign(stringVariable).Set(Crumb.GetParameter(0));
                        setterCode.Assign(member.Property.BackingField).Set(stringVariable).Return();
                    });

                    setterCode.Assign(member.Property.BackingField).Set(Crumb.GetParameter(0));
                }
                else
                    setterCode.Assign(member.Property.BackingField).Set(Crumb.GetParameter(0));
            }
            else
                setterCode.Assign(member.Property.BackingField).Set(Crumb.GetParameter(0));

            setterCode.Return().Replace();
        }

        private void ImplementTypeWidePropertyInterception(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();

            var doNotInterceptAttribute = builder.GetType("DoNotInterceptAttribute");
            var types = builder
                .FindTypesByAttributes(attributes)
                .GroupBy(x => x.Type)
                .Select(x => new
                {
                    Key = x.Key,
                    Item = x.ToArray()
                })
                .ToArray();

            foreach (var type in types)
            {
                this.Log($"Implementing interceptors in type {type.Key.Fullname}");

                foreach (var property in type.Key.Properties)
                {
                    if (property.BackingField == null)
                    {
                        this.Log(LogTypes.Error, property, "Unable to detect the backing field of property: " + property);
                        continue;
                    }

                    if (property.CustomAttributes.HasAttribute(doNotInterceptAttribute))
                    {
                        property.CustomAttributes.Remove(doNotInterceptAttribute);
                        continue;
                    }

                    for (int i = 0; i < type.Item.Length; i++)
                        property.CustomAttributes.Copy(type.Item[i].Attribute);
                }

                for (int i = 0; i < type.Item.Length; i++)
                    type.Item[i].Remove();
            }

            stopwatch.Stop();
            this.Log($"Implementing class wide property interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        private void InterceptProperties(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();

            var iPropertyInterceptorInitialize = new __IPropertyInterceptorInitialize(builder);
            var iPropertyGetterInterceptor = new __IPropertyGetterInterceptor(builder);
            var iPropertySetterInterceptor = new __IPropertySetterInterceptor(builder);
            var propertyInterceptionInfo = new __PropertyInterceptionInfo(builder);

            var properties = builder
                .FindPropertiesByAttributes(attributes)
                .GroupBy(x => x.Property)
                .Select(x => new PropertyBuilderInfo(x.Key, x.Select(y => new PropertyBuilderInfoItem(y, y.Property,
                         y.Attribute.Type.Implements(iPropertyGetterInterceptor.Type.Fullname) ? iPropertyGetterInterceptor : null,
                         y.Attribute.Type.Implements(iPropertySetterInterceptor.Type.Fullname) ? iPropertySetterInterceptor : null,
                         y.Attribute.Type.Implements(iPropertyInterceptorInitialize.Type.Fullname) ? iPropertyInterceptorInitialize : null))))
                .ToArray();

            foreach (var member in properties)
            {
                this.Log($"Implementing interceptors in property {member.Property}");

                if (member.Property.BackingField == null)
                {
                    this.Log(LogTypes.Error, member.Property, "Unable to detect the backing field of property: " + member.Property);
                    continue;
                }

                var propertyField = member.Property.CreateField(propertyInterceptionInfo.Type, $"<{member.Property.Name}>p__propertyInfo");

                var actionObjectCtor = builder.Import(typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
                var propertySetter = member.Property.DeclaringType.CreateMethod(member.Property.Modifiers.GetPrivate(), $"<{member.Property.Name}>m__setterMethod", builder.GetType(typeof(object)));

                CreatePropertySetterDelegate(builder, member, propertySetter);

                var indexer = 0;
                var interceptorFields = member.InterceptorInfos.ToDictionary(x => x.Attribute.Identification,
                    x => member.Property.DeclaringType.CreateField(x.Property.Modifiers.GetPrivate(), x.Attribute.Attribute.Type,
                        $"<{x.Property.Name}>_attrib{indexer++}_{x.Attribute.Identification}"));

                if (member.HasInitializer)
                    AddPropertyInitializeInterception(builder, propertyInterceptionInfo, member, propertyField, actionObjectCtor, propertySetter, interceptorFields);

                if (member.HasGetterInterception)
                    AddPropertyGetterInterception(builder, propertyInterceptionInfo, member, propertyField, actionObjectCtor, propertySetter, interceptorFields);

                if (member.HasSetterInterception)
                    AddPropertySetterInterception(builder, propertyInterceptionInfo, member, propertyField, actionObjectCtor, propertySetter, interceptorFields);

                // Do this at the end to ensure that syncroot init is always on the top
                if (member.RequiresSyncRootField)
                {
                    if (member.SyncRoot.IsStatic)
                        member.Property.DeclaringType.CreateStaticConstructor().NewCode()
                            .Assign(member.SyncRoot).NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor)
                            .Insert(InsertionPosition.Beginning);
                    else
                        foreach (var ctors in member.Property.DeclaringType.GetRelevantConstructors().Where(x => x.Name == ".ctor"))
                            ctors.NewCode().Assign(member.SyncRoot).NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor).Insert(InsertionPosition.Beginning);
                }

                // Also remove the compilergenerated attribute
                member.Property.Getter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
                member.Property.Setter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
            }

            stopwatch.Stop();
            this.Log($"Implementing property interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }
    }
}