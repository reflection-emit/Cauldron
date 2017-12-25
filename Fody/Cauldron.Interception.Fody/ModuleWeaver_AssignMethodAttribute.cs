using Cauldron.Interception.Cecilator;
using System;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        public static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos, Field interceptorInstance, ICode coder, bool isCtor) =>
            ImplementAssignMethodAttribute(builder, assignMethodAttributeInfos,
                x => coder.Assign(interceptorInstance, x.assignMethodAttributeInfo.AttributeField).NewObj(x.delegateCtor, x.method), isCtor);

        public static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos, LocalVariable interceptorInstance, ICode coder, bool isCtor) =>
            ImplementAssignMethodAttribute(builder, assignMethodAttributeInfos,
                x => coder.Assign(interceptorInstance, x.assignMethodAttributeInfo.AttributeField).NewObj(x.delegateCtor, x.method), isCtor);

        private static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos,
                    Action<(Method delegateCtor, Method method, AssignMethodAttributeInfo assignMethodAttributeInfo)> @delegate, bool isCtor)
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate));

            foreach (var item in assignMethodAttributeInfos)
            {
                if (item.IsCtor && !isCtor)
                    continue;

                var method = item.TargetMethod;

                if (method == null)
                {
                    builder.Log(item.ThrowError ? LogTypes.Error : LogTypes.Info,
                        item.AttributedMethod,
                        $"Unable to find matching method for the interceptor '{item.AttributeField.OriginType.Fullname}'. The interceptor requires a method with the name '{item.TargetMethodName}' and a return type of '{item.TargetMethodReturnType}'.");
                    continue;
                }

                var delegateCtor = item.AttributeField.FieldType.IsGenericInstance ?
                    builder.Import(item.AttributeField.FieldType.GetMethod(".ctor", true, new Type[] { typeof(object), typeof(IntPtr) }).MakeGeneric(item.AttributeField.FieldType.GenericArguments().ToArray())) :
                    builder.Import(item.AttributeField.FieldType.GetMethod(".ctor", true, new Type[] { typeof(object), typeof(IntPtr) }));

                builder.Log(LogTypes.Info, $"- Implementing AssignMethodAttribute for '{method.Name}'.");
                @delegate((delegateCtor, method, item));
            }
        }
    }
}