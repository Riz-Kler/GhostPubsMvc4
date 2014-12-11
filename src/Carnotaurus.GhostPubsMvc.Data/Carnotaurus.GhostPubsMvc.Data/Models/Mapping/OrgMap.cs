using System.Data.Entity.ModelConfiguration;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
{
    public class OrgMap : EntityTypeConfiguration<Org>
    {
        public OrgMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TradingName)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.AlternateName)
                .HasMaxLength(300);

            this.Property(t => t.SimpleName)
                .HasMaxLength(300);

            this.Property(t => t.Locality)
                .HasMaxLength(300);

            this.Property(t => t.PostalTown)
                .HasMaxLength(300);
             
            this.Property(t => t.Postcode)
                .HasMaxLength(8);

            this.Property(t => t.Address)
                .HasMaxLength(300);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            this.Property(t => t.Twitter)
                .HasMaxLength(300);

            this.Property(t => t.Email)
                .HasMaxLength(300);

            this.Property(t => t.Facebook)
                .HasMaxLength(300);

            this.Property(t => t.Website)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("Org", "Organisation");
            this.Property(t => t.Id).HasColumnName("ID");
            this.Property(t => t.Created).HasColumnName("Created");
            this.Property(t => t.Modified).HasColumnName("Modified");
            this.Property(t => t.Deleted).HasColumnName("Deleted");
            this.Property(t => t.AddressTypeId).HasColumnName("AddressTypeID");
            this.Property(t => t.AuthorityId).HasColumnName("AuthorityId");
            this.Property(t => t.ParentId).HasColumnName("ParentID");
            this.Property(t => t.TradingStatus).HasColumnName("TradingStatus");
            this.Property(t => t.HauntedStatus).HasColumnName("HauntedStatus");
            this.Property(t => t.TradingName).HasColumnName("TradingName");
            this.Property(t => t.AlternateName).HasColumnName("AlternateName");
            this.Property(t => t.SimpleName).HasColumnName("SimpleName");
            this.Property(t => t.Locality).HasColumnName("Locality");
            this.Property(t => t.PostalTown).HasColumnName("Town");
            this.Property(t => t.Postcode).HasColumnName("Postcode");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Twitter).HasColumnName("Twitter");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Facebook).HasColumnName("Facebook");
            this.Property(t => t.Website).HasColumnName("Website");
            this.Property(t => t.OsX).HasColumnName("OSX");
            this.Property(t => t.OsY).HasColumnName("OSY");
            this.Property(t => t.Lat).HasColumnName("Lat");
            this.Property(t => t.Lon).HasColumnName("Lon");
            this.Property(t => t.Tried).HasColumnName("Tried");
            this.Property(t => t.GoogleMapData).HasColumnName("GoogleMapData");
            this.Property(t => t.LaCode).HasColumnName("LaCode");
            this.Property(t => t.LaTried).HasColumnName("LaTried");
            this.Property(t => t.LaData).HasColumnName("LaData");

            // Relationships
            this.HasOptional(t => t.AddressType)
                .WithMany(t => t.Orgs)
                .HasForeignKey(d => d.AddressTypeId);
            this.HasOptional(t => t.Authority)
                .WithMany(t => t.Orgs)
                .HasForeignKey(d => d.AuthorityId);
        }
    }
}