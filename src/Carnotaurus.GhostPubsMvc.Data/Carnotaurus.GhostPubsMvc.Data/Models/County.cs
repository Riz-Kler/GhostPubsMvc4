using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class County : IEntity
    {
        public County()
        {
            this.Orgs = new List<Org>();
        }

        public int RegionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Org> Orgs { get; set; }
        public int Id { get; set; }
    }
}