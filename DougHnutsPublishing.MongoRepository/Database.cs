#region Copyright Notice

/**************************************************
 * Copyright 2015 DougHnuts Publishing LLC.
 * All rights Reserved
 **************************************************/

#endregion

using MongoDB.Bson;
using MongoDB.Driver;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

// namespaces...
namespace DougHnutsPublishing.MongoRepository
{
    // public classes...
    public class Database<T> : IDatabase<T>, IDisposable where T : class, new()
    {
        // private fields...
        private string _collectionName;
        private static string _connectionString = ConfigurationManager.ConnectionStrings["dhpdb"].ConnectionString;
        private MongoClient _client = new MongoClient(_connectionString);
        private IMongoDatabase _db;
        private StaticPagedList<T> _staticPagedList;
        private bool _isDisposed;

        // public constructors...
        public Database(string collectionName)
        {
            _collectionName = collectionName;
            _db = _client.GetDatabase(MongoUrl.Create(_connectionString).DatabaseName);
        }

        // protected properties...
        protected IMongoCollection<T> _collection
        {
            get { return _db.GetCollection<T>(_collectionName); }
            set
            {
                if (_collection == value) return;
                _collection = value;
            }
        }

        // public methods...
        public async Task<IEnumerable<T>> Add(IEnumerable<T> items)
        {
            await _collection.InsertManyAsync((IEnumerable<T>)items, null, new CancellationToken());
            return items;
        }
        public async Task<T> Add(T item)
        {
            await _collection.InsertOneAsync(item, new CancellationToken());
            return item;
        }
        public async Task<IEnumerable<T>> All()
        {
            var findFluent = IMongoCollectionExtensions.Find<T>(_collection, new BsonDocument(), null);
            return await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, new CancellationToken());
        }
        public async Task<IEnumerable<T>> All(SortDefinition<T> sortBy)
        {
            var findFluent = IMongoCollectionExtensions.Find<T>(_collection, new BsonDocument(), null).Sort(sortBy);
            return await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, new CancellationToken());
        }
        public async Task<IEnumerable<T>> All(Expression<Func<T, bool>> query, SortDefinition<T> sortBy = null)
        {
            IEnumerable<T> listAsync;
            var findFluent = IMongoCollectionExtensions.Find<T>(_collection, query, null);
            if (sortBy == null)
            {
                listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, new CancellationToken());
            }
            else
            {
                var findFluentSorted = findFluent.Sort(sortBy);
                listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluentSorted, new CancellationToken());
            }
            return listAsync;
        }
        public async Task<IEnumerable<T>> All(FilterDefinition<T> query, SortDefinition<T> sortBy = null)
        {
            IEnumerable<T> listAsync;
            var findFluent = IMongoCollectionExtensions.Find<T>(_collection, query, null);
            if (sortBy == null)
            {
                listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, new CancellationToken());
            }
            else
            {
                var findFluentSorted = findFluent.Sort(sortBy);
                listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluentSorted, new CancellationToken());
            }
            return listAsync;
        }
        public async Task<DeleteResult> Delete(Expression<Func<T, bool>> query)
        {
            return await _collection.DeleteOneAsync(query, new CancellationToken());
        }
        public async Task<DeleteResult> Delete(FilterDefinition<T> query)
        {
            return await _collection.DeleteOneAsync(query, new CancellationToken());
        }
        public async Task<DeleteResult> DeleteMany(Expression<Func<T, bool>> query)
        {
            return await _collection.DeleteManyAsync(query, new CancellationToken());
        }

        public async Task<DeleteResult> DeleteMany(FilterDefinition<T> filter)
        {
            return await _collection.DeleteManyAsync(filter, new CancellationToken());
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<ReplaceOneResult> Edit(Expression<Func<T, bool>> query, T item)
        {
            FilterDefinition<T> querydef = query;
            return await _collection.ReplaceOneAsync(querydef, item, null, new CancellationToken());
        }
        public async Task<ReplaceOneResult> Edit(FilterDefinition<T> query, T item)
        {
            return await _collection.ReplaceOneAsync(query, item, null, new CancellationToken());
        }
        public async Task<T> Find(FilterDefinition<T> query)
        {
            var findFluent = IMongoCollectionExtensions.Find<T>(_collection, query, null);
            return await IFindFluentExtensions.FirstOrDefaultAsync<T, T>(findFluent, new CancellationToken());
        }
        public async Task<T> Find(Expression<Func<T, bool>> query)
        {
            var findFluent = IMongoCollectionExtensions.Find<T>(_collection, query, null);
            return await IFindFluentExtensions.FirstOrDefaultAsync<T, T>(findFluent, new CancellationToken());
        }
        public async Task<StaticPagedList<T>> Pagination(int page, int total, SortDefinition<T> sortBy, Expression<Func<T, bool>> query = null)
        {
            long num = 0;
            IEnumerable<T> listAsync = null;
            if (query != null)
            {
                FilterDefinition<T> querydef = query;
                num = await _collection.CountAsync(querydef, null, new CancellationToken());
                var findFluent = IMongoCollectionExtensions.Find<T>(_collection, querydef, null).Skip(new int?((page - 1) * total)).Limit(new int?(total)).Sort(sortBy);
                listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, new CancellationToken());
                _staticPagedList = new StaticPagedList<T>(listAsync, page, total, (int)num);
            }
            else
            {
                FilterDefinition<T> bsonDocument = new BsonDocument();
                num = await _collection.CountAsync(bsonDocument, null, new CancellationToken());
                var findFluentSorted = IMongoCollectionExtensions.Find<T>(_collection, new BsonDocument(), null).Skip(new int?((page - 1) * total)).Limit(new int?(total)).Sort(sortBy);
                listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluentSorted, new CancellationToken());
                _staticPagedList = new StaticPagedList<T>(listAsync, page, total, (int)num);
            }
            return _staticPagedList;
        }
        public async Task<StaticPagedList<T>> Pagination(int page, int total, FilterDefinition<T> query, SortDefinition<T> sortBy)
        {
            IEnumerable<T> listAsync = null;
            var num = await _collection.CountAsync(query, null, new CancellationToken());
            var findFluent = IMongoCollectionExtensions.Find<T>(_collection, query, null).Skip(new int?((page - 1) * total)).Limit(new int?(total)).Sort(sortBy);
            listAsync = await IAsyncCursorSourceExtensions.ToListAsync<T>(findFluent, new CancellationToken());
            return new StaticPagedList<T>(listAsync, page, total, (int)num);
        }
        public async Task<UpdateResult> Update(Expression<Func<T, bool>> query, UpdateDefinition<T> update)
        {
            FilterDefinition<T> querydef = query;
            return await _collection.UpdateOneAsync(querydef, update, null, new CancellationToken());
        }
        public async Task<UpdateResult> Update(FilterDefinition<T> query, UpdateDefinition<T> update)
        {
            return await _collection.UpdateOneAsync(query, update, null, new CancellationToken());
        }
        public async Task<UpdateResult> UpdateMany(Expression<Func<T, bool>> query, UpdateDefinition<T> update)
        {
            FilterDefinition<T> querydef = query;
            return await _collection.UpdateOneAsync(querydef, update, null, new CancellationToken());
        }
        public async Task<UpdateResult> UpdateMany(FilterDefinition<T> query, UpdateDefinition<T> update)
        {
            return await _collection.UpdateManyAsync(query, update, null, new CancellationToken());
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _client = null;
                    _db = null;
                }
                _isDisposed = true;
            }
        }
    }
}
