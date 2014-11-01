//using System.Data.Entity.ModelConfiguration;
//using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

//namespace Carnotaurus.GhostPubsMvc.Data.Models.Mapping
//{
//    public class LocalAuthorityMap : EntityTypeConfiguration<LocalAuthority>
//    {
//        public LocalAuthorityMap()
//        {
//            // Primary Key
//            this.HasKey(t => t.Id);

//            // Properties
//            this.Property(t => t.Name)
//                .HasMaxLength(255);

//            this.Property(t => t.Region)
//                .HasMaxLength(255);

//            // Table & Column Mappings
//            this.ToTable("LocalAuthorities", "oe");
//            this.Property(t => t.Id).HasColumnName("Id");
//            this.Property(t => t.Name).HasColumnName("Local Authority");
//            this.Property(t => t.Region).HasColumnName("Region");
//            this.Property(t => t.NumberOfBreweries).HasColumnName("Number of breweries");
//            this.Property(t => t.NumberOfPubs).HasColumnName("Number of pubs");
//            this.Property(t => t.Employment).HasColumnName("Employment");
//            this.Property(t => t.F6).HasColumnName("F6");
//            this.Property(t => t.F7).HasColumnName("F7");
//            this.Property(t => t.F8).HasColumnName("F8");
//            this.Property(t => t.Wages).HasColumnName("Wages (£m)");
//            this.Property(t => t.F10).HasColumnName("F10");
//            this.Property(t => t.F11).HasColumnName("F11");
//            this.Property(t => t.F12).HasColumnName("F12");
//            this.Property(t => t.GVA).HasColumnName("GVA (£m)");
//            this.Property(t => t.F14).HasColumnName("F14");
//            this.Property(t => t.F15).HasColumnName("F15");
//            this.Property(t => t.F16).HasColumnName("F16");
//            this.Property(t => t.F17).HasColumnName("F17");
//            this.Property(t => t.DirectEmploymentByAge).HasColumnName("Direct employment by age");
//            this.Property(t => t.F19).HasColumnName("F19");
//            this.Property(t => t.F20).HasColumnName("F20");
//            this.Property(t => t.F21).HasColumnName("F21");
//            this.Property(t => t.F22).HasColumnName("F22");
//            this.Property(t => t.F23).HasColumnName("F23");
//            this.Property(t => t.F24).HasColumnName("F24");
//            this.Property(t => t.DirectEmploymentByStatus).HasColumnName("Direct employment by status");
//            this.Property(t => t.F26).HasColumnName("F26");
//            this.Property(t => t.F27).HasColumnName("F27");
//            this.Property(t => t.NetCapitalExpenditure).HasColumnName("Net capital expenditure (£m)");
//            this.Property(t => t.F29).HasColumnName("F29");
//            this.Property(t => t.TotalTaxEstimates).HasColumnName("Total tax estimates (£m)");
//            this.Property(t => t.F31).HasColumnName("F31");
//            this.Property(t => t.F32).HasColumnName("F32");
//            this.Property(t => t.F33).HasColumnName("F33");
//            this.Property(t => t.F34).HasColumnName("F34");
//            this.Property(t => t.F35).HasColumnName("F35");
//            this.Property(t => t.DirectTaxEstimates).HasColumnName("Direct tax estimates (£m)");
//            this.Property(t => t.F37).HasColumnName("F37");
//            this.Property(t => t.F38).HasColumnName("F38");
//            this.Property(t => t.F39).HasColumnName("F39");
//            this.Property(t => t.F40).HasColumnName("F40");
//        }
//    }
//}

