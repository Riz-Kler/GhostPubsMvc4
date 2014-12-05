using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class EnumExtensions
    {
        public static T ParseEnum<T>(this String value) where T : struct
        {
            return (T) Enum.Parse(typeof (T), value, true);
        }

        public static List<SelectListItem> GetEnumItems<T>(this T selected) where T : struct, IConvertible
        {
            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var values = Enum.GetValues(typeof (T))
                .Cast<T>();

            var items =
                values.Select(action => new SelectListItem
                {
                    Text = action.ToString(CultureInfo.InvariantCulture),
                    Value = (Convert.ToInt32(action)).ToString(CultureInfo.InvariantCulture)
                })
                    .ToList();

            var result = Convert.ToInt32(selected);

            if (result == 0) return items;

            var item = items.FirstOrDefault(x => x.Value.ToInt32() == result);

            if (item != null)
            {
                item.Selected = true;
            }

            return items;
        }
    }
}