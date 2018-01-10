using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.Extensions;
using Cauldron.Interception.Fody.HelperTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private static void AddPropertyGetterInterception(Builder builder, __PropertyInterceptionInfo propertyInterceptionInfo, PropertyBuilderInfo member, Field propertyField, Method actionObjectCtor, Method propertySetter, Dictionary<string, Field> interceptorFields)
        {
            var syncRoot = new __ISyncRoot();
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
                                    y.Load(field).As(__ISyncRoot.Type.Import()).Call(syncRoot.SyncRoot, member.SyncRoot);

                                ImplementAssignMethodAttribute(builder, legalGetterInterceptors[i].AssignMethodAttributeInfos, field, item.Attribute.Attribute.Type, y);
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
                                    propertySetter == null ? null : y.NewCode().NewObj(actionObjectCtor, propertySetter)));
                    })
                    .Try(x =>
                    {
                        if (member.Property.BackingField == null)
                        {
                            var returnValue = x.GetReturnVariable();
                            x.OriginalBodyNewMethod().StoreLocal(returnValue);

                            for (int i = 0; i < legalGetterInterceptors.Length; i++)
                            {
                                var item = legalGetterInterceptors[i];
                                var field = interceptorFields[item.Attribute.Identification];
                                x.Load(field).As(item.InterfaceGetter.ToBuilderType).Call(item.InterfaceGetter.OnGet, propertyField, returnValue);
                            }

                            x.Load(returnValue).Return();
                        }
                        else
                        {
                            for (int i = 0; i < legalGetterInterceptors.Length; i++)
                            {
                                var item = legalGetterInterceptors[i];
                                var field = interceptorFields[item.Attribute.Identification];
                                x.Load(field).As(item.InterfaceGetter.ToBuilderType).Call(item.InterfaceGetter.OnGet, propertyField, member.Property.BackingField);
                            }

                            x.OriginalBody();
                        }
                    })
                    .Catch(typeof(Exception), x =>
                    {
                        for (int i = 0; i < legalGetterInterceptors.Length; i++)
                        {
                            var item = legalGetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(legalGetterInterceptors[i].InterfaceGetter.ToBuilderType).Call(legalGetterInterceptors[i].InterfaceGetter.OnException, x.Exception);

                            if (legalGetterInterceptors.Length - 1 < i)
                                x.Or();
                        }

                        x.IsTrue().Then(y => x.Rethrow());
                        x.ReturnDefault();
                    })
                    .Finally(x =>
                    {
                        for (int i = 0; i < legalGetterInterceptors.Length; i++)
                        {
                            var item = legalGetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(legalGetterInterceptors[i].InterfaceGetter.ToBuilderType).Call(legalGetterInterceptors[i].InterfaceGetter.OnExit);
                        }
                    })
                    .EndTry()
                    .Return()
                .Replace();
        }

        private static void AddPropertyInitializeInterception(Builder builder, __PropertyInterceptionInfo propertyInterceptionInfo, PropertyBuilderInfo member, Field propertyField, Method actionObjectCtor, Method propertySetter, Dictionary<string, Field> interceptorFields)
        {
            var declaringType = member.Property.OriginType;
            var syncRoot = new __ISyncRoot();
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
                                x.Load(field).As(__ISyncRoot.Type.Import()).Call(syncRoot.SyncRoot, member.SyncRoot);

                            ImplementAssignMethodAttribute(builder, legalInitInterceptors[i].AssignMethodAttributeInfos, field, item.Attribute.Attribute.Type, x);
                        }

                        x.Assign(propertyField)
                                .NewObj(propertyInterceptionInfo.Ctor,
                                    member.Property.Getter,
                                    member.Property.Setter,
                                    member.Property.Name,
                                    member.Property.ReturnType,
                                    Crumb.This,
                                    member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                    propertySetter == null ? null : x.NewCode().NewObj(actionObjectCtor, propertySetter));

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
            var syncRoot = new __ISyncRoot();
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
                                    y.Load(field).As(syncRoot.ToBuilderType.Import()).Call(syncRoot.SyncRoot, member.SyncRoot);

                                ImplementAssignMethodAttribute(builder, legalSetterInterceptors[i].AssignMethodAttributeInfos, field, item.Attribute.Attribute.Type, y);
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
                                    propertySetter == null ? null : y.NewCode().NewObj(actionObjectCtor, propertySetter)));
                    })
                    .Try(x =>
                    {
                        if (member.Property.BackingField == null)
                        {
                            var oldvalue = member.Property.Getter == null ? null : x.CreateVariable(member.Property.ReturnType);

                            if (oldvalue != null)
                            {
                                var getter = member.Property.Getter.Copy();
                                x.Call(getter.IsStatic ? null : Crumb.This, getter).StoreLocal(oldvalue);
                            }

                            for (int i = 0; i < legalSetterInterceptors.Length; i++)
                            {
                                var item = legalSetterInterceptors[i];
                                var field = interceptorFields[item.Attribute.Identification];
                                x.Load(field).As(legalSetterInterceptors[i].InterfaceSetter.ToBuilderType).Call(item.InterfaceSetter.OnSet, propertyField, oldvalue, Crumb.GetParameter(0));

                                x.IsFalse().Then(y => y.OriginalBodyNewMethod());
                            }
                        }
                        else
                            for (int i = 0; i < legalSetterInterceptors.Length; i++)
                            {
                                var item = legalSetterInterceptors[i];
                                var field = interceptorFields[item.Attribute.Identification];
                                x.Load(field).As(legalSetterInterceptors[i].InterfaceSetter.ToBuilderType).Call(item.InterfaceSetter.OnSet, propertyField, member.Property.BackingField, Crumb.GetParameter(0));

                                x.IsFalse().Then(y => y.OriginalBody());
                            }
                    })
                    .Catch(typeof(Exception), x =>
                    {
                        for (int i = 0; i < legalSetterInterceptors.Length; i++)
                        {
                            var item = legalSetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(legalSetterInterceptors[i].InterfaceSetter.ToBuilderType).Call(legalSetterInterceptors[i].InterfaceSetter.OnException, x.Exception);

                            if (legalSetterInterceptors.Length - 1 < i)
                                x.Or();
                        }

                        x.IsTrue().Then(y => x.Rethrow());
                        x.Return();
                    })
                    .Finally(x =>
                    {
                        for (int i = 0; i < legalSetterInterceptors.Length; i++)
                        {
                            var item = legalSetterInterceptors[i];
                            var field = interceptorFields[item.Attribute.Identification];
                            x.Load(field).As(legalSetterInterceptors[i].InterfaceSetter.ToBuilderType).Call(legalSetterInterceptors[i].InterfaceSetter.OnExit);
                        }
                    })
                    .EndTry()
                    .Return()
                .Replace();
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

                CreateSetterDelegate(builder, propertySetter, member.Property.ReturnType,
                    x => x.Call(getter.IsStatic ? null : Crumb.This, getter),
                    (coder, value) => coder.Call(setter.IsStatic ? null : Crumb.This, setter, value()));
            }
            else if (member.Property.BackingField != null && !member.Property.BackingField.FieldType.IsGenericType)
            {
                CreateSetterDelegate(builder, propertySetter, member.Property.BackingField.FieldType,
                    x => x.Load(member.Property.BackingField) as ICode,
                    (coder, value) => coder.Assign(member.Property.BackingField).Set(value()));
            }
            else if (member.Property.BackingField == null && member.Property.Setter != null)
            {
                var methodSetter = member.Property.Setter.Copy();
                propertySetter.NewCode().Call(methodSetter.IsStatic ? null : Crumb.This, methodSetter, Crumb.GetParameter(0)).Return().Replace();
            }
            else if (member.Property.BackingField == null && member.Property.Getter != null)
            {
                // This shouldn't be a thing
            }
            else
                propertySetter.NewCode().Assign(member.Property.BackingField).Set(Crumb.GetParameter(0)).Return().Replace();
        }

        private static void CreateSetterDelegate(Builder builder, Method setterDelegateMethod, BuilderType propertyType,
                    Func<ICode, ICode> loadValue, Func<ICode, Func<object>, ICode> setValue)
        {
            var extensions = new __Extensions();
            var iList = new __IList();
            var setterCode = setterDelegateMethod.NewCode();

            if (propertyType.ParameterlessContructor != null && propertyType.ParameterlessContructor.IsPublic)
                loadValue(setterCode).IsNull().Then(y => setValue(y, () => setterCode.NewCode().NewObj(propertyType.ParameterlessContructor)));

            // Only this if the property implements idisposable
            if (propertyType.Implements(typeof(IDisposable)))
                setterCode.Call(extensions.TryDisposeInternal, loadValue(setterCode.NewCode()));

            setterCode.Load(Crumb.GetParameter(0)).IsNull().Then(x =>
            {
                // Just clear if its clearable
                if (propertyType.Implements(__IList.Type.Fullname))
                    loadValue(x).Callvirt(iList.Clear).Return();
                // Otherwise if the property is not a value type and nullable
                else if (!propertyType.IsValueType || propertyType.IsNullable || propertyType.IsArray)
                    setValue(x, () => null).Return();
                else // otherwise... throw an exception
                    x.ThrowNew(typeof(NotSupportedException), "Value types does not accept null values.");
            });

            if (propertyType.IsArray)
                setterCode.Load(Crumb.GetParameter(0)).Is(typeof(IEnumerable))
                    .Then(x => setValue(x, () => Crumb.GetParameter(0)).Return())
                    .ThrowNew(typeof(NotSupportedException), "Value does not inherits from IEnumerable");
            else if (propertyType.Implements(__IList.Type.Fullname) && propertyType.ParameterlessContructor != null)
            {
                var addRange = propertyType.GetMethod("AddRange", 1, false);
                if (addRange == null)
                {
                    var add = propertyType.GetMethod("Add", 1);
                    var array = setterCode.CreateVariable(propertyType.ChildType.MakeArray());
                    setterCode.Assign(array).Set(Crumb.GetParameter(0));
                    setterCode.For(array, (x, item) => loadValue(x).Callvirt(add, item));
                    if (!add.ReturnType.IsVoid)
                        setterCode.Pop();
                }
                else
                    loadValue(setterCode).Callvirt(addRange, Crumb.GetParameter(0));
            }
            else if (propertyType.IsEnum)
            {
                // Enums requires special threatment
                setterCode.Load(Crumb.GetParameter(0)).Is(typeof(string)).Then(x =>
                {
                    var stringVariable = setterCode.CreateVariable(typeof(string));
                    setterCode.Assign(stringVariable).Set(Crumb.GetParameter(0));
                    setValue(setterCode, () => stringVariable).Return();
                });

                setValue(setterCode, () => Crumb.GetParameter(0));
            }
            else
                setValue(setterCode, () => Crumb.GetParameter(0));

            setterCode.Return().Replace();
        }

        private void ImplementTypeWidePropertyInterception(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            using (new StopwatchLog(this, "class wide property"))
            {
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
                        for (int i = 0; i < type.Item.Length; i++)
                            property.CustomAttributes.Copy(type.Item[i].Attribute);
                    }

                    for (int i = 0; i < type.Item.Length; i++)
                        type.Item[i].Remove();
                }
            }
        }

        private void InterceptProperties(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            using (new StopwatchLog(this, "property"))
            {
                var propertyInterceptionInfo = __PropertyInterceptionInfo.Instance;

                var properties = builder
                    .FindPropertiesByAttributes(attributes)
                    .GroupBy(x => x.Property)
                    .Select(x => new PropertyBuilderInfo(x.Key, x.Select(y => new PropertyBuilderInfoItem(y, y.Property,
                             y.Attribute.Type.Implements(__IPropertyGetterInterceptor.Type.Fullname) ? __IPropertyGetterInterceptor.Instance : null,
                             y.Attribute.Type.Implements(__IPropertySetterInterceptor.Type.Fullname) ? __IPropertySetterInterceptor.Instance : null,
                             y.Attribute.Type.Implements(__IPropertyInterceptorInitialize.Type.Fullname) ? __IPropertyInterceptorInitialize.Instance : null))))
                    .ToArray();

                foreach (var member in properties)
                {
                    this.Log($"Implementing interceptors in property {member.Property}");

                    if (!member.HasGetterInterception && !member.HasSetterInterception && !member.HasInitializer)
                        continue;

                    var propertyField = member.Property.CreateField(__PropertyInterceptionInfo.Type, $"<{member.Property.Name}>p__propertyInfo");
                    propertyField.CustomAttributes.AddNonSerializedAttribute();

                    var actionObjectCtor = builder.Import(typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
                    var propertySetter = member.Property.Setter == null ?
                        null :
                        member.Property.OriginType.CreateMethod(member.Property.Modifiers.GetPrivate(), $"<{member.Property.Name}>m__setterMethod", builder.GetType(typeof(object)));

                    CreatePropertySetterDelegate(builder, member, propertySetter);

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
                        AddPropertyInitializeInterception(builder, propertyInterceptionInfo, member, propertyField, actionObjectCtor, propertySetter, interceptorFields);

                    if (member.HasGetterInterception && member.Property.Getter != null)
                        AddPropertyGetterInterception(builder, propertyInterceptionInfo, member, propertyField, actionObjectCtor, propertySetter, interceptorFields);

                    if (member.HasSetterInterception && member.Property.Setter != null)
                        AddPropertySetterInterception(builder, propertyInterceptionInfo, member, propertyField, actionObjectCtor, propertySetter, interceptorFields);

                    // Do this at the end to ensure that syncroot init is always on the top
                    if (member.RequiresSyncRootField)
                    {
                        if (member.SyncRoot.IsStatic)
                            member.Property.OriginType.CreateStaticConstructor().NewCode()
                                .Assign(member.SyncRoot).NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor)
                                .Insert(InsertionPosition.Beginning);
                        else
                            foreach (var ctors in member.Property.OriginType.GetRelevantConstructors().Where(x => x.Name == ".ctor"))
                                ctors.NewCode().Assign(member.SyncRoot).NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor).Insert(InsertionPosition.Beginning);
                    }

                    // Also remove the compilergenerated attribute
                    member.Property.Getter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
                    member.Property.Setter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
                }
            }
        }
    }
}