using System.Data.Entity.ModelConfiguration;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class BookMap : EntityTypeConfiguration<Book>
    {
        public BookMap()
        {
            // Primary Key
            this.HasKey(t => t. Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Book");
            this.Property(t => t. Id).HasColumnName("BookID");
            this.Property(t => t.Created).HasColumnName("Created");
            this.Property(t => t.Modified).HasColumnName("Modified");
            this.Property(t => t.Deleted).HasColumnName("Deleted");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}