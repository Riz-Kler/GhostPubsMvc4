using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Book : IEntity
    {
        public Book()
        {
            this.BookItems = new List<BookItem>();
        }

        public int  Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public string Name { get; set; }
        public virtual ICollection<BookItem> BookItems { get; set; }
    }
}