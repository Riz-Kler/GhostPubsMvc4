using System.Data.Entity.ModelConfiguration;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class AuthorityMap : EntityTypeConfiguration<Authority>
    {
        public AuthorityMap()
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
            this.Property(t => t.Id).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Population).HasColumnName("Population");
            this.Property(t => t.Hectares).HasColumnName("Hectares");
            this.Property(t => t.ParentId).HasColumnName("ParentID");

            // Ghost Specialist
            this.Property(t => t.LocalGhostSpecialistName).HasColumnName("LocalGhostSpecialistName");
            this.Property(t => t.LocalGhostSpecialistUrl).HasColumnName("LocalGhostSpecialistUrl");

            //// Ghost Tour Specialist
            this.Property(t => t.LocalGhostTourSpecialistName).HasColumnName("LocalGhostTourSpecialistName");
            this.Property(t => t.LocalGhostTourSpecialistUrl).HasColumnName("LocalGhostTourSpecialistUrl");

            //// Ghost Hunt Specialist
            this.Property(t => t.LocalGhostHuntSpecialistName).HasColumnName("LocalGhostHuntSpecialistName");
            this.Property(t => t.LocalGhostHuntSpecialistUrl).HasColumnName("LocalGhostHuntSpecialistUrl");

            //// Medium
            this.Property(t => t.LocalMediumName).HasColumnName("LocalMediumName");
            this.Property(t => t.LocalMediumUrl).HasColumnName("LocalMediumUrl");

            // // Council
            this.Property(t => t.LocalGhostCouncilName).HasColumnName("LocalGhostCouncilName");
            this.Property(t => t.LocalGhostCouncilUrl).HasColumnName("LocalGhostCouncilUrl");

            //// Brewery
            this.Property(t => t.LocalGhostBreweryName).HasColumnName("LocalGhostBreweryName");
            this.Property(t => t.LocalGhostBreweryUrl).HasColumnName("LocalGhostBreweryUrl");

            //// Pub Chain
            this.Property(t => t.LocalGhostPubChainName).HasColumnName("LocalGhostPubChainName");
            this.Property(t => t.LocalGhostPubChainUrl).HasColumnName("LocalGhostPubChainUrl");

            //// Heritage Society
            this.Property(t => t.LocalGhostHeritageSocietyName).HasColumnName("LocalGhostHeritageSocietyName");
            this.Property(t => t.LocalGhostHeritageSocietytUrl).HasColumnName("LocalGhostHeritageSocietytUrl");

            //// Special events
            this.Property(t => t.LocalGhostSpecialEventsName).HasColumnName("LocalGhostSpecialEventsName");
            this.Property(t => t.LocalGhostSpecialEventsUrl).HasColumnName("LocalGhostSpecialEventsUrl");

        }
    }
}