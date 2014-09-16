using System;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class Note : IEntity
    {
        public DateTime LastModified { get; set; }
        public DateTime CreateDate { get; set; }
        public int OrgId { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public virtual Org Org { get; set; }
        public int Id { get; set; }
    }
}