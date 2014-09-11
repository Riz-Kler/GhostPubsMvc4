using System;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class BookItem : IEntity
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public int? BookId { get; set; }
        public int? OrgId { get; set; }
        public string County { get; set; }
        public string Town { get; set; }
        public string AlternativeTown { get; set; }
        public string TradingName { get; set; }
        public string Text { get; set; }
        public string Postcode { get; set; }
        public virtual Book Book { get; set; }
        public virtual Org Org { get; set; }
        public Boolean? IsUsed { get; set; }
        public int Id { get; set; }
    }
}