using Common.Configurations;
using Data;
using Entities.Users;
using Microsoft.Extensions.DependencyInjection;

namespace WebFramework.Configuration;

public static class IdentityConfigurationExtensions
{
    public static IServiceCollection AddCustomIdentity(this IServiceCollection services, IdentitySettings settings)
    {
        services.AddIdentity<User, Role>(identityOptions =>
        {
            //Password Settings
            identityOptions.Password.RequireDigit = settings.PasswordRequireDigit;
            identityOptions.Password.RequiredLength = settings.PasswordRequiredLength;
            identityOptions.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanumic; //#@!
            identityOptions.Password.RequireUppercase = settings.PasswordRequireUppercase;
            identityOptions.Password.RequireLowercase = settings.PasswordRequireLowercase;

            //UserName Settings
            identityOptions.User.RequireUniqueEmail = settings.RequireUniqueEmail;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();
        //.AddDefaultTokenProviders();


        return services;
    }
}
