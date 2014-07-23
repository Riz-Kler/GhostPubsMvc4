using System;
using System.Collections.Generic;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class AddressType
    {
        public AddressType()
        {
            this.Orgs = new List<Org>();
        }

        public int AddressTypeId { get; set; }
        public DateTime LastModified { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual ICollection<Org> Orgs { get; set; }
    }
}