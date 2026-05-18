using System.Data;
using System.Globalization;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;
using Microsoft.Data.SqlClient;

namespace iLiniumTech.Backend.Infrastructure.Polizas;

public sealed class SqlPolizasRepository(
    IPolizasConnectionStringProvider connectionStringProvider,
    IPolizasExecutionContextAccessor? executionContextAccessor = null,
    PolizasSqlQueryBuilder? queryBuilder = null)
    : IPolizasRepository
{
    private readonly PolizasSqlQueryBuilder _queryBuilder = queryBuilder ?? new PolizasSqlQueryBuilder();

    public SqlPolizasRepository(string connectionString, PolizasSqlQueryBuilder? queryBuilder = null)
        : this(new StaticPolizasConnectionStringProvider(connectionString), null, queryBuilder)
    {
    }

    public async Task<PagedResult<PolizaListItem>> SearchAsync(
        PolizasSearchRequest request,
        PolizasSort sort,
        CancellationToken cancellationToken)
    {
        var connectionString = await connectionStringProvider.GetConnectionStringAsync(cancellationToken);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await SqlServerSessionContext.ApplyAsync(connection, executionContextAccessor?.Current, cancellationToken);

        var total = await ExecuteCountAsync(connection, _queryBuilder.BuildCountQuery(request), cancellationToken);
        var items = await ExecuteListAsync(connection, _queryBuilder.BuildSearchQuery(request, sort), cancellationToken);

        return new PagedResult<PolizaListItem>(items, request.Page, request.PageSize, checked((int)total));
    }

    public async Task<PolizaDetail?> GetByIdAsync(string id, CancellationToken cancellationToken, string? ramo = null)
    {
        var connectionString = await connectionStringProvider.GetConnectionStringAsync(cancellationToken);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await SqlServerSessionContext.ApplyAsync(connection, executionContextAccessor?.Current, cancellationToken);

        await using var command = CreateCommand(connection, _queryBuilder.BuildDetailQuery(id, ramo));
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);

        return await reader.ReadAsync(cancellationToken)
            ? ReadDetail(reader)
            : null;
    }

    public Task<PolizasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(PolizasCatalogProvider.CreateSqlMvpDefaults());

    private static async Task<long> ExecuteCountAsync(
        SqlConnection connection,
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, query);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result, CultureInfo.InvariantCulture);
    }

    private static async Task<IReadOnlyList<PolizaListItem>> ExecuteListAsync(
        SqlConnection connection,
        PolizasSqlQuery query,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, query);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var items = new List<PolizaListItem>();
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
            command.Parameters.AddWithValue(parameter.Name, parameter.Value);
        }

        return command;
    }

    private static PolizaListItem ReadListItem(IDataRecord record) =>
        new(
            Id: GetString(record, "Id"),
            Numero: GetString(record, "Numero"),
            Aplicacion: GetString(record, "Aplicacion"),
            Estado: GetString(record, "Estado"),
            Ramo: GetString(record, "Ramo"),
            ClienteId: GetString(record, "ClienteId"),
            ClienteNombre: GetString(record, "ClienteNombre"),
            Compania: GetString(record, "Compania"),
            FechaEfecto: GetDateOnly(record, "FechaEfecto"),
            FechaVencimiento: GetDateOnly(record, "FechaVencimiento"),
            PrimaAnual: GetDecimal(record, "PrimaAnual"),
            Moneda: GetString(record, "Moneda"));

    private static PolizaDetail ReadDetail(IDataRecord record) =>
        new(
            Id: GetString(record, "Id"),
            Numero: GetString(record, "Numero"),
            Certificado: GetString(record, "Certificado"),
            TipoPoliza: GetString(record, "TipoPoliza"),
            Aplicacion: GetString(record, "Aplicacion"),
            Estado: GetString(record, "Estado"),
            Ramo: GetString(record, "Ramo"),
            ClienteId: GetString(record, "ClienteId"),
            ClienteNombre: GetString(record, "ClienteNombre"),
            Compania: GetString(record, "Compania"),
            Riesgo: GetString(record, "Riesgo"),
            FechaEfecto: GetDateOnly(record, "FechaEfecto"),
            FechaVencimiento: GetDateOnly(record, "FechaVencimiento"),
            PrimaAnual: GetDecimal(record, "PrimaAnual"),
            Moneda: GetString(record, "Moneda"),
            Oficina: GetString(record, "Oficina"),
            Division: GetString(record, "Division"),
            Colaborador1: GetString(record, "Colaborador1"),
            Administrativo: GetString(record, "Administrativo"),
            Comercial: GetString(record, "Comercial"),
            Siniestros: GetString(record, "Siniestros"),
            Gestor: GetString(record, "Gestor"),
            CanalCobro: GetString(record, "CanalCobro"),
            FraccionPago: GetString(record, "FraccionPago"),
            Ccaa: GetString(record, "Ccaa"),
            Documento: GetString(record, "Documento"),
            Apellido1: GetString(record, "Apellido1"),
            Apellido2: GetString(record, "Apellido2"),
            Nombre: GetString(record, "Nombre"),
            Sexo: GetString(record, "Sexo"),
            FechaNacimiento: GetDateOnly(record, "FechaNacimiento"),
            Edad: GetInt32(record, "Edad"),
            EstadoCivil: GetString(record, "EstadoCivil"),
            Hijos: GetInt32(record, "Hijos"),
            RegimenLaboral: GetString(record, "RegimenLaboral"),
            Profesion: GetString(record, "Profesion"),
            Email: GetString(record, "Email"),
            Telefono: GetString(record, "Telefono"));

    private static string GetString(IDataRecord record, string name)
    {
        var value = record[name];
        return value is DBNull ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static DateOnly GetDateOnly(IDataRecord record, string name)
    {
        var value = record[name];
        if (value is DBNull)
        {
            return DateOnly.MinValue;
        }

        if (value is DateOnly dateOnly)
        {
            return dateOnly;
        }

        if (value is DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }

        return DateOnly.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var parsed)
            ? parsed
            : DateOnly.MinValue;
    }

    private static decimal GetDecimal(IDataRecord record, string name)
    {
        var value = record[name];
        return value is DBNull ? 0m : Convert.ToDecimal(value, CultureInfo.InvariantCulture);
    }

    private static int GetInt32(IDataRecord record, string name)
    {
        var value = record[name];
        return value is DBNull ? 0 : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }
}
