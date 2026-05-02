using Common.Configurations;
using Common.Exceptions;
using Common.Utilities.Extensions;
using Common.Utilities.Helpers;
using Data;
using Entities.Users;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Services.Contracts.Authentications;
using Services.Contracts.Caching;
using Services.DataInitializer;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebFramework.HealthChecks;
using WebFramework.Infrastructure.Caching;
using WebFramework.Infrastructure.Performance;
using Microsoft.AspNetCore.Http;

namespace WebFramework.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            if (webHostEnvironment.IsDevelopment())
                options.EnableSensitiveDataLogging();

            var dbType = configuration.GetValue<string>("DataBaseType");
            var isPostgreSQL = dbType != null && dbType.ToLower() == "p";
            var slowQueryInterceptor = sp.GetRequiredService<SlowQueryLoggingInterceptor>();

            if (isPostgreSQL)
                options.UseNpgsql(configuration.GetConnectionString("postgress"));
            else
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));

            options.AddInterceptors(slowQueryInterceptor);
        });
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    static readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins,
                              builder =>
                              {
                                  builder.WithOrigins("http://example.com", "http://test.com")
                                                      .AllowAnyHeader()
                                                      ;
                                  builder.WithMethods("PUT", "DELETE", "GET", "POST");
                              });
        });

        return services;
    }

    public static IServiceCollection AddSessionStorage(this IServiceCollection services)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(180);
            options.Cookie.IsEssential = true;
        });
        return services;
    }

    public static IServiceCollection AddPerformanceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind performance section once so tuning can happen via config/environment without code changes.
        var settings = configuration.GetSection(nameof(PerformanceSettings)).Get<PerformanceSettings>() ?? new PerformanceSettings();
        services.Configure<PerformanceSettings>(configuration.GetSection(nameof(PerformanceSettings)));
        services.AddSingleton<SlowQueryLoggingInterceptor>();
        services.AddSingleton<AppTelemetry>();
        RegisterOpenTelemetry(services, settings.Telemetry);

        // Register cache provider (Redis or in-memory fallback).
        RegisterDistributedCache(services, settings.Caching);

        // App-layer cache abstraction used by services.
        services.AddScoped<IAppCacheService, DistributedAppCacheService>();

        // Liveness/Readiness checks for runtime orchestration and monitoring systems.
        services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"])
            .AddDbContextCheck<ApplicationDbContext>("database", tags: ["ready"])
            .AddCheck<DistributedCacheHealthCheck>("distributed_cache", tags: ["ready"]);

        return services;
    }

    private static void RegisterOpenTelemetry(IServiceCollection services, TelemetrySettings telemetrySettings)
    {
        if (!telemetrySettings.Enabled)
            return;

        var serviceName = string.IsNullOrWhiteSpace(telemetrySettings.ServiceName)
            ? "babak_base.api"
            : telemetrySettings.ServiceName;
        var activitySourceName = string.IsNullOrWhiteSpace(telemetrySettings.ActivitySourceName)
            ? "babak_base.api"
            : telemetrySettings.ActivitySourceName;
        var meterName = string.IsNullOrWhiteSpace(telemetrySettings.MeterName)
            ? "babak_base.api"
            : telemetrySettings.MeterName;

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                var samplingRatio = telemetrySettings.TraceSamplingRatio;
                if (samplingRatio < 0d)
                    samplingRatio = 0d;
                else if (samplingRatio > 1d)
                    samplingRatio = 1d;

                tracing
                    .SetSampler(new TraceIdRatioBasedSampler(samplingRatio))
                    .AddSource(activitySourceName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = httpContext => !IsExcludedTelemetryPath(httpContext.Request.Path, telemetrySettings.ExcludedPaths);
                    })
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();
                // TODO(Telemetry): if EF instrumentation exposes SQL statement toggles in future versions,
                // wire telemetrySettings.IncludeDbStatementInTraces here.

                AddOtlpExporterIfConfigured(tracing, telemetrySettings);
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(meterName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();

                AddOtlpExporterIfConfigured(metrics, telemetrySettings);
            });
    }

    private static void AddOtlpExporterIfConfigured(TracerProviderBuilder tracing, TelemetrySettings settings)
    {
        if (!Uri.TryCreate(settings.OtlpEndpoint, UriKind.Absolute, out var endpoint))
            return;

        tracing.AddOtlpExporter(options =>
        {
            options.Endpoint = endpoint;
            options.Protocol = ParseOtlpProtocol(settings.OtlpProtocol);
        });
    }

    private static void AddOtlpExporterIfConfigured(MeterProviderBuilder metrics, TelemetrySettings settings)
    {
        if (!Uri.TryCreate(settings.OtlpEndpoint, UriKind.Absolute, out var endpoint))
            return;

        metrics.AddOtlpExporter(options =>
        {
            options.Endpoint = endpoint;
            options.Protocol = ParseOtlpProtocol(settings.OtlpProtocol);
        });
    }

    private static OtlpExportProtocol ParseOtlpProtocol(string? value)
    {
        if (string.Equals(value, "grpc", StringComparison.OrdinalIgnoreCase))
            return OtlpExportProtocol.Grpc;

        return OtlpExportProtocol.HttpProtobuf;
    }

    private static bool IsExcludedTelemetryPath(PathString requestPath, IReadOnlyCollection<string>? excludedPaths)
    {
        if (excludedPaths == null || excludedPaths.Count == 0)
            return false;

        foreach (var raw in excludedPaths)
        {
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            var candidate = raw.Trim();
            if (requestPath.StartsWithSegments(candidate, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static void RegisterDistributedCache(IServiceCollection services, CachingSettings caching)
    {
        if (caching.Enabled && caching.UseRedis && !string.IsNullOrWhiteSpace(caching.RedisConnection))
        {
            // Redis for multi-instance deployments and heavier load.
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = caching.RedisConnection;
                options.InstanceName = caching.RedisInstanceName;
            });
            return;
        }

        // Safe default for local/dev environments.
        services.AddDistributedMemoryCache();
    }

    public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.Encryptkey);

            var parameters = new TokenValidationParameters()
            {
                ValidAudience = jwtSettings.Audience,
                ValidIssuer = jwtSettings.Issuer,
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true, // Ensure the token hasn't expired
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey),
                ClockSkew = TimeSpan.Zero // No tolerance for clock skew
            };

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = parameters;
            options.Events = new JwtBearerEvents
            {
                //token exists in header but is invalid
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                    logger.LogError(context.Exception, "Authentication failed.");

                    if (context.Exception != null)
                        throw new AppException("Authentication failed.", HttpStatusCode.Unauthorized, context.Exception, null);

                    return Task.CompletedTask;
                },
                OnTokenValidated = async context =>
                {
                    var cookieToken = CookieManager.Get(context.HttpContext, CookieManager.CookieKeys.SiteJwtToken);
                    if (string.IsNullOrWhiteSpace(cookieToken))
                    {
                        context.Fail("Jwt cookie token is missing.");
                        return;
                    }

                    if (context.Principal?.Identity?.IsAuthenticated != true)
                    {
                        context.Fail("Token principal is not authenticated.");
                        return;
                    }

                    if (context.SecurityToken is not System.IdentityModel.Tokens.Jwt.JwtSecurityToken jwtSecurityToken)
                    {
                        context.Fail("Security token type is invalid.");
                        return;
                    }

                    if (!string.Equals(jwtSecurityToken.RawData, cookieToken, StringComparison.Ordinal))
                    {
                        context.Fail("Jwt token and cookie token are not equal.");
                        return;
                    }

                    var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                    var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                    var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();


                    var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                    if (claimsIdentity?.Claims?.Any() != true)
                    {
                        context.Fail("This token has no claims.");
                        return;
                    }

                    var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                    if (!securityStamp.HasValue())
                    {
                        context.Fail("This token has no security stamp.");
                        return;
                    }

                    var userIdValue = claimsIdentity.GetUserId();
                    if (!Guid.TryParse(userIdValue, out var userId))
                    {
                        context.Fail("User id claim is invalid.");
                        return;
                    }

                    //Find user and token from database and perform your custom validation
                    var user = await userRepository.TableNoTracking.FirstOrDefaultAsync(u => u.Id == userId, context.HttpContext.RequestAborted);
                    if (user is null)
                    {
                        context.Fail("User not found.");
                        return;
                    }

                    if (!user.IsActive)
                    {
                        context.Fail("User is inactive.");
                        return;
                    }

                    if (await userManager.IsLockedOutAsync(user))
                    {
                        context.Fail("User is locked out.");
                        return;
                    }

                    var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                    if (validatedUser == null)
                    {
                        context.Fail("Token security stamp is not valid.");
                        return;
                    }
                },
                //token is not exists in header
                OnChallenge = context =>
                {
                    if (context.AuthenticateFailure != null)
                        throw new AppException("Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);

                    throw new AppException("You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);
                }
            };
        });
    }

    public static IServiceCollection AddControllersConfig(this IServiceCollection services)
    {
        services.AddControllers();

        //TODO Fluent Check Is Working
        //services.AddValidatorsFromAssemblyContaining<IServiceLayerMarker>();

        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = false;
            config.ReportApiVersions = true;
        });
        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        var validatorType = typeof(IValidator<>);

        var validatorConcretions = AssemblyHelper.FindAllTypes().Where(type => type
            .FindInterfacesThatClose(validatorType).Any()).ToList();

        validatorConcretions = validatorConcretions.Where(type => type.IsConcrete()).ToList();

        foreach (var type in validatorConcretions)
        {
            var serviceType = type.GetInterfaces().First(i => i.GetGenericTypeDefinition() == validatorType);
            services.AddScoped(serviceType, type);
        }


        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;

        return services;
    }

    public static IServiceCollection AddDataInitializers(this IServiceCollection services)
    {
        services.AddScoped<IDataInitializer, RoleDataInitializer>();
        services.AddScoped<IDataInitializer, UserDataInitializer>();
        return services;
    }
}
