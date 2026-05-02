using Common.Utilities.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace WebFramework.Swagger;

public static class SwaggerConfigurationExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        Assert.NotNull(services, nameof(services));

        //More info : https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters

        #region AddSwaggerExamples
        //Add services to use Example Filters in swagger
        //If you want to use the Request and Response example filters (and have called options.ExampleFilters() above), then you MUST also call
        //This method to register all ExamplesProvider classes form the assembly
        //services.AddSwaggerExamplesFromAssemblyOf<PersonRequestExample>();

        //We call this method for by reflection with the Startup type of entry assmebly (MyApi assembly)
        var mainAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(); // => MyApi project assembly
        var mainType = mainAssembly.GetExportedTypes().FirstOrDefault() ?? typeof(SwaggerConfigurationExtensions);

        const string methodName = nameof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions.AddSwaggerExamplesFromAssemblyOf);
        //MethodInfo method = typeof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions).GetMethod(methodName);
        MethodInfo? method = typeof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions)
            .GetRuntimeMethods()
            .FirstOrDefault(x => x.Name == methodName && x.IsGenericMethod);
        if (method != null)
        {
            MethodInfo generic = method.MakeGenericMethod(mainType);
            generic.Invoke(null, new object[] { services });
        }
        #endregion

        //Add services and configuration to use swagger
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Babak.Site.API.V1" });
            options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "Babak.Site.API.V2" });

            #region Filters
            //Enable to use [SwaggerRequestExample] & [SwaggerResponseExample]
            options.ExampleFilters();

            //Set summary of action if not already set
            options.OperationFilter<ApplySummariesOperationFilter>();

            #region Add UnAuthorized to Response
            //Add 401 response and security requirements (Lock icon) to actions that need authorization
            options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "Bearer");
            #endregion

            #region Add Jwt Authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", document, null),
                    new List<string>()
                }
            });
            #endregion

            #region Versioning
            // Remove version parameter from all Operations
            options.OperationFilter<RemoveVersionParameters>();

            //set version "api/v{version}/[controller]" from current swagger doc verion
            options.DocumentFilter<SetVersionInPaths>();

            //Seperate and categorize end-points by doc version
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;
                if (methodInfo.DeclaringType is null) return false;

                var versions = methodInfo.DeclaringType
                    .GetCustomAttributes<ApiVersionAttribute>(true)
                    .SelectMany(attr => attr.Versions);

                return versions.Any(v => $"v{v}" == docName);
            });
            #endregion

            //If use FluentValidation then must be use this package to show validation in swagger (MicroElements.Swashbuckle.FluentValidation)
            //options.AddFluentValidationRules();
            #endregion
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerAndUI(this IApplicationBuilder app, bool isDevelopment)
    {
        if (isDevelopment)
        {
            Assert.NotNull(app, nameof(app));

            //More info : https://github.com/domaindrivendev/Swashbuckle.AspNetCore

            //Swagger middleware for generate "Open API Documentation" in swagger.json
            app.UseSwagger();

            //Swagger middleware for generate UI from swagger.json
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2 Docs");

                options.DocExpansion(DocExpansion.None);
            });

            //ReDoc UI middleware. ReDoc UI is an alternative to swagger-ui
            app.UseReDoc(options =>
            {
                options.SpecUrl("/swagger/v1/swagger.json");
                options.SpecUrl("/swagger/v2/swagger.json");

                #region Customizing
                options.EnableUntrustedSpec();
                options.ScrollYOffset(10);
                options.HideHostname();
                options.HideDownloadButton();
                options.ExpandResponses("200,201");
                options.RequiredPropsFirst();
                options.NoAutoAuth();
                options.PathInMiddlePanel();
                options.HideLoading();
                options.NativeScrollbars();
                options.DisableSearch();
                options.OnlyRequiredInSamples();
                options.SortPropsAlphabetically();
                #endregion
            });
        }

        return app;
    }
}
