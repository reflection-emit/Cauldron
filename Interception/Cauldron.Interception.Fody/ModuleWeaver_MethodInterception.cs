using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.Extensions;
using Cauldron.Interception.Fody.HelperTypes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void ImplementTypeWideMethodInterception(Builder builder, IEnumerable<BuilderType> attributes)
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
                this.LogInfo($"Implementing interceptors in type {type.Key.Fullname}");

                foreach (var method in type.Key.Methods)
                {
                    if (method.Name == ".ctor" || method.Name == ".cctor")
                        continue;

                    if (method.CustomAttributes.HasAttribute(doNotInterceptAttribute))
                    {
                        method.CustomAttributes.Remove(doNotInterceptAttribute);
                        continue;
                    }

                    for (int i = 0; i < type.Item.Length; i++)
                        method.CustomAttributes.Copy(type.Item[i].Attribute);
                }

                for (int i = 0; i < type.Item.Length; i++)
                    type.Item[i].Remove();
            }

            stopwatch.Stop();
            this.LogInfo($"Implementing class wide method interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        private void InterceptMethods(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();

            var iMethodInterceptor = new __IMethodInterceptor(builder);
            var asyncTaskMethodBuilder = new __AsyncTaskMethodBuilder(builder);
            var asyncTaskMethodBuilderGeneric = new __AsyncTaskMethodBuilder_1(builder);
            var task = new __Task(builder);
            var exception = new __Exception(builder);

            var methods = builder
                .FindMethodsByAttributes(attributes)
                .GroupBy(x => new MethodKey(x.Method, x.AsyncMethod))
                .Select(x => new MethodBuilderInfo(x.Key, x.Select(y => new MethodBuilderInfoItem(y, iMethodInterceptor))))
                .ToArray();

            foreach (var method in methods)
            {
                this.LogInfo($"Implementing interceptors in method {method.Key}");

                var targetedMethod = method.Key.AsyncMethod == null ? method.Key.Method : method.Key.AsyncMethod;
                var attributedMethod = method.Key.Method;

                method.AddThisReferenceToAsyncMethod();

                var typeInstance = method.GetAsyncMethodTypeInstace();
                var interceptorField = new Field[method.Item.Length];

                targetedMethod
                .NewCode()
                    .Context(x =>
                    {
                        for (int i = 0; i < method.Item.Length; i++)
                        {
                            var item = method.Item[i];
                            var name = $"<{targetedMethod.Name}>_{i}_{item.Attribute.Identification}";
                            interceptorField[i] = targetedMethod.DeclaringType.CreateField(targetedMethod.Modifiers.GetPrivate(), item.Interface.Type, name);

                            x.Load(interceptorField[i]).IsNull().Then(y => y.Assign(interceptorField[i]).NewObj(item.Attribute));
                            item.Attribute.Remove();
                        }
                    })
                    .Try(x =>
                    {
                        for (int i = 0; i < method.Item.Length; i++)
                        {
                            var item = method.Item[i];
                            x.Load(interceptorField[i]).Callvirt(item.Interface.OnEnter, attributedMethod.DeclaringType, typeInstance, attributedMethod, x.GetParametersArray());
                        }

                        x.OriginalBody();

                        // Special case for async methods
                        if (method.Key.AsyncMethod != null && method.Key.Method.ReturnType.Fullname == task.Type.Fullname) // Task return
                        {
                            var exceptionVar = x.CreateVariable(exception.Type);

                            x.Assign(exceptionVar).Set(
                                x.NewCode().Call(method.Key.AsyncMethod.DeclaringType.GetField("<>t__builder"), asyncTaskMethodBuilder.GetTask)
                                .Call(task.GetException));

                            x.Load(exceptionVar).IsNotNull().Then(y =>
                            {
                                for (int i = 0; i < method.Item.Length; i++)
                                    y.Load(interceptorField[i]).Callvirt(method.Item[i].Interface.OnException, exceptionVar);
                            });
                        }
                        else if (method.Key.AsyncMethod != null) // Task<> return
                        {
                            var exceptionVar = x.CreateVariable(exception.Type);
                            var taskArgument = method.Key.Method.ReturnType.GetGenericArgument(0);

                            x.Assign(exceptionVar).Set(
                                x.NewCode().Call(method.Key.AsyncMethod.DeclaringType.GetField("<>t__builder"), asyncTaskMethodBuilderGeneric.GetTask.MakeGeneric(taskArgument))
                                .Call(task.GetException));

                            x.Load(exceptionVar).IsNotNull().Then(y =>
                            {
                                for (int i = 0; i < method.Item.Length; i++)
                                    y.Load(interceptorField[i]).Callvirt(method.Item[i].Interface.OnException, exceptionVar);
                            });
                        }
                    })
                    .Catch(exception.Type, x =>
                    {
                        if (method.Key.AsyncMethod == null)
                            for (int i = 0; i < method.Item.Length; i++)
                                x.Load(interceptorField[i]).Callvirt(method.Item[i].Interface.OnException, x.Exception);

                        x.Rethrow();
                    })
                    .Finally(x =>
                    {
                        for (int i = 0; i < method.Item.Length; i++)
                            x.Load(interceptorField[i]).Callvirt(method.Item[i].Interface.OnExit);
                    })
                    .EndTry()
                    .Return()
                .Replace();
            };

            stopwatch.Stop();
            this.LogInfo($"Implementing method interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }
    }
}