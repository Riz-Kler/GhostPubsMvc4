using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class OrgOutputModel : IOrgOutputModel
    {
        public OrgOutputModel()
        {
            this.Links = new List<LinkModel>();
            this.Tags = new List<String>();
        }

        public String Description { get; set; }

        public String Link { get; set; }

        public String Tag { get; set; }

        public List<LinkModel> Links { get; set; }

        public String Unc { get; set; }

        public KeyValuePair<String, String> Parent { get; set; }

        public List<String> Tags { get; set; }
        public Int32? Total { get; set; }

        public String JumboTitle { get; set; }

        public String Action { get; set; }

        public String Priority { get; set; }

    }
}