using System.Data;
using System.Globalization;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;
using Microsoft.Data.SqlClient;

namespace iLiniumTech.Backend.Infrastructure.Polizas;

public sealed class SqlPolizasWriteRepository(
    IPolizasConnectionStringProvider connectionStringProvider,
    IPolizasExecutionContextAccessor? executionContextAccessor = null,
    PolizasSqlCommandBuilder? commandBuilder = null)
    : IPolizasWriteRepository
{
    private readonly PolizasSqlCommandBuilder _commandBuilder = commandBuilder ?? new PolizasSqlCommandBuilder();

    public async Task<PolizaCreateResult> CreateAsync(PolizaCreateRequest request, CancellationToken cancellationToken)
    {
        var connectionString = await connectionStringProvider.GetConnectionStringAsync(cancellationToken);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await SqlServerSessionContext.ApplyAsync(connection, executionContextAccessor?.Current, cancellationToken);

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await using var command = CreateCommand(connection, transaction, _commandBuilder.BuildCreateCommand(request));
            var result = await command.ExecuteScalarAsync(cancellationToken);
            var id = Convert.ToString(result, CultureInfo.InvariantCulture) ?? string.Empty;
            if (!int.TryParse(id, CultureInfo.InvariantCulture, out var parsedId))
            {
                throw new InvalidOperationException("Polizas SQL create did not return a numeric identifier.");
            }

            await EnsureCreatedRowIsVisibleAsync(connection, transaction, parsedId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new PolizaCreateResult(id);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, PolizaUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await ExecuteScalarInTransactionAsync(
            _commandBuilder.BuildUpdateCommand(id, request),
            cancellationToken);

        return Convert.ToInt32(result, CultureInfo.InvariantCulture) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var result = await ExecuteScalarInTransactionAsync(
            _commandBuilder.BuildDeleteCommand(id),
            cancellationToken);

        return Convert.ToInt32(result, CultureInfo.InvariantCulture) > 0;
    }

    private async Task<object?> ExecuteScalarInTransactionAsync(
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        var connectionString = await connectionStringProvider.GetConnectionStringAsync(cancellationToken);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await SqlServerSessionContext.ApplyAsync(connection, executionContextAccessor?.Current, cancellationToken);

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await using var command = CreateCommand(connection, transaction, query);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task EnsureCreatedRowIsVisibleAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int id,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, transaction, _commandBuilder.BuildCreatedVisibilityQuery(id));
        var visibleRows = Convert.ToInt64(await command.ExecuteScalarAsync(cancellationToken), CultureInfo.InvariantCulture);
        if (visibleRows <= 0)
        {
            throw new PolizasValidationException(
                "Created MVP poliza is not visible for the active broker/client context.");
        }
    }

    private static SqlCommand CreateCommand(
        SqlConnection connection,
        SqlTransaction transaction,
        PolizasSqlQuery query)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = query.CommandText;
        command.CommandType = CommandType.Text;

        foreach (var parameter in query.Parameters)
        {
            var sqlParameter = command.Parameters.Add(parameter.Name, parameter.DbType ?? SqlDbType.Variant);
            sqlParameter.Value = parameter.Value;
            if (parameter.Size is not null)
            {
                sqlParameter.Size = parameter.Size.Value;
            }
        }

        return command;
    }
}
