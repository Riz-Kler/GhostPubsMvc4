using System;

namespace Carnotaurus.GhostPubsMvc.Web
{
    public partial class Org
    {
        public string PostcodePrimaryPart
        {
            get
            {
                var prepare = Postcode.Trim();

                var output = String.Empty;

                if (prepare.Length > 2)
                {
                    output = prepare
                        .Substring(0, prepare.Length - 3)
                        .Trim();
                }

                return output;
            }
        }
    }
}