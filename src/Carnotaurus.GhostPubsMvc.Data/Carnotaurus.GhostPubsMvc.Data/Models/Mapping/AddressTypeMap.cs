using System.Data.Entity.ModelConfiguration;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
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
            this.Property(t => t.Id).HasColumnName("AddressTypeID");
            this.Property(t => t.LastModified).HasColumnName("LastModified");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Deleted).HasColumnName("Deleted");
        }
    }
}