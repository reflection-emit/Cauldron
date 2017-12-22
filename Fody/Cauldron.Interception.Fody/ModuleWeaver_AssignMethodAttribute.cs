using Cauldron.Interception.Cecilator;
using System;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        public static void ImplementAssignMethodAttribute(Builder builder, AssignMethodAttributeInfo[] assignMethodAttributeInfos, ICode coder)
        {
            var actionObjectCtor = builder.Import(typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));

            foreach (var item in assignMethodAttributeInfos)
            {
                var method = item.GetToBeAssignedMethod();

                if (method == null)
                {
                    builder.Log(LogTypes.Warning, $"Unable to find matching method for the interceptor '{item.AttributeField.DeclaringType.Fullname}'. The interceptor requires a method with the name '{item.TargetMethodName}' and a return type of '{item.TargetMethodReturnType}'.");
                    continue;
                }

                if (item.TargetMethodIsVoid)
                    coder.Assign(item.AttributeField).NewObj(actionObjectCtor, method);
            }
        }
    }
}