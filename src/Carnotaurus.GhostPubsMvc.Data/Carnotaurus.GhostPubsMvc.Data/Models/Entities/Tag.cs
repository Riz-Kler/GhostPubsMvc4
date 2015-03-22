using System;
using System.ComponentModel.DataAnnotations.Schema;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    [Table("Tag", Schema = "Organisation")]
    public class Tag : IEntity
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }

        public int OrgId { get; set; }
        public int FeatureId { get; set; }
        public virtual Feature Feature { get; set; }
        public virtual Org Org { get; set; }
        public int Id { get; set; }
    }
}