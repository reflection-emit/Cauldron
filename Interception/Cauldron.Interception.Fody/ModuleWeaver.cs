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
            var attributes = builder.FindAttributesByInterfaces(
                "Cauldron.Interception.ILockableMethodInterceptor",
                "Cauldron.Interception.IMethodInterceptor");

            builder.FindTypes(SearchContext.AllReferencedModules, "Lockable");

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

            var semaphoreSlim = builder.GetType(typeof(SemaphoreSlim)).New(x => new
            {
                Ctor = x.GetMethod(".ctor", typeof(int), typeof(int)),
                Release = x.GetMethod("Release"),
                CurrentCount = x.GetMethod("get_CurrentCount")
            });

            foreach (var method in methods)
            {
                this.LogInfo($"Implementing interceptors in {method.Key}");
                var variablesAndAttribute = method.Item.Select(x => new { Variable = method.Key.CreateVariable(x.Interface.Type), Content = x }).ToArray();
                var lockable = variablesAndAttribute.Any(x => x.Content.Interface.Lockable);
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
                        foreach (var item in variablesAndAttribute)
                        {
                            x.Assign(item.Variable).NewObj(item.Content.Attribute);
                            item.Content.Attribute.Remove();
                        }
                    })
                    .Try(x =>
                    {
                        foreach (var item in variablesAndAttribute)
                        {
                            if (item.Content.Interface.Lockable)
                                x.Load(item.Variable).Callvirt(item.Content.Interface.OnEnter, method.Key.DeclaringType.Fields[semaphoreFieldName], item.Content.Method.DeclaringType, x.This, item.Content.Method, x.Parameters);
                            else
                                x.Load(item.Variable).Callvirt(item.Content.Interface.OnEnter, item.Content.Method.DeclaringType, x.This, item.Content.Method, x.Parameters);
                        }
                        x.Load(x.This).Call(x.Copy(Modifiers.Private, "bla" + method.Key.Name));
                        //x.OriginalBody();
                    })
                    .Catch(typeof(Exception), x =>
                    {
                        foreach (var item in variablesAndAttribute)
                            x.Load(item.Variable).Callvirt(item.Content.Interface.OnException, x.Exception);

                        x.Rethrow();
                    })
                    .Finally(x =>
                    {
                        //if (lockable)
                        //    x.LoadField(semaphoreFieldName)
                        //        .Call(semaphoreSlim.Release);

                        foreach (var item in variablesAndAttribute)
                            x.Load(item.Variable).Callvirt(item.Content.Interface.OnExit);
                    })
                    .EndTry()
                    .Return()
                .Replace();
            }

            //var fields = builder.FindFieldsByAttributes(attributes);
            //var fieldUsage = fields.SelectMany(x => x.Field.FindUsages());

            //var type = builder.FindTypes("FieldOf_Implementer_Test").FirstOrDefault();
            //var field = type.Fields["field2"];

            //foreach (var item in type.GetRelevantConstructors())
            //{
            //    item.Code
            //        .Assign(field)
            //        .Set("This is a test")
            //        .Insert(Cecilator.InsertionPosition.End);
            //}

            //type.GetMethod("Static_Private_Field_Info")
            //    .Code
            //    .Assign(field)
            //    .Set("My god it works")
            //    .Insert(Cecilator.InsertionPosition.End);

            //Extensions.ModuleWeaver = this;

            //// Check if th module has a reference to Cauldron.Interception
            //var assemblyNameReference = this.ModuleDefinition.AllReferencedAssemblies().FirstOrDefault(x => x.Name.Name == "Cauldron.Interception");
            //if (assemblyNameReference == null)
            //    return;

            //foreach (var weaverType in this.weavers)
            //{
            //    var weaver = Activator.CreateInstance(weaverType, this) as ModuleWeaverBase;
            //    try
            //    {
            //        weaver.Implement();
            //    }
            //    catch (NotImplementedException e)
            //    {
            //        this.LogWarning(e.Message);
            //    }
            //    catch (Exception e)
            //    {
            //        this.LogError(e.GetStackTrace());
            //        this.LogError(e.StackTrace);
            //    }
            //}
        }
    }
}