using Cauldron.Interception.Cecilator;
using System;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        public static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos, Field interceptorInstance, ICode coder) =>
            ImplementAssignMethodAttribute(builder, assignMethodAttributeInfos,
                x => coder.Assign(interceptorInstance, x.assignMethodAttributeInfo.AttributeField).NewObj(x.delegateObjectCtor, x.method),
                x => coder.Assign(interceptorInstance, x.assignMethodAttributeInfo.AttributeField).NewObj(x.delegateObjectCtor, x.method));

        public static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos, LocalVariable interceptorInstance, ICode coder) =>
            ImplementAssignMethodAttribute(builder, assignMethodAttributeInfos,
                x => coder.Assign(interceptorInstance, x.assignMethodAttributeInfo.AttributeField).NewObj(x.delegateObjectCtor, x.method),
                x => coder.Assign(interceptorInstance, x.assignMethodAttributeInfo.AttributeField).NewObj(x.delegateObjectCtor, x.method));

        private static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos,
                    Action<(Method delegateObjectCtor, Method method, AssignMethodAttributeInfo assignMethodAttributeInfo)> action,
                    Action<(Method delegateObjectCtor, Method method, AssignMethodAttributeInfo assignMethodAttributeInfo)> func)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var actionObjectCtor = builder.Import(typeof(Action).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
            var funcObjectCtor = builder.Import(typeof(Func<>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));

            foreach (var item in assignMethodAttributeInfos)
            {
                var method = item.TargetMethod;

                if (method == null)
                {
                    builder.Log(LogTypes.Warning, $"Unable to find matching method for the interceptor '{item.AttributeField.OriginType.Fullname}'. The interceptor requires a method with the name '{item.TargetMethodName}' and a return type of '{item.TargetMethodReturnType}'.");
                    continue;
                }

                builder.Log(LogTypes.Info, $"- Implementing AssignMethodAttribute for '{method.Name}'.");

                if (item.TargetMethodIsVoid)
                    action((actionObjectCtor, method, item));
                else
                    func((funcObjectCtor.MakeGeneric(item.TargetMethodReturnType), method, item));
            }
        }
    }
}