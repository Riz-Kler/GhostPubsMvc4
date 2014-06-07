using System;
using System.Collections.Generic;
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
        public String JumboTitle { get; set; }

        public String Action { get; set; }
    }


    public class PageModel
    {
        public Int32? PageID { get; set; }


        public Int32? CategoryID { get; set; }
        public Int32? OrderID { get; set; }
        public Int32? PageTemplateID { get; set; }

        public String HtmlText { get; set; }

        public String Title { get; set; }


        public String Description { get; set; }

        public String Link { get; set; }

        public String Tag { get; set; }

        public String Lead { get; set; }

        public String WellHeader { get; set; }

        public String Name { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public DateTime? Deleted { get; set; }
    }
}