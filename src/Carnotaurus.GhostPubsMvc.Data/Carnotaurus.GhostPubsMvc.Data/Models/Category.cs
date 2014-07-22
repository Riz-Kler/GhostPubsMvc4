using System.Collections.Generic;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Category
    {
        public Category()
        {
            this.ContentPages = new List<ContentPage>();
        }

        public int CategoryID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ContentPage> ContentPages { get; set; }
    }
}