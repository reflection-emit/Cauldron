using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Extensions;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void ImplementTimedCache(Builder builder)
        {
            if (!__TimedCacheAttribute.IsReferenced)
                return;

            var timedCacheAttribute = __TimedCacheAttribute.Instance;
            var methods = builder.FindMethodsByAttribute(__TimedCacheAttribute.Type.Fullname);

            if (!methods.Any())
                return;

            var task = __Task.Instance;
            var task_1 = __Task_1.Instance;

            foreach (var method in methods)
            {
                this.Log($"Implementing TimedCache in method {method.Method.Name}");

                if (method.Method.ReturnType.Fullname == "System.Void")
                {
                    this.Log(LogTypes.Warning, method.Method, "TimedCacheAttribute does not support void return types");
                    continue;
                }

                var keyName = "<>timedcache_key";
                var timecacheVarName = "<>timedcache";

                if (method.AsyncMethod == null && method.Method.ReturnType.Inherits(__Task.Type.Fullname))
                    this.Log(LogTypes.Error, method.Method, $"- TimedCacheAttribute for method {method.Method.Name} will not be implemented. Methods that returns 'Task' without async are not supported.");
                else if (method.AsyncMethod == null)
                    method.Method.NewCoder()
                        .Context(context =>
                        {
                            var timedCache = context.AssociatedMethod.GetOrCreateVariable(method.Attribute.Type, timecacheVarName);
                            var key = context.AssociatedMethod.GetOrCreateVariable(timedCacheAttribute.CreateKey.ReturnType, keyName);
                            var returnVariable = context.GetOrCreateReturnVariable();

                            context.SetValue(timedCache, x => x.NewObj(method));

                            // Create a cache key
                            context.SetValue(key, x => x.Call(timedCacheAttribute.CreateKey, method.Method.Fullname, context.GetParametersArray()));

                            // check
                            context.If(x => x.Load(timedCache).Call(timedCacheAttribute.HasCache, key).Is(true), then =>
                                      {
                                          return then.SetValue(returnVariable, x => x.Load(timedCache).Call(timedCacheAttribute.GetCache, key).As(method.Method.ReturnType))
                                              .Return();
                                      });

                            context.SetValue(returnVariable, x => x.OriginalBody(true));

                            // Set the cache
                            context.Load(timedCache).Call(timedCacheAttribute.SetCache, key, returnVariable);

                            return context.Load(returnVariable).Return();
                        })
                        .Replace();
                else if (method.AsyncMethod != null)
                {
                    method.Method.NewCoder()
                        .Context(context =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var timedCache = context.AssociatedMethod.GetOrCreateVariable(method.Attribute.Type, timecacheVarName);
                            var cacheKey = context.AssociatedMethod.GetOrCreateVariable(typeof(string));

                            context.SetValue(cacheKey, x => x.Call(timedCacheAttribute.CreateKey, method.Method.Fullname, context.GetParametersArray()));
                            context.SetValue(timedCache, x => x.NewObj(method));
                            context.If(x => x.Load(timedCache).Call(timedCacheAttribute.HasCache, cacheKey).Is(true), then =>
                                  {
                                      return then.Call(task_1.FromResult.MakeGeneric(taskReturnType), x => x.Load(timedCache).Call(timedCacheAttribute.GetCache, cacheKey).As(taskReturnType))
                                         .Return();
                                  });

                            return context;
                        }).Insert(InsertionPosition.Beginning);

                    method.Method.NewCoder()
                        .Context(context =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var returnVariable = context.GetOrCreateReturnVariable();
                            return context
                                .Load(variable: x => x.GetVariable(2))
                                    .Call(timedCacheAttribute.SetCache, x => x.AssociatedMethod.GetVariable(3), x => x.Load(returnVariable).Call(task_1.GetResult.MakeGeneric(taskReturnType)))
                                    .End;
                        }).Insert(InsertionPosition.End);
                }

                method.Attribute.Remove();
            }
        }
    }
}