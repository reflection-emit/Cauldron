using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class Weaver_Method
{
    public static string Name = "Method Interceptors";
    public static int Priority = 0;
    private static IEnumerable<BuilderType> methodInterceptionAttributes;

    static Weaver_Method()
    {
        methodInterceptionAttributes = Builder.Current.FindAttributesByInterfaces(
           "Cauldron.Interception.IMethodInterceptor");
    }

    [Display("Type-Wide Method Interception")]
    public static void ImplementTypeWideMethodInterception(Builder builder) => ImplementTypeWideMethodInterception(builder, methodInterceptionAttributes);

    [Display("Method Interception")]
    public static void InterceptMethods(Builder builder)
    {
        if (!methodInterceptionAttributes.Any())
            return;

        var asyncTaskMethodBuilder = new __AsyncTaskMethodBuilder();
        var asyncTaskMethodBuilderGeneric = new __AsyncTaskMethodBuilder_1();
        var syncRoot = new __ISyncRoot();
        var task = new __Task();
        var exception = new __Exception();

        var methods = builder
            .FindMethodsByAttributes(methodInterceptionAttributes)
            .Where(x => !x.Method.IsPropertyGetterSetter)
            .GroupBy(x => new MethodKey(x.Method, x.AsyncMethod))
            .Select(x => new MethodBuilderInfo<MethodBuilderInfoItem<__IMethodInterceptor>>(x.Key, x.Select(y => new MethodBuilderInfoItem<__IMethodInterceptor>(y, __IMethodInterceptor.Instance))))
            .OrderBy(x => x.Key.Method.DeclaringType.Fullname)
            .ToArray();

        Coder codeMe(Coder coder, MethodBuilderInfoItem<__IMethodInterceptor> attribute, Field field, LocalVariable localVariable, Func<FieldCoder, Coder> funcA, Func<VariableCoder, Coder> funcB)
        {
            if (attribute.InterceptorInfo.AlwaysCreateNewInstance)
                return funcA(coder.Load(field));
            else
                return funcB(coder.Load(localVariable));
        }

        foreach (var method in methods)
        {
            if (method.Item == null || method.Item.Length == 0 || method.Key.Method.IsAbstract)
                continue;

            builder.Log(LogTypes.Info, $"Implementing method interceptors: {method.Key.Method.DeclaringType.Name.PadRight(40, ' ')} {method.Key.Method.Name}({string.Join(", ", method.Key.Method.Parameters.Select(x => x.Name))})");

            var targetedMethod = method.Key.AsyncMethod ?? method.Key.Method;
            var attributedMethod = method.Key.Method;
            var interceptor = new CecilatorBase[method.Item.Length];

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

            var coder = targetedMethod
                .NewCoder()
                .Context(x =>
                {
                    for (int i = 0; i < method.Item.Length; i++)
                    {
                        var item = method.Item[i];
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
                                (interceptorInstanceCoder.Load<ICasting>(newInterceptor).As(__ISyncRoot.Type) as ICallMethod<CallCoder>).Call(syncRoot.SyncRoot, method.SyncRoot);

                            ModuleWeaver.ImplementAssignMethodAttribute(builder, method.Item[i].AssignMethodAttributeInfos, newInterceptor, item.Attribute.Attribute.Type, interceptorInstanceCoder);
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
                .Try(x =>
                {
                    for (int i = 0; i < method.Item.Length; i++)
                        x.Load<ICallMethod<CallCoder>>(interceptor[i]).Call(method.Item[i].Interface.OnEnter, attributedMethod.OriginType, CodeBlocks.This, attributedMethod,
                            method.Key.Method.Parameters.Length > 0 ? x.GetParametersArray() : null);

                    return x.OriginalBody();
                });

            if (method.Key.AsyncMethod == null)
                coder.Catch(__Exception.Type, (eCoder, e) => eCoder.If(x =>
                    {
                        var or = x.Load<ICallMethod<BooleanExpressionCallCoder>>(interceptor[0]).Call(method.Item[0].Interface.OnException, e());
                        for (int i = 1; i < method.Item.Length; i++)
                            or.Or(y => y.Load<ICallMethod<CallCoder>>(interceptor[i]).Call(method.Item[i].Interface.OnException, e()));

                        return or.Is(true);
                    }, then => eCoder.NewCoder().Rethrow())
                        .DefaultValue().Return());

            coder.Finally(x =>
            {
                for (int i = 0; i < method.Item.Length; i++)
                    x.Load<ICallMethod<CallCoder>>(interceptor[i]).Call(method.Item[i].Interface.OnExit);

                return x;
            })
            .EndTry()
            .Return()
            .Replace();

            if (method.Key.AsyncMethod != null)
            {
                // Special case for async methods
                var exceptionBlock = method.Key.Method.AsyncMethodHelper.GetAsyncStateMachineExceptionBlock();
                targetedMethod
                    .NewCoder().Context(context =>
                    {
                        var exceptionVariable = method.Key.Method.AsyncMethodHelper.GetAsyncStateMachineExceptionVariable();

                        return context.If(x =>
                         {
                             var or = x.Load(interceptor[0] as Field).Call(method.Item[0].Interface.OnException, exceptionVariable);

                             for (int i = 1; i < method.Item.Length; i++)
                                 or.Or(y => y.Load(interceptor[i] as Field).Call(method.Item[i].Interface.OnException, exceptionVariable));

                             return or.Is(false);
                         }, x => x.Jump(exceptionBlock.Item1.End));
                    }).Insert(InsertionAction.After, exceptionBlock.Item1.Beginning);
            }
        };
    }

    internal static void ImplementTypeWideMethodInterception(Builder builder, IEnumerable<BuilderType> attributes)
    {
        if (!methodInterceptionAttributes.Any())
            return;

        var types = builder
            .FindTypesByAttributes(methodInterceptionAttributes)
            .GroupBy(x => x.Type)
            .Select(x => new
            {
                x.Key,
                Item = x.ToArray()
            })
            .ToArray();

        foreach (var type in types)
        {
            builder.Log(LogTypes.Info, $"Implementing interceptors in type {type.Key.Fullname}");

            foreach (var method in type.Key.Methods)
            {
                if (method.IsConstructor || method.IsPropertyGetterSetter)
                    continue;

                for (int i = 0; i < type.Item.Length; i++)
                    method.CustomAttributes.Copy(type.Item[i].Attribute);
            }

            for (int i = 0; i < type.Item.Length; i++)
                type.Item[i].Remove();
        }
    }
}