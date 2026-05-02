using Autofac;
using Common.Configurations;
using Common.Markers;
using Data;
using Entities.Common;
using Services.Contracts.Authentications;
using Services.Contracts.Repositories;
using Services.Services.Authentications;
using Services.Services.Repositories;
using Module = Autofac.Module;

namespace WebFramework.Configuration.IoCConfigurations;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(ServiceRepository<,,,>)).As(typeof(IServiceRepository<,,,>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(ServiceRepository<,,>)).As(typeof(IServiceRepository<,,>)).InstancePerLifetimeScope();
        containerBuilder.RegisterGeneric(typeof(ServiceRepository<,>)).As(typeof(IServiceRepository<,>)).InstancePerLifetimeScope();

        var commonAssembly = typeof(SiteSettings).Assembly;
        var entitiesAssembly = typeof(IEntity).Assembly;
        var dataAssembly = typeof(ApplicationDbContext).Assembly;
        var servicesAssembly = typeof(JwtService).Assembly;

        containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
            .AssignableTo<IScopedDependency>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
            .AssignableTo<ITransientDependency>()
            .AsImplementedInterfaces()
            .InstancePerDependency();

        containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
            .AssignableTo<ISingletonDependency>()
            .AsImplementedInterfaces()
            .SingleInstance();

        base.Load(containerBuilder);
    }
}
