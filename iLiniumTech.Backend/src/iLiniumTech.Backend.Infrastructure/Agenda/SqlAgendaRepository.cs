using System.Data;
using System.Globalization;
using iLiniumTech.Backend.Application.Agenda;
using iLiniumTech.Backend.Domain.Agenda;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;
using Microsoft.Data.SqlClient;

namespace iLiniumTech.Backend.Infrastructure.Agenda;

public sealed class SqlAgendaRepository(
    IPolizasConnectionStringProvider connectionStringProvider,
    IPolizasExecutionContextAccessor executionContextAccessor,
    SqlAgendaQueryBuilder? queryBuilder = null)
    : IAgendaRepository
{
    private readonly SqlAgendaQueryBuilder _queryBuilder = queryBuilder ?? new SqlAgendaQueryBuilder();

    public async Task<PagedResult<AgendaEventListItem>> SearchAsync(
        AgendaSearchRequest request,
        AgendaSort sort,
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

        return new PagedResult<AgendaEventListItem>(items, request.Page, request.PageSize, checked((int)total));
    }

    public Task<AgendaCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(new AgendaCatalogs(
            Estados: ["Pendiente", "Programado", "Cerrado"],
            Prioridades: ["Alta", "Media", "Baja"],
            Origenes: [AgendaMvpWriteDefaults.Origen]));

    private PolizasExecutionContext RequireExecutionContext() =>
        executionContextAccessor.Current
        ?? throw new InvalidOperationException("Agenda SQL repository requires a broker execution context.");

    private static async Task<long> ExecuteCountAsync(
        SqlConnection connection,
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, query);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result, CultureInfo.InvariantCulture);
    }

    private static async Task<IReadOnlyList<AgendaEventListItem>> ExecuteListAsync(
        SqlConnection connection,
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, query);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var items = new List<AgendaEventListItem>();
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

    private static AgendaEventListItem ReadListItem(IDataRecord record) =>
        new(
            Id: GetString(record, "Id"),
            Referencia: GetString(record, "Referencia"),
            Titulo: GetString(record, "Titulo"),
            Inicio: GetDateTime(record, "Inicio"),
            Fin: GetNullableDateTime(record, "Fin"),
            Estado: GetString(record, "Estado"),
            Prioridad: GetString(record, "Prioridad"),
            Origen: GetString(record, "Origen"),
            ObjetoRelacionadoTipo: GetString(record, "ObjetoRelacionadoTipo"));

    private static string GetString(IDataRecord record, string name)
    {
        var value = record[name];
        return value is DBNull ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static DateTime GetDateTime(IDataRecord record, string name)
    {
        var value = record[name];
        return value is DBNull ? DateTime.MinValue : Convert.ToDateTime(value, CultureInfo.InvariantCulture);
    }

    private static DateTime? GetNullableDateTime(IDataRecord record, string name)
    {
        var value = record[name];
        return value is DBNull ? null : Convert.ToDateTime(value, CultureInfo.InvariantCulture);
    }
}
