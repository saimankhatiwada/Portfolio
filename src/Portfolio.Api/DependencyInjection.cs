using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Portfolio.Api.MediaTypes;
using Portfolio.Application;
using Portfolio.Infrastructure;
using Portfolio.Api.Swagger;
using Portfolio.Api.Services;

namespace Portfolio.Api;

internal static class DependencyInjection
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        ConfigureMvcServices(builder);

        ConfigureApplicationAndInfrastructure(builder);

        ConfigureSwagger(builder);

        builder.Services.AddTransient<DataShapingService>();
        builder.Services.AddTransient<LinkService>();

        return builder;
    }

    private static void ConfigureMvcServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            })
            .AddNewtonsoftJson(options => 
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
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

    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();

        builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

        builder.Services.ConfigureOptions<ConfigureSwaggerUIOptions>();
    }
}
