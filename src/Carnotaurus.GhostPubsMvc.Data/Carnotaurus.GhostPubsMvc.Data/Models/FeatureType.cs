using System;
using System.Collections.Generic;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class FeatureType
    {
        public FeatureType()
        {
            this.Features = new List<Feature>();
        }

        public int FeatureTypeId { get; set; }
        public int? ParentFeatureTypeId { get; set; }
        public DateTime LastModified { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual ICollection<Feature> Features { get; set; }
    }
}