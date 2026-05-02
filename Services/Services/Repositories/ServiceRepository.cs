using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Common.Markers;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Services.Contracts.Repositories;
using Services.DTOs.Common;
using System.Globalization;

namespace Services.Services.Repositories;

public class ServiceRepository<TDto, TSelectDto, TEntity, TKey> : IServiceRepository<TDto, TSelectDto, TEntity, TKey>, IScopedDependency
     where TDto : BaseDTO<TDto, TEntity, TKey>, new()
     where TSelectDto : BaseDTO<TSelectDto, TEntity, TKey>, new()
     where TEntity : BaseEntity<TKey>, new()
{
    private readonly IMapper mapper;
    private readonly IRepository<TEntity> repository;
    public ServiceRepository(IMapper mapper, IRepository<TEntity> repository)
    {
        this.mapper = mapper;
        this.repository = repository;
    }

    public virtual async Task<List<TSelectDto>> Get(CancellationToken cancellationToken = default)
    {
        var list = await repository.TableNoTracking.ProjectTo<TSelectDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return list;
    }

    public virtual async Task<PagingDTO<TSelectDto>> Select(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0)
            throw new BadRequestException($"'{nameof(pageNumber)}' must be greater than or equal to 1, {nameof(pageNumber)} : {pageNumber}");

        if (pageSize <= 0)
            throw new BadRequestException($"'{nameof(pageSize)}' must be greater than or equal to 1, {nameof(pageSize)} : {pageSize}");

        var data = await repository.TableNoTracking
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ProjectTo<TSelectDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var totalCount = await repository.TableNoTracking.CountAsync(cancellationToken);
        var pagingRequest = new PagingRequest
        {
            PageIndex = pageNumber,
            PageSize = pageSize,
        };

        return new PagingDTO<TSelectDto>(data, pagingRequest, totalCount);
    }

    public virtual async Task<CursorPagingDTO<TSelectDto>> SelectByCursor<TOrderKey>(
        CursorPagingRequest request,
        Expression<Func<TEntity, TOrderKey>> orderKeySelector,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool descending = true)
        where TOrderKey : notnull, IComparable<TOrderKey>
    {
        var page = await repository.SelectByCursorAsync(
            request,
            orderKeySelector,
            cancellationToken,
            predicate,
            descending,
            asNoTracking: true);

        var mappedData = mapper.Map<List<TSelectDto>>(page.Data);
        if (mappedData is null)
            throw new LogicException($"Unable to map {typeof(TEntity).Name} list to {typeof(TSelectDto).Name} list.");

        return new CursorPagingDTO<TSelectDto>(
            mappedData,
            page.PageSize,
            page.HasNext,
            page.NextCursor,
            page.CurrentCursor);
    }

    public virtual async Task<TSelectDto> Get(TKey id, CancellationToken cancellationToken = default)
    {
        var model = await repository.GetByIdAsync(cancellationToken, id!);
        if (model is null)
            throw new NotFoundException($"not found {typeof(TEntity).Name} entity to PKey(UserId) : '{id}'");

        return MapToSelectDto(model);
    }

    public virtual async Task<TSelectDto> Create(TDto dto, CancellationToken cancellationToken = default)
    {
        var model = dto.ToEntity(mapper);

        await repository.AddAsync(model, cancellationToken);

        return MapToSelectDto(model);
    }

    public virtual async Task<TSelectDto> Update(TKey id, TDto dto, CancellationToken cancellationToken = default)
    {
        dto.Id = id;
        var model = await repository.GetByIdAsync(cancellationToken, id!);

        if (model == null)
            throw new NotFoundException($"not found {typeof(TEntity).Name} entity to PKey(UserId) : '{dto.Id}'");

        model = dto.ToEntity(mapper, model);

        await repository.UpdateAsync(model, cancellationToken);

        return MapToSelectDto(model);
    }

    public virtual async Task<TSelectDto> Update(TDto dto, CancellationToken cancellationToken = default)
    {
        var key = RequireKey(dto.Id, nameof(dto.Id));
        var model = await repository.GetByIdAsync(cancellationToken, key);

        if (model == null)
            throw new NotFoundException($"not found {typeof(TEntity).Name} entity to PKey(UserId) : '{dto.Id}'");

        model = dto.ToEntity(mapper, model);

        await repository.UpdateAsync(model, cancellationToken);

        return MapToSelectDto(model);
    }

    public async Task<TSelectDto> UpdateCustomProperties(TDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new NotFoundException("'dto' can not NULL");

        if (dto.UpdateProperties == null || !dto.UpdateProperties.Any())
            throw new NotFoundException("'dto.UpdateProperties' can't be NULL or Empty");

        var key = RequireKey(dto.Id, nameof(dto.Id));
        var model = await repository.GetByIdAsync(cancellationToken, key);

        if (model == null)
            throw new NotFoundException($"not found {typeof(TEntity).Name} entity to PKey(UserId) : '{dto.Id}'");

        model = dto.ToEntity(mapper, model);

        await repository.UpdateCustomPropertiesAsync(model, cancellationToken, true, dto.UpdateProperties.ToArray());

        return MapToSelectDto(model);
    }

    public virtual async Task<TSelectDto> UpdateProperty(TKey id, PropertyValueDTO dto, CancellationToken cancellationToken = default)
    {
        var entityType = typeof(TEntity);
        var property = entityType.GetProperty(dto.PropertyName);
        if (property == null)
            throw new NotFoundException($"property {dto.PropertyName} not gound in entity : {typeof(TEntity).Name}");

        var model = await repository.GetByIdAsync(cancellationToken, id!);

        if (model == null)
            throw new NotFoundException($"not found {typeof(TEntity).Name} entity to PKey(UserId) : '{id}'");

        if (dto.PropertyValue != null)
        {
            if (property.PropertyType == typeof(Guid))
            {
                property.SetValue(model, Guid.Parse(dto.PropertyValue.ToString() ?? string.Empty));
            }
            else if (property.PropertyType == typeof(Guid?))
            {
                property.SetValue(model, Guid.Parse(dto.PropertyValue.ToString() ?? string.Empty));
            }
            else
            {
                var value = Convert.ChangeType(dto.PropertyValue, property.PropertyType, CultureInfo.InvariantCulture);
                property.SetValue(model, value);
            }
        }
        else
        {
            //todo check property support null value
            var propertyType = property.PropertyType;
            bool canBeNull = !propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) != null;
            if (canBeNull)
                property.SetValue(model, null);
            else
                throw new BadRequestException($"property {dto.PropertyName} can not be set to null in entity : {typeof(TEntity).Name}");
        }

        await repository.UpdateAsync(model, cancellationToken);

        return MapToSelectDto(model);
    }

    public virtual async Task Delete(TKey id, CancellationToken cancellationToken = default)
    {
        //var model = await repository.GetByIdAsync(cancellationToken, id);

        //await repository.DeleteAsync(model, cancellationToken);

        //OR
        await repository.DeleteByIdAsync(id!, cancellationToken);
    }

    private static object RequireKey(TKey? key, string keyName)
    {
        if (key is null)
            throw new BadRequestException($"'{keyName}' can't be null.");

        return key;
    }

    private TSelectDto MapToSelectDto(TEntity model)
    {
        var dto = mapper.Map<TSelectDto>(model);
        return dto ?? throw new LogicException($"Unable to map {typeof(TEntity).Name} to {typeof(TSelectDto).Name}.");
    }
}

public class ServiceRepository<TDto, TEntity> : ServiceRepository<TDto, TDto, TEntity, int>, IServiceRepository<TDto, TEntity>
  where TDto : BaseDTO<TDto, TEntity, int>, new()
  where TEntity : BaseEntity<int>, new()
{
    public ServiceRepository(IMapper mapper, IRepository<TEntity> repository) : base(mapper, repository)
    {
    }
}

public class ServiceRepository<TDto, TSelectDto, TEntity> : ServiceRepository<TDto, TSelectDto, TEntity, int>, IServiceRepository<TDto, TSelectDto, TEntity>
    where TDto : BaseDTO<TDto, TEntity, int>, new()
    where TSelectDto : BaseDTO<TSelectDto, TEntity, int>, new()
    where TEntity : BaseEntity<int>, new()
{
    public ServiceRepository(IMapper mapper, IRepository<TEntity> repository) : base(mapper, repository)
    {
    }
}
