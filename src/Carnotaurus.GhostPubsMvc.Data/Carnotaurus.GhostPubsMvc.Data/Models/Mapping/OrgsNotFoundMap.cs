//using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.ModelConfiguration;

//namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
//{
//    public class OrgsNotFoundMap : EntityTypeConfiguration<OrgsNotFound>
//    {
//        public OrgsNotFoundMap()
//        {
//            // Primary Key
//            this.HasKey(t => new {OrgID = t.OrgId, t.Created, t.Modified, t.TradingName});

//            // Properties
//            this.Property(t => t.OrgId)
//                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

//            this.Property(t => t.TradingName)
//                .IsRequired()
//                .HasMaxLength(300);

//            this.Property(t => t.AlternateName)
//                .HasMaxLength(300);

//            this.Property(t => t.SearchName)
//                .HasMaxLength(300);

//            this.Property(t => t.Locality)
//                .HasMaxLength(300);

//            this.Property(t => t.Town)
//                .HasMaxLength(300);

//            this.Property(t => t.AdministrativeAreaLevel2)
//                .HasMaxLength(300);

//            this.Property(t => t.Postcode)
//                .HasMaxLength(8);

//            this.Property(t => t.Address)
//                .HasMaxLength(300);

//            this.Property(t => t.Phone)
//                .HasMaxLength(50);

//            this.Property(t => t.Twitter)
//                .HasMaxLength(300);

//            this.Property(t => t.Email)
//                .HasMaxLength(300);

//            this.Property(t => t.Facebook)
//                .HasMaxLength(300);

//            this.Property(t => t.Website)
//                .HasMaxLength(300);

//            // Table & Column Mappings
//            this.ToTable("OrgsNotFound");
//            this.Property(t => t.OrgId).HasColumnName("OrgID");
//            this.Property(t => t.Created).HasColumnName("Created");
//            this.Property(t => t.Modified).HasColumnName("Modified");
//            this.Property(t => t.Deleted).HasColumnName("Deleted");
//            this.Property(t => t.AddressTypeId).HasColumnName("AddressTypeID");
//            this.Property(t => t.CountyId).HasColumnName("CountyID");
//            this.Property(t => t.ParentId).HasColumnName("ParentID");
//            this.Property(t => t.TradingStatus).HasColumnName("TradingStatus");
//            this.Property(t => t.HauntedStatus).HasColumnName("HauntedStatus");
//            this.Property(t => t.TradingName).HasColumnName("TradingName");
//            this.Property(t => t.AlternateName).HasColumnName("AlternateName");
//            this.Property(t => t.SearchName).HasColumnName("SearchName");
//            this.Property(t => t.Locality).HasColumnName("Locality");
//            this.Property(t => t.Town).HasColumnName("Town");
//            this.Property(t => t.AdministrativeAreaLevel2).HasColumnName("Administrative_area_level_2");
//            this.Property(t => t.Postcode).HasColumnName("Postcode");
//            this.Property(t => t.Address).HasColumnName("Address");
//            this.Property(t => t.Phone).HasColumnName("Phone");
//            this.Property(t => t.Twitter).HasColumnName("Twitter");
//            this.Property(t => t.Email).HasColumnName("Email");
//            this.Property(t => t.Facebook).HasColumnName("Facebook");
//            this.Property(t => t.Website).HasColumnName("Website");
//            this.Property(t => t.OsX).HasColumnName("OSX");
//            this.Property(t => t.OsY).HasColumnName("OSY");
//            this.Property(t => t.Lat).HasColumnName("Lat");
//            this.Property(t => t.Lon).HasColumnName("Lon");
//            this.Property(t => t.Tried).HasColumnName("Tried");
//            this.Property(t => t.GoogleMapData).HasColumnName("GoogleMapData");
//            this.Property(t => t.ManualConfirmDate).HasColumnName("ManualConfirmDate");
//        }
//    }
//}

