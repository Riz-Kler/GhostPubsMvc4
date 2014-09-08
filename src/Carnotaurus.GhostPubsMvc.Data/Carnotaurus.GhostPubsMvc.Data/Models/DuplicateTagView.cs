namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DuplicateTagView")]
    public partial class DuplicateTagView
    {
        public int? Expr1 { get; set; }

        public int? OrgID { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FeatureID { get; set; }
    }
}
