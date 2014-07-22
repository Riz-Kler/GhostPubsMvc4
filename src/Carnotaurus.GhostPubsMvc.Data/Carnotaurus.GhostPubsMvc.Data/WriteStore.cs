using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Data
{
    public class WriteStore : IWriteStore
    {
        private readonly IDataContext _dataContext;

        public WriteStore(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IQueryable<T> Items<T>() where T : class, IEntity
        {
            return _dataContext.Items<T>();
        }


        public T Load<T>(Int32 id) where T : class, IEntity
        {
            return _dataContext.Load<T>(id);
        }

        public void Insert<T>(T item) where T : class, IEntity
        {
            _dataContext.Insert(item);
        }

        public void Insert<T>(List<T> items) where T : class, IEntity
        {
            _dataContext.Insert(items);
        }

        public void Delete<T>(T item) where T : class, IEntity
        {
            _dataContext.Delete(item);
        }

        public void Delete<T>(List<T> items) where T : class, IEntity
        {
            _dataContext.Delete(items);
        }

        public int SaveChanges()
        {
            int returnCode;
            using (var tran = new TransactionScope(TransactionScopeOption.Required))
            {
                returnCode = _dataContext.SaveChanges();

                tran.Complete();
            }

            return returnCode;
        }

        public IQueryable<T> Items<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            return _dataContext.Items(predicate);
        }
    }
}