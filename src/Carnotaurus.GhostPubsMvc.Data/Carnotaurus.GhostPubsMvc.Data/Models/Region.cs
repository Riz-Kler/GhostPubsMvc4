using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Region : IEntity
    {
        public Region()
        {
            this.Counties = new List<County>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual ICollection<County> Counties { get; set; }
        public int Id { get; set; }
    }
}