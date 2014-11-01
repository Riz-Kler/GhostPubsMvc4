using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class Authority : IEntity
    {
        public Authority()
        {
            this.Orgs = new List<Org>();
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public int? Population { get; set; }
        public int? Hectares { get; set; }
        public double? Density { get; set; }
        public int ParentId { get; set; }

        //public virtual LaCounty LaCounty { get; set; }
        public virtual ICollection<Org> Orgs { get; set; }
        public int Id { get; set; }
    }
}