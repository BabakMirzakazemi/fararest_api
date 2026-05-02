using Common.Exceptions;
using Common.Markers;
using Common.Configurations;
using Common.Utilities.Helpers;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Services.Contracts.Repositories;
using System.Globalization;
using System.Linq.Expressions;

namespace Services.Services.Repositories;

public class Repository<TEntity> : IRepository<TEntity>, IScopedDependency
    where TEntity : class, IEntity
{
    private const int DefaultCursorPageSize = 20;
    private const int MaxCursorPageSize = 200;

    protected readonly DbContext DbContext;

    public DbSet<TEntity> Entities { get; }

    public virtual IQueryable<TEntity> Table => Entities;

    public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

    public Repository(DbContext dbContext)
    {
        DbContext = dbContext;
        Entities = DbContext.Set<TEntity>(); // City => Cities
    }

    #region Async Method
    public virtual Task<TEntity?> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
    {
        //return Entities.FindAsync(ids, cancellationToken);
        return Entities.FindAsync(ids, cancellationToken).AsTask();
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
    {
        Assert.NotNull(entity, nameof(entity));
        await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
    {
        Assert.NotNull(entities, nameof(entities));
        await Entities.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
    {
        Assert.NotNull(entity, nameof(entity));
        DbContext.Entry(entity).State = EntityState.Modified;
        Entities.Update(entity);
        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken);
    }

    private List<IProperty> GetPrimaryKey(TEntity entity)
    {
        var entityType = DbContext.Model.FindEntityType(typeof(TEntity))
            ?? throw new LogicException($"Cannot resolve entity type metadata for {typeof(TEntity).Name}.");

        var primaryKey = entityType.FindPrimaryKey()
            ?? throw new LogicException($"Primary key metadata for {typeof(TEntity).Name} is missing.");

        return primaryKey.Properties.ToList();
    }

    public virtual async Task UpdateCustomPropertiesAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true, params string[] properties)
    {
        Assert.NotNull(entity, nameof(entity));
        if (properties == null || properties.Length == 0)
            throw new LogicException("'properties' can't be NULL or Empty");

        if (DbContext.Entry(entity).State != EntityState.Detached)
            DbContext.Entry(entity).State = EntityState.Detached;

        var primaryKey = GetPrimaryKey(entity).First();
        var primaryKeyType = primaryKey.PropertyInfo?.PropertyType ?? typeof(object);
        TEntity? localEntity = null;

        if (primaryKeyType == typeof(Guid) || primaryKeyType == typeof(Guid?))
        {
            localEntity = DbContext.Set<TEntity>().Local.FirstOrDefault(p =>
                TryGetPropertyValueAsGuid(p, primaryKey.Name, out var localGuid) &&
                TryGetPropertyValueAsGuid(entity, primaryKey.Name, out var entityGuid) &&
                localGuid == entityGuid);
        }
        else
        {
            localEntity = DbContext.Set<TEntity>().Local
                .FirstOrDefault(p =>
                    TryGetPropertyValue(p, primaryKey.Name, out var localValue) &&
                    TryGetPropertyValue(entity, primaryKey.Name, out var entityValue) &&
                    localValue is not null &&
                    entityValue is not null &&
                    Equals(
                        Convert.ChangeType(localValue, primaryKeyType, CultureInfo.InvariantCulture),
                        Convert.ChangeType(entityValue, primaryKeyType, CultureInfo.InvariantCulture)));
        }

        if (localEntity != null)
            DbContext.Entry(localEntity).State = EntityState.Detached;

        Entities.Attach(entity);

        var entityProperties = entity.GetType().GetProperties();
        var keyPropertyNames = GetPrimaryKey(entity)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var propertyName in properties)
        {
            if (!entityProperties.Any(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
                throw new MissingFieldException(entity.GetType().Name, propertyName);
            else
            {
                if (keyPropertyNames.Contains(propertyName))
                    continue;

                var selectedProperty = entityProperties
                    .Single(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

                DbContext.Entry(entity)
                    .Property(selectedProperty.Name)
                    .IsModified = true;
            }
        }

        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
    {
        Assert.NotNull(entities, nameof(entities));
        Entities.UpdateRange(entities);
        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
    {
        Assert.NotNull(entity, nameof(entity));
        Entities.Remove(entity);
        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
    {
        Assert.NotNull(entities, nameof(entities));
        Entities.RemoveRange(entities);
        if (saveNow)
            await DbContext.SaveChangesAsync(cancellationToken);
    }

    //my custom method
    public virtual async Task DeleteByIdAsync(object id, CancellationToken cancellationToken, bool saveNow = true)
    {
        Assert.NotNull(id, nameof(id));
        var entity = await GetByIdAsync(cancellationToken, id);

        if (entity == null)
            throw new NotFoundException($"not found {typeof(TEntity).Name} entity to PKey(UserId) : '{id}'");

        await DeleteAsync(entity, cancellationToken, saveNow);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await DbContext.SaveChangesAsync(cancellationToken);
    }
    #endregion

    #region Sync Methods
    public virtual TEntity? GetById(params object[] ids)
    {
        return Entities.Find(ids);
    }

    public virtual void Add(TEntity entity, bool saveNow = true)
    {
        Assert.NotNull(entity, nameof(entity));
        Entities.Add(entity);
        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void AddRange(IEnumerable<TEntity> entities, bool saveNow = true)
    {
        Assert.NotNull(entities, nameof(entities));
        Entities.AddRange(entities);
        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void Update(TEntity entity, bool saveNow = true)
    {
        Assert.NotNull(entity, nameof(entity));
        Entities.Update(entity);
        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true)
    {
        Assert.NotNull(entities, nameof(entities));
        Entities.UpdateRange(entities);
        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void Delete(TEntity entity, bool saveNow = true)
    {
        Assert.NotNull(entity, nameof(entity));
        Entities.Remove(entity);
        if (saveNow)
            DbContext.SaveChanges();
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true)
    {
        Assert.NotNull(entities, nameof(entities));
        Entities.RemoveRange(entities);
        if (saveNow)
            DbContext.SaveChanges();
    }

    //my custom method
    public virtual void DeleteById(object id, CancellationToken cancellationToken, bool saveNow = true)
    {
        Assert.NotNull(id, nameof(id));
        var entity = GetById(id);
        if (entity == null)
            throw new NotFoundException($"not found {typeof(TEntity).Name} entity to PKey(UserId) : '{id}'");

        Delete(entity, saveNow);
    }

    public virtual int SaveChanges()
    {
        return DbContext.SaveChanges();
    }

    #endregion

    #region Attach & Detach
    public virtual void Detach(TEntity entity)
    {
        Assert.NotNull(entity, nameof(entity));
        var entry = DbContext.Entry(entity);
        if (entry != null)
            entry.State = EntityState.Detached;
    }

    public virtual void Attach(TEntity entity)
    {
        Assert.NotNull(entity, nameof(entity));
        if (DbContext.Entry(entity).State == EntityState.Detached)
            Entities.Attach(entity);
    }
    #endregion

    #region Explicit Loading
    public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken)
        where TProperty : class
    {
        Assert.NotNull(entity, nameof(entity));
        Attach(entity);

        var collection = DbContext.Entry(entity).Collection(collectionProperty);
        if (!collection.IsLoaded)
            await collection.LoadAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty)
        where TProperty : class
    {
        Assert.NotNull(entity, nameof(entity));
        Attach(entity);
        var collection = DbContext.Entry(entity).Collection(collectionProperty);
        if (!collection.IsLoaded)
            collection.Load();
    }

    public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> referenceProperty, CancellationToken cancellationToken)
        where TProperty : class
    {
        Assert.NotNull(entity, nameof(entity));
        Attach(entity);
        var reference = DbContext.Entry(entity).Reference(referenceProperty);
        if (!reference.IsLoaded)
            await reference.LoadAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> referenceProperty)
        where TProperty : class
    {
        Assert.NotNull(entity, nameof(entity));
        Attach(entity);
        var reference = DbContext.Entry(entity).Reference(referenceProperty);
        if (!reference.IsLoaded)
            reference.Load();
    }
    #endregion

    #region Select Methods

    /// <summary>
    /// این متد یک موجودیت را با استفاده از شناسه آن پیدا کرده و اجازه نمی دهد که کانتکست انتیتی فریمورک آن را تراک کند-NoTracking
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual TEntity? GetByCondition(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true)
    {
        var entities = asNoTracking ? TableNoTracking : Table;

        var entity = entities.FirstOrDefault(predicate);
        return entity;
    }

    /// <summary>
    /// این متد یک موجودیت را با استفاده از شناسه آن پیدا کرده و اجازه نمی دهد که کانتکست انتیتی فریمورک آن را تراک کند-NoTracking
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual async Task<TEntity?> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, bool asNoTracking = true)
    {
        var entities = asNoTracking ? TableNoTracking : Table;

        var entity = await entities.FirstOrDefaultAsync(predicate, cancellationToken);
        return entity;
    }

    public async Task<ColumnType> GetColumnValueAsync<ColumnType>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, string columnName = "UserId")
    {
        var entityType = typeof(TEntity);
        var propertyInfo = entityType.GetProperty(columnName);
        if (propertyInfo is null)
            return default!;

        var entity = await TableNoTracking.Where(predicate).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        if (entity == null)
            return default!;

        var value = propertyInfo.GetValue(entity, null);
        return value is null ? default! : (ColumnType)value;
    }

    public ColumnType GetColumnValue<ColumnType>(Expression<Func<TEntity, bool>> predicate, string columnName = "UserId")
    {
        Type entityType = typeof(TEntity);
        var propertyInfo = entityType.GetProperty(columnName);
        if (propertyInfo is null)
            return default!;

        var entity = TableNoTracking.Where(predicate).FirstOrDefault();
        if (entity == null)
            return default!;

        var value = propertyInfo.GetValue(entity, null);
        return value is null ? default! : (ColumnType)value;
    }

    public IQueryable<TResult> CreatePage<TResult>(IQueryable<TResult> query, int pageNumber, int pageSize)
    {
        Assert.NotNull(query, nameof(query));
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public async Task<CursorPagingDTO<TEntity>> SelectByCursorAsync<TKey>(
        CursorPagingRequest request,
        Expression<Func<TEntity, TKey>> orderKeySelector,
        CancellationToken cancellationToken,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool descending = true,
        bool asNoTracking = true)
        where TKey : notnull, IComparable<TKey>
    {
        Assert.NotNull(request, nameof(request));
        Assert.NotNull(orderKeySelector, nameof(orderKeySelector));

        var pageSize = NormalizeCursorPageSize(request.PageSize);
        IQueryable<TEntity> query = asNoTracking ? TableNoTracking : Table;

        if (predicate is not null)
            query = query.Where(predicate);

        var hasCursor = !string.IsNullOrWhiteSpace(request.Cursor);
        if (hasCursor)
        {
            if (!TryDecodeCursorValue(request.Cursor, out TKey cursorValue))
                throw new BadRequestException(ApplicationMessages.InvalidPaginationCursor);

            var seekPredicate = BuildSeekPredicate(orderKeySelector, cursorValue, descending);
            query = query.Where(seekPredicate);
        }

        query = descending ? query.OrderByDescending(orderKeySelector) : query.OrderBy(orderKeySelector);

        // Fetch one extra row to determine whether there is a next page.
        var fetchedRows = await query.Take(pageSize + 1).ToListAsync(cancellationToken).ConfigureAwait(false);
        var hasNext = fetchedRows.Count > pageSize;
        var pageData = hasNext ? fetchedRows.Take(pageSize).ToList() : fetchedRows;

        string? nextCursor = null;
        if (hasNext && pageData.Count > 0)
        {
            var lastKey = orderKeySelector.Compile().Invoke(pageData[^1]);
            nextCursor = EncodeCursorValue(lastKey);
        }

        return new CursorPagingDTO<TEntity>(
            pageData,
            pageSize,
            hasNext,
            nextCursor,
            string.IsNullOrWhiteSpace(request.Cursor) ? null : request.Cursor);
    }

    public async Task<List<TEntity>> SelectByAsync(Expression<Func<TEntity, bool>> predicate,
                                                   CancellationToken cancellationToken,
                                                   bool asNoTracking = true,
                                                   int? pageNumber = null,
                                                   int? pageSize = null,
                                                   params Expression<Func<TEntity, object>>[] navigationPropertyPaths)
    {
        IQueryable<TEntity> query = asNoTracking ? TableNoTracking : Table;

        foreach (var navigationProperty in navigationPropertyPaths)
            query = query.Include(navigationProperty);

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            return await CreatePage(query.Where(predicate), pageNumber.Value, pageSize.Value).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        else
            return await query.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public IQueryable<TEntity> SelectBy(Expression<Func<TEntity, bool>> predicate,
                                        bool asNoTracking = true,
                                        int? pageNumber = null,
                                        int? pageSize = null,
                                        params Expression<Func<TEntity, object>>[] navigationPropertyPaths)
    {
        IQueryable<TEntity> query = asNoTracking ? TableNoTracking : Table;

        foreach (var navigationProperty in navigationPropertyPaths)
            query = query.Include(navigationProperty);

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            return CreatePage(query.Where(predicate), pageNumber.Value, pageSize.Value);
        }
        else
            return query.Where(predicate);
    }

    public async Task<List<TResult>> SelectByAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                            CancellationToken cancellationToken,
                                                            bool asNoTracking = true,
                                                            int? pageNumber = null,
                                                            int? pageSize = null)
        where TResult : class
    {
        IQueryable<TEntity> query = asNoTracking ? TableNoTracking : Table;

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            return await CreatePage(query.Select(selector), pageNumber.Value, pageSize.Value).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        else
            return await query.Select(selector).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public IQueryable<TResult> SelectBy<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                            bool asNoTracking = true,
                                                            int? pageNumber = null,
                                                            int? pageSize = null)
        where TResult : class
    {
        IQueryable<TEntity> query = asNoTracking ? TableNoTracking : Table;

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            return CreatePage(query.Select(selector), pageNumber.Value, pageSize.Value);
        }
        else
            return query.Select(selector);
    }

    public async Task<List<TResult>> SelectByAsync<TResult>(Expression<Func<TEntity, bool>> predicate,
                                                            Expression<Func<TEntity, TResult>> selector,
                                                            CancellationToken cancellationToken,
                                                            bool asNoTracking = true,
                                                            int? pageNumber = null,
                                                            int? pageSize = null)
        where TResult : class
    {
        IQueryable<TEntity> query = asNoTracking ? TableNoTracking : Table;

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            return await CreatePage(query.Where(predicate).Select(selector), pageNumber.Value, pageSize.Value)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        else
            return await query.Where(predicate).Select(selector).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public IQueryable<TResult> SelectBy<TResult>(Expression<Func<TEntity, bool>> predicate,
                                                 Expression<Func<TEntity, TResult>> selector,
                                                 CancellationToken cancellationToken,
                                                 bool asNoTracking = true,
                                                 int? pageNumber = null,
                                                 int? pageSize = null) where TResult : class
    {
        IQueryable<TEntity> query = asNoTracking ? TableNoTracking : Table;

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            return CreatePage(query.Where(predicate).Select(selector), pageNumber.Value, pageSize.Value);
        }
        else
            return query.Where(predicate).Select(selector);
    }

    public virtual async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, bool asNoTracking = true)
    {
        var query = asNoTracking ? TableNoTracking : Table;
        return await query.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> IsNotDuplicateAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, bool asNoTracking = true)
    {
        var query = asNoTracking ? TableNoTracking : Table;
        return !await query.AnyAsync(predicate, cancellationToken);
    }

    public virtual bool IsExist(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true)
    {
        var query = asNoTracking ? TableNoTracking : Table;
        return query.Any(predicate);
    }
    #endregion

    #region Count Methods

    public async Task<int> CountByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        => await TableNoTracking.CountAsync(predicate, cancellationToken).ConfigureAwait(false);

    #endregion

    private static bool TryGetPropertyValue(object source, string propertyName, out object? value)
    {
        value = null;
        var propertyInfo = source.GetType().GetProperty(propertyName);
        if (propertyInfo is null)
            return false;

        value = propertyInfo.GetValue(source, null);
        return true;
    }

    private static bool TryGetPropertyValueAsGuid(object source, string propertyName, out Guid value)
    {
        value = Guid.Empty;
        if (!TryGetPropertyValue(source, propertyName, out var rawValue) || rawValue is null)
            return false;

        if (rawValue is Guid guid)
        {
            value = guid;
            return true;
        }

        return Guid.TryParse(rawValue.ToString(), out value);
    }

    private static int NormalizeCursorPageSize(int requestedPageSize)
    {
        if (requestedPageSize <= 0)
            return DefaultCursorPageSize;

        return Math.Min(requestedPageSize, MaxCursorPageSize);
    }

    private static Expression<Func<TEntity, bool>> BuildSeekPredicate<TKey>(
        Expression<Func<TEntity, TKey>> orderKeySelector,
        TKey cursorValue,
        bool descending)
        where TKey : notnull, IComparable<TKey>
    {
        var parameter = orderKeySelector.Parameters[0];
        var keyExpression = orderKeySelector.Body;
        var cursorExpression = Expression.Constant(cursorValue, typeof(TKey));

        var compareToMethod = typeof(TKey).GetMethod(nameof(IComparable<TKey>.CompareTo), new[] { typeof(TKey) });
        if (compareToMethod is null)
            throw new LogicException($"Type '{typeof(TKey).Name}' does not support CompareTo for cursor pagination.");

        var compareCall = Expression.Call(keyExpression, compareToMethod, cursorExpression);
        var zero = Expression.Constant(0);
        var comparison = descending
            ? Expression.LessThan(compareCall, zero)
            : Expression.GreaterThan(compareCall, zero);

        return Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
    }

    private static string EncodeCursorValue<TKey>(TKey value)
    {
        string payload = value switch
        {
            DateTime dateTime => dateTime.Ticks.ToString(CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffset => dateTimeOffset.Ticks.ToString(CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => value?.ToString() ?? string.Empty
        };

        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payload));
    }

    private static bool TryDecodeCursorValue<TKey>(string? cursorToken, out TKey value)
        where TKey : notnull
    {
        value = default!;
        if (string.IsNullOrWhiteSpace(cursorToken))
            return false;

        string payload;
        try
        {
            var decodedBytes = Convert.FromBase64String(cursorToken.Trim());
            payload = System.Text.Encoding.UTF8.GetString(decodedBytes);
        }
        catch (FormatException)
        {
            return false;
        }

        var targetType = typeof(TKey);
        object? parsed = null;

        if (targetType == typeof(Guid) && Guid.TryParse(payload, out var guid))
            parsed = guid;
        else if (targetType == typeof(int) && int.TryParse(payload, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
            parsed = intValue;
        else if (targetType == typeof(long) && long.TryParse(payload, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
            parsed = longValue;
        else if (targetType == typeof(short) && short.TryParse(payload, NumberStyles.Integer, CultureInfo.InvariantCulture, out var shortValue))
            parsed = shortValue;
        else if (targetType == typeof(byte) && byte.TryParse(payload, NumberStyles.Integer, CultureInfo.InvariantCulture, out var byteValue))
            parsed = byteValue;
        else if (targetType == typeof(decimal) && decimal.TryParse(payload, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
            parsed = decimalValue;
        else if (targetType == typeof(double) && double.TryParse(payload, NumberStyles.Number, CultureInfo.InvariantCulture, out var doubleValue))
            parsed = doubleValue;
        else if (targetType == typeof(float) && float.TryParse(payload, NumberStyles.Number, CultureInfo.InvariantCulture, out var floatValue))
            parsed = floatValue;
        else if (targetType == typeof(DateTime) && long.TryParse(payload, NumberStyles.Integer, CultureInfo.InvariantCulture, out var dateTicks))
        {
            try
            {
                parsed = new DateTime(dateTicks);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
        else if (targetType == typeof(DateTimeOffset) && long.TryParse(payload, NumberStyles.Integer, CultureInfo.InvariantCulture, out var dateOffsetTicks))
        {
            try
            {
                parsed = new DateTimeOffset(dateOffsetTicks, TimeSpan.Zero);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
        else if (targetType == typeof(string))
            parsed = payload;

        if (parsed is null)
            return false;

        value = (TKey)parsed;
        return true;
    }
}
