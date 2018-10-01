#r "System.Security"

using System;
using System.Text;
using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using System.Security.Cryptography;
using System.Linq;
using Cauldron.Interception.Cecilator.Coders;

public static class TimedCacheInterceptorWeaver
{
    public const string Name = "TimedCache Interceptor";
    public const int Priority = 50;

    [Display("TimedCache Interceptor")]
    public static void ImplementTimedCache(Builder builder)
    {
        if (!__TimedCacheAttribute.IsReferenced)
            return;

        var timedCacheAttribute = __TimedCacheAttribute.Instance;
        var methods = builder.FindMethodsByAttribute(__TimedCacheAttribute.Type.Fullname);

        if (!methods.Any())
            return;

        foreach (var method in methods)
        {
            var targetMethod = method.AsyncMethod ?? method.Method;

            builder.Log(LogTypes.Info, $"Implementing TimedCache in method {method.Method.Name}");

            if (method.Method.ReturnType.Fullname == "System.Void")
            {
                builder.Log(LogTypes.Warning, method.Method, "TimedCacheAttribute does not support void return types");
                continue;
            }

            if (method.AsyncMethod == null && method.Method.ReturnType.Inherits(BuilderTypes.Task.BuilderType.Fullname))
            {
                builder.Log(LogTypes.Error, method.Method, $"- TimedCacheAttribute for method {method.Method.Name} will not be implemented. Methods that returns 'Task' without async are not supported.");
                continue;
            }

            var keyName = "<>timedcache_key";
            var timecacheVarName = "<>timedcache";

            targetMethod.NewCoder()
                    .Context(context =>
                    {
                        var timedCache = context.AssociatedMethod.GetOrCreateVariable(method.Attribute.Type, timecacheVarName);
                        var key = context.AssociatedMethod.GetOrCreateVariable(timedCacheAttribute.CreateKey.ReturnType, keyName);
                        var returnVariable = context.GetOrCreateReturnVariable();

                        context.SetValue(timedCache, x => x.NewObj(method));

                        // Create a cache key
                        context.SetValue(key, x => x.Call(timedCacheAttribute.CreateKey, GetHash(method.Method.Fullname), context.GetParametersArray()));

                        // check
                        context.If(x => x.Load(timedCache).Call(timedCacheAttribute.HasCache, key).Is(true), then =>
                        {
                            if (method.AsyncMethod == null)
                                return then.Load(timedCache).Call(timedCacheAttribute.GetCache, key).As(method.Method.ReturnType).Return();
                            else
                                return then.SetValue(returnVariable, x => x.Load(timedCache).Call(timedCacheAttribute.GetCache, key)).Return();
                        });

                        if (method.AsyncMethod == null)
                            context.SetValue(returnVariable, x => x.OriginalBody(true));
                        else
                            context.OriginalBody();

                        // Set the cache
                        // The async methods need a little more love
                        if (method.AsyncMethod != null)
                        {
                            context.SetValue(timedCache, x => x.NewObj(method));
                            context.SetValue(key, x => x.Call(timedCacheAttribute.CreateKey, GetHash(method.Method.Fullname), context.GetParametersArray()));
                        }

                        context.Load(timedCache).Call(timedCacheAttribute.SetCache, key, returnVariable);

                        return context.Load(returnVariable).Return();
                    })
                    .Replace();
        }
    }

    private static string GetHash(this string target)
    {
        using (var md5 = new MD5CryptoServiceProvider())
            return BitConverter.ToString(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(target))).Replace("-", "");
    }
}