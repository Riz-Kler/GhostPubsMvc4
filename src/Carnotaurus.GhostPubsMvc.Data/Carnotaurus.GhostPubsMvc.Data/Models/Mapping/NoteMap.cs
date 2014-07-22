using System.Data.Entity.ModelConfiguration;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class NoteMap : EntityTypeConfiguration<Note>
    {
        public NoteMap()
        {
            // Primary Key
            this.HasKey(t => t.NoteID);

            // Properties
            this.Property(t => t.Text)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Note", "Organisation");
            this.Property(t => t.NoteID).HasColumnName("NoteID");
            this.Property(t => t.LastModified).HasColumnName("LastModified");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.OrgID).HasColumnName("OrgID");
            this.Property(t => t.Text).HasColumnName("Text");
            this.Property(t => t.Source).HasColumnName("Source");

            // Relationships
            this.HasRequired(t => t.Org)
                .WithMany(t => t.Notes)
                .HasForeignKey(d => d.OrgID);
        }
    }
}