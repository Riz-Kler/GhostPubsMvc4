using System;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class GuidExtensions
    {
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
            return (!value.HasValue) || (value.Value.IsEmpty());
        }

        public static String RemoveSpecialCharacters(this Guid input)
        {
            return input.ToString().ToUpper().RemoveSpecialCharacters(false);
        }
    }
}