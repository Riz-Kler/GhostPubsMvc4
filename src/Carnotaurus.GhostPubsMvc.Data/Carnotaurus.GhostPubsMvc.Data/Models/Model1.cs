namespace Carnotaurus.GhostPubsMvc.Data.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<FeatureType> FeatureTypes { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookItem> BookItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ContentPage> ContentPages { get; set; }
        public virtual DbSet<County> Counties { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Rural> Rurals { get; set; }
        public virtual DbSet<LocalAuthority> LocalAuthorities { get; set; }
        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Org> Orgs { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<DuplicateTagView> DuplicateTagViews { get; set; }
        public virtual DbSet<Haunted1Notmapped> Haunted1Notmapped { get; set; }
        public virtual DbSet<Haunted2MissingDetail> Haunted2MissingDetail { get; set; }
        public virtual DbSet<Haunted3Mismapped> Haunted3Mismapped { get; set; }
        public virtual DbSet<HauntedOrg> HauntedOrgs { get; set; }
        public virtual DbSet<HauntedWithoutANote> HauntedWithoutANotes { get; set; }
        public virtual DbSet<HauntedWithoutATag> HauntedWithoutATags { get; set; }
        public virtual DbSet<OrgsNotFound> OrgsNotFounds { get; set; }
        public virtual DbSet<OrgsOpenPossibleDupesGrouped> OrgsOpenPossibleDupesGroupeds { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeatureType>()
                .HasMany(e => e.Features)
                .WithRequired(e => e.FeatureType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Region>()
                .HasMany(e => e.Counties)
                .WithRequired(e => e.Region)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.Wages___m_)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F10)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F11)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F12)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.GVA___m_)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F14)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F15)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F16)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F17)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.Net_capital_expenditure___m_)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F29)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.Total_tax_estimates___m_)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F31)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F32)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F33)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F34)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F35)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.Direct_tax_estimates___m_)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F37)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F38)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F39)
                .HasPrecision(19, 4);

            modelBuilder.Entity<LocalAuthority>()
                .Property(e => e.F40)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Feature>()
                .HasMany(e => e.Tags)
                .WithRequired(e => e.Feature)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Org>()
                .HasMany(e => e.Notes)
                .WithRequired(e => e.Org)
                .WillCascadeOnDelete(false);
        }
    }
}
