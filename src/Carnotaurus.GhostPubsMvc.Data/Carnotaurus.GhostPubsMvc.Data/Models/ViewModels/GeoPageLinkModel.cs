using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Extensions;
using Carnotaurus.GhostPubsMvc.Common.Extensions;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    [Serializable]
    public class GeoPageLinkModel : IPageLinkModel
    { 
        public String Url
        {
            get
            {
                if (Filename.IsNullOrEmpty()) return String.Empty;

                var pattern =  "http://www.ghostpubs.com/haunted-pubs/{0}.html";

                var url = String.Format(pattern, Filename.Replace("\\", "/"));

                return url.Dashify().ToLower();
            }
        }

        public String Filename { get; set; }

        public String Title { get; set; }

        public String Text { get; set; }

        public Int32 Id { get; set; }

        public List<IPageLinkModel> Links { get; set; }
    }
}