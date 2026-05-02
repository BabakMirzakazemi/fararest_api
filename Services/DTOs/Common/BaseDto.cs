using AutoMapper;
using Common.Markers;
using Entities.Common;

namespace Services.DTOs.Common;


public abstract class BaseDTO<TDto, TEntity, TKey> : IHaveCustomMapping, IBaseDTO
  where TDto : class, new()
  where TEntity : BaseEntity<TKey>, new()
{
    public TKey? Id { get; set; }

    /// <summary>
    /// add property name to list to update just column in database
    /// </summary>
    public List<string>? UpdateProperties { get; set; }

    public TEntity ToEntity(IMapper mapper)
    {
        return mapper.Map<TEntity>(CastToDerivedClass(mapper, this));
        //return mapper.Map<TEntity>(this);
    }

    public TEntity ToEntity(IMapper mapper, TEntity entity)
    {
        return mapper.Map(CastToDerivedClass(mapper, this), entity);
    }

    public static TDto FromEntity(IMapper mapper, TEntity model)
    {
        return mapper.Map<TDto>(model);
    }

    protected TDto CastToDerivedClass(IMapper mapper, BaseDTO<TDto, TEntity, TKey> baseInstance)
    {
        return mapper.Map<TDto>(baseInstance);
    }




    public virtual void CreateMappings(Profile profile)
    {
        var mappingExpression = profile.CreateMap<TDto, TEntity>();

        var dtoType = typeof(TDto);
        var entityType = typeof(TEntity);
        //Ignore any property of source/entity (like Post.Author) that dose not contains in destination(DTO)
        foreach (var property in entityType.GetProperties())
        {
            if (dtoType.GetProperty(property.Name) == null)
                mappingExpression.ForMember(property.Name, opt => opt.Ignore());
        }

        CustomMappings(mappingExpression);
        CustomMappings(mappingExpression.ReverseMap());
    }

    public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mappingExpression)
    {
    }

    public virtual void CustomMappings(IMappingExpression<TDto, TEntity> mappingExpression)
    {
    }
}

public abstract class BaseDTO<TDto, TEntity> : BaseDTO<TDto, TEntity, int>
    where TDto : class, new()
    where TEntity : BaseEntity<int>, new()
{

}
public class BoolResultDto
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; } = string.Empty;
}
