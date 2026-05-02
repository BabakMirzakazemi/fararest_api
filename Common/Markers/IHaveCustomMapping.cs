using AutoMapper;

namespace Common.Markers;

public interface IHaveCustomMapping
{
    void CreateMappings(Profile profile);
}
