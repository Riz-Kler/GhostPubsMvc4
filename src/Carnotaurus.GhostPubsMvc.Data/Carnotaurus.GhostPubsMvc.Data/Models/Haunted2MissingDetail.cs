namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Haunted2MissingDetail
    {
        [Key]
        [Column(Order = 0)]
        public int OrgID { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime Created { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }

        public int? AddressTypeID { get; set; }

        public int? CountyID { get; set; }

        public int? ParentID { get; set; }

        public int? TradingStatus { get; set; }

        public int? HauntedStatus { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(300)]
        public string TradingName { get; set; }

        [StringLength(300)]
        public string AlternateName { get; set; }

        [StringLength(300)]
        public string SearchName { get; set; }

        [StringLength(300)]
        public string Locality { get; set; }

        [StringLength(300)]
        public string Town { get; set; }

        [StringLength(300)]
        public string Administrative_area_level_2 { get; set; }

        [StringLength(8)]
        public string Postcode { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(300)]
        public string Twitter { get; set; }

        [StringLength(300)]
        public string Email { get; set; }

        [StringLength(300)]
        public string Facebook { get; set; }

        [StringLength(300)]
        public string Website { get; set; }

        public int? OSX { get; set; }

        public int? OSY { get; set; }

        public double? Lat { get; set; }

        public double? Lon { get; set; }

        public int? Tried { get; set; }

        public string GoogleMapData { get; set; }

        public DateTime? ManualConfirmDate { get; set; }
    }
}
