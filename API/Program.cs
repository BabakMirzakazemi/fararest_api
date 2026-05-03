using Common.Configurations;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using WebFramework.Configuration;
using WebFramework.Configuration.LogConfigurations;
using WebFramework.CustomMapping;
using WebFramework.Swagger;
var builder = WebApplication.CreateBuilder(args);


builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

//builder.Services.Configure<IranianBankInfo>(builder.Configuration.GetSection(nameof(IranianBankInfo)));

builder.Services.AddCors(o =>
{
    o.AddPolicy("CorsPolicy", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var siteSetting = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
var identitySettings = builder.Configuration.GetSection(nameof(IdentitySettings)).Get<IdentitySettings>()
                      ?? siteSetting?.IdentitySettings;
var jwtSettings = siteSetting?.JwtSettings;

if (siteSetting == null || identitySettings == null || jwtSettings == null)
    throw new ArgumentNullException("SiteSettings", "Site Settings Not Found");

var seqSetting = builder.Configuration.GetSection(nameof(SeqSettings)).Get<SeqSettings>() ?? new SeqSettings();
//if (seqSetting == null || string.IsNullOrEmpty(seqSetting.SeqUrl) || string.IsNullOrEmpty(seqSetting.SeqApiKey))
//    throw new ArgumentNullException("Seq", "Seq Url Or Api Key Not Found");


var configuration = builder.Configuration;
// Add services to the container.

builder
    .UseSerilog(seqSetting)
    .AddAutofac()
    .ConfigureSection<SiteSettings>(nameof(SiteSettings))
    .ConfigureSection<PerformanceSettings>(nameof(PerformanceSettings))
    //.ConfigureSection<SeqSettings>(nameof(SeqSettings))
    .Services
    .AddControllersConfig()
    .AddSwagger()
    .AddOptions()
    .AddFluentValidation()
    .AddDataInitializers()
    .AddDbContext(configuration, builder.Environment)
    // Register cache, health checks and performance config in one place.
    .AddPerformanceInfrastructure(configuration)
    .AddSessionStorage()
    .AddCorsPolicy()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .InitializeAutoMapper()
    .AddCustomIdentity(identitySettings)
    .AddJwtAuthentication(jwtSettings);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var dataProtectionKeyPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtection-Keys");
Directory.CreateDirectory(dataProtectionKeyPath);
builder.Services
    .AddDataProtection()
    .SetApplicationName("babak_base")
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyPath));
var app = builder.Build();

// One-shot bootstrap mode for CI/local setup:
// applies migrations + SQL objects + data initializers, then exits.
if (args.Any(x => string.Equals(x, "--bootstrap-db", StringComparison.OrdinalIgnoreCase)))
{
    app.InitializeDatabase();
    return;
}

app
    .UseSwaggerAndUI(true)
    .InitializeDatabase()
    // Adds correlation-id scope to logs/response header.
    .UseRequestContextEnrichment()
    // Adds rate limiting and health endpoints (/health/live, /health/ready).
    .UsePerformancePipeline()
    .UseCustomExceptionHandler()
    .UseHttpsRedirection()
    .UseSession()
    //.UseCustomStaticFiles()
    .UseHsts(app.Environment)
    .UseCors("CorsPolicy")
    .UseAuthentication()
    .UseAuthorization()
    .UseCustomSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
