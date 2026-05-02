using AutoMapper;
using Common.Markers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WebFramework.CustomMapping;

public static class AutoMapperConfiguration
{

    public static IServiceCollection InitializeAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        //With AutoMapper Instance, you need to call AddAutoMapper services and pass assemblies that contains automapper Profile class
        //services.AddAutoMapper(assembly1, assembly2, assembly3);
        //See http://docs.automapper.org/en/stable/Configuration.html
        //And https://code-maze.com/automapper-net-core/

        services.AddAutoMapper(config =>
        {
            config.AddCustomMappingProfile();
        }, assemblies);

        return services;
    }

    public static void AddCustomMappingProfile(this IMapperConfigurationExpression config)
    {
        //config.AddCustomMappingProfile(Assembly.GetEntryAssembly());
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        config.AddCustomMappingProfile(assemblies);
    }

    public static void AddCustomMappingProfile(this IMapperConfigurationExpression config, params Assembly[] assemblies)
    {
        var allTypes = assemblies.SelectMany(a => a.ExportedTypes);
        //var allTypes = assemblies.SelectMany(assembly => assembly.GetTypes());
        var list = allTypes.Where(type => type.IsClass && !type.IsAbstract &&
            type.GetInterfaces().Contains(typeof(IHaveCustomMapping)))
            .Select(type => Activator.CreateInstance(type) as IHaveCustomMapping)
            .Where(mapping => mapping is not null)
            .Cast<IHaveCustomMapping>();

        var profile = new CustomMappingProfile(list);
        
        config.AddProfile(profile);
    }
}
