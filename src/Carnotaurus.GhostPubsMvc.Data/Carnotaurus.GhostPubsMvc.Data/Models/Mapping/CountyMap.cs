using System.Data.Entity.ModelConfiguration;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class CountyMap : EntityTypeConfiguration<County>
    {
        public CountyMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(300);

            this.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("County");
            this.Property(t => t.Id).HasColumnName("CountyID");
            this.Property(t => t.RegionId).HasColumnName("RegionID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasRequired(t => t.Region)
                .WithMany(t => t.Counties)
                .HasForeignKey(d => d.RegionId);
        }
    }
}