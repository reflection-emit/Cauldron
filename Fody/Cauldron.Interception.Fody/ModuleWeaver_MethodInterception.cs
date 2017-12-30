using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.Extensions;
using Cauldron.Interception.Fody.HelperTypes;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void ImplementTypeWideMethodInterception(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            using (new StopwatchLog(this, "class wide method"))
            {
                var types = builder
                    .FindTypesByAttributes(attributes)
                    .GroupBy(x => x.Type)
                    .Select(x => new
                    {
                        x.Key,
                        Item = x.ToArray()
                    })
                    .ToArray();

                foreach (var type in types)
                {
                    this.Log($"Implementing interceptors in type {type.Key.Fullname}");

                    foreach (var method in type.Key.Methods)
                    {
                        if (method.Name == ".ctor" || method.Name == ".cctor" || method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                            continue;

                        for (int i = 0; i < type.Item.Length; i++)
                            method.CustomAttributes.Copy(type.Item[i].Attribute);
                    }

                    for (int i = 0; i < type.Item.Length; i++)
                        type.Item[i].Remove();
                }
            }
        }

        private void InterceptMethods(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            using (new StopwatchLog(this, "method"))
            {
                var asyncTaskMethodBuilder = new __AsyncTaskMethodBuilder();
                var asyncTaskMethodBuilderGeneric = new __AsyncTaskMethodBuilder_1();
                var syncRoot = new __ISyncRoot();
                var task = new __Task();
                var exception = new __Exception();

                var methods = builder
                    .FindMethodsByAttributes(attributes)
                    .GroupBy(x => new MethodKey(x.Method, x.AsyncMethod))
                    .Select(x => new MethodBuilderInfo<MethodBuilderInfoItem<__IMethodInterceptor>>(x.Key, x.Select(y => new MethodBuilderInfoItem<__IMethodInterceptor>(y, __IMethodInterceptor.Instance))))
                    .ToArray();

                foreach (var method in methods)
                {
                    this.Log($"Implementing interceptors in method {method.Key.Method}");

                    if (method.Item == null || method.Item.Length == 0)
                        continue;

                    var targetedMethod = method.Key.AsyncMethod ?? method.Key.Method;
                    var attributedMethod = method.Key.Method;

                    method.AddThisReferenceToAsyncMethod();

                    var typeInstance = method.GetAsyncMethodTypeInstace();
                    var interceptorField = new Field[method.Item.Length];

                    if (method.RequiresSyncRootField)
                    {
                        if (method.SyncRoot.IsStatic)
                            targetedMethod.OriginType.CreateStaticConstructor().NewCode()
                                .Assign(method.SyncRoot).NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor)
                                .Insert(InsertionPosition.Beginning);
                        else
                            foreach (var ctors in targetedMethod.OriginType.GetRelevantConstructors().Where(x => x.Name == ".ctor"))
                                ctors.NewCode().Assign(method.SyncRoot).NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor).Insert(InsertionPosition.Beginning);
                    }

                    targetedMethod
                    .NewCode()
                        .Context(x =>
                        {
                            for (int i = 0; i < method.Item.Length; i++)
                            {
                                var item = method.Item[i];
                                var name = $"<{targetedMethod.Name}>_attrib{i}_{item.Attribute.Identification}";
                                interceptorField[i] = targetedMethod.OriginType.CreateField(targetedMethod.Modifiers.GetPrivate(), item.Attribute.Attribute.Type, name);
                                interceptorField[i].CustomAttributes.AddNonSerializedAttribute();

                                x.Load(interceptorField[i]).IsNull().Then(y =>
                                {
                                    y.Assign(interceptorField[i]).NewObj(item.Attribute);
                                    if (item.HasSyncRootInterface)
                                        y.Load(interceptorField[i]).As(__ISyncRoot.Type).Call(syncRoot.SyncRoot, method.SyncRoot);

                                    ImplementAssignMethodAttribute(builder, method.Item[i].AssignMethodAttributeInfos, interceptorField[i], x, false);
                                });
                                item.Attribute.Remove();
                            }
                        })
                        .Try(x =>
                        {
                            for (int i = 0; i < method.Item.Length; i++)
                            {
                                var item = method.Item[i];
                                x.Load(interceptorField[i]).As(item.Interface.ToBuilderType).Call(item.Interface.OnEnter, attributedMethod.OriginType, typeInstance, attributedMethod, x.GetParametersArray());
                            }

                            x.OriginalBody();

                            // Special case for async methods
                            if (method.Key.AsyncMethod != null && method.Key.Method.ReturnType.Fullname == __Task.Type.Fullname) // Task return
                            {
                                var exceptionVar = x.CreateVariable(__Exception.Type);

                                x.Assign(exceptionVar).Set(
                                    x.NewCode().Call(method.Key.AsyncMethod.OriginType.GetField("<>t__builder"), asyncTaskMethodBuilder.GetTask)
                                    .Call(task.GetException));

                                x.Load(exceptionVar).IsNotNull().Then(y =>
                                {
                                    for (int i = 0; i < method.Item.Length; i++)
                                        y.Load(interceptorField[i]).Callvirt(method.Item[i].Interface.OnException, exceptionVar);
                                });
                            }
                            else if (method.Key.AsyncMethod != null) // Task<> return
                            {
                                var exceptionVar = x.CreateVariable(__Exception.Type);
                                var taskArgument = method.Key.Method.ReturnType.GetGenericArgument(0);

                                x.Assign(exceptionVar).Set(
                                    x.NewCode().Call(method.Key.AsyncMethod.OriginType.GetField("<>t__builder"), asyncTaskMethodBuilderGeneric.GetTask.MakeGeneric(taskArgument))
                                    .Call(task.GetException));

                                x.Load(exceptionVar).IsNotNull().Then(y =>
                                {
                                    for (int i = 0; i < method.Item.Length; i++)
                                        y.Load(interceptorField[i]).Callvirt(method.Item[i].Interface.OnException, exceptionVar);
                                });
                            }
                        })
                        .Catch(__Exception.Type, x =>
                        {
                            if (method.Key.AsyncMethod == null)
                                for (int i = 0; i < method.Item.Length; i++)
                                    x.Load(interceptorField[i]).As(method.Item[i].Interface.ToBuilderType).Call(method.Item[i].Interface.OnException, x.Exception);

                            x.Rethrow();
                        })
                        .Finally(x =>
                        {
                            for (int i = 0; i < method.Item.Length; i++)
                                x.Load(interceptorField[i]).As(method.Item[i].Interface.ToBuilderType).Call(method.Item[i].Interface.OnExit);
                        })
                        .EndTry()
                        .Return()
                    .Replace();
                };
            }
        }
    }
}