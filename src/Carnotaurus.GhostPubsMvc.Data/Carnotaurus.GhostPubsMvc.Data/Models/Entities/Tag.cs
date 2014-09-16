using System;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class Tag : IEntity
    {
        public DateTime LastModified { get; set; }
        public int? OrgId { get; set; }
        public int FeatureId { get; set; }
        public virtual Feature Feature { get; set; }
        public virtual Org Org { get; set; }
        public int Id { get; set; }
    }
}