using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Middleware;
using Portfolio.Infrastructure;

namespace Portfolio.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring the application's request pipeline.
/// </summary>
/// <remarks>
/// This static class contains methods that extend the functionality of <see cref="IApplicationBuilder"/>.
/// It includes methods for applying database migrations and configuring custom middleware.
/// </remarks>
internal static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Applies pending database migrations for the application's database context.
    /// </summary>
    /// <param name="app">
    /// An instance of <see cref="IApplicationBuilder"/> used to configure the application's request pipeline.
    /// </param>
    /// <remarks>
    /// This method creates a service scope to resolve the <see cref="ApplicationDbContext"/> and applies
    /// any pending migrations to the database. It ensures that the database schema is up-to-date
    /// before the application starts handling requests.
    /// </remarks>
    /// <example>
    /// To use this method, call it during application startup:
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// var app = builder.Build();
    /// app.ApplyMigrations();
    /// app.Run();
    /// </code>
    /// </example>
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }

    /// <summary>
    /// Configures the application to use custom exception handling middleware.
    /// </summary>
    /// <param name="app">
    /// An instance of <see cref="IApplicationBuilder"/> used to configure the application's request pipeline.
    /// </param>
    /// <remarks>
    /// This method adds the <see cref="ExceptionHandlingMiddleware"/> to the application's middleware pipeline.
    /// It ensures that unhandled exceptions are logged and appropriate error responses are returned to the client.
    /// </remarks>
    /// <example>
    /// To use this method, call it during application startup:
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// var app = builder.Build();
    /// app.UseCustomExceptionHandler();
    /// app.Run();
    /// </code>
    /// </example>
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
