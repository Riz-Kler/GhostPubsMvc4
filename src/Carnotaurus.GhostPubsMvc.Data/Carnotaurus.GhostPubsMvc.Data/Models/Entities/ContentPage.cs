using System;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class ContentPage : IEntity
    {
        public int? CategoryId { get; set; }
        public int? OrderId { get; set; }
        public int? PageTemplateId { get; set; }
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
        public int Id { get; set; }
    }
}