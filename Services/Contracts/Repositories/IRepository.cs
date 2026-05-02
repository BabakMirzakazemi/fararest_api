using Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Services.Contracts.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    DbSet<TEntity> Entities { get; }
    IQueryable<TEntity> Table { get; }
    IQueryable<TEntity> TableNoTracking { get; }
    void Add(TEntity entity, bool saveNow = true);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
    void AddRange(IEnumerable<TEntity> entities, bool saveNow = true);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
    void Attach(TEntity entity);
    void Delete(TEntity entity, bool saveNow = true);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
    void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
    Task DeleteByIdAsync(object id, CancellationToken cancellationToken, bool saveNow = true);
    void DeleteById(object id, CancellationToken cancellationToken, bool saveNow = true);
    void Detach(TEntity entity);
    TEntity? GetById(params object[] ids);
    Task<TEntity?> GetByIdAsync(CancellationToken cancellationToken, params object[] ids);
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty) where TProperty : class;
    Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken) where TProperty : class;
    void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> referenceProperty) where TProperty : class;
    Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> referenceProperty, CancellationToken cancellationToken) where TProperty : class;
    void Update(TEntity entity, bool saveNow = true);
    //Task UpdateDisconnectedAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
    Task UpdateCustomPropertiesAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true, params string[] properties);
    void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);

    #region Select Methods

    /// <summary>
    /// این متد یک موجودیت را با استفاده از شناسه آن پیدا کرده و اجازه نمی دهد که کانتکست انتیتی فریمورک آن را تراک کند-NoTracking
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="includePathEnities"></param>
    /// <returns></returns>
    TEntity? GetByCondition(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true);

    /// <summary>
    /// این متد یک موجودیت را با استفاده از شناسه آن پیدا کرده و اجازه نمی دهد که کانتکست انتیتی فریمورک آن را تراک کند-NoTracking
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="includePathEnities"></param>
    /// <returns></returns>
    Task<TEntity?> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, bool asNoTracking = true);

    Task<ColumnType> GetColumnValueAsync<ColumnType>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, string columnName = "UserId");

    ColumnType GetColumnValue<ColumnType>(Expression<Func<TEntity, bool>> predicate, string columnName = "UserId");

    IQueryable<TResult> CreatePage<TResult>(IQueryable<TResult> query, int pageNumber, int pageSize);

    Task<List<TEntity>> SelectByAsync(Expression<Func<TEntity, bool>> predicate,
                                                             CancellationToken cancellationToken,
                                                             bool asNoTracking = true,
                                                             int? pageNumber = null,
                                                             int? pageSize = null,
                                                             params Expression<Func<TEntity, object>>[] navigationPropertyPaths);

    IQueryable<TEntity> SelectBy(Expression<Func<TEntity, bool>> predicate,
                                                   bool asNoTracking = true,
                                                   int? pageNumber = null,
                                                   int? pageSize = null,
                                                   params Expression<Func<TEntity, object>>[] navigationPropertyPaths);

    Task<List<TResult>> SelectByAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                        CancellationToken cancellationToken,
                                                        bool asNoTracking = true,
                                                        int? pageNumber = null,
                                                        int? pageSize = null)
        where TResult : class;

    IQueryable<TResult> SelectBy<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                            bool asNoTracking = true,
                                                            int? pageNumber = null,
                                                            int? pageSize = null)
        where TResult : class;

    Task<List<TResult>> SelectByAsync<TResult>(Expression<Func<TEntity, bool>> predicate,
                                                           Expression<Func<TEntity, TResult>> selector,
                                                           CancellationToken cancellationToken,
                                                           bool asNoTracking = true,
                                                           int? pageNumber = null,
                                                           int? pageSize = null)
                                                           where TResult : class;

    IQueryable<TResult> SelectBy<TResult>(Expression<Func<TEntity, bool>> predicate,
                                                          Expression<Func<TEntity, TResult>> selector,
                                                          CancellationToken cancellationToken,
                                                          bool asNoTracking = true,
                                                          int? pageNumber = null,
                                                          int? pageSize = null) where TResult : class;

    Task<CursorPagingDTO<TEntity>> SelectByCursorAsync<TKey>(
        CursorPagingRequest request,
        Expression<Func<TEntity, TKey>> orderKeySelector,
        CancellationToken cancellationToken,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool descending = true,
        bool asNoTracking = true)
        where TKey : notnull, IComparable<TKey>;

    Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, bool asNoTracking = true);

    Task<bool> IsNotDuplicateAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, bool asNoTracking = true);

    bool IsExist(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true);
    #endregion

    #region Count Methods
    Task<int> CountByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    #endregion
}
