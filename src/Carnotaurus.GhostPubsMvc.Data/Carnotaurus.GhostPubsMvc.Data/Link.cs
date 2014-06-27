using System;
using Carnotaurus.GhostPubsMvc.Web.Extensions;

namespace Carnotaurus.GhostPubsMvc.Web.Models
{
    public class Link
    {
        public String Url
        {
            get
            {
                if (!Text.IsNullOrEmpty())
                {
                    return Text.Replace(" ", "_");
                }
                return String.Empty;
            }
        }

        public String Unc { get; set; }

        public String Title { get; set; }

        public String Text { get; set; }

        public Int32 Id { get; set; }
    }
}