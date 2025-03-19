using Npgsql;
using System.Data;
using Portfolio.Application.Abstractions.Data;

namespace Portfolio.Infrastructure.Data;

internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        
        connection.Open();
        
        return connection;
    }
}
