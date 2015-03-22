using System.Data.Entity.ModelConfiguration;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    // Todo - get rid of the mappings - use proper code first

    public class AddressTypeMap : EntityTypeConfiguration<AddressType>
    {
        public AddressTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("AddressType", "Category");
            this.Property(t => t.Id).HasColumnName("ID");
            this.Property(t => t.Modified).HasColumnName("Modified");
            this.Property(t => t.Created).HasColumnName("Created");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Deleted).HasColumnName("Deleted");
        }
    }
}