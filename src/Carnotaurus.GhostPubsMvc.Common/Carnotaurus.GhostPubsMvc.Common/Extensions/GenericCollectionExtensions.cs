using System;
using System.Collections.Generic;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class GenericCollectionExtensions
    {
        public static void AddIf<T>(this List<T> items, T toAdd)
        {
            if (items == null) throw new ArgumentNullException("items");

            if (!items.Contains(toAdd))
            {
                items.Add(toAdd);
            }
        }
    }
}