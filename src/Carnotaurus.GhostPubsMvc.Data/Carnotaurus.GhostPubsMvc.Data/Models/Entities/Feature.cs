using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class Feature : IEntity
    {
        public Feature()
        {
            this.Tags = new List<Tag>();
        }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }

        public int FeatureTypeId { get; set; }
        public string Name { get; set; }
        public virtual FeatureType FeatureType { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public int Id { get; set; }
    }
}