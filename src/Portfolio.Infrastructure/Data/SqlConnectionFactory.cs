using Npgsql;
using System.Data;
using Portfolio.Application.Abstractions.Data;

namespace Portfolio.Infrastructure.Data;

/// <summary>
/// A factory for creating and managing instances of <see cref="System.Data.IDbConnection"/> 
/// specifically configured for PostgreSQL database connections.
/// </summary>
/// <remarks>
/// This class provides an implementation of the <see cref="Portfolio.Application.Abstractions.Data.ISqlConnectionFactory"/> 
/// interface, enabling the creation of open database connections using a predefined connection string.
/// </remarks>
internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates and opens a new instance of <see cref="System.Data.IDbConnection"/> configured for PostgreSQL.
    /// </summary>
    /// <returns>
    /// An open <see cref="System.Data.IDbConnection"/> instance.
    /// </returns>
    /// <remarks>
    /// This method initializes a new connection using the predefined connection string and opens it
    /// before returning. Ensure proper disposal of the connection after use to avoid resource leaks.
    /// </remarks>
    /// <exception cref="Npgsql.NpgsqlException">
    /// Thrown if the connection cannot be established.
    /// </exception>
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        
        connection.Open();
        
        return connection;
    }
}
