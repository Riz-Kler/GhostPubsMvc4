using System.Data.Entity.ModelConfiguration;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class TagMap : EntityTypeConfiguration<Tag>
    {
        public TagMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Tag", "Organisation");
            this.Property(t => t.Id).HasColumnName("ID");
            this.Property(t => t.Created).HasColumnName("Created");
            this.Property(t => t.Modified).HasColumnName("Modified");
            this.Property(t => t.OrgId).HasColumnName("OrgID");
            this.Property(t => t.FeatureId).HasColumnName("FeatureID");

            // Relationships
            this.HasRequired(t => t.Feature)
                .WithMany(t => t.Tags)
                .HasForeignKey(d => d.FeatureId);
            this.HasRequired(t => t.Org)
                .WithMany(t => t.Tags)
                .HasForeignKey(d => d.OrgId);
        }
    }
}