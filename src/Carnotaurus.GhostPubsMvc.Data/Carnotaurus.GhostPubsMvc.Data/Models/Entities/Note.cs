//using System;
//using System.ComponentModel.DataAnnotations.Schema;
//using Carnotaurus.GhostPubsMvc.Data.Interfaces;

//namespace Carnotaurus.GhostPubsMvc.Data.Models.Entities
//{
//    [Table("Note", Schema = "Organisation")]
//    public class Note : IEntity
//    {
//        public DateTime Created { get; set; }
//        public DateTime Modified { get; set; }
//        public DateTime? Deleted { get; set; }

//        public int OrgId { get; set; }
//        public string Text { get; set; }
//        public string Source { get; set; }
//        public bool IsApproved { get; set; }
//        public virtual Org Org { get; set; }
//        public int Id { get; set; }
//    }
//}