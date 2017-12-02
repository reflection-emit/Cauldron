using Cauldron.Interception.Cecilator;
using System.Linq;

namespace Cauldron.Interception.Fody.Extensions
{
    internal static class BuilderExtensions
    {
        public static void AddThisReferenceToAsyncMethod(this MethodBuilderInfo method)
        {
            var attributedMethod = method.Key.Method;
            var targetedMethod = method.Key.AsyncMethod == null ? method.Key.Method : method.Key.AsyncMethod;

            if (method.Key.AsyncMethod != null && !targetedMethod.DeclaringType.Fields.Any(x => x.Name == "<>4__this"))
            {
                var thisField = targetedMethod.DeclaringType.CreateField(Modifiers.Public, attributedMethod.DeclaringType, "<>4__this");
                var position = attributedMethod.AsyncMethodHelper.GetAsyncTaskMethodBuilderInitialization();

                if (position == null)
                    attributedMethod.NewCode().LoadVariable(0).Assign(thisField).Set(attributedMethod.NewCode().This).Insert(InsertionPosition.Beginning);
                else
                    attributedMethod.NewCode().LoadVariable(0).Assign(thisField).Set(attributedMethod.NewCode().This).Insert(InsertionAction.After, position);
            }
        }

        public static object GetAsyncMethodTypeInstace(this MethodBuilderInfo method)
        {
            var targetedMethod = method.Key.AsyncMethod == null ? method.Key.Method : method.Key.AsyncMethod;
            return method.Key.AsyncMethod == null ? (object)targetedMethod.NewCode().This : targetedMethod.DeclaringType.Fields.FirstOrDefault(x => x.Name == "<>4__this");
        }

        public static Modifiers GetPrivate(this Modifiers value) => value.HasFlag(Modifiers.Static) ? Modifiers.PrivateStatic : Modifiers.Private;
    }
}