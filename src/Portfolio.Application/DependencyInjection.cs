using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Abstractions.Behaviors;

namespace Portfolio.Application;

/// <summary>
/// Provides extension methods for configuring application-level services and behaviors in the dependency injection container.
/// </summary>
/// <remarks>
/// This class includes methods to register MediatR behaviors, validators, and other application-specific services
/// into the <see cref="IServiceCollection"/>. It ensures that the application is properly configured with
/// required behaviors such as validation and query caching.
/// </remarks>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application-level services, behaviors, and validators into the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the application services will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    /// <remarks>
    /// This method configures MediatR with custom behaviors such as <see cref="ValidationBehavior{TRequest, TResponse}"/> 
    /// and <see cref="QueryCachingBehavior{TRequest, TResponse}"/>. It also registers all validators from the assembly 
    /// containing the <see cref="DependencyInjection"/> class.
    /// </remarks>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));

            configuration.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
