using System.Data.Entity.ModelConfiguration;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class LaCouncilMap : EntityTypeConfiguration<Authority>
    {
        public LaCouncilMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(300);

            this.Property(t => t.Code)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Authority");
            this.Property(t => t.Id).HasColumnName("AuthorityID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Population).HasColumnName("Population");
            this.Property(t => t.Hectares).HasColumnName("Hectares");
            this.Property(t => t.Density).HasColumnName("Density");

            //// Relationships
            //this.HasRequired(t => t.Region)
            //    .WithMany(t => t.Counties)
            //    .HasForeignKey(d => d.RegionId);
        }
    }
}