using System;
using System.Linq;
using System.Linq.Expressions;

namespace Carnotaurus.GhostPubsMvc.Data.Interfaces
{
    public interface IReadStore
    {
        IQueryable<T> Items<T>() where T : class, IEntity;
        IQueryable<T> Items<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity;
        T Load<T>(Int32 id) where T : class, IEntity;
    }
}