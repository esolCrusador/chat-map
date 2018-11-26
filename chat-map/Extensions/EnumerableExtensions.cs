using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatMap.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TResult> FullOuterGroupJoin<TLeft, TRight, TKey, TResult>(
            IEnumerable<TLeft> leftEnumerable,
            IEnumerable<TRight> rightEnumerable,
            Func<TLeft, TKey> leftKeySelector,
            Func<TRight, TKey> rightKeySelector,
            Func<IEnumerable<TLeft>, IEnumerable<TRight>, TResult> resultSelector,
            EqualityComparer<TKey> comparer = null)
        {
            if (comparer == null)
                comparer = EqualityComparer<TKey>.Default;

            var leftLookup = leftEnumerable.ToLookup(leftKeySelector, comparer);
            var rightLookup = rightEnumerable.ToLookup(rightKeySelector, comparer);

            foreach (var left in leftLookup)
            {
                yield return resultSelector(left, rightLookup[left.Key]);
            }

            foreach (var right in rightLookup.Where(right => !leftLookup.Contains(right.Key)))
            {
                yield return resultSelector(Enumerable.Empty<TLeft>(), right);
            }
        }

        public static JoinBuilder<TLeft, TRight> JoinTo<TLeft, TRight>(this IEnumerable<TLeft> source, IEnumerable<TRight> right)
        {
            return new JoinBuilder<TLeft, TRight>(source, right);
        }

        public static bool TryFirst<TElement>(this IEnumerable<TElement> source, out TElement element)
        {
            using (IEnumerator<TElement> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    element = enumerator.Current;
                    return true;
                }
                else
                {
                    element = default(TElement);
                    return false;
                }
            }
        }
    }
}
