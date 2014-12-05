using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static string ExtractUnc(this IEnumerable<String> list)
        {
            const string pattern = "{0}\\{1}";

            var result = list.Extract(pattern);

            return result;
        }

        public static string Extract(this IEnumerable<String> list, string pattern)
        {
            var result = list.Aggregate(string.Empty, (current, item) =>
                string.Format(pattern, current, item));

            return result;
        }

        public static List<String> ReverseItems(this IEnumerable<String> list)
        {
            var reversed = new List<string>(from c in list.Select((value, index) => new {value, index})
                orderby c.index descending
                select c.value);

            return reversed;
        }

        public static string ToCommaSeparatedString(this IEnumerable<string> inputList)
        {
            var output = String.Join(",", inputList.ToArray());
            return output;
        }

        public static string ToBackslashSeparatedString(this IEnumerable<string> inputList)
        {
            var output = String.Join(@"\", inputList.ToArray());
            return output;
        }

        public static Int32 ToInt32(this NameValueCollection collection, String key)
        {
            var result = collection[key].ToInt32();

            return result;
        }

        public static IDictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k]);
            }

            return dict;
        }

        public static string Join(this IEnumerable<string> collection, String delimiter)
        {
            return String.Join(delimiter, collection);
        }

        public static string JoinWithComma(this IEnumerable<string> collection)
        {
            return collection.Join(", ");
        }

        public static string JoinWithCommaReserve(this IEnumerable<string> collection)
        {
            collection = collection.Reverse();

            return collection.Join(", ");
        }

        public static string OxbridgeAnd(this IEnumerable<string> collection)
        {
            var output = String.Empty;

            var list = collection.ToList();

            if (list.Count > 1)
            {
                var delimited = String.Join(", ", list.Take(list.Count - 1));

                output = String.Concat(delimited, ", and ", list.LastOrDefault());
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
                descending
                    ? grouped.OrderByDescending(g => g.Key, comparer)
                    : grouped.OrderBy(g => g.Key, comparer);

            var totalRank = 1;
            foreach (var group in ordered)
            {
                var rank = totalRank;
                foreach (var item in group)
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
                descending
                    ? grouped.OrderByDescending(g => g.Key, comparer)
                    : grouped.OrderBy(g => g.Key, comparer);

            var rank = 1;
            foreach (var group in ordered)
            {
                foreach (var item in group)
                {
                    yield return resultSelector(item, rank);
                }
                rank++;
            }
        }

        #endregion
    }
}