using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class LaRegion : IEntity
    {
        public LaRegion()
        {
            this.LaCounties = new List<LaCounty>();
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public int Population { get; set; }
        public int Males { get; set; }
        public int Females { get; set; }
        public int Hectares { get; set; }
        public float Density { get; set; }

        public virtual ICollection<LaCounty> LaCounties { get; set; }
        public int Id { get; set; }
    }
}