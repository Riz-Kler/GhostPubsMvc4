//using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.ModelConfiguration;

//namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
//{
//    public class Haunted1NotmappedMap : EntityTypeConfiguration<Haunted1Notmapped>
//    {
//        public Haunted1NotmappedMap()
//        {
//            // Primary Key
//            this.HasKey(t => new {BookItemID = t.BookItemId, t.Created, t.Modified});

//            // Properties
//            this.Property(t => t.BookItemId)
//                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

//            this.Property(t => t.Postcode)
//                .HasMaxLength(50);

//            // Table & Column Mappings
//            this.ToTable("Haunted1Notmapped");
//            this.Property(t => t.BookItemId).HasColumnName("BookItemID");
//            this.Property(t => t.Created).HasColumnName("Created");
//            this.Property(t => t.Modified).HasColumnName("Modified");
//            this.Property(t => t.Deleted).HasColumnName("Deleted");
//            this.Property(t => t.BookId).HasColumnName("BookID");
//            this.Property(t => t.OrgId).HasColumnName("OrgID");
//            this.Property(t => t.County).HasColumnName("County");
//            this.Property(t => t.Town).HasColumnName("Town");
//            this.Property(t => t.AlternativeTown).HasColumnName("AlternativeTown");
//            this.Property(t => t.TradingName).HasColumnName("TradingName");
//            this.Property(t => t.Text).HasColumnName("Text");
//            this.Property(t => t.Postcode).HasColumnName("Postcode");
//        }
//    }
//}