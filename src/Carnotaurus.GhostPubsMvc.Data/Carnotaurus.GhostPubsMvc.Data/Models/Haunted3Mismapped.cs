namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Haunted3Mismapped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BookItemId { get; set; }

        public int? OrgId { get; set; }

        public string TradingName { get; set; }

        [StringLength(300)]
        public string Name { get; set; }

        public string Location { get; set; }

        public string Text { get; set; }
    }
}
