using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class Haunted3MismappedMap : EntityTypeConfiguration<Haunted3Mismapped>
    {
        public Haunted3MismappedMap()
        {
            // Primary Key
            this.HasKey(t => t.BookItemId);

            // Properties
            this.Property(t => t.BookItemId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("Haunted3Mismapped");
            this.Property(t => t.BookItemId).HasColumnName("BookItemId");
            this.Property(t => t.OrgId).HasColumnName("OrgId");
            this.Property(t => t.TradingName).HasColumnName("TradingName");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.Text).HasColumnName("Text");
        }
    }
}