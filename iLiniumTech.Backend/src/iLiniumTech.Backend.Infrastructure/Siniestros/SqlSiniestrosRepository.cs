using System.Data;
using System.Globalization;
using iLiniumTech.Backend.Application.Siniestros;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Siniestros;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;
using Microsoft.Data.SqlClient;

namespace iLiniumTech.Backend.Infrastructure.Siniestros;

public sealed class SqlSiniestrosRepository(
    IPolizasConnectionStringProvider connectionStringProvider,
    IPolizasExecutionContextAccessor executionContextAccessor,
    SqlSiniestrosQueryBuilder? queryBuilder = null)
    : ISiniestrosRepository
{
    private readonly SqlSiniestrosQueryBuilder _queryBuilder = queryBuilder ?? new SqlSiniestrosQueryBuilder();

    public async Task<PagedResult<SiniestroListItem>> SearchAsync(
        SiniestrosSearchRequest request,
        SiniestrosSort sort,
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

        return new PagedResult<SiniestroListItem>(items, request.Page, request.PageSize, checked((int)total));
    }

    public Task<SiniestrosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(new SiniestrosCatalogs(
            Estados: ["No informado", "situacion-AL", "situacion-BA"],
            Prioridades: ["Alta", "Media", "Baja", "No informado"]));

    private PolizasExecutionContext RequireExecutionContext() =>
        executionContextAccessor.Current
        ?? throw new InvalidOperationException("Siniestros SQL repository requires a broker execution context.");

    private static async Task<long> ExecuteCountAsync(
        SqlConnection connection,
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, query);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result, CultureInfo.InvariantCulture);
    }

    private static async Task<IReadOnlyList<SiniestroListItem>> ExecuteListAsync(
        SqlConnection connection,
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, query);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var items = new List<SiniestroListItem>();
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

    private static SiniestroListItem ReadListItem(IDataRecord record) =>
        new(
            Id: GetString(record, "Id"),
            Referencia: GetString(record, "Referencia"),
            Poliza: GetString(record, "Poliza"),
            Cliente: GetString(record, "Cliente"),
            Compania: GetString(record, "Compania"),
            Situacion: GetString(record, "Situacion"),
            Estado: GetString(record, "Estado"),
            Prioridad: GetString(record, "Prioridad"),
            FechaSiniestro: GetDateOnly(record, "FechaSiniestro"),
            FechaParte: GetDateOnly(record, "FechaParte"),
            Tramitador: GetString(record, "Tramitador"));

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
