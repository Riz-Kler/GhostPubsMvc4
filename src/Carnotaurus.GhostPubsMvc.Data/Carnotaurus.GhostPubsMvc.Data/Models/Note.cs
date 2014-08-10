using System;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Note : IEntity
    {
        public int  Id { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime CreateDate { get; set; }
        public int OrgId { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public virtual Org Org { get; set; }
    }
}