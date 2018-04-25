using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using System;
using System.Linq;

internal static class ModuleWeaver
{
    public static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos, Field interceptorInstance, BuilderType contentType, Coder coder) =>
        ImplementAssignMethodAttribute(builder, assignMethodAttributeInfos,
            (delegateCtor, method, assignMethodAttributeInfo) => coder.Load(interceptorInstance)
                .As(contentType)
                .SetValue(assignMethodAttributeInfo.AttributeField, y => y.NewObj(delegateCtor, method.ThisOrNull(), method)));

    public static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos, LocalVariable interceptorInstance, BuilderType contentType, Coder coder) =>
        ImplementAssignMethodAttribute(builder, assignMethodAttributeInfos,
            (delegateCtor, method, assignMethodAttributeInfo) => coder.Load(interceptorInstance)
                .As(contentType)
                .SetValue(assignMethodAttributeInfo.AttributeField, y => y.NewObj(delegateCtor, method.ThisOrNull(), method)));

    private static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos,
                Action<Method, Method, AssignMethodAttributeInfo> @delegate)
    {
        if (@delegate == null)
            throw new ArgumentNullException(nameof(@delegate));

        foreach (var item in assignMethodAttributeInfos)
        {
            var method = item.TargetMethod?.Import();

            if (method == null)
            {
                builder.Log(item.ThrowError ? LogTypes.Error : LogTypes.Info,
                    item.AttributedMethod,
                    $"Unable to find matching method for the interceptor '{item.AttributeField.OriginType.Fullname}'. The interceptor requires a method with the name '{item.TargetMethodName}' and a return type of '{item.TargetMethodReturnType}'.");
                continue;
            }

            var delegateCtor = item.AttributeField.FieldType.IsGenericInstance ?
                item.AttributeField.FieldType
                    .GetMethod(".ctor", true, new Type[] { typeof(object), typeof(IntPtr) })
                    .MakeGeneric(item.AttributeField.FieldType.GenericArguments().ToArray())
                    .Import() :
                item.AttributeField.FieldType
                    .GetMethod(".ctor", true, new Type[] { typeof(object), typeof(IntPtr) })
                    .Import();

            builder.Log(LogTypes.Info, $"- Implementing AssignMethodAttribute for '{method.Name}'.");
            @delegate(delegateCtor, method, item);
        }
    }
}