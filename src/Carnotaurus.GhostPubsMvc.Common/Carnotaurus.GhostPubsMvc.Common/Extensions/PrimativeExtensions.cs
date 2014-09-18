using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class PrimativeExtensions
    {
        public static string After(this string s, string searchString)
        {
            if (String.IsNullOrEmpty(searchString)) return s;
            var idx = s.IndexOf(searchString, StringComparison.Ordinal);
            return (idx < 0 ? "" : s.Substring(idx + searchString.Length));
        }

        public static string Before(this string s, string searchString)
        {
            if (String.IsNullOrEmpty(searchString)) return s;
            var idx = s.IndexOf(searchString, StringComparison.Ordinal);
            return (idx < 0 ? "" : s.Substring(0, idx));
        }

        public static string AfterLast(this string s, string searchString)
        {
            if (String.IsNullOrEmpty(searchString)) return s;
            var idx = s.LastIndexOf(searchString, StringComparison.Ordinal);
            return (idx < 0 ? "" : s.Substring(idx + searchString.Length));
        }

        public static string BeforeLast(this string s, string searchString)
        {
            if (String.IsNullOrEmpty(searchString)) return s;
            var idx = s.LastIndexOf(searchString, StringComparison.Ordinal);
            return (idx < 0 ? "" : s.Substring(0, idx));
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

        public static Boolean IsNotEmpty(this Guid value)
        {
            return !value.IsEmpty();
        }

        public static Boolean IsEmpty(this Guid value)
        {
            return value == Guid.Empty;
        }

        public static Boolean IsNullOrEmpty(this Guid? value)
        {
            return !value.HasValue || value.Value.IsEmpty();
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
            var c = ',';

            var output = commaSeparatedString.SplitOn(c);

            return output;
        }


        public static List<string> SplitOn(this string commaSeparatedString, char splitOn)
        {
            var output = commaSeparatedString.Split(splitOn).ToList();

            return output;
        }

        public static List<string> SplitOnSlash(this string commaSeparatedString)
        {
            var output = commaSeparatedString.SplitOn('\\');
            return output;
        }

        public static string FirstCharToUpper(this string input)
        {
            if (String.IsNullOrEmpty(input))
                return input;

            return input.First().ToString(CultureInfo.CurrentCulture).ToUpper()
                   + String.Join(String.Empty, input.Skip(1));
        }

        public static string CamelCaseToWords(this string input)
        {
            return Regex.Replace(input.FirstCharToUpper(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }
    }
}