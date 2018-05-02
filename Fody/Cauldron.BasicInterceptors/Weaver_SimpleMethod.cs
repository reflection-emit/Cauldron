using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil.Cil;
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
    public static void InterceptSimpleMethods(Builder builder)
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
            if (method.Item == null || method.Item.Length == 0)
                continue;

            builder.Log(LogTypes.Info, $"Implementing method interceptors: {method.Key.Method.DeclaringType.Name.PadRight(40, ' ')} {method.Key.Method.Name}({string.Join(", ", method.Key.Method.Parameters.Select(x => x.Name))})");

            var targetedMethod = method.Key.AsyncMethod ?? method.Key.Method;
            var attributedMethod = method.Key.Method;

            var interceptorField = new Field[method.Item.Length];
            var typeInstance = method.Key.Method.AsyncMethodHelper.Instance.With(x =>
            {
                if (x is Field field)
                    return field.Resolve(method.Key.Method.AsyncMethod.DeclaringType);

                return x;
            });

            if (method.RequiresSyncRootField)
            {
                if (method.SyncRoot.IsStatic)
                    targetedMethod.AsyncOriginType.CreateStaticConstructor().NewCoder()
                        .SetValue(method.SyncRoot, x => x.NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor))
                        .Insert(InsertionPosition.Beginning);
                else
                    foreach (var ctors in targetedMethod.AsyncOriginType.GetRelevantConstructors().Where(x => x.Name == ".ctor"))
                        ctors.NewCoder().SetValue(method.SyncRoot, x => x.NewObj(builder.GetType(typeof(object)).Import().ParameterlessContructor)).Insert(InsertionPosition.Beginning);
            }

            var coder = targetedMethod
             .NewCoder()
                 .Context(context =>
                 {
                     for (int i = 0; i < method.Item.Length; i++)
                     {
                         var item = method.Item[i];
                         var name = $"<{targetedMethod.Name}>_attrib{i}_{item.Attribute.Identification}";
                         interceptorField[i] = targetedMethod.AsyncOriginType.CreateField(targetedMethod.Modifiers.GetPrivate(), item.Interface.ToBuilderType, name);
                         interceptorField[i].CustomAttributes.AddNonSerializedAttribute();

                         context.If(x => x.Load(interceptorField[i]).IsNull(), then =>
                         {
                             then.SetValue(interceptorField[i], x => x.NewObj(item.Attribute));
                             if (item.HasSyncRootInterface)
                                 then.Load(interceptorField[i]).As(__ISyncRoot.Type).Call(syncRoot.SyncRoot, method.SyncRoot);

                             ModuleWeaver.ImplementAssignMethodAttribute(builder, method.Item[i].AssignMethodAttributeInfos, interceptorField[i], item.Attribute.Attribute.Type, context);
                             return then;
                         });

                         context.Load(interceptorField[i]).Call(item.Interface.OnEnter, attributedMethod.OriginType, typeInstance, attributedMethod, context.GetParametersArray());

                         item.Attribute.Remove();
                     }

                     return context;
                 });

            var position = coder.GetFirstOrDefaultPosition(x => x.OpCode == OpCodes.Stelem_Ref);

            if (position == null)
                coder.Insert(InsertionPosition.Beginning);
            else
                coder.Insert(InsertionAction.After, position);
        };
    }
}