using System;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    public class PageModel
    {
        public Int32? PageId { get; set; }
        public Int32? CategoryId { get; set; }
        public Int32? OrderId { get; set; }
        public Int32? PageTemplateId { get; set; }

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