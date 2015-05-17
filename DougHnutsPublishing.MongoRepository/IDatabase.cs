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
using System.Linq.Expressions;
using System.Threading.Tasks;

// namespaces...
namespace DougHnutsPublishing.MongoRepository
    {
    // public interfaces...
    public interface IDatabase<T> where T: class, new()
        {
        // methods...
        Task<IEnumerable<T>> Add(IEnumerable<T> items);
        Task<T> Add(T item);
        Task<IEnumerable<T>> All();
        Task<IEnumerable<T>> All(SortDefinition<T> sortBy);
        Task<IEnumerable<T>> All(Expression<Func<T, bool>> query, SortDefinition<T> sortBy = null);
        Task<IEnumerable<T>> All(FilterDefinition<T> query, SortDefinition<T> sortBy = null);
        Task<DeleteResult> Delete(Expression<Func<T, bool>> query);
        Task<DeleteResult> Delete(FilterDefinition<T> query);
        Task<DeleteResult> DeleteMany(Expression<Func<T, bool>> query);
        Task<DeleteResult> DeleteMany(FilterDefinition<T> filter);
        Task<ReplaceOneResult> Edit(Expression<Func<T, bool>> query, T item);
        Task<ReplaceOneResult> Edit(FilterDefinition<T> query, T item);
        Task<T> Find(FilterDefinition<T> query);
        Task<T> Find(Expression<Func<T, bool>> query);
        Task<StaticPagedList<T>> Pagination(int page, int total, SortDefinition<T> sortBy, Expression<Func<T, bool>> query = null);
        Task<StaticPagedList<T>> Pagination(int page, int total, FilterDefinition<T> query, SortDefinition<T> sortBy);
        Task<UpdateResult> Update(Expression<Func<T, bool>> query, UpdateDefinition<T> update);
        Task<UpdateResult> Update(FilterDefinition<T> query, UpdateDefinition<T> update);
        Task<UpdateResult> UpdateMany(Expression<Func<T, bool>> query, UpdateDefinition<T> update);
        Task<UpdateResult> UpdateMany(FilterDefinition<T> query, UpdateDefinition<T> update);
    }
    }
