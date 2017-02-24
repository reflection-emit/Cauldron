using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System;
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
                var semaphoreFieldName = "<>lock_" + method.Key.Identification;

                if (lockable)
                    foreach (var ctor in method.Key.DeclaringType.GetRelevantConstructors())
                        ctor.Code
                            .Assign(method.Key.CreateField(typeof(SemaphoreSlim), semaphoreFieldName))
                            .NewObj(semaphoreSlim.Ctor, 1, 1)
                            .Insert(Cecilator.InsertionPosition.Beginning);

                method.Key
                .Code
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
                                x.LoadVariable("<>interceptor_" + i).Callvirt(item.Interface.OnEnter, method.Key.DeclaringType.Fields[semaphoreFieldName], item.Method.DeclaringType, x.This, item.Method, x.Parameters);
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
                    OnEnter = x.GetMethod("OnGet", 3),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iLockablePropertySetterInterceptor = builder.GetType("Cauldron.Interception.ILockablePropertySetterInterceptor")
                .New(x => new
                {
                    Lockable = true,
                    Type = x,
                    OnEnter = x.GetMethod("OnSet", 4),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iPropertyGetterInterceptor = builder.GetType("Cauldron.Interception.IPropertyGetterInterceptor")
                .New(x => new
                {
                    Lockable = false,
                    Type = x,
                    OnEnter = x.GetMethod("OnGet", 2),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iPropertySetterInterceptor = builder.GetType("Cauldron.Interception.IPropertySetterInterceptor")
                .New(x => new
                {
                    Lockable = false,
                    Type = x,
                    OnEnter = x.GetMethod("OnSet", 3),
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
                var semaphoreFieldName = "<>lock_" + property.Key.Identification;

                if (lockable)
                    foreach (var ctor in property.Key.DeclaringType.GetRelevantConstructors())
                        ctor.Code
                            .Assign(property.Key.CreateField(typeof(SemaphoreSlim), semaphoreFieldName))
                            .NewObj(semaphoreSlim.Ctor, 1, 1)
                            .Insert(Cecilator.InsertionPosition.Beginning);

                this.LogInfo($"{property.Key.Name} {property.Key.IsAutoProperty}");
            }
        }
    }
}