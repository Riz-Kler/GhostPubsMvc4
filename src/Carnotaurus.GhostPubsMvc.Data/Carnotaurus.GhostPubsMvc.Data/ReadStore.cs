using System;
using System.Linq;
using System.Linq.Expressions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data
{
    public class ReadStore : IReadStore
    {
        private readonly IDataContext _dataContext;

        public ReadStore(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IQueryable<T> Items<T>() where T : class, IEntity
        {
            return _dataContext.Items<T>();
        }

        public IQueryable<T> Items<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            return _dataContext.Items(predicate);
        }

        public T Load<T>(Int32 id) where T : class, IEntity
        {
            return _dataContext.Load<T>(id);
        }
    }
}