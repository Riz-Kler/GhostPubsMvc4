using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class FeatureType : IEntity
    {
        public FeatureType()
        {
            this.Features = new List<Feature>();
        }

        public int? ParentFeatureTypeId { get; set; }
        public DateTime LastModified { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual ICollection<Feature> Features { get; set; }
        public int Id { get; set; }
    }
}