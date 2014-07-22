using System.Data.Entity.ModelConfiguration;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class FeatureTypeMap : EntityTypeConfiguration<FeatureType>
    {
        public FeatureTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.FeatureTypeID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("FeatureType", "Category");
            this.Property(t => t.FeatureTypeID).HasColumnName("FeatureTypeID");
            this.Property(t => t.ParentFeatureTypeID).HasColumnName("ParentFeatureTypeID");
            this.Property(t => t.LastModified).HasColumnName("LastModified");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Deleted).HasColumnName("Deleted");
        }
    }
}