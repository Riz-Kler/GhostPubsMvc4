using System;
using System.Collections.Generic;
 
namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Region
    {
        public Region()
        {
            this.Counties = new List<County>();
        }

        public int RegionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual ICollection<County> Counties { get; set; }
    }
}