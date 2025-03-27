using System.Reflection;
using Portfolio.Application.Abstractions.Messaging;
using Portfolio.Domain.Abstractions;
using Portfolio.Infrastructure;

namespace Portfolio.ArchitectureTest.Infrastructure;

public class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(IBaseCommand).Assembly;

    protected static readonly Assembly DomainAssembly = typeof(IEntity).Assembly;

    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;

    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
