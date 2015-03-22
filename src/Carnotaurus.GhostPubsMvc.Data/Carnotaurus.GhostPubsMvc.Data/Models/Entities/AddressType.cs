using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    [Table("AddressType", Schema = "Category")]
    public class AddressType : IEntity
    {
        public AddressType()
        {
            this.Orgs = new List<Org>();
        }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Org> Orgs { get; set; }
        public int Id { get; set; }
    }
}