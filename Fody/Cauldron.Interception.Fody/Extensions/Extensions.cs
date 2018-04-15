using System;
using System.Collections.Generic;

namespace Cauldron.Interception.Fody.Extensions
{
    internal static class Extensions
    {
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
    }
}