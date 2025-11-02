using Npgsql;

namespace DataAccess.Utils.Helpers.Interfaces;

public interface IDbCommandExecutor
{
    Task ExecuteAsync(
        string sql,
        Action<NpgsqlParameterCollection> configureParameters,
        CancellationToken ct = default);

    Task<T> ExecuteScalarAsync<T>(
        string sql,
        Action<NpgsqlParameterCollection> configureParameters,
        CancellationToken ct = default);

    IAsyncEnumerable<T> QueryAsync<T>(
        string sql,
        Action<NpgsqlParameterCollection> configureParameters,
        Func<NpgsqlDataReader, T> map,
        CancellationToken ct = default);
}