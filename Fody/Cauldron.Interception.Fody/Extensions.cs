using Cauldron.Interception.Cecilator;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cauldron.Interception.Fody
{
    public static class Extensions
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

        public static string GetMd5Hash(this byte[] target)
        {
            using (var md5 = new MD5CryptoServiceProvider())
                return BitConverter.ToString(md5.ComputeHash(target)).Replace("-", "");
        }

        public static Modifiers GetPrivate(this Modifiers value) => value.HasFlag(Modifiers.Static) ? Modifiers.PrivateStatic : Modifiers.Private;

        public static TSource MaxVersion<TSource>(this IEnumerable<TSource> source, Func<TSource, Version> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (selector == null)
                throw new ArgumentNullException("selector");

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");

                var max = sourceIterator.Current;
                var maxKey = selector(max);

                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);

                    if (candidateProjected > maxKey)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }

                return max;
            }
        }

        public static string UpperCaseFirstLetter(this string target)
        {
            if (string.IsNullOrEmpty(target))
                return target;

            if (target.Length > 1)
                return char.ToUpper(target[0]) + target.Substring(1, target.Length - 1);

            return target;
        }

        internal static IEnumerable<JContainer> GetChildren(this JContainer container)
        {
            foreach (var item in container.Children())
            {
                yield return item as JContainer;
            }
        }

        internal static IEnumerable<JContainer> GetChildren<T>(this JContainer container, Func<T, bool> predicate) where T : JContainer
        {
            foreach (var item in container.Children())
            {
                var data = item as T;
                if (data == null)
                    continue;

                if (predicate(data))
                    yield return item as JContainer;
            }
        }
    }
}