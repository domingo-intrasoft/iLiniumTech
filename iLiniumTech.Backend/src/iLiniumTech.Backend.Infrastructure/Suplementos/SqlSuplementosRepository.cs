using System.Data;
using System.Globalization;
using iLiniumTech.Backend.Application.Suplementos;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Suplementos;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;
using Microsoft.Data.SqlClient;

namespace iLiniumTech.Backend.Infrastructure.Suplementos;

public sealed class SqlSuplementosRepository(
    IPolizasConnectionStringProvider connectionStringProvider,
    IPolizasExecutionContextAccessor executionContextAccessor,
    SqlSuplementosQueryBuilder? queryBuilder = null)
    : ISuplementosRepository
{
    private readonly SqlSuplementosQueryBuilder _queryBuilder = queryBuilder ?? new SqlSuplementosQueryBuilder();

    public async Task<PagedResult<SuplementoListItem>> SearchAsync(
        SuplementosSearchRequest request,
        SuplementosSort sort,
        CancellationToken cancellationToken)
    {
        var executionContext = RequireExecutionContext();
        var connectionString = await connectionStringProvider.GetConnectionStringAsync(cancellationToken);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await SqlServerSessionContext.ApplyAsync(connection, executionContext, cancellationToken);

        var total = await ExecuteCountAsync(
            connection,
            _queryBuilder.BuildCountQuery(request, executionContext.BrokerId),
            cancellationToken);
        var items = await ExecuteListAsync(
            connection,
            _queryBuilder.BuildSearchQuery(request, sort, executionContext.BrokerId),
            cancellationToken);

        return new PagedResult<SuplementoListItem>(items, request.Page, request.PageSize, checked((int)total));
    }

    public Task<SuplementosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(new SuplementosCatalogs(
            Tipos: ["No informado"],
            Situaciones: ["No informado"]));

    private PolizasExecutionContext RequireExecutionContext() =>
        executionContextAccessor.Current
        ?? throw new InvalidOperationException("Suplementos SQL repository requires a broker execution context.");

    private static async Task<long> ExecuteCountAsync(
        SqlConnection connection,
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, query);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result, CultureInfo.InvariantCulture);
    }

    private static async Task<IReadOnlyList<SuplementoListItem>> ExecuteListAsync(
        SqlConnection connection,
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, query);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var items = new List<SuplementoListItem>();
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(ReadListItem(reader));
        }

        return items;
    }

    private static SqlCommand CreateCommand(SqlConnection connection, PolizasSqlQuery query)
    {
        var command = connection.CreateCommand();
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

    private static SuplementoListItem ReadListItem(IDataRecord record) =>
        new(
            Id: GetString(record, "Id"),
            Referencia: GetString(record, "Referencia"),
            Poliza: GetString(record, "Poliza"),
            Tipo: GetString(record, "Tipo"),
            Situacion: GetString(record, "Situacion"),
            FechaEfecto: GetDateOnly(record, "FechaEfecto"),
            Concepto: GetString(record, "Concepto"),
            Resumen: GetString(record, "Resumen"));

    private static string GetString(IDataRecord record, string name)
    {
        var value = record[name];
        return value is DBNull ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static DateOnly GetDateOnly(IDataRecord record, string name)
    {
        var value = record[name];
        return value is DBNull
            ? DateOnly.MinValue
            : DateOnly.FromDateTime(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
    }
}
