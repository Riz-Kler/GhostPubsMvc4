using System;
using System.Collections.Generic;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Feature
    {
        public Feature()
        {
            this.Tags = new List<Tag>();
        }

        public int FeatureID { get; set; }
        public DateTime LastModified { get; set; }
        public int FeatureTypeID { get; set; }
        public string Name { get; set; }
        public virtual FeatureType FeatureType { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}