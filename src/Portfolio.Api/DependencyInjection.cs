using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Portfolio.Api.MediaTypes;
using Portfolio.Application;
using Portfolio.Infrastructure;
using Portfolio.Api.Swagger;

namespace Portfolio.Api;

/// <summary>
/// Provides extension methods for configuring dependency injection in the Portfolio API.
/// </summary>
/// <remarks>
/// This static class contains methods to register and configure services required by the Portfolio API.
/// It includes methods for setting up MVC services, application services, and infrastructure services.
/// </remarks>
internal static class DependencyInjection
{
    /// <summary>
    /// Adds and configures API services for the Portfolio application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
    /// <returns>The updated <see cref="WebApplicationBuilder"/> instance.</returns>
    /// <remarks>
    /// This method sets up the necessary services for the Portfolio API, including MVC services, 
    /// application services, and infrastructure services. It ensures that the API is properly configured 
    /// to handle requests, serialize responses, and integrate with the application's infrastructure.
    /// </remarks>
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        ConfigureMvcServices(builder);

        ConfigureApplicationAndInfrastructure(builder);

        ConfigureSwagger(builder);

        return builder;
    }

    /// <summary>
    /// Configures MVC services for the Portfolio API.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
    /// <remarks>
    /// This method sets up the MVC services by enabling controllers, configuring JSON serialization
    /// to use camel case property names, and adding support for XML serialization. It also customizes
    /// the supported media types for JSON output formatters, including custom media types defined
    /// in <see cref="Portfolio.Api.MediaTypes.CustomMediaTypeNames"/>.
    /// </remarks>
    private static void ConfigureMvcServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            })
            .AddNewtonsoftJson(options => options
                .SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
            .AddXmlSerializerFormatters();

        builder.Services.Configure<MvcOptions>(options =>
        {
            NewtonsoftJsonOutputFormatter formatter = options.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()
                .First();

            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.JsonV1);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.JsonV2);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJson);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJsonV1);
            formatter.SupportedMediaTypes.Add(CustomMediaTypeNames.Application.HateoasJsonV2);
        });
    }

    /// <summary>
    /// Configures application and infrastructure services for the Portfolio API.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
    /// <remarks>
    /// This method registers application-level services by invoking <see cref="Portfolio.Application.DependencyInjection.AddApplication(IServiceCollection)"/> 
    /// and configures infrastructure services using <see cref="Portfolio.Infrastructure.DependencyInjection.AddInfrastructure(IServiceCollection, IConfiguration, IWebHostEnvironment)"/>. 
    /// Additionally, it sets up logging with OpenTelemetry, enabling options for scopes and formatted messages.
    /// </remarks>
    private static void ConfigureApplicationAndInfrastructure(WebApplicationBuilder builder)
    {
        builder.Services.AddApplication();

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
        });

        builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
    }

    /// <summary>
    /// Configures Swagger services for the Portfolio API.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
    /// <remarks>
    /// This method registers and configures Swagger services, including the generation of Swagger documentation
    /// and the setup of the Swagger UI. It utilizes custom configuration options defined in 
    /// <see cref="Portfolio.Api.Swagger.ConfigureSwaggerGenOptions"/> and 
    /// <see cref="Portfolio.Api.Swagger.ConfigureSwaggerUIOptions"/> to tailor the Swagger experience
    /// for the Portfolio API.
    /// </remarks>
    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();

        builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

        builder.Services.ConfigureOptions<ConfigureSwaggerUIOptions>();
    }
}
