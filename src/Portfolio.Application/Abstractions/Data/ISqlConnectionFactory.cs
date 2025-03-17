using System.Data;

namespace Portfolio.Application.Abstractions.Data;

/// <summary>
/// Provides a factory for creating instances of <see cref="System.Data.IDbConnection"/>.
/// </summary>
/// <remarks>
/// This interface abstracts the creation of database connections, enabling better testability
/// and flexibility in managing database access within the application.
/// </remarks>
public interface ISqlConnectionFactory
{
    /// <summary>
    /// Creates and returns a new instance of <see cref="System.Data.IDbConnection"/>.
    /// </summary>
    /// <returns>
    /// An open <see cref="System.Data.IDbConnection"/> instance configured with the appropriate connection string.
    /// </returns>
    /// <remarks>
    /// The caller is responsible for properly disposing of the returned <see cref="System.Data.IDbConnection"/> instance
    /// to ensure that database resources are released.
    /// </remarks>
    IDbConnection CreateConnection();
}
