using System.Data.Entity.ModelConfiguration;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class TagMap : EntityTypeConfiguration<Tag>
    {
        public TagMap()
        {
            // Primary Key
            this.HasKey(t => t.TagId);

            // Properties
            // Table & Column Mappings
            this.ToTable("Tag", "Organisation");
            this.Property(t => t.TagId).HasColumnName("TagID");
            this.Property(t => t.LastModified).HasColumnName("LastModified");
            this.Property(t => t.OrgId).HasColumnName("OrgID");
            this.Property(t => t.FeatureId).HasColumnName("FeatureID");

            // Relationships
            this.HasRequired(t => t.Feature)
                .WithMany(t => t.Tags)
                .HasForeignKey(d => d.FeatureId);
            this.HasOptional(t => t.Org)
                .WithMany(t => t.Tags)
                .HasForeignKey(d => d.OrgId);
        }
    }
}