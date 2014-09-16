using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class AddressType : IEntity
    {
        public AddressType()
        {
            this.Orgs = new List<Org>();
        }

        public DateTime LastModified { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual ICollection<Org> Orgs { get; set; }
        public int Id { get; set; }
    }
}