namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Haunted1Notmapped
    {
        [Key]
        [Column(Order = 0)]
        public int BookItemID { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime Created { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }

        public int? BookID { get; set; }

        public int? OrgID { get; set; }

        public string County { get; set; }

        public string Town { get; set; }

        public string AlternativeTown { get; set; }

        public string TradingName { get; set; }

        public string Text { get; set; }

        [StringLength(50)]
        public string Postcode { get; set; }
    }
}
