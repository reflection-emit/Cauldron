using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.Extensions;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void ImplementMethodCache(Builder builder)
        {
            var methods = builder.FindMethodsByAttribute("Cauldron.Interception.CacheAttribute");

            if (!methods.Any())
                return;

            var task = new __Task(builder);
            var task_1 = new __Task_1(builder);

            foreach (var method in methods)
            {
                this.Log($"Implementing Cache for method {method.Method.Name}");

                if (method.Method.ReturnType.Fullname == "System.Void")
                {
                    this.Log(LogTypes.Warning, method.Method, "CacheAttribute does not support void return types");
                    continue;
                }

                var cacheField = $"<{method.Method.Name}>m__MethodCache_{method.Identification}";

                if (method.AsyncMethod == null && method.Method.ReturnType.Inherits(task.Type.Fullname))
                    this.Log(LogTypes.Warning, method.Method, $"- CacheAttribute for method {method.Method.Name} will not be implemented. Methods that returns 'Task' without async are not supported.");
                else if (method.AsyncMethod == null)
                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var cache = method.Method.OriginType.CreateField(method.Method.Modifiers.GetPrivate(), method.Method.ReturnType, cacheField);
                            var returnVariable = x.GetReturnVariable();

                            x.Load(cache).IsNull().Then(y =>
                            {
                                y.OriginalBody().StoreLocal(returnVariable).Return();
                            })
                            .Load(cache).Return();
                        })
                        .Replace();
                else if (method.AsyncMethod != null)
                {
                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var cache = method.Method.OriginType.CreateField(method.Method.Modifiers.GetPrivate(), taskReturnType, cacheField);

                            x.Load(cache).IsNotNull().Then(y =>
                            {
                                y.Call(task_1.FromResult.MakeGeneric(taskReturnType), cache).Return();
                            });
                        }).Insert(InsertionPosition.Beginning);

                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var cache = method.Method.OriginType.GetField(cacheField);
                            var returnVariable = x.GetReturnVariable();
                            x.Assign(cache).Set(x.NewCode().Call(returnVariable, task_1.GetResult.MakeGeneric(taskReturnType)));
                        }).Insert(InsertionPosition.End);
                }

                method.Attribute.Remove();
            }
        }
    }
}