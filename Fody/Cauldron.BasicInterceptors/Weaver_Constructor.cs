using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using System.Collections.Generic;
using System.Linq;

public static class Weaver_Constructor
{
    public static string Name = "Constructor Interceptors";
    public static int Priority = 0;
    private static IEnumerable<BuilderType> constructorInterceptionAttributes;

    static Weaver_Constructor()
    {
        constructorInterceptionAttributes = Builder.Current.FindAttributesByInterfaces(BuilderTypes2.IConstructorInterceptor.BuilderType.Fullname);
    }

    [Display("Constructor Interception")]
    public static void InterceptConstructors(Builder builder)
    {
        if (!constructorInterceptionAttributes.Any())
            return;

        var constructors = builder
            .FindMethodsByAttributes(constructorInterceptionAttributes)
            .Where(x => !x.Method.OriginType.IsInterface)
            .GroupBy(x => new MethodKey(x.Method, null))
            .Select(x => new MethodBuilderInfo<MethodBuilderInfoItem<BuilderTypeIConstructorInterceptor>>(x.Key, x.Select(y => new MethodBuilderInfoItem<BuilderTypeIConstructorInterceptor>(y, BuilderTypes2.IConstructorInterceptor))))
            .ToArray();

        foreach (var constructor in constructors)
        {
            if (constructor.Items == null || constructor.Items.Length == 0)
                continue;

            var targetedConstrutor = constructor.Key.Method;

            if (!targetedConstrutor.IsCtor && !targetedConstrutor.IsCCtor)
                continue;

            builder.Log(LogTypes.Info, $"Implementing constructors interceptors: {constructor.Key.Method.DeclaringType.Name.PadRight(40, ' ')} {constructor.Key.Method.Name}({string.Join(", ", constructor.Key.Method.Parameters.Select(x => x.Name))})");

            if (constructor.RequiresSyncRootField)
                builder.Log(LogTypes.Warning, targetedConstrutor, $"An interceptor applied to the constructor has implemented ISyncRoot. This is not supported. The interceptor may not work correctly.");

            CodeBlock parametersArray = null;
            var localVariables = new LocalVariable[constructor.Items.Length];
            Coder InterceptorInit(Coder contextCoder, bool isBeforeInit)
            {
                parametersArray = contextCoder.GetParametersArray();

                for (int i = 0; i < constructor.Items.Length; i++)
                {
                    var item = constructor.Items[i];
                    localVariables[i] = contextCoder.AssociatedMethod.GetOrCreateVariable(item.Interface.BuilderType);

                    contextCoder.SetValue(localVariables[i], x => x.NewObj(item.Attribute));
                    contextCoder.Load(localVariables[i]).Call(item.Interface.GetMethod_OnBeforeInitialization(), targetedConstrutor.OriginType, targetedConstrutor, parametersArray);

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

                    for (int i = 0; i < constructor.Items.Length; i++)
                    {
                        var item = constructor.Items[i];
                        ModuleWeaver.ImplementAssignMethodAttribute(builder, item.AssignMethodAttributeInfos, localVariables[i], item.Attribute.Attribute.Type, x);

                        item.Attribute.Remove();
                    }

                    return x;
                })
                .Try(x =>
                {
                    for (int i = 0; i < constructor.Items.Length; i++)
                    {
                        var item = constructor.Items[i];
                        x.Load(localVariables[i]).Call(item.Interface.GetMethod_OnEnter(), targetedConstrutor.OriginType, CodeBlocks.This, targetedConstrutor, parametersArray);
                    }

                    return x;
                })
                .Catch(BuilderTypes.Exception.BuilderType, (ex, e) =>
                {
                    return ex.If(x => x.Or(constructor.Items, (coder, y, i) => coder.Load(localVariables[i]).Call(y.Interface.GetMethod_OnException(), e())).Is(true),
                            then => ex.NewCoder().Rethrow())
                        .DefaultValue()
                        .Return();
                })
                .Finally(x =>
                {
                    for (int i = 0; i < constructor.Items.Length; i++)
                        x.Load(localVariables[i]).Call(constructor.Items[i].Interface.GetMethod_OnException());
                    return x;
                })
                .EndTry()
                .Return()
            .Replace();
        }
    }
}