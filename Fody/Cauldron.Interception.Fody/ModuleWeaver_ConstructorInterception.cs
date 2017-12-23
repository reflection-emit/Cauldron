using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void InterceptConstructors(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();

            var iConstructorInterceptor = new __IConstructorInterceptor(builder);
            var syncRoot = new __ISyncRoot(builder);
            var exception = new __Exception(builder);

            var constructors = builder
                .FindMethodsByAttributes(attributes)
                .Where(x => !x.Method.OriginType.IsInterface)
                .GroupBy(x => new MethodKey(x.Method, null))
                .Select(x => new MethodBuilderInfo<MethodBuilderInfoItem<__IConstructorInterceptor>>(x.Key, x.Select(y => new MethodBuilderInfoItem<__IConstructorInterceptor>(y, iConstructorInterceptor))))
                .ToArray();

            foreach (var constructor in constructors)
            {
                this.Log($"Implementing constructors in method {constructor.Key.Method}");

                var targetedConstrutor = constructor.Key.Method;

                if (constructor.RequiresSyncRootField)
                    this.Log(LogTypes.Warning, targetedConstrutor, $"An interceptor applied to the constructor has implemented ISyncRoot. This is not supported. The interceptor may not work correctly.");

                Crumb parametersArray = null;
                var localVariables = new LocalVariable[constructor.Item.Length];
                var interceptorInit = new Action<ICode>(x =>
                {
                    parametersArray = x.GetParametersArray();

                    for (int i = 0; i < constructor.Item.Length; i++)
                    {
                        var item = constructor.Item[i];
                        localVariables[i] = x.CreateVariable(item.Interface.Type);

                        x.Assign(localVariables[i]).NewObj(item.Attribute);
                        x.Load(localVariables[i]).As(item.Interface.Type)
                            .Call(item.Interface.OnBeforeInitialization, targetedConstrutor.OriginType, targetedConstrutor, parametersArray);

                        ImplementAssignMethodAttribute(builder, constructor.Item[i].AssignMethodAttributeInfos, localVariables[i], x);

                        item.Attribute.Remove();
                    }
                });

                if (targetedConstrutor.IsCtor)
                    targetedConstrutor.NewCode()
                        .Context(x => interceptorInit(x))
                        .Insert(InsertionPosition.CtorBeforeInit);

                targetedConstrutor.NewCode()
                    .Context(x =>
                    {
                        if (targetedConstrutor.IsCCtor)
                            interceptorInit(x);
                    })
                    .Try(x =>
                    {
                        for (int i = 0; i < constructor.Item.Length; i++)
                        {
                            var item = constructor.Item[i];
                            x.Load(localVariables[i]).As(item.Interface.Type)
                                .Call(item.Interface.OnEnter, targetedConstrutor.OriginType, Crumb.This, targetedConstrutor, parametersArray);
                        }

                        x.OriginalBody();
                    })
                    .Catch(exception.Type, x =>
                    {
                        if (constructor.Key.AsyncMethod == null)
                            for (int i = 0; i < constructor.Item.Length; i++)
                                x.Load(localVariables[i]).As(constructor.Item[i].Interface.Type).Call(constructor.Item[i].Interface.OnException, x.Exception);

                        x.Rethrow();
                    })
                    .Finally(x =>
                    {
                        for (int i = 0; i < constructor.Item.Length; i++)
                            x.Load(localVariables[i]).As(constructor.Item[i].Interface.Type).Call(constructor.Item[i].Interface.OnExit);
                    })
                    .EndTry()
                    .Return()
                .Replace();
            }

            stopwatch.Stop();
            this.Log($"Implementing constructor interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }
    }
}