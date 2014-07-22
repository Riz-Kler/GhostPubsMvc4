using System;

namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class HauntedWithoutATag
    {
        public int OrgID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public int? AddressTypeID { get; set; }
        public int? CountyID { get; set; }
        public int? ParentID { get; set; }
        public int? TradingStatus { get; set; }
        public int? HauntedStatus { get; set; }
        public string TradingName { get; set; }
        public string AlternateName { get; set; }
        public string SearchName { get; set; }
        public string Locality { get; set; }
        public string Town { get; set; }
        public string Administrative_area_level_2 { get; set; }
        public string Postcode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Twitter { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
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