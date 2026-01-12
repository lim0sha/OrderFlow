using DataAccess.Utils.Helpers.Interfaces;
using Npgsql;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DataAccess.Utils.Helpers;

public class DbCommandExecutor : IDbCommandExecutor
{
    private readonly NpgsqlDataSource _dataSource;

    public DbCommandExecutor(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "SQL is loaded from trusted embedded resource")]
    public async Task ExecuteAsync(
        string sql,
        Action<NpgsqlParameterCollection> configureParameters,
        CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        configureParameters(command.Parameters);
        await command.ExecuteNonQueryAsync(ct);
    }

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "SQL is loaded from trusted embedded resource")]
    public async Task<T> ExecuteScalarAsync<T>(
        string sql,
        Action<NpgsqlParameterCollection> configureParameters,
        CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        configureParameters(command.Parameters);
        object? result = await command.ExecuteScalarAsync(ct);

        if (result == null)
        {
            throw new InvalidOperationException(
                "The query returned NULL, but a non-nullable value was expected.");
        }

        return (T)result;
    }

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "SQL is loaded from trusted embedded resource")]
    public async IAsyncEnumerable<T> QueryAsync<T>(
        string sql,
        Action<NpgsqlParameterCollection> configureParameters,
        Func<NpgsqlDataReader, T> map,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        configureParameters(command.Parameters);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            yield return map(reader);
        }
    }

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "SQL is loaded from trusted embedded resource")]
    public async Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        Action<NpgsqlParameterCollection> configureParameters,
        Func<NpgsqlDataReader, T> map,
        CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        configureParameters(command.Parameters);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        if (await reader.ReadAsync(ct))
        {
            return map(reader);
        }

        return default;
    }
}