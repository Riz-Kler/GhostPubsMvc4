using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class Authority : IEntity
    {
        public Authority()
        {
            this.Orgs = new List<Org>();
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public int? Population { get; set; }
        public int? Hectares { get; set; }
        public double? Density { get; set; }

        public virtual ICollection<Org> Orgs { get; set; }

        // recursive fix
        public int ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual Authority ParentAuthority { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }

    }
}