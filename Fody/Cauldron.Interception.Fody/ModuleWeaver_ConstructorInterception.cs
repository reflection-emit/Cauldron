using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody.HelperTypes;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void InterceptConstructors(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            using (new StopwatchLog(this, "constructor"))
            {
                var iConstructorInterceptor = new __IConstructorInterceptor();
                var syncRoot = new __ISyncRoot();
                var exception = new __Exception();

                var constructors = builder
                    .FindMethodsByAttributes(attributes)
                    .Where(x => !x.Method.OriginType.IsInterface)
                    .GroupBy(x => new MethodKey(x.Method, null))
                    .Select(x => new MethodBuilderInfo<MethodBuilderInfoItem<__IConstructorInterceptor>>(x.Key, x.Select(y => new MethodBuilderInfoItem<__IConstructorInterceptor>(y, iConstructorInterceptor))))
                    .ToArray();

                foreach (var constructor in constructors)
                {
                    if (constructor.Item == null || constructor.Item.Length == 0)
                        continue;

                    var targetedConstrutor = constructor.Key.Method;

                    if (!targetedConstrutor.IsCtor && !targetedConstrutor.IsCCtor)
                        continue;

                    this.Log($"Implementing constructors interceptors: {constructor.Key.Method.DeclaringType.Name.PadRight(40, ' ')} {constructor.Key.Method.Name}({string.Join(", ", constructor.Key.Method.Parameters.Select(x => x.Name))})");

                    if (constructor.RequiresSyncRootField)
                        this.Log(LogTypes.Warning, targetedConstrutor, $"An interceptor applied to the constructor has implemented ISyncRoot. This is not supported. The interceptor may not work correctly.");

                    CodeBlock parametersArray = null;
                    var localVariables = new LocalVariable[constructor.Item.Length];
                    Coder InterceptorInit(Coder contextCoder, bool isBeforeInit)
                    {
                        parametersArray = contextCoder.GetParametersArray();

                        for (int i = 0; i < constructor.Item.Length; i++)
                        {
                            var item = constructor.Item[i];
                            localVariables[i] = contextCoder.AssociatedMethod.GetOrCreateVariable(item.Interface.ToBuilderType);

                            contextCoder.SetValue(localVariables[i], x => x.NewObj(item.Attribute));
                            contextCoder.Load(localVariables[i]).Call(item.Interface.OnBeforeInitialization, targetedConstrutor.OriginType, targetedConstrutor, parametersArray);

                            item.Attribute.Remove();
                        }

                        return contextCoder;
                    }

                    if (targetedConstrutor.IsCtor)
                        targetedConstrutor.NewCoder()
                            .Context(x => InterceptorInit(x, true))
                            .Insert(InsertionPosition.CtorBeforeInit);

                    targetedConstrutor.NewCoder()
                        .Context(x =>
                        {
                            if (targetedConstrutor.IsCCtor)
                                InterceptorInit(x, false);

                            for (int i = 0; i < constructor.Item.Length; i++)
                            {
                                var item = constructor.Item[i];
                                ImplementAssignMethodAttribute(builder, item.AssignMethodAttributeInfos, localVariables[i], item.Attribute.Attribute.Type, x);

                                item.Attribute.Remove();
                            }

                            return x;
                        })
                        .Try(x =>
                        {
                            for (int i = 0; i < constructor.Item.Length; i++)
                            {
                                var item = constructor.Item[i];
                                x.Load(localVariables[i]).Call(item.Interface.OnEnter, targetedConstrutor.OriginType, CodeBlocks.This, targetedConstrutor, parametersArray);
                            }

                            return x.OriginalBody();
                        })
                        .Catch(exception.ToBuilderType, (ex, e) =>
                        {
                            return ex.If(x => x.Or(constructor.Item, (coder, y, i) => coder.Load(localVariables[i]).Call(y.Interface.OnException, e())).Is(true),
                                    then => ex.NewCoder().Rethrow())
                                .DefaultValue()
                                .Return();
                        })
                        .Finally(x =>
                        {
                            for (int i = 0; i < constructor.Item.Length; i++)
                                x.Load(localVariables[i]).Call(constructor.Item[i].Interface.OnExit);
                            return x;
                        })
                        .EndTry()
                        .Return()
                    .Replace();
                }
            }
        }
    }
}