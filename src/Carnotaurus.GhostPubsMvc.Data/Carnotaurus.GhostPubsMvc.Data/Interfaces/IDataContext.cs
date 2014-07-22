using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Carnotaurus.GhostPubsMvc.Data.Interfaces
{
    public interface IDataContext
    {
        IQueryable<T> Items<T>() where T : class, IEntity;
        IQueryable<T> Items<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity;
        T Load<T>(Int32 id) where T : class, IEntity;
        void Insert<T>(T item) where T : class, IEntity;
        void Insert<T>(List<T> items) where T : class, IEntity;
        void Delete<T>(T item) where T : class, IEntity;
        void Delete<T>(List<T> items) where T : class, IEntity;
        int SaveChanges();
    }
}