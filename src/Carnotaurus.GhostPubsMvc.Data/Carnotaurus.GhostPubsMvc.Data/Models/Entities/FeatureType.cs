using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    [Table("FeatureType", Schema = "Category")]
    public class FeatureType : IEntity
    {
        public FeatureType()
        {
            this.Features = new HashSet<Feature>();
        }

        public int? ParentFeatureTypeId { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Feature> Features { get; set; }
        public int Id { get; set; }
    }
}