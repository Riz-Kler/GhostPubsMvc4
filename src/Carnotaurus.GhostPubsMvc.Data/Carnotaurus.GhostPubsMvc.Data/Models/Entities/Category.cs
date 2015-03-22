using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
{
    public class Category : IEntity
    {
        public Category()
        {
            this.ContentPages = new List<ContentPage>();
        }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ContentPage> ContentPages { get; set; }
        public int Id { get; set; }
    }
}