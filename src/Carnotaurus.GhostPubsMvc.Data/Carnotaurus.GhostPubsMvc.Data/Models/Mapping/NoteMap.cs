using System.Data.Entity.ModelConfiguration;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class NoteMap : EntityTypeConfiguration<Note>
    {
        public NoteMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Text)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Note", "Organisation");
            this.Property(t => t.Id).HasColumnName("NoteID");
            this.Property(t => t.LastModified).HasColumnName("LastModified");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.OrgId).HasColumnName("OrgID");
            this.Property(t => t.Text).HasColumnName("Text");
            this.Property(t => t.Source).HasColumnName("Source");

            // Relationships
            this.HasRequired(t => t.Org)
                .WithMany(t => t.Notes)
                .HasForeignKey(d => d.OrgId);
        }
    }
}