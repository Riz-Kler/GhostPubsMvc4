using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Category : IEntity
    {
        public Category()
        {
            this.ContentPages = new List<ContentPage>();
        }

        public string Name { get; set; }

        public virtual ICollection<ContentPage> ContentPages { get; set; }
        public int Id { get; set; }
    }
}