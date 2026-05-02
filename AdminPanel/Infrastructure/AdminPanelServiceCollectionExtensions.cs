using Common.Markers;
using Data;
using Entities.Users;
using AutoMapper;
using Common.Configurations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Contracts.Caching;
using Services.Contracts.Notifiers;
using Services.Contracts.Repositories;
using Services.DTOs.Admins;
using Services.DTOs.Users;
using Services.Services.Notifiers;
using Services.Services.Repositories;
using System.Reflection;

namespace AdminPanel.Infrastructure;

public static class AdminPanelServiceCollectionExtensions
{
    public static IServiceCollection AddAdminPanelInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var securitySettings = configuration.GetSection(AdminPanelSecuritySettings.SectionName).Get<AdminPanelSecuritySettings>() ?? new AdminPanelSecuritySettings();
        services.Configure<AdminPanelSecuritySettings>(configuration.GetSection(AdminPanelSecuritySettings.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }

            var dbType = configuration.GetValue<string>("DataBaseType");
            var isPostgres = !string.IsNullOrWhiteSpace(dbType) && dbType.Equals("p", StringComparison.OrdinalIgnoreCase);
            if (isPostgres)
            {
                options.UseNpgsql(configuration.GetConnectionString("postgress"));
            }
            else
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            }
        });
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddAutoMapper(cfg =>
        {
            // Minimal mapping set required by allow-listed admin operations.
            cfg.CreateMap<User, UserListDTO>();
            cfg.CreateMap<Role, RoleViewDTO>();
        });
        // Provide default performance options for service constructors that depend on IOptions<PerformanceSettings>.
        services.Configure<PerformanceSettings>(_ => { });
        // Keep AdminPanel independent from distributed cache infra (Redis); no-op cache is enough here.
        services.AddScoped<IAppCacheService, NoOpAppCacheService>();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = securitySettings.Authentication.LoginPath;
                options.AccessDeniedPath = securitySettings.Authentication.AccessDeniedPath;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

        services.AddAuthorization();

        services.AddIdentityCore<User>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager<SignInManager<User>>();

        // Register open-generic repositories explicitly (same pattern as Autofac module)
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IServiceRepository<,,,>), typeof(ServiceRepository<,,,>));
        services.AddScoped(typeof(IServiceRepository<,,>), typeof(ServiceRepository<,,>));
        services.AddScoped(typeof(IServiceRepository<,>), typeof(ServiceRepository<,>));

        RegisterMarkedServices(services);

        services.AddScoped<IEntityAdminService, EntityAdminService>();
        services.AddScoped<IOperationCatalogService, OperationCatalogService>();
        services.AddScoped<IAdminPanelAuthService, AdminPanelAuthService>();
        return services;
    }

    private static void RegisterMarkedServices(IServiceCollection services)
    {
        var assemblies = new[]
        {
            typeof(IScopedDependency).Assembly,
            typeof(SenderService).Assembly
        };

        RegisterByMarker<IScopedDependency>(services, assemblies, ServiceLifetime.Scoped);
        RegisterByMarker<ITransientDependency>(services, assemblies, ServiceLifetime.Transient);
        RegisterByMarker<ISingletonDependency>(services, assemblies, ServiceLifetime.Singleton);

        // Register only the service-layer dependencies currently needed by AdminPanel.
        services.AddScoped<ISenderService, SenderService>();
    }

    private static void RegisterByMarker<TMarker>(IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime)
    {
        var types = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters && typeof(TMarker).IsAssignableFrom(t))
            .ToList();

        foreach (var implementationType in types)
        {
            var interfaces = implementationType
                .GetInterfaces()
                .Where(i => i != typeof(TMarker) && !typeof(IScopedDependency).IsAssignableFrom(i) && !typeof(ITransientDependency).IsAssignableFrom(i) && !typeof(ISingletonDependency).IsAssignableFrom(i))
                .ToList();

            if (interfaces.Count == 0)
            {
                services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
                continue;
            }

            foreach (var interfaceType in interfaces)
            {
                services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));
            }
        }
    }
}
