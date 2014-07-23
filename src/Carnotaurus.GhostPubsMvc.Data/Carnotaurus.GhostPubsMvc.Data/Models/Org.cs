using System;
using System.Collections.Generic;
 
namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    public class Org
    {
        public Org()
        {
            this.BookItems = new List<BookItem>();
            this.Notes = new List<Note>();
            this.Tags = new List<Tag>();
        }

        public string PostcodePrimaryPart
        {
            get
            {
                var prepare = Postcode.Trim();

                var output = String.Empty;

                if (prepare.Length > 2)
                {
                    output = prepare
                        .Substring(0, prepare.Length - 3)
                        .Trim();
                }

                return output;
            }
        }

        public int OrgId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public int? AddressTypeId { get; set; }
        public int? CountyId { get; set; }
        public int? ParentId { get; set; }
        public int? TradingStatus { get; set; }
        public int? HauntedStatus { get; set; }
        public string TradingName { get; set; }
        public string AlternateName { get; set; }
        public string SearchName { get; set; }
        public string Locality { get; set; }
        public string Town { get; set; }
        public string AdministrativeAreaLevel2 { get; set; }
        public string Postcode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Twitter { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Website { get; set; }
        public int? OsX { get; set; }
        public int? OsY { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public int? Tried { get; set; }
        public string GoogleMapData { get; set; }
        public DateTime? ManualConfirmDate { get; set; }
        public virtual AddressType AddressType { get; set; }
        public virtual ICollection<BookItem> BookItems { get; set; }
        public virtual County County { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}