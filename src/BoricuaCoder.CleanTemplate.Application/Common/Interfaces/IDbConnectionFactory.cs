using System.Data;

namespace BoricuaCoder.CleanTemplate.Application.Common.Interfaces;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
