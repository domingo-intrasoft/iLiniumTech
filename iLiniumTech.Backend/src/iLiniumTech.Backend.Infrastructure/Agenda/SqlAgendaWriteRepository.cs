using System.Data;
using System.Globalization;
using iLiniumTech.Backend.Application.Agenda;
using iLiniumTech.Backend.Domain.Agenda;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;
using Microsoft.Data.SqlClient;

namespace iLiniumTech.Backend.Infrastructure.Agenda;

public sealed class SqlAgendaWriteRepository(
    IPolizasConnectionStringProvider connectionStringProvider,
    IPolizasExecutionContextAccessor executionContextAccessor,
    SqlAgendaCommandBuilder? commandBuilder = null)
    : IAgendaWriteRepository
{
    private readonly SqlAgendaCommandBuilder _commandBuilder = commandBuilder ?? new SqlAgendaCommandBuilder();

    public async Task<AgendaCreateResult> CreateAsync(AgendaCreateRequest request, CancellationToken cancellationToken)
    {
        var executionContext = RequireExecutionContext();
        var connectionString = await connectionStringProvider.GetConnectionStringAsync(cancellationToken);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await SqlServerSessionContext.ApplyAsync(connection, executionContext, cancellationToken);

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await using var command = CreateCommand(
                connection,
                transaction,
                _commandBuilder.BuildCreateCommand(request, executionContext.BrokerId, executionContext.UserId));
            var result = await command.ExecuteScalarAsync(cancellationToken);
            var id = Convert.ToString(result, CultureInfo.InvariantCulture) ?? string.Empty;
            if (!int.TryParse(id, CultureInfo.InvariantCulture, out var parsedId))
            {
                throw new InvalidOperationException("Agenda SQL create did not return a numeric identifier.");
            }

            await EnsureCreatedRowIsVisibleAsync(
                connection,
                transaction,
                parsedId,
                executionContext.BrokerId,
                cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new AgendaCreateResult(id);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(string id, AgendaUpdateRequest request, CancellationToken cancellationToken)
    {
        var parsedId = AgendaWriteValidator.ValidateAndParseId(id);
        var executionContext = RequireExecutionContext();
        var result = await ExecuteScalarInTransactionAsync(
            _commandBuilder.BuildUpdateCommand(parsedId, request, executionContext.BrokerId, executionContext.UserId),
            cancellationToken);

        return Convert.ToInt32(result, CultureInfo.InvariantCulture) > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var parsedId = AgendaWriteValidator.ValidateAndParseId(id);
        var executionContext = RequireExecutionContext();
        var result = await ExecuteScalarInTransactionAsync(
            _commandBuilder.BuildDeleteCommand(parsedId, executionContext.BrokerId, executionContext.UserId),
            cancellationToken);

        return Convert.ToInt32(result, CultureInfo.InvariantCulture) > 0;
    }

    private async Task<object?> ExecuteScalarInTransactionAsync(
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        var executionContext = RequireExecutionContext();
        var connectionString = await connectionStringProvider.GetConnectionStringAsync(cancellationToken);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await SqlServerSessionContext.ApplyAsync(connection, executionContext, cancellationToken);

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
        int brokerId,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(
            connection,
            transaction,
            _commandBuilder.BuildCreatedVisibilityQuery(id, brokerId));
        var visibleRows = Convert.ToInt64(await command.ExecuteScalarAsync(cancellationToken), CultureInfo.InvariantCulture);
        if (visibleRows <= 0)
        {
            throw new AgendaValidationException("Created MVP agenda event is not visible for the active broker context.");
        }
    }

    private PolizasExecutionContext RequireExecutionContext() =>
        executionContextAccessor.Current
        ?? throw new InvalidOperationException("Agenda SQL repository requires a broker execution context.");

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
            var sqlParameter = parameter.Size.HasValue && parameter.DbType.HasValue
                ? command.Parameters.Add(parameter.Name, parameter.DbType.Value, parameter.Size.Value)
                : parameter.DbType.HasValue
                    ? command.Parameters.Add(parameter.Name, parameter.DbType.Value)
                    : command.Parameters.AddWithValue(parameter.Name, parameter.Value);
            sqlParameter.Value = parameter.Value;
        }

        return command;
    }
}
