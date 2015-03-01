using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Humanizer;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class StringExtensions
    {
        public static string In(this string a, string b)
        {
            return string.Format("{0} in {1}", a, b);
        }

        public static string In(this string a, string b, bool allowHyphens)
        {
            return (a.In(b)).Clean(allowHyphens);
        }

        public static string Between(this string text, string a, string b)
        {
            var position = text.IndexOf(a, StringComparison.Ordinal) + a.Length;
            var cutFromRight = text.Right(text.Length + a.Length - position);
            var indexOf = cutFromRight.IndexOf(b, StringComparison.Ordinal);
            var left = cutFromRight.Left(indexOf);
            var between = left.Substring(a.Length, left.Length - a.Length);

            return between;
        }

        public static string Left(this string input, int indexOf)
        {
            var left = input.Substring(0, indexOf);

            return left;
        }

        public static string Right(this string input, int length)
        {
            var right = length >= input.Length ? input : input.Substring(input.Length - length);

            return right;
        }

        public static String DoubleApostrophes(this String value)
        {
            var isNullOrEmpty = value.Replace("'", "''");

            return isNullOrEmpty;
        }

        public static Boolean IsNotNullOrEmpty(this String value)
        {
            var isNullOrEmpty = !value.IsNullOrEmpty();

            return isNullOrEmpty;
        }

        public static Boolean IsNullOrEmpty(this String value)
        {
            var isNullOrEmpty = String.IsNullOrEmpty(value);

            return isNullOrEmpty;
        }

        public static List<string> SplitOnComma(this string commaSeparatedString)
        {
            var result = commaSeparatedString.Split(',').ToList();

            return result;
        }

        public static String RemoveSpecialCharacters(this String input, bool exceptHyphen)
        {
            var pattern = exceptHyphen ? "[^a-zA-Z0-9 -]" : "[^0-9a-zA-Z]+";

            var result = Regex.Replace(input, pattern, String.Empty);

            return result;
        }

        public static string After(this string input, string find)
        {
            if (String.IsNullOrEmpty(find)) return input;
            var idx = input.IndexOf(find, StringComparison.Ordinal);
            return (idx < 0 ? String.Empty : input.Substring(idx + find.Length));
        }

        public static string Before(this string input, string find)
        {
            if (String.IsNullOrEmpty(find)) return input;
            var idx = input.IndexOf(find, StringComparison.Ordinal);
            return (idx < 0 ? String.Empty : input.Substring(0, idx));
        }

        public static string AfterLast(this string input, string find)
        {
            if (String.IsNullOrEmpty(find)) return input;
            var idx = input.LastIndexOf(find, StringComparison.Ordinal);
            return (idx < 0 ? String.Empty : input.Substring(idx + find.Length));
        }

        public static string BeforeLast(this string input, string find)
        {
            if (String.IsNullOrEmpty(find)) return input;
            var idx = input.LastIndexOf(find, StringComparison.Ordinal);
            return (idx < 0 ? String.Empty : input.Substring(0, idx));
        }

        public static List<string> SplitOn(this string concatenated, char splitOn)
        {
            var output = concatenated.Split(splitOn).ToList();

            return output;
        }

        public static List<string> SplitOnSlash(this string slashSeparated)
        {
            var output = slashSeparated.SplitOn('\\');
            return output;
        }

        public static string FirstCharToUpper(this string input)
        {
            if (String.IsNullOrEmpty(input))
                return input;

            return String.Format("{0}{1}",
                input.First().ToString(CultureInfo.CurrentCulture).ToUpper(),
                String.Join(String.Empty, input.Skip(1))
                );
        }

        public static string CamelCaseToWords(this string input)
        {
            return Regex.Replace(input.FirstCharToUpper(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public static string Clean(this string input)
        {
            return input.Clean(true);
        }

        public static string Clean(this string input, bool allowHyphen)
        {
            var result = input.IsNullOrEmpty()
                ? input
                : input.ToLower().Underscore().Hyphenate().RemoveSpecialCharacters(allowHyphen);

            return result;
        }

        public static string SeoMetaDescriptionTruncate(this string text)
        {
            const int max = 156;

            var result = string.Empty;

            try
            {
                result = text.Wrap(max).First();
            }
            catch (Exception ex)
            {
                var m = ex.InnerException;
            }

            return result;
        }

        public static String[] Wrap(this string text, int max)
        {
            var charCount = 0;
            
            var lines = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return lines.GroupBy(w => (charCount += (((charCount % max) + w.Length + 1 >= max)
                ? max - (charCount % max)
                : 0) + w.Length + 1) / max)
                .Select(g => string.Join(" ", g.ToArray()))
                .ToArray();
        }

        public static string RemoveSpaces(this string input)
        {
            return input.Replace(" ", String.Empty);
        }

        public static string ReplaceHyphens(this string input)
        {
            return input.Replace("-", "_");
        }

        public static bool IsNullOrWhiteSpace(this string input)
        {
            return String.IsNullOrWhiteSpace(input);
        }
    }
}