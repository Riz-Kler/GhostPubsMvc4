using System.Collections.Generic;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class County
    {
        public County()
        {
            this.Orgs = new List<Org>();
        }

        public int CountyID { get; set; }
        public int RegionID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Org> Orgs { get; set; }
    }
}