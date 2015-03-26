using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

//using Carnotaurus.GhostPubsMvc.Data.Models.Mapping;

namespace Carnotaurus.GhostPubsMvc.Data
{
    public class CmsContext : DbContext, IDataContext
    {
        static CmsContext()
        {
            Database.SetInitializer<CmsContext>(null);
        }

        public CmsContext()
            : base("Name=CmsContext")
        {
        }

        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<FeatureType> FeatureTypes { get; set; }
        public DbSet<Authority> Authorities { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Org> Orgs { get; set; }
        public DbSet<Tag> Tags { get; set; }

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
            // modelBuilder.Configurations.Add(new AddressTypeMap());
            //modelBuilder.Configurations.Add(new FeatureTypeMap());
            // modelBuilder.Configurations.Add(new CategoryMap());
            //modelBuilder.Configurations.Add(new ContentPageMap());
            // modelBuilder.Configurations.Add(new AuthorityMap());
            //modelBuilder.Configurations.Add(new FeatureMap());
            //modelBuilder.Configurations.Add(new NoteMap());
            //modelBuilder.Configurations.Add(new OrgMap());
            //modelBuilder.Configurations.Add(new TagMap());
        }
    }
}