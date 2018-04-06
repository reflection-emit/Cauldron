using Cauldron.Interception.Cecilator;
using System.Linq;

namespace Cauldron.Interception.Fody.Extensions
{
    internal static class BuilderExtensions
    {
        public static string EnclosedIn(this string target, string start = "<", string end = ">")
        {
            if (string.IsNullOrEmpty(target))
                return target;

            int startPos = target.IndexOf(start) + start.Length;

            if (startPos < 0)
                return target;

            int endPos = target.IndexOf(end, startPos);

            if (endPos <= startPos)
                endPos = target.Length;

            return target.Substring(startPos, endPos - startPos);
        }

        public static Modifiers GetPrivate(this Modifiers value) => value.HasFlag(Modifiers.Static) ? Modifiers.PrivateStatic : Modifiers.Private;

        public static string UpperCaseFirstLetter(this string target)
        {
            if (string.IsNullOrEmpty(target))
                return target;

            if (target.Length > 1)
                return char.ToUpper(target[0]) + target.Substring(1, target.Length - 1);

            return target;
        }
    }
}