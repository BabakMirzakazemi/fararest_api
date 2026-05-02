using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebFramework.Configuration.IoCConfigurations;

namespace WebFramework.Configuration;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAutofac(this WebApplicationBuilder builder)
    {
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) => builder.RegisterModule(new AutofacModule()));
        return builder;
    }

    public static WebApplicationBuilder ConfigureSection<T>(this WebApplicationBuilder builder, string sectionName) where T : class
    {
        builder.Services.Configure<T>(builder.Configuration.GetSection(sectionName));
        return builder;
    }
}
