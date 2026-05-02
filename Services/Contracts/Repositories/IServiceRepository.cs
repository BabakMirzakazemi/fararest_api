using Entities.Common;
using Services.DTOs.Common;

namespace Services.Contracts.Repositories;

public interface IServiceRepository<TDto, TSelectDto, TEntity, TKey>
     where TDto : BaseDTO<TDto, TEntity, TKey>, new()
     where TSelectDto : BaseDTO<TSelectDto, TEntity, TKey>, new()
     where TEntity : BaseEntity<TKey>, new()
{
    Task<List<TSelectDto>> Get(CancellationToken cancellationToken = default);

    Task<PagingDTO<TSelectDto>> Select(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<CursorPagingDTO<TSelectDto>> SelectByCursor<TOrderKey>(
        CursorPagingRequest request,
        Expression<Func<TEntity, TOrderKey>> orderKeySelector,
        CancellationToken cancellationToken = default,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool descending = true)
        where TOrderKey : notnull, IComparable<TOrderKey>;

    Task<TSelectDto> Get(TKey id, CancellationToken cancellationToken = default);

    Task<TSelectDto> Create(TDto dto, CancellationToken cancellationToken = default);

    Task<TSelectDto> Update(TKey id, TDto dto, CancellationToken cancellationToken = default);

    Task<TSelectDto> Update(TDto dto, CancellationToken cancellationToken = default);

    Task<TSelectDto> UpdateCustomProperties(TDto dto, CancellationToken cancellationToken = default);

    Task<TSelectDto> UpdateProperty(TKey id, PropertyValueDTO dto, CancellationToken cancellationToken = default);

    Task Delete(TKey id, CancellationToken cancellationToken = default);
}

public interface IServiceRepository<TDto, TEntity> : IServiceRepository<TDto, TDto, TEntity, int>
    where TDto : BaseDTO<TDto, TEntity, int>, new()
     where TEntity : BaseEntity<int>, new()
{

}

public interface IServiceRepository<TDto, TSelectDto, TEntity> : IServiceRepository<TDto, TSelectDto, TEntity, int>
      where TDto : BaseDTO<TDto, TEntity, int>, new()
     where TSelectDto : BaseDTO<TSelectDto, TEntity, int>, new()
     where TEntity : BaseEntity<int>, new()
{

}
