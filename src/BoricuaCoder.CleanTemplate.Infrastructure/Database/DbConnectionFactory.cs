using System.Data;
using BoricuaCoder.CleanTemplate.Application.Common.Interfaces;
using Npgsql;

namespace BoricuaCoder.CleanTemplate.Infrastructure.Database;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
