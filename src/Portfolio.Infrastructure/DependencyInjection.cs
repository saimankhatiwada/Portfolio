using Asp.Versioning;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Portfolio.Application.Abstractions.Authentication;
using Portfolio.Application.Abstractions.Caching;
using Portfolio.Application.Abstractions.Clock;
using Portfolio.Application.Abstractions.Data;
using Portfolio.Application.Abstractions.Storage;
using Portfolio.Domain.Abstractions;
using Portfolio.Domain.Blogs;
using Portfolio.Domain.Tags;
using Portfolio.Domain.Users;
using Portfolio.Infrastructure.Authentication;
using Portfolio.Infrastructure.Authorization;
using Portfolio.Infrastructure.Caching;
using Portfolio.Infrastructure.Clock;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Outbox;
using Portfolio.Infrastructure.Repositories;
using Portfolio.Infrastructure.Services.Sorting;
using Portfolio.Infrastructure.Services.Sorting.Mappings;
using Portfolio.Infrastructure.Storage;
using Quartz;
using StackExchange.Redis;
using AuthenticationOptions = Portfolio.Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Portfolio.Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Portfolio.Application.Abstractions.Authentication.IAuthenticationService;

namespace Portfolio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        
        AddDatabaseProviders(services, configuration);

        AddCacheServices(services, configuration);

        AddAuthenticationServices(services, configuration);

        AddAuthorizationServices(services);

        ApplyApiVersioning(services);

        AddOutboxAndQuartzConfiguration(services, configuration);

        AddBlobStorageServices(services, configuration);

        AddTelemetryInstrumentation(services, environment);

        AddSortMappingServices(services);

        return services;
    }

    private static void AddDatabaseProviders(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database") ??
                                  throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ITagRepository, TagRepository>();

        services.AddScoped<IBlogRepository, BlogRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
    }

    private static void AddCacheServices(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Cache") ??
                                  throw new ArgumentNullException(nameof(configuration));

        IConnectionMultiplexer redisConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);

        services.AddSingleton(redisConnectionMultiplexer);

        services.AddStackExchangeRedisCache(options => options.ConnectionMultiplexerFactory = () => Task.FromResult(redisConnectionMultiplexer));

        //services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

        services.AddSingleton<ICacheService, CacheService>();
    }

    private static void AddAuthenticationServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.Name));

        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
        {
            KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        })
        .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        {
            KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();
    }

    private static void AddAuthorizationServices(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

    private static void ApplyApiVersioning(IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector = new DefaultApiVersionSelector(options);

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new MediaTypeApiVersionReader(),
                    new MediaTypeApiVersionReaderBuilder()
                        .Template("application/vnd.portfolio.hateoas.{version}+json")
                        .Build());
            })
            .AddMvc()
            .AddApiExplorer();
    }

    private static void AddOutboxAndQuartzConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.Name));

        services.AddQuartz();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
    }

    private static void AddBlobStorageServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobOptions>(configuration.GetSection(BlobOptions.Blob));

        services.AddSingleton(_ => new BlobServiceClient(configuration.GetConnectionString("BlobStorage")));

        services.AddSingleton<IBlobStorage, BlobStorage>();
    }

    private static void AddTelemetryInstrumentation(IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(environment.ApplicationName))
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRedisInstrumentation()
                .AddNpgsql())
            .WithMetrics(metrics => metrics
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation())
            .UseOtlpExporter();
    }

    private static void AddSortMappingServices(IServiceCollection services)
    {
        services.AddTransient<SortMappingProvider>();

        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<User>>(_ =>
            UserSortMapping.SortMapping);

        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<Tag>>(_ =>
            TagSortMapping.SortMapping);
    }
}
