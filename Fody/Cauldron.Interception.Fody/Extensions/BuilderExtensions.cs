using Cauldron.Interception.Cecilator;
using System.Linq;

namespace Cauldron.Interception.Fody.Extensions
{
    internal static class BuilderExtensions
    {
        public static void AddThisReferenceToAsyncMethod<T>(this MethodBuilderInfo<T> method) where T : IMethodBuilderInfoItem
        {
            var attributedMethod = method.Key.Method;
            var targetedMethod = method.Key.AsyncMethod ?? method.Key.Method;

            if (method.Key.AsyncMethod != null && !targetedMethod.DeclaringType.Fields.Any(x => x.Name == "<>4__this"))
            {
                var thisField = targetedMethod.DeclaringType.CreateField(Modifiers.Public, attributedMethod.DeclaringType, "<>4__this");
                var position = attributedMethod.AsyncMethodHelper.GetAsyncTaskMethodBuilderInitialization();

                if (position == null)
                    attributedMethod.NewCode().LoadVariable(0).Assign(thisField).Set(Crumb.This).Insert(InsertionPosition.Beginning);
                else
                    attributedMethod.NewCode().LoadVariable(0).Assign(thisField).Set(Crumb.This).Insert(InsertionAction.After, position);
            }
        }

        public static object GetAsyncMethodTypeInstace<T>(this MethodBuilderInfo<T> method) where T : IMethodBuilderInfoItem
        {
            var targetedMethod = method.Key.AsyncMethod ?? method.Key.Method;
            return method.Key.AsyncMethod == null ? (object)Crumb.This : targetedMethod.DeclaringType.Fields.FirstOrDefault(x => x.Name == "<>4__this");
        }

        public static Modifiers GetPrivate(this Modifiers value) => value.HasFlag(Modifiers.Static) ? Modifiers.PrivateStatic : Modifiers.Private;
    }
}