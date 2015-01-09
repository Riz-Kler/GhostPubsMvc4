using System;
using System.Collections.Generic;
using System.Linq;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class EnumerableCollectionExtensions
    {
        public static int? FindNext(this IEnumerable<int> enumerable, int toFind)
        {
            var ints = enumerable.ToList();

            if (ints.Count() <= 1) return null;

            var findIndex = ints.FindIndex(i => i == toFind);

            var nextIndex = findIndex + 1;

            var maxIndex = ints.Count;

            if (nextIndex >= maxIndex)
            {
                nextIndex = 0;
            }

            int? nextId = ints[nextIndex];

            return nextId;
        }

        public static string ExtractFilename(this IEnumerable<string> enumerable)
        {
            const string pattern = "{0}\\{1}";

            var result = enumerable.Extract(pattern);

            return result;
        }

        public static string Extract(this IEnumerable<string> enumerable, string pattern)
        {
            var result = enumerable.Aggregate(String.Empty, (current, item) =>
                String.Format(pattern, current, item));

            return result;
        }

        public static List<string> ReverseItems(this IEnumerable<string> enumerable)
        {
            var result = new List<string>(from c in enumerable.Select((value, index) => new { value, index })
                                          orderby c.index descending
                                          select c.value);

            return result;
        }

        public static string Dash(this IEnumerable<string> enumerable)
        {
            var result = JoinWithSpace(enumerable.ToArray()).Clean(true);

            return result;
        }

        public static string JoinWithSpace(this IEnumerable<string> enumerable)
        {
            var result = Join(enumerable.ToArray(), " ");

            return result;
        }

        public static string JoinWithBackslash(this IEnumerable<string> enumerable)
        {
            var result = Join(enumerable.ToArray(), @"\");

            return result;
        }

        public static string Join(this IEnumerable<string> enumerable, String delimiter)
        {
            return String.Join(delimiter, enumerable);
        }

        public static string JoinWithComma(this IEnumerable<string> enumerable)
        {
            return enumerable.Join(", ");
        }

        public static string JoinWithCommaReserve(this IEnumerable<string> enumerable)
        {
            enumerable = enumerable.Reverse();

            return enumerable.Join(", ");
        }

        public static string OxfordAnd(this IEnumerable<string> enumerable)
        {
            var list = enumerable.ToList();

            string output;

            switch (list.Count)
            {
                case 1:
                    output = list.First();
                    break;
                case 2:
                    output = JoinWithSpace(list);
                    break;
                default:
                {
                    var delimited = JoinWithComma(list.Take(list.Count - 1));

                    output = String.Format("{0}, and {1}",
                        delimited,
                        list.LastOrDefault()
                        );
                }
                    break;
            }

            return output;
        }

        #region RankBy

        public static IEnumerable<TResult> RankBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, int, TResult> resultSelector)
        {
            return source.RankBy(keySelector, null, false, resultSelector);
        }

        public static IEnumerable<TResult> RankBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            Func<TSource, int, TResult> resultSelector)
        {
            return source.RankBy(keySelector, comparer, false, resultSelector);
        }

        public static IEnumerable<TResult> RankByDescending<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            Func<TSource, int, TResult> resultSelector)
        {
            return source.RankBy(keySelector, comparer, true, resultSelector);
        }

        public static IEnumerable<TResult> RankByDescending<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, int, TResult> resultSelector)
        {
            return source.RankBy(keySelector, null, true, resultSelector);
        }

        private static IEnumerable<TResult> RankBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            bool descending,
            Func<TSource, int, TResult> resultSelector)
        {
            comparer = comparer ?? Comparer<TKey>.Default;

            var grouped = source.GroupBy(keySelector);
            var ordered =
                @descending
                    ? grouped.OrderByDescending(g => g.Key, comparer)
                    : grouped.OrderBy(g => g.Key, comparer);

            var totalRank = 1;
            foreach (var group in ordered)
            {
                var rank = totalRank;
                foreach (var item in @group)
                {
                    yield return resultSelector(item, rank);
                    totalRank++;
                }
            }
        }

        #endregion

        #region DenseRankBy

        public static IEnumerable<TResult> DenseRankBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, int, TResult> resultSelector)
        {
            return source.DenseRankBy(keySelector, null, false, resultSelector);
        }

        public static IEnumerable<TResult> DenseRankBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            Func<TSource, int, TResult> resultSelector)
        {
            return source.DenseRankBy(keySelector, comparer, false, resultSelector);
        }

        public static IEnumerable<TResult> DenseRankByDescending<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            Func<TSource, int, TResult> resultSelector)
        {
            return source.DenseRankBy(keySelector, comparer, true, resultSelector);
        }

        public static IEnumerable<TResult> DenseRankByDescending<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, int, TResult> resultSelector)
        {
            return source.DenseRankBy(keySelector, null, true, resultSelector);
        }

        private static IEnumerable<TResult> DenseRankBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            bool descending,
            Func<TSource, int, TResult> resultSelector)
        {
            comparer = comparer ?? Comparer<TKey>.Default;

            var grouped = source.GroupBy(keySelector);
            var ordered =
                @descending
                    ? grouped.OrderByDescending(g => g.Key, comparer)
                    : grouped.OrderBy(g => g.Key, comparer);

            var rank = 1;
            foreach (var group in ordered)
            {
                foreach (var item in @group)
                {
                    yield return resultSelector(item, rank);
                }
                rank++;
            }
        }

        #endregion
    }
}