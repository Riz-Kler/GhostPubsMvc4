using System.Data.Entity.ModelConfiguration;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class ContentPageMap : EntityTypeConfiguration<ContentPage>
    {
        public ContentPageMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Title)
                .HasMaxLength(150);

            this.Property(t => t.Lead)
                .HasMaxLength(1000);

            this.Property(t => t.WellHeader)
                .HasMaxLength(1000);

            this.Property(t => t.Description)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("ContentPage");
            this.Property(t => t.Id).HasColumnName("ID");
            this.Property(t => t.CategoryId).HasColumnName("CategoryID");
            this.Property(t => t.OrderId).HasColumnName("OrderID");
            this.Property(t => t.PageTemplateId).HasColumnName("PageTemplateID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Lead).HasColumnName("Lead");
            this.Property(t => t.WellHeader).HasColumnName("WellHeader");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.HtmlText).HasColumnName("HtmlText");
            this.Property(t => t.Modified).HasColumnName("Modified");
            this.Property(t => t.Modified).HasColumnName("Modified");
            this.Property(t => t.Deleted).HasColumnName("Deleted");

            // Relationships
            this.HasOptional(t => t.Category)
                .WithMany(t => t.ContentPages)
                .HasForeignKey(d => d.CategoryId);
        }
    }
}