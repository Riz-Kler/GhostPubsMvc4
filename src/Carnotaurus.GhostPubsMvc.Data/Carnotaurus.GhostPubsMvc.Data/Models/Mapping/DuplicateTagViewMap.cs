using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class DuplicateTagViewMap : EntityTypeConfiguration<DuplicateTagView>
    {
        public DuplicateTagViewMap()
        {
            // Primary Key
            this.HasKey(t => t.FeatureId);

            // Properties
            this.Property(t => t.FeatureId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("DuplicateTagView");
            this.Property(t => t.Expr1).HasColumnName("Expr1");
            this.Property(t => t.OrgId).HasColumnName("OrgID");
            this.Property(t => t.FeatureId).HasColumnName("FeatureID");
        }
    }
}