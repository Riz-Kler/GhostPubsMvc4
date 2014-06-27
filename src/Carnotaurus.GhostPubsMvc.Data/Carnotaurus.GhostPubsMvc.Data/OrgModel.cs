using System;
using System.Collections.Generic;

namespace Carnotaurus.GhostPubsMvc.Web.Models
{
    public class OrgModel : IOrgModel
    {
        public OrgModel()
        {
            this.Links = new List<Link>();
            this.Tags = new List<String>();
        }

        public String Description { get; set; }

        public String Link { get; set; }

        public String Tag { get; set; }

        public List<Link> Links { get; set; }

        public String Unc { get; set; }

        public KeyValuePair<String, String> Parent { get; set; }

        public List<String> Tags { get; set; }
        public Int32? Total { get; set; }

        public String JumboTitle { get; set; }

        public String Action { get; set; }
    }
}