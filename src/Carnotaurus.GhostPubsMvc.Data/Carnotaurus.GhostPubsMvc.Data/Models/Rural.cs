namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("nso.Rural")]
    public partial class Rural
    {
        public int ID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Region { get; set; }

        [StringLength(255)]
        public string Classification { get; set; }

        public int? Population { get; set; }

        public double? PopulationPercent { get; set; }

        public int? ClaimantCount { get; set; }

        [StringLength(255)]
        public string ClaimantCountPercent { get; set; }

        [StringLength(255)]
        public string InsolvencyRate { get; set; }

        public int? InsolvencyNumber { get; set; }

        [StringLength(255)]
        public string UnitsInAgriculture { get; set; }

        [StringLength(255)]
        public string UnitsInWholesale { get; set; }

        [StringLength(255)]
        public string UnitsInProfessional { get; set; }

        [StringLength(255)]
        public string UnitsInConstruction { get; set; }

        [StringLength(255)]
        public string UnitsInTourism { get; set; }

        [StringLength(255)]
        public string UnitsInPublicSector { get; set; }

        public int? UnitsInAdmin { get; set; }

        public int? UnitsInManufacturing { get; set; }

        [StringLength(255)]
        public string UnitsInOther { get; set; }

        [StringLength(255)]
        public string UnitsInTotal { get; set; }

        [StringLength(255)]
        public string AvgHousePriceDetached { get; set; }

        [StringLength(255)]
        public string AvgHousePriceSemiDetached { get; set; }

        [StringLength(255)]
        public string AvgHousePriceFlat { get; set; }
    }
}
