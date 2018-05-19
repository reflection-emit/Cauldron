using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class Weaver_SimpleMethod
{
    public static string Name = "Simple Method Interceptors";

    public static int Priority = 1;

    private static IEnumerable<BuilderType> simpleMethodInterceptionAttributes;

    static Weaver_SimpleMethod()
    {
        simpleMethodInterceptionAttributes = Builder.Current.FindAttributesByInterfaces(
            "Cauldron.Interception.ISimpleMethodInterceptor");
    }

    [Display("Type-Wide Simple Method Interception")]
    public static void ImplementTypeWideMethodInterception(Builder builder) => Weaver_Method.ImplementTypeWideMethodInterception(builder, simpleMethodInterceptionAttributes);

    [Display("Simple Method Interception")]
    public static void InterceptSimpleMethods(Builder builder, int o)
    {
        if (!simpleMethodInterceptionAttributes.Any())
            return;

        var asyncTaskMethodBuilder = new __AsyncTaskMethodBuilder();
        var asyncTaskMethodBuilderGeneric = new __AsyncTaskMethodBuilder_1();
        var syncRoot = new __ISyncRoot();
        var task = new __Task();
        var exception = new __Exception();

        var methods = builder
            .FindMethodsByAttributes(simpleMethodInterceptionAttributes)
            .Where(x => !x.Method.IsPropertyGetterSetter)
            .GroupBy(x => new MethodKey(x.Method, x.AsyncMethod))
            .Select(x => new MethodBuilderInfo<MethodBuilderInfoItem<__ISimpleMethodInterceptor>>(x.Key, x.Select(y => new MethodBuilderInfoItem<__ISimpleMethodInterceptor>(y, __ISimpleMethodInterceptor.Instance))))
            .OrderBy(x => x.Key.Method.DeclaringType.Fullname)
            .ToArray();

        foreach (var method in methods)
        {
            if (method.Items == null || method.Items.Length == 0 || method.Key.Method.IsAbstract)
                continue;

            builder.Log(LogTypes.Info, $"Implementing method interceptors: {method.Key.Method.DeclaringType.Name.PadRight(40, ' ')} {method.Key.Method.Name}({string.Join(", ", method.Key.Method.Parameters.Select(x => x.Name))})");

            var targetedMethod = method.Key.AsyncMethod ?? method.Key.Method;
            var attributedMethod = method.Key.Method;
            var interceptor = new CecilatorBase[method.Items.Length];

            if (method.RequiresSyncRootField)
            {
                if (method.SyncRoot.IsStatic)
                    method.Key.Method.DeclaringType.CreateStaticConstructor().NewCoder()
                        .SetValue(method.SyncRoot, x => x.NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor))
                        .Insert(InsertionPosition.Beginning);
                else
                    foreach (var ctors in method.Key.Method.DeclaringType.GetRelevantConstructors().Where(x => x.Name == ".ctor"))
                        ctors.NewCoder().SetValue(method.SyncRoot, x => x.NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor))
                            .Insert(InsertionPosition.Beginning);
            }

            targetedMethod
                .NewCoder()
                .Context(x =>
                {
                    for (int i = 0; i < method.Items.Length; i++)
                    {
                        var item = method.Items[i];
                        var alwaysCreateNewInstance = item.InterceptorInfo.AlwaysCreateNewInstance;
                        var name = $"<{targetedMethod.Name}>_attrib{i}_{item.Attribute.Identification}";
                        var methodCoder = method.Key.Method.IsAsync ? method.Key.Method.NewCoder() : x;
                        var newInterceptor = alwaysCreateNewInstance ?
                            method.Key.Method.GetOrCreateVariable(item.Interface.ToBuilderType) as CecilatorBase :
                            method.Key.Method.DeclaringType.CreateField(method.Key.Method.Modifiers.GetPrivate(), item.Interface.ToBuilderType, name);

                        Coder codeInterceptorInstance(Coder interceptorInstanceCoder)
                        {
                            interceptorInstanceCoder.SetValue(newInterceptor, z => z.NewObj(item.Attribute));
                            if (item.HasSyncRootInterface)
                                interceptorInstanceCoder.Load<ICasting>(newInterceptor).As(__ISyncRoot.Type).To<ICallMethod<CallCoder>>().Call(syncRoot.SyncRoot, method.SyncRoot);

                            ModuleWeaver.ImplementAssignMethodAttribute(builder, method.Items[i].AssignMethodAttributeInfos, newInterceptor, item.Attribute.Attribute.Type, interceptorInstanceCoder);
                            return interceptorInstanceCoder;
                        }

                        if (alwaysCreateNewInstance)
                            codeInterceptorInstance(methodCoder);
                        else
                            methodCoder.If(y => y.Load<IRelationalOperators>(newInterceptor).IsNull(), y => codeInterceptorInstance(y));

                        if (method.Key.Method.IsAsync)
                            methodCoder.Insert(InsertionPosition.Beginning);

                        interceptor[i] = method.Key.Method.IsAsync ?
                            method.Key.Method.AsyncMethodHelper.InsertFieldToAsyncStateMachine(name, item.Interface.ToBuilderType, z => newInterceptor) :
                            newInterceptor;
                        (interceptor[i] as Field)?.CustomAttributes.AddNonSerializedAttribute();
                        item.Attribute.Remove();
                    }

                    return x;
                })
                .Context(x =>
                {
                    for (int i = 0; i < method.Items.Length; i++)
                        x.Load<ICallMethod<CallCoder>>(interceptor[i]).Call(method.Items[i].Interface.OnEnter, attributedMethod.OriginType, CodeBlocks.This, attributedMethod,
                            method.Key.Method.Parameters.Length > 0 ? x.GetParametersArray() : null);

                    return x.OriginalBody();
                })
            .Return()
            .Replace();
        };
    }
}