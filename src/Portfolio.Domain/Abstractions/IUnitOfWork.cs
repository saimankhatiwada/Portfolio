namespace Portfolio.Domain.Abstractions;

/// <summary>
/// Represents a contract for coordinating a sequence of operations within a single, atomic transaction.
/// </summary>
/// <remarks>
/// This interface is typically utilized to maintain consistency across various repositories or data sources,
/// ensuring that all changes are either fully committed or entirely reverted.
/// </remarks>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

