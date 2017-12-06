using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void ImplementTimedCache(Builder builder)
        {
            if (!builder.TypeExists(__TimedCacheAttribute.TypeName))
                return;

            var timedCacheAttribute = new __TimedCacheAttribute(builder);
            var methods = builder.FindMethodsByAttribute(timedCacheAttribute.Type.Fullname);

            if (!methods.Any())
                return;

            var task = new __Task(builder);
            var task_1 = new __Task_1(builder);

            foreach (var method in methods)
            {
                this.LogInfo($"Implementing TimedCache in method {method.Method.Name}");

                if (method.Method.ReturnType.Fullname == "System.Void")
                {
                    this.LogWarning("TimedCacheAttribute does not support void return types");
                    continue;
                }

                var keyName = "<>timedcache_key";
                var timecacheVarName = "<>timedcache";

                if (method.AsyncMethod == null && method.Method.ReturnType.Inherits(task.Type.Fullname))
                    this.LogError($"- TimedCacheAttribute for method {method.Method.Name} will not be implemented. Methods that returns 'Task' without async are not supported.");
                else if (method.AsyncMethod == null)
                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var timedCache = x.CreateVariable(timecacheVarName, method.Attribute.Type);
                            var key = x.CreateVariable(keyName, timedCacheAttribute.CreateKey);
                            var returnVariable = x.GetReturnVariable();

                            x.Assign(timedCache).NewObj(method);

                            // Create a cache key
                            x.Call(timedCacheAttribute.CreateKey, method.Method.Fullname, x.GetParametersArray())
                                    .StoreLocal(key);

                            // check
                            x.Load(timedCache).Call(timedCacheAttribute.HasCache, key)
                                    .IsTrue().Then(y =>
                                    {
                                        y.Load(timedCache).Call(timedCacheAttribute.GetCache, key)
                                            .As(method.Method.ReturnType)
                                            .StoreLocal(returnVariable)
                                            .Return();
                                    });

                            x.OriginalBodyNewMethod().StoreLocal(returnVariable);

                            // Set the cache
                            x.Load(timedCache).Call(timedCacheAttribute.SetCache, key, returnVariable);

                            x.Load(returnVariable).Return();
                        })
                        .Replace();
                else if (method.AsyncMethod != null)
                {
                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var timedCache = x.CreateVariable(timecacheVarName, method.Attribute.Type);
                            var cacheKey = x.CreateVariable(typeof(string));

                            x.Assign(cacheKey).Set(x.NewCode().Call(timedCacheAttribute.CreateKey, method.Method.Fullname, x.GetParametersArray()));
                            x.Assign(timedCache).NewObj(method);
                            x.Load(timedCache).Call(timedCacheAttribute.HasCache, cacheKey)
                                .IsTrue().Then(y =>
                                {
                                    y.Call(task_1.FromResult.MakeGeneric(taskReturnType), y.NewCode().Call(timedCache, timedCacheAttribute.GetCache, cacheKey).As(taskReturnType))
                                        .Return();
                                });
                        }).Insert(InsertionPosition.Beginning);

                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var returnVariable = x.GetReturnVariable();
                            x.LoadVariable(2).Call(timedCacheAttribute.SetCache, x.NewCode().LoadVariable(3), x.NewCode().Call(returnVariable, task_1.GetResult.MakeGeneric(taskReturnType)));
                        }).Insert(InsertionPosition.End);
                }

                method.Attribute.Remove();
            }
        }
    }
}