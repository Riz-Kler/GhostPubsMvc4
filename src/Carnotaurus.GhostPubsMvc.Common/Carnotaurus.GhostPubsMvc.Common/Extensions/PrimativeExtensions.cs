using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Humanizer;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class PrimativeExtensions
    {
        public static string Between(this string text, string a, string b)
        {
            var position = text.IndexOf(a) + a.Length;
            var cutFromRight = text.Right(text.Length + a.Length - position);
            var indexOf = cutFromRight.IndexOf(b);
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

        public static Guid? ToNullableGuid(this String value)
        {
            Guid result;

            if (Guid.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static Int32? ToNullableInt32(this String value)
        {
            int result;

            if (Int32.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static Int32 ToInt32(this String value)
        {
            var result = value.ToNullableInt32().ToInt32();

            return result;
        }

        public static Decimal ToDecimal(this String value)
        {
            return value.ToNullableDecimal() ?? 0;
        }

        public static Decimal? ToNullableDecimal(this String value)
        {
            Decimal result;

            if (Decimal.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static Int32 ToInt32(this Int32? nullable)
        {
            var result = 0;

            if (nullable.HasValue)
            {
                result = nullable.Value;
            }

            return result;
        }

        public static Boolean IsAboveZero(this Int32 value)
        {
            var result = value > 0;

            return result;
        }

        public static List<string> SplitOnComma(this string commaSeparatedString)
        {
            var result = commaSeparatedString.Split(',').ToList();

            return result;
        }

        public static String RemoveSpecialCharacters(this String input)
        {
            var result = Regex.Replace(input, "[^0-9a-zA-Z]+", String.Empty);

            return result;
        }

        public static string After(this string input, string find)
        {
            if (String.IsNullOrEmpty(find)) return input;
            var idx = input.IndexOf(find, StringComparison.Ordinal);
            return (idx < 0 ? string.Empty : input.Substring(idx + find.Length));
        }

        public static string Before(this string input, string find)
        {
            if (String.IsNullOrEmpty(find)) return input;
            var idx = input.IndexOf(find, StringComparison.Ordinal);
            return (idx < 0 ? string.Empty : input.Substring(0, idx));
        }

        public static string AfterLast(this string input, string find)
        {
            if (String.IsNullOrEmpty(find)) return input;
            var idx = input.LastIndexOf(find, StringComparison.Ordinal);
            return (idx < 0 ? string.Empty : input.Substring(idx + find.Length));
        }

        public static string BeforeLast(this string input, string find)
        {
            if (String.IsNullOrEmpty(find)) return input;
            var idx = input.LastIndexOf(find, StringComparison.Ordinal);
            return (idx < 0 ? string.Empty : input.Substring(0, idx));
        }

        public static double? ToNullableDouble(this String input)
        {
            double result;

            if (double.TryParse(input, out result))
            {
                return result;
            }

            return null;
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

            return string.Format("{0}{1}",
                input.First().ToString(CultureInfo.CurrentCulture).ToUpper(),
                String.Join(String.Empty, input.Skip(1))
                );
        }

        public static string CamelCaseToWords(this string input)
        {
            return Regex.Replace(input.FirstCharToUpper(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public static string RedirectionalFormat(this string input)
        {
            var replace = input.ToLower().SeoFormat().ReplaceHyphens();

            return replace;
        }

        public static string SeoFormat(this string input)
        {
            return input.Underscore().Hyphenate();
        }

        public static string RemoveSpaces(this string input)
        {
            return input.Replace(" ", "");
        }

        public static string ReplaceHyphens(this string input)
        {
            return input.Replace("-", "_");
        }
    }
}