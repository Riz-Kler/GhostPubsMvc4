using System;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Note
    {
        public int NoteID { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime CreateDate { get; set; }
        public int OrgID { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public virtual Org Org { get; set; }
    }
}