using System;
 
namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class ContentPage
    {
        public int PageID { get; set; }
        public int? CategoryID { get; set; }
        public int? OrderID { get; set; }
        public int? PageTemplateID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Lead { get; set; }
        public string WellHeader { get; set; }
        public string Description { get; set; }
        public string HtmlText { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual Category Category { get; set; }
    }
}