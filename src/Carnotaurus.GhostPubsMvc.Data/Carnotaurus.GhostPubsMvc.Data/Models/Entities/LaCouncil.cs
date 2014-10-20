using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class LaCouncil : IEntity
    {
        public LaCouncil()
        {
            this.Orgs = new List<Org>();
        }

        public int LaCountyId { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public int Population { get; set; }
        public int Males { get; set; }
        public int Females { get; set; }
        public int Hectares { get; set; }
        public float Density { get; set; }

        public virtual LaCounty LaCounty { get; set; }
        public virtual ICollection<Org> Orgs { get; set; }
        public int Id { get; set; }
    }
}