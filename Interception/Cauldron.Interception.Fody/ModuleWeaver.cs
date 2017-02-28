using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            this.InterceptMethods(builder);
            this.InterceptProperties(builder);
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
                        Interface = y.Attribute.Implements(iLockableMethodInterceptor.Type.Fullname) ? iLockableMethodInterceptor : iMethodInterceptor,
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

        private void InterceptProperties(Builder builder)
        {
            var attributes = builder.FindAttributesByInterfaces(
                "Cauldron.Interception.IPropertyInterceptor",
                "Cauldron.Interception.ILockablePropertyGetterInterceptor",
                "Cauldron.Interception.ILockablePropertySetterInterceptor",
                "Cauldron.Interception.IPropertyGetterInterceptor",
                "Cauldron.Interception.IPropertySetterInterceptor");

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
                        InterfaceSetter = y.Attribute.Implements(iLockablePropertySetterInterceptor.Type.Fullname) ? iLockablePropertySetterInterceptor : iPropertySetterInterceptor,
                        InterfaceGetter = y.Attribute.Implements(iLockablePropertyGetterInterceptor.Type.Fullname) ? iLockablePropertyGetterInterceptor : iPropertyGetterInterceptor,
                        Attribute = y,
                        Property = y.Property
                    }).ToArray()
                });

            foreach (var property in properties)
            {
                this.LogInfo($"Implementing interceptors in property {property.Key}");
                var lockable = property.Item.Any(x => x.InterfaceGetter.Lockable | x.InterfaceSetter.Lockable);
                var semaphoreFieldName = $"<{property.Key.Name}>lock_" + property.Key.Identification;

                if (lockable)
                    foreach (var ctor in property.Key.DeclaringType.GetRelevantConstructors())
                        ctor.NewCode()
                            .Assign(property.Key.CreateField(typeof(SemaphoreSlim), semaphoreFieldName))
                            .NewObj(semaphoreSlim.Ctor, 1, 1)
                            .Insert(Cecilator.InsertionPosition.Beginning);

                var propertyField = property.Key.CreateField(propertyInterceptionInfo.Type, $"<{property.Key.Name}>p__propertyInfo");

                if (!property.Key.IsAutoProperty)
                {
                    this.LogWarning($"{property.Key.Name}: The current version of the property interceptor only supports auto-properties.");
                    continue;
                }

                var actionObjectCtor = builder.Import(typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
                var propertySetter = property.Key.DeclaringType.CreateMethod(property.Key.IsStatic ? Modifiers.PrivateStatic : Modifiers.Private, $"<{property.Key.Name}>m__setterMethod", builder.GetType(typeof(object)));

                #region Setter "Delegate"

                var tryDisposeMethod = builder.GetType("Cauldron.Interception.Extensions").GetMethod("TryDispose", 1);

                var setterCode = propertySetter.NewCode();

                if (property.Key.BackingField.FieldType.ParameterlessContructor != null)
                    setterCode.Load(property.Key.BackingField).IsNotNull().Then(y =>
                        y.Assign(property.Key.BackingField).Set(propertySetter.NewCode()
                            .NewObj(property.Key.BackingField.FieldType.ParameterlessContructor)));

                // Only this if the property implements idisposable
                if (property.Key.BackingField.FieldType.Implements(typeof(IDisposable)))
                    setterCode.Call(tryDisposeMethod, property.Key.BackingField);

                setterCode.Load(propertySetter.NewCode().Parameters[0]).IsNull().Then(x =>
                {
                    // Just clear if its clearable
                    if (property.Key.BackingField.FieldType.Implements(typeof(IList)))
                        x.Load(property.Key.BackingField).Callvirt(builder.GetType(typeof(IList)).GetMethod("Clear"));
                    // Otherwise if the property is not a value type and nullable
                    else if (!property.Key.BackingField.FieldType.IsValueType || property.Key.BackingField.FieldType.IsNullable || property.Key.BackingField.FieldType.IsArray)
                        x.Assign(property.Key.BackingField).Set(null).Return();
                    else // otherwise... throw an exception
                        x.ThrowNew(typeof(NotSupportedException), "Value types does not accept null values.");
                });

                if (property.Key.BackingField.FieldType.IsArray)
                    setterCode.Load(propertySetter.NewCode().Parameters[0]).Is(typeof(IEnumerable)).Then(x => x.Assign(property.Key.BackingField).Set(propertySetter.NewCode().Parameters[0]));
                else if (property.Key.BackingField.FieldType.Implements(typeof(IList)) && property.Key.BackingField.FieldType.ParameterlessContructor != null)
                {
                    var addRange = property.Key.BackingField.FieldType.GetMethod("AddRange", 1);
                    if (addRange == null)
                    {
                        var add = property.Key.BackingField.FieldType.GetMethod("Add", 1);
                        var array = setterCode.CreateVariable(property.Key.BackingField.FieldType.ChildType.MakeArray());
                        setterCode.Assign(array).Set(propertySetter.NewCode().Parameters[0]);
                        setterCode.For(array, (x, item) => x.Load(property.Key.BackingField).Callvirt(add, item));
                        if (!add.ReturnType.IsVoid)
                            setterCode.Pop();
                    }
                    else
                        setterCode.Callvirt(addRange, propertySetter.NewCode().Parameters[0]);
                }
                else
                    setterCode.Load(propertySetter.NewCode().Parameters[0]).Is(property.Key.ReturnType).Then(x => x.Assign(property.Key.BackingField).Set(propertySetter.NewCode().Parameters[0]));

                setterCode.Context(x =>
                {
                    //var ctor = property.Key.ReturnType.ParameterlessContructor;
                    //this.LogInfo(ctor ?? "x");

                    //if (ctor != null && ctor.IsPublic)
                    //    x.Load(property.Key.BackingField).IsNull().Then(y => propertySetter.NewCode().NewObj(ctor));
                })
                .Return()
                .Replace();

                #endregion Setter "Delegate"

                #region Getter implementation

                property.Key.Getter
                    .NewCode()
                        .Context(x =>
                        {
                            for (int i = 0; i < property.Item.Length; i++)
                            {
                                var item = property.Item[i];
                                x.Assign(x.CreateVariable("<>interceptor_" + i, item.InterfaceGetter.Type)).NewObj(item.Attribute);
                                item.Attribute.Remove();
                            }

                            x.Load(propertyField).IsNull().Then(y =>
                                y.Assign(propertyField)
                                    .NewObj(propertyInterceptionInfo.Ctor,
                                        property.Key.Getter,
                                        property.Key.Setter,
                                        property.Key.Name,
                                        property.Key.ReturnType,
                                        y.This,
                                        null,
                                        y.NewCode().NewObj(actionObjectCtor, null, propertySetter)));
                        })
                        .Try(x =>
                        {
                            for (int i = 0; i < property.Item.Length; i++)
                            {
                                var item = property.Item[i];
                                if (item.InterfaceGetter.Lockable)
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceGetter.OnGet, x.NewCode().LoadField(semaphoreFieldName), propertyField, x.NewCode().Load(propertyField));
                                else
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceGetter.OnGet, propertyField, x.NewCode().Load(propertyField));
                            }
                        })
                        .Catch(typeof(Exception), x =>
                        {
                            for (int i = 0; i < property.Item.Length; i++)
                                x.LoadVariable("<>interceptor_" + i).Callvirt(property.Item[i].InterfaceGetter.OnException, x.Exception);

                            x.Rethrow();
                        })
                        .Finally(x =>
                        {
                            if (lockable)
                                x.LoadField(semaphoreFieldName)
                                .Call(semaphoreSlim.CurrentCount)
                                    .EqualTo(0)
                                        .Then(y => y.LoadField(semaphoreFieldName).Call(semaphoreSlim.Release).Pop());

                            for (int i = 0; i < property.Item.Length; i++)
                                x.LoadVariable("<>interceptor_" + i).Callvirt(property.Item[i].InterfaceGetter.OnExit);
                        })
                        .EndTry()
                        .Load(property.Key.BackingField)
                        .Return()
                    .Replace();

                #endregion Getter implementation
            }
        }
    }
}