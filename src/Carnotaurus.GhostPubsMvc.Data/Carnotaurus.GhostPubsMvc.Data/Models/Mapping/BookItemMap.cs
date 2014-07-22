using System.Data.Entity.ModelConfiguration;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class BookItemMap : EntityTypeConfiguration<BookItem>
    {
        public BookItemMap()
        {
            // Primary Key
            this.HasKey(t => t.BookItemID);

            // Properties
            this.Property(t => t.Postcode)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("BookItem");
            this.Property(t => t.BookItemID).HasColumnName("BookItemID");
            this.Property(t => t.Created).HasColumnName("Created");
            this.Property(t => t.Modified).HasColumnName("Modified");
            this.Property(t => t.Deleted).HasColumnName("Deleted");
            this.Property(t => t.BookID).HasColumnName("BookID");
            this.Property(t => t.OrgID).HasColumnName("OrgID");
            this.Property(t => t.County).HasColumnName("County");
            this.Property(t => t.Town).HasColumnName("Town");
            this.Property(t => t.AlternativeTown).HasColumnName("AlternativeTown");
            this.Property(t => t.TradingName).HasColumnName("TradingName");
            this.Property(t => t.Text).HasColumnName("Text");
            this.Property(t => t.Postcode).HasColumnName("Postcode");

            // Relationships
            this.HasOptional(t => t.Book)
                .WithMany(t => t.BookItems)
                .HasForeignKey(d => d.BookID);
            this.HasOptional(t => t.Org)
                .WithMany(t => t.BookItems)
                .HasForeignKey(d => d.OrgID);
        }
    }
}