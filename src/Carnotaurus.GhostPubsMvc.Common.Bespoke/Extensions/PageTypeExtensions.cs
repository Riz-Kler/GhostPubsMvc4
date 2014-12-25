using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carnotaurus.GhostPubsMvc.Common.Bespoke.Extensions
{
    public static class PageTypeExtensions
    {
        public static bool IsGeo(this PageTypeEnum pageTypeEnum)
        {
            var isGeo = pageTypeEnum == PageTypeEnum.Country ||
                        pageTypeEnum == PageTypeEnum.Region ||
                        pageTypeEnum == PageTypeEnum.Authority ||
                        pageTypeEnum == PageTypeEnum.Locality ||
                        pageTypeEnum == PageTypeEnum.Pub;

            return isGeo;
        }

    }
}
