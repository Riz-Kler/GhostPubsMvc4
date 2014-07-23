using System;
using System.Collections.Generic;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Book
    {
        public Book()
        {
            this.BookItems = new List<BookItem>();
        }

        public int BookId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public string Name { get; set; }
        public virtual ICollection<BookItem> BookItems { get; set; }
    }
}