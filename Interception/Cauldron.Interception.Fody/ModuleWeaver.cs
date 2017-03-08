using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cauldron.Interception.Fody
{
    public sealed class ModuleWeaver : IWeaver
    {
        private List<Type> weavers = new List<Type>
        {
            typeof(AnonymouseTypeToInterfaceWeaver),
            typeof(FieldInterceptorWeaver),
            typeof(MethodInterceptorWeaver),
            typeof(PropertyInterceptorWeaver),
            typeof(ChildOfWeaver),
            typeof(MethodOfWeaver),
            typeof(FieldOfWeaver)
        };

        public Action<string> LogError { get; set; }

        public Action<string> LogInfo { get; set; }

        public Action<string> LogWarning { get; set; }

        public ModuleDefinition ModuleDefinition { get; set; }

        public void Execute()
        {
            var builder = this.CreateBuilder();
            var propertyInterceptingAttributes = builder.FindAttributesByInterfaces(
                "Cauldron.Interception.IPropertyInterceptor",
                "Cauldron.Interception.ILockablePropertyGetterInterceptor",
                "Cauldron.Interception.ILockablePropertySetterInterceptor",
                "Cauldron.Interception.IPropertyGetterInterceptor",
                "Cauldron.Interception.IPropertySetterInterceptor");

            this.InterceptFields(builder, propertyInterceptingAttributes);
            this.InterceptMethods(builder);
            this.InterceptProperties(builder, propertyInterceptingAttributes);
        }

        private void InterceptFields(Builder builder, IEnumerable<BuilderType> attributes)
        {
            var fields = builder.FindFieldsByAttributes(attributes).ToArray();

            foreach (var field in fields)
            {
                this.LogInfo($"Implementing interceptors in fields {field.Field}");

                if (field.Field.Modifiers.HasFlag(Modifiers.Public))
                {
                    this.LogWarning($"The current version of the field interceptor only intercepts private fields. Field '{field.Field.Name}' in type '{field.Field.DeclaringType.Name}'");
                    continue;
                }

                var usage = field.Field.FindUsages().ToArray();
                var type = field.Field.DeclaringType;
                var property = type.CreateProperty(field.Field);

                property.CustomAttributes.AddCompilerGeneratedAttribute();
                field.Attribute.MoveTo(property);

                foreach (var item in usage)
                {
                    // TODO - fields has to be replaced by property if ctor is not calling base... instead this
                    if (item.Method.Name != ".ctor" && item.Method.Name != ".cctor" ||
                        (item.Method.Name == ".ctor" && !item.Method.IsConstructorWithBaseCall))
                        item.Replace(property);
                }
            }
        }

        private void InterceptMethods(Builder builder)
        {
            var attributes = builder.FindAttributesByInterfaces(
                "Cauldron.Interception.ILockableMethodInterceptor",
                "Cauldron.Interception.IMethodInterceptor");

            #region define Interfaces and the methods we want to invoke

            var iLockableMethodInterceptor = builder.GetType("Cauldron.Interception.ILockableMethodInterceptor")
                .New(x => new
                {
                    Lockable = true,
                    Type = x,
                    OnEnter = x.GetMethod("OnEnter", 5),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iMethodInterceptor = builder.GetType("Cauldron.Interception.IMethodInterceptor")
                .New(x => new
                {
                    Lockable = false,
                    Type = x,
                    OnEnter = x.GetMethod("OnEnter", 4),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            #endregion define Interfaces and the methods we want to invoke

            var semaphoreSlim = builder.GetType(typeof(SemaphoreSlim)).New(x => new
            {
                Ctor = x.GetMethod(".ctor", typeof(int), typeof(int)),
                Release = x.GetMethod("Release"),
                CurrentCount = x.GetMethod("get_CurrentCount")
            });

            var methods = builder
                .FindMethodsByAttributes(attributes)
                .GroupBy(x => x.Method)
                .Select(x => new
                {
                    Key = x.Key,
                    Item = x.Select(y => new
                    {
                        Interface = y.Attribute.Type.Implements(iLockableMethodInterceptor.Type.Fullname) ? iLockableMethodInterceptor : iMethodInterceptor,
                        Attribute = y,
                        Method = y.Method
                    }).ToArray()
                });

            foreach (var method in methods)
            {
                this.LogInfo($"Implementing interceptors in method {method.Key}");
                var lockable = method.Item.Any(x => x.Interface.Lockable);
                var semaphoreFieldName = $"<{method.Key.Name}>lock_" + method.Key.Identification;

                if (lockable)
                    foreach (var ctor in method.Key.DeclaringType.GetRelevantConstructors())
                        ctor.NewCode()
                            .Assign(method.Key.CreateField(typeof(SemaphoreSlim), semaphoreFieldName))
                            .NewObj(semaphoreSlim.Ctor, 1, 1)
                            .Insert(Cecilator.InsertionPosition.Beginning);

                method.Key
                .NewCode()
                    .Context(x =>
                    {
                        for (int i = 0; i < method.Item.Length; i++)
                        {
                            var item = method.Item[i];
                            x.Assign(x.CreateVariable("<>interceptor_" + i, item.Interface.Type)).NewObj(item.Attribute);
                            item.Attribute.Remove();
                        }
                    })
                    .Try(x =>
                    {
                        for (int i = 0; i < method.Item.Length; i++)
                        {
                            var item = method.Item[i];
                            if (item.Interface.Lockable)
                                x.LoadVariable("<>interceptor_" + i).Callvirt(item.Interface.OnEnter, x.NewCode().LoadField(semaphoreFieldName), item.Method.DeclaringType, x.This, item.Method, x.Parameters);
                            else
                                x.LoadVariable("<>interceptor_" + i).Callvirt(item.Interface.OnEnter, item.Method.DeclaringType, x.This, item.Method, x.Parameters);
                        }
                        x.OriginalBody();
                    })
                    .Catch(typeof(Exception), x =>
                    {
                        for (int i = 0; i < method.Item.Length; i++)
                            x.LoadVariable("<>interceptor_" + i).Callvirt(method.Item[i].Interface.OnException, x.Exception);

                        x.Rethrow();
                    })
                    .Finally(x =>
                    {
                        if (lockable)
                            x.LoadField(semaphoreFieldName)
                            .Call(semaphoreSlim.CurrentCount)
                                .EqualTo(0)
                                    .Then(y => y.LoadField(semaphoreFieldName).Call(semaphoreSlim.Release).Pop());

                        for (int i = 0; i < method.Item.Length; i++)
                            x.LoadVariable("<>interceptor_" + i).Callvirt(method.Item[i].Interface.OnExit);
                    })
                    .EndTry()
                    .Return()
                .Replace();
            }
        }

        private void InterceptProperties(Builder builder, IEnumerable<BuilderType> attributes)
        {
            #region define Interfaces and the methods we want to invoke

            var iLockablePropertyGetterInterceptor = builder.GetType("Cauldron.Interception.ILockablePropertyGetterInterceptor")
                .New(x => new
                {
                    Lockable = true,
                    Type = x,
                    OnGet = x.GetMethod("OnGet", 3),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iLockablePropertySetterInterceptor = builder.GetType("Cauldron.Interception.ILockablePropertySetterInterceptor")
                .New(x => new
                {
                    Lockable = true,
                    Type = x,
                    OnSet = x.GetMethod("OnSet", 4),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iPropertyGetterInterceptor = builder.GetType("Cauldron.Interception.IPropertyGetterInterceptor")
                .New(x => new
                {
                    Lockable = false,
                    Type = x,
                    OnGet = x.GetMethod("OnGet", 2),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iPropertySetterInterceptor = builder.GetType("Cauldron.Interception.IPropertySetterInterceptor")
                .New(x => new
                {
                    Lockable = false,
                    Type = x,
                    OnSet = x.GetMethod("OnSet", 3),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            #endregion define Interfaces and the methods we want to invoke

            var semaphoreSlim = builder.GetType(typeof(SemaphoreSlim)).New(x => new
            {
                Ctor = x.GetMethod(".ctor", typeof(int), typeof(int)),
                Release = x.GetMethod("Release"),
                CurrentCount = x.GetMethod("get_CurrentCount")
            });

            var propertyInterceptionInfo = builder.GetType("Cauldron.Interception.PropertyInterceptionInfo").New(x => new
            {
                Type = x,
                Ctor = x.GetMethod(".ctor", 7)
            });

            var properties = builder
                .FindPropertiesByAttributes(attributes)
                .GroupBy(x => x.Property)
                .Select(x => new
                {
                    Key = x.Key,
                    Item = x.Select(y => new
                    {
                        InterfaceSetter = y.Attribute.Type.Implements(iLockablePropertySetterInterceptor.Type.Fullname) ? iLockablePropertySetterInterceptor : y.Attribute.Type.Implements(iPropertySetterInterceptor.Type.Fullname) ? iPropertySetterInterceptor : null,
                        InterfaceGetter = y.Attribute.Type.Implements(iLockablePropertyGetterInterceptor.Type.Fullname) ? iLockablePropertyGetterInterceptor : y.Attribute.Type.Implements(iPropertyGetterInterceptor.Type.Fullname) ? iPropertyGetterInterceptor : null,
                        Attribute = y,
                        Property = y.Property
                    }).ToArray()
                })
                .Select(x => new
                {
                    Property = x.Key,
                    InterceptorInfos = x.Item,
                    HasGetterInterception = x.Item.Any(y => y.InterfaceGetter != null),
                    HasSetterInterception = x.Item.Any(y => y.InterfaceSetter != null),
                    RequiredLocking = x.Item.Any(y => (y.InterfaceGetter != null && y.InterfaceGetter.Lockable) || (y.InterfaceSetter != null && y.InterfaceSetter.Lockable))
                })
                .ToArray();

            foreach (var member in properties)
            {
                this.LogInfo($"Implementing interceptors in property {member.Property}");
                var semaphoreFieldName = $"<{member.Property.Name}>lock_" + member.Property.Identification;

                if (member.RequiredLocking)
                    foreach (var ctor in member.Property.DeclaringType.GetRelevantConstructors())
                        ctor.NewCode()
                            .Assign(member.Property.CreateField(typeof(SemaphoreSlim), semaphoreFieldName))
                            .NewObj(semaphoreSlim.Ctor, 1, 1)
                            .Insert(Cecilator.InsertionPosition.Beginning);

                var propertyField = member.Property.CreateField(propertyInterceptionInfo.Type, $"<{member.Property.Name}>p__propertyInfo");

                if (!member.Property.IsAutoProperty)
                {
                    this.LogWarning($"{member.Property.Name}: The current version of the property interceptor only supports auto-properties.");
                    continue;
                }

                var actionObjectCtor = builder.Import(typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
                var propertySetter = member.Property.DeclaringType.CreateMethod(member.Property.IsStatic ? Modifiers.PrivateStatic : Modifiers.Private, $"<{member.Property.Name}>m__setterMethod", builder.GetType(typeof(object)));

                #region Setter "Delegate"

                var setterCode = propertySetter.NewCode();
                if (!member.Property.BackingField.FieldType.IsGenericType)
                {
                    var tryDisposeMethod = builder.GetType("Cauldron.Interception.Extensions").GetMethod("TryDispose", 1);

                    if (member.Property.BackingField.FieldType.ParameterlessContructor != null)
                        setterCode.Load(member.Property.BackingField).IsNull().Then(y =>
                            y.Assign(member.Property.BackingField).Set(propertySetter.NewCode()
                                .NewObj(member.Property.BackingField.FieldType.ParameterlessContructor)));

                    // Only this if the property implements idisposable
                    if (member.Property.BackingField.FieldType.Implements(typeof(IDisposable)))
                        setterCode.Call(tryDisposeMethod, member.Property.BackingField);

                    setterCode.Load(propertySetter.NewCode().Parameters[0]).IsNull().Then(x =>
                    {
                        // Just clear if its clearable
                        if (member.Property.BackingField.FieldType.Implements(typeof(IList)))
                            x.Load(member.Property.BackingField).Callvirt(builder.GetType(typeof(IList)).GetMethod("Clear")).Return();
                        // Otherwise if the property is not a value type and nullable
                        else if (!member.Property.BackingField.FieldType.IsValueType || member.Property.BackingField.FieldType.IsNullable || member.Property.BackingField.FieldType.IsArray)
                            x.Assign(member.Property.BackingField).Set(null).Return();
                        else // otherwise... throw an exception
                            x.ThrowNew(typeof(NotSupportedException), "Value types does not accept null values.");
                    });

                    if (member.Property.BackingField.FieldType.IsArray)
                        setterCode.Load(propertySetter.NewCode().Parameters[0]).Is(typeof(IEnumerable))
                            .Then(x => x.Assign(member.Property.BackingField).Set(propertySetter.NewCode().Parameters[0]).Return())
                            .ThrowNew(typeof(NotSupportedException), "Value does not inherits from IEnumerable");
                    else if (member.Property.BackingField.FieldType.Implements(typeof(IList)) && member.Property.BackingField.FieldType.ParameterlessContructor != null)
                    {
                        var addRange = member.Property.BackingField.FieldType.GetMethod("AddRange", 1);
                        if (addRange == null)
                        {
                            var add = member.Property.BackingField.FieldType.GetMethod("Add", 1);
                            var array = setterCode.CreateVariable(member.Property.BackingField.FieldType.ChildType.MakeArray());
                            setterCode.Assign(array).Set(propertySetter.NewCode().Parameters[0]);
                            setterCode.For(array, (x, item) => x.Load(member.Property.BackingField).Callvirt(add, item));
                            if (!add.ReturnType.IsVoid)
                                setterCode.Pop();
                        }
                        else
                            setterCode.Load(member.Property.BackingField).Callvirt(addRange, propertySetter.NewCode().Parameters[0]);
                    }
                    else
                        setterCode.Assign(member.Property.BackingField).Set(propertySetter.NewCode().Parameters[0]);
                }
                else
                    setterCode.Assign(member.Property.BackingField).Set(propertySetter.NewCode().Parameters[0]);

                setterCode.Return().Replace();

                #endregion Setter "Delegate"

                #region Getter implementation

                if (member.HasGetterInterception)
                    member.Property.Getter
                        .NewCode()
                            .Context(x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                {
                                    var item = member.InterceptorInfos[i];
                                    x.Assign(x.CreateVariable("<>interceptor_" + i, item.InterfaceGetter.Type)).NewObj(item.Attribute);
                                    item.Attribute.Remove();
                                }

                                x.Load(propertyField).IsNull().Then(y =>
                                    y.Assign(propertyField)
                                        .NewObj(propertyInterceptionInfo.Ctor,
                                            member.Property.Getter,
                                            member.Property.Setter,
                                            member.Property.Name,
                                            member.Property.ReturnType,
                                            y.This,
                                            member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                            y.NewCode().NewObj(actionObjectCtor, y.NewCode().This, propertySetter)));
                            })
                            .Try(x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                {
                                    var item = member.InterceptorInfos[i];
                                    if (item.InterfaceGetter.Lockable)
                                        x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceGetter.OnGet, x.NewCode().LoadField(semaphoreFieldName), propertyField, member.Property.BackingField);
                                    else
                                        x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceGetter.OnGet, propertyField, member.Property.BackingField);
                                }
                            })
                            .Catch(typeof(Exception), x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(member.InterceptorInfos[i].InterfaceGetter.OnException, x.Exception);

                                x.Rethrow();
                            })
                            .Finally(x =>
                            {
                                if (member.RequiredLocking)
                                    x.LoadField(semaphoreFieldName)
                                    .Call(semaphoreSlim.CurrentCount)
                                        .EqualTo(0)
                                            .Then(y => y.LoadField(semaphoreFieldName).Call(semaphoreSlim.Release).Pop());

                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(member.InterceptorInfos[i].InterfaceGetter.OnExit);
                            })
                            .EndTry()
                            .Load(member.Property.BackingField)
                            .Return()
                        .Replace();

                #endregion Getter implementation

                #region Setter implementation

                if (member.HasSetterInterception)
                    member.Property.Setter
                        .NewCode()
                            .Context(x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                {
                                    var item = member.InterceptorInfos[i];
                                    x.Assign(x.CreateVariable("<>interceptor_" + i, item.InterfaceSetter.Type)).NewObj(item.Attribute);
                                    item.Attribute.Remove();
                                }

                                x.Load(propertyField).IsNull().Then(y =>
                                    y.Assign(propertyField)
                                        .NewObj(propertyInterceptionInfo.Ctor,
                                            member.Property.Getter,
                                            member.Property.Setter,
                                            member.Property.Name,
                                            member.Property.ReturnType,
                                            y.This,
                                            member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                            y.NewCode().NewObj(actionObjectCtor, y.NewCode().This, propertySetter)));
                            })
                            .Try(x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                {
                                    var item = member.InterceptorInfos[i];
                                    if (item.InterfaceSetter.Lockable)
                                        x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceSetter.OnSet, x.NewCode().LoadField(semaphoreFieldName), propertyField, member.Property.BackingField, member.Property.Setter.NewCode().Parameters[0]);
                                    else
                                        x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceSetter.OnSet, propertyField, member.Property.BackingField, member.Property.Setter.NewCode().Parameters[0]);

                                    x.IsFalse().Then(y => y.Assign(member.Property.BackingField).Set(member.Property.Setter.NewCode().Parameters[0]));
                                }
                            })
                            .Catch(typeof(Exception), x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(member.InterceptorInfos[i].InterfaceSetter.OnException, x.Exception);

                                x.Rethrow();
                            })
                            .Finally(x =>
                            {
                                if (member.RequiredLocking)
                                    x.LoadField(semaphoreFieldName)
                                    .Call(semaphoreSlim.CurrentCount)
                                        .EqualTo(0)
                                            .Then(y => y.LoadField(semaphoreFieldName).Call(semaphoreSlim.Release).Pop());

                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(member.InterceptorInfos[i].InterfaceSetter.OnExit);
                            })
                            .EndTry()
                            .Return()
                        .Replace();

                #endregion Setter implementation

                // Also remove the compilergenerated attribute
                member.Property.Getter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
                member.Property.Setter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
            }
        }
    }
}