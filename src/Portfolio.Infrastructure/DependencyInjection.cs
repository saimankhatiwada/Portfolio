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

/// <summary>
/// Provides methods for configuring and registering infrastructure services in the application.
/// </summary>
/// <remarks>
/// This static class contains extension methods for setting up various infrastructure components, 
/// such as database providers, caching, authentication, authorization, API versioning, outbox and Quartz configuration, 
/// blob storage, and telemetry instrumentation. It ensures that all necessary services are properly registered 
/// in the dependency injection container.
/// </remarks>
public static class DependencyInjection
{
    /// <summary>
    /// Configures and registers infrastructure services for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <param name="configuration">The application's configuration settings.</param>
    /// <param name="environment">The hosting environment in which the application is running.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    /// <remarks>
    /// This method sets up various infrastructure components, including database providers, caching, 
    /// authentication, authorization, API versioning, outbox and Quartz configuration, blob storage, 
    /// and telemetry instrumentation. It ensures that all necessary infrastructure services are 
    /// properly registered in the dependency injection container.
    /// </remarks>
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

    /// <summary>
    /// Configures and registers database-related services and providers in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the database services will be added.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to retrieve database configuration settings.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the connection string for the database is not found in the provided <paramref name="configuration"/>.
    /// </exception>
    /// <remarks>
    /// This method performs the following tasks:
    /// <list type="bullet">
    /// <item>Registers the <see cref="ApplicationDbContext"/> with PostgreSQL support and snake case naming convention.</item>
    /// <item>Registers the <see cref="IUserRepository"/> implementation.</item>
    /// <item>Registers the <see cref="IUnitOfWork"/> implementation using <see cref="ApplicationDbContext"/>.</item>
    /// <item>Registers a singleton instance of <see cref="ISqlConnectionFactory"/> with the provided connection string.</item>
    /// </list>
    /// </remarks>
    private static void AddDatabaseProviders(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database") ??
                                  throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
    }

    /// <summary>
    /// Configures and registers caching services for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the caching services will be added.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to retrieve configuration settings.</param>
    /// <remarks>
    /// This method sets up a Redis-based caching mechanism using StackExchange.Redis. It establishes a connection
    /// to the Redis server, registers the <see cref="IConnectionMultiplexer"/> for dependency injection, and configures
    /// the application to use Redis as the caching provider. Additionally, it registers the <see cref="ICacheService"/>
    /// implementation to enable caching functionality throughout the application.
    /// </remarks>
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

    /// <summary>
    /// Configures and registers authentication services for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the authentication services will be added.</param>
    /// <param name="configuration">The application's configuration, used to retrieve authentication-related settings.</param>
    /// <remarks>
    /// This method sets up JWT Bearer authentication, configures authentication options, and registers necessary services
    /// such as <see cref="IAuthenticationService"/>, <see cref="IJwtService"/>, and <see cref="IUserContext"/>.
    /// It also configures HTTP clients for interacting with authentication endpoints.
    /// </remarks>
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

    /// <summary>
    /// Configures and registers services required for authorization in the application.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the authorization services will be added.
    /// </param>
    /// <remarks>
    /// This method registers the following services:
    /// <list type="bullet">
    /// <item><description><see cref="AuthorizationService"/> as a scoped service.</description></item>
    /// <item><description><see cref="CustomClaimsTransformation"/> as a transient implementation of <see cref="IClaimsTransformation"/>.</description></item>
    /// <item><description><see cref="PermissionAuthorizationHandler"/> as a transient implementation of <see cref="IAuthorizationHandler"/>.</description></item>
    /// <item><description><see cref="PermissionAuthorizationPolicyProvider"/> as a transient implementation of <see cref="IAuthorizationPolicyProvider"/>.</description></item>
    /// </list>
    /// </remarks>
    private static void AddAuthorizationServices(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

    /// <summary>
    /// Configures API versioning for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the API versioning services are added.</param>
    /// <remarks>
    /// This method sets up API versioning with the following configurations:
    /// <list type="bullet">
    /// <item><description>Sets the default API version to 1.0.</description></item>
    /// <item><description>Assumes the default version when it is not specified in the request.</description></item>
    /// <item><description>Reports the supported API versions in the response headers.</description></item>
    /// <item><description>Combines multiple API version readers, including a custom media type version reader.</description></item>
    /// </list>
    /// Additionally, it integrates API versioning with MVC and the API Explorer for enhanced versioning support.
    /// </remarks>
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

    /// <summary>
    /// Configures the outbox mechanism and Quartz job scheduling for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to retrieve configuration settings.</param>
    /// <remarks>
    /// This method performs the following tasks:
    /// <list type="bullet">
    /// <item><description>Binds the <see cref="OutboxOptions"/> configuration section to the corresponding options class.</description></item>
    /// <item><description>Registers Quartz services for job scheduling.</description></item>
    /// <item><description>Configures the Quartz hosted service to wait for jobs to complete on shutdown.</description></item>
    /// <item><description>Sets up the Quartz job for processing outbox messages using <see cref="ProcessOutboxMessagesJobSetup"/>.</description></item>
    /// </list>
    /// </remarks>
    private static void AddOutboxAndQuartzConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.Name));

        services.AddQuartz();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
    }

    /// <summary>
    /// Configures and registers services required for interacting with Azure Blob Storage.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the blob storage services will be added.</param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance used to retrieve configuration settings for blob storage.
    /// </param>
    /// <remarks>
    /// This method performs the following actions:
    /// <list type="bullet">
    /// <item><description>Configures <see cref="BlobOptions"/> using the "Blob" section of the configuration.</description></item>
    /// <item><description>Registers a singleton instance of <see cref="BlobServiceClient"/> using the connection string from the configuration.</description></item>
    /// <item><description>Registers <see cref="IBlobStorage"/> with its implementation <see cref="BlobStorage"/> as a singleton service.</description></item>
    /// </list>
    /// </remarks>
    private static void AddBlobStorageServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobOptions>(configuration.GetSection(BlobOptions.Blob));

        services.AddSingleton(_ => new BlobServiceClient(configuration.GetConnectionString("BlobStorage")));

        services.AddSingleton<IBlobStorage, BlobStorage>();
    }

    /// <summary>
    /// Configures and adds OpenTelemetry instrumentation to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the OpenTelemetry instrumentation will be added.</param>
    /// <param name="environment">The <see cref="IWebHostEnvironment"/> providing information about the web hosting environment.</param>
    /// <remarks>
    /// This method sets up OpenTelemetry for both tracing and metrics. It includes instrumentation for HTTP clients, 
    /// ASP.NET Core, and PostgreSQL. Additionally, it configures runtime metrics and utilizes the OTLP exporter 
    /// to export telemetry data. The resource configuration is enriched with the application name from the hosting environment.
    /// </remarks>
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

    /// <summary>
    /// Registers services related to sorting and sort mappings in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <remarks>
    /// This method adds the <see cref="SortMappingProvider"/> as a transient service and registers 
    /// a singleton instance of <see cref="ISortMappingDefinition"/> for user-specific sort mappings.
    /// </remarks>
    private static void AddSortMappingServices(IServiceCollection services)
    {
        services.AddTransient<SortMappingProvider>();

        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<User>>(_ =>
            UserSortMapping.SortMapping);
    }
}
