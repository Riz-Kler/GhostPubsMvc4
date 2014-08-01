using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models;
using Carnotaurus.GhostPubsMvc.Data.Models.Mapping;

namespace Carnotaurus.GhostPubsMvc.Data
{
    public class CmsContext : DbContext, IDataContext
    {
        static CmsContext()
        {
            Database.SetInitializer<CmsContext>(null);
        }

        public CmsContext()
            : base("Name=CMSContext")
        {
        }

        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<FeatureType> FeatureTypes { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookItem> BookItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ContentPage> ContentPages { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<LocalAuthority> LocalAuthorities { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Org> Orgs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        //public DbSet<DuplicateTagView> DuplicateTagViews { get; set; }
        //public DbSet<Haunted1Notmapped> Haunted1Notmapped { get; set; }
        //public DbSet<Haunted2MissingDetail> Haunted2MissingDetail { get; set; }
        //public DbSet<Haunted3Mismapped> Haunted3Mismapped { get; set; }
        //public DbSet<HauntedOrg> HauntedOrgs { get; set; }
        //public DbSet<HauntedWithoutANote> HauntedWithoutANotes { get; set; }
        //public DbSet<HauntedWithoutATag> HauntedWithoutATags { get; set; }
        //public DbSet<OrgsNotFound> OrgsNotFounds { get; set; }
        //public DbSet<OrgsOpenPossibleDupesGrouped> OrgsOpenPossibleDupesGroupeds { get; set; }


        public IQueryable<T> Items<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            return Items<T>().Where(predicate);
        }

        public IQueryable<T> Items<T>() where T : class, IEntity
        {
            return Set<T>();
        }

        public T Load<T>(Int32 id) where T : class, IEntity
        {
            return Set<T>().FirstOrDefault(x => x.Id.Equals(id));
        }

        public void Insert<T>(List<T> items) where T : class, IEntity
        {
            Set<T>().AddRange(items);
        }

        public void Insert<T>(T item) where T : class, IEntity
        {
            Set<T>().Add(item);
        }

        public void Delete<T>(T item) where T : class, IEntity
        {
            Set<T>().Remove(item);
        }

        public void Delete<T>(List<T> items) where T : class, IEntity
        {
            Set<T>().RemoveRange(items);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AddressTypeMap());
            modelBuilder.Configurations.Add(new FeatureTypeMap());
            modelBuilder.Configurations.Add(new BookMap());
            modelBuilder.Configurations.Add(new BookItemMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new ContentPageMap());
            modelBuilder.Configurations.Add(new CountyMap());
            modelBuilder.Configurations.Add(new RegionMap());
            modelBuilder.Configurations.Add(new LocalAuthorityMap());
            modelBuilder.Configurations.Add(new FeatureMap());
            modelBuilder.Configurations.Add(new NoteMap());
            modelBuilder.Configurations.Add(new OrgMap());
            modelBuilder.Configurations.Add(new TagMap());
            //modelBuilder.Configurations.Add(new DuplicateTagViewMap());
            //modelBuilder.Configurations.Add(new Haunted1NotmappedMap());
            //modelBuilder.Configurations.Add(new Haunted2MissingDetailMap());
            //modelBuilder.Configurations.Add(new Haunted3MismappedMap());
            //modelBuilder.Configurations.Add(new HauntedOrgMap());
            //modelBuilder.Configurations.Add(new HauntedWithoutANoteMap());
            //modelBuilder.Configurations.Add(new HauntedWithoutATagMap());
            //modelBuilder.Configurations.Add(new OrgsNotFoundMap());
            //modelBuilder.Configurations.Add(new OrgsOpenPossibleDupesGroupedMap());
        }
    }
}