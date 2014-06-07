//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Carnotaurus.GhostPubsMvc.Web
{
    using System;
    using System.Collections.Generic;
    
    public partial class Org
    {
        public Org()
        {
            this.Notes = new HashSet<Note>();
            this.Tags = new HashSet<Tag>();
            this.BookItems = new HashSet<BookItem>();
        }
    
        public int OrgID { get; set; }
        public Nullable<System.DateTime> Deleted { get; set; }
        public Nullable<int> AddressTypeID { get; set; }
        public Nullable<int> CountyID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> TradingStatus { get; set; }
        public Nullable<int> HauntedStatus { get; set; }
        public string TradingName { get; set; }
        public string SearchName { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Twitter { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public Nullable<int> OSX { get; set; }
        public Nullable<int> OSY { get; set; }
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Lon { get; set; }
        public Nullable<int> Tried { get; set; }
        public string GoogleMapData { get; set; }
        public string Locality { get; set; }
        public string Administrative_area_level_2 { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Modified { get; set; }
        public Nullable<System.DateTime> ManualConfirmDate { get; set; }
        public string AlternateName { get; set; }
    
        public virtual County County { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<BookItem> BookItems { get; set; }
        public virtual AddressType AddressType { get; set; }
    }
}
