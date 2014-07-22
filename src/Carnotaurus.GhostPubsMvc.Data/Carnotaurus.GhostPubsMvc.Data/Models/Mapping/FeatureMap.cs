using System.Data.Entity.ModelConfiguration;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class FeatureMap : EntityTypeConfiguration<Feature>
    {
        public FeatureMap()
        {
            // Primary Key
            this.HasKey(t => t.FeatureID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("Feature", "Organisation");
            this.Property(t => t.FeatureID).HasColumnName("FeatureID");
            this.Property(t => t.LastModified).HasColumnName("LastModified");
            this.Property(t => t.FeatureTypeID).HasColumnName("FeatureTypeID");
            this.Property(t => t.Name).HasColumnName("Name");

            // Relationships
            this.HasRequired(t => t.FeatureType)
                .WithMany(t => t.Features)
                .HasForeignKey(d => d.FeatureTypeID);
        }
    }
}