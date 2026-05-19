using System.Data;
using iLiniumTech.Backend.Application.Clientes;
using iLiniumTech.Backend.Domain.Clientes;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Infrastructure.Clientes;

public sealed class SqlClientesQueryBuilder
{
    private const string FechaAltaExpression =
        "CONVERT(date, COALESCE([cliente].[FCR], [identidad].[FCR], SYSUTCDATETIME()))";

    private static readonly IReadOnlyDictionary<string, string> SortColumns =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["referencia"] = "[Referencia]",
            ["alias"] = "[Alias]",
            ["estado"] = "[Estado]",
            ["segmento"] = "[Segmento]",
            ["fechaAlta"] = "[FechaAlta]"
        };

    public PolizasSqlQuery BuildCountQuery(ClientesSearchRequest request, int brokerId) =>
        new($"""
            SELECT COUNT_BIG(1)
            FROM [dbo].[IdentidadCliente] AS [cliente]
            INNER JOIN [dbo].[Identidad] AS [identidad] ON [identidad].[Id] = [cliente].[ClienteId]
            {BuildWhereClause(request)}
            """,
            BuildParameters(request, brokerId));

    public PolizasSqlQuery BuildSearchQuery(ClientesSearchRequest request, ClientesSort sort, int brokerId)
    {
        var orderByColumn = SortColumns.TryGetValue(sort.Field, out var column) ? column : "[Referencia]";
        var orderDirection = sort.Descending ? "DESC" : "ASC";
        var offset = (request.Page - 1) * request.PageSize;
        var parameters = BuildParameters(request, brokerId);
        parameters.Add(Int("@offset", offset));
        parameters.Add(Int("@pageSize", request.PageSize));

        return new PolizasSqlQuery(
            $"""
            WITH [cliente_rows] AS (
                SELECT
                    CONVERT(nvarchar(100), [cliente].[ClienteId]) AS [Id],
                    CONCAT(N'CLI-', CONVERT(nvarchar(50), [cliente].[ClienteId])) AS [Referencia],
                    COALESCE(
                        NULLIF(LTRIM(RTRIM(CAST([identidad].[RazonSocial] AS nvarchar(250)))), ''),
                        NULLIF(LTRIM(RTRIM(CAST([identidad].[NombreCompleto] AS nvarchar(250)))), ''),
                        CONCAT(N'Cliente ', CONVERT(nvarchar(50), [cliente].[ClienteId]))) AS [Alias],
                    N'Activo' AS [Estado],
                    CASE
                        WHEN NULLIF(LTRIM(RTRIM(CAST([identidad].[RazonSocial] AS nvarchar(250)))), '') IS NULL
                            THEN N'Particular'
                        ELSE N'Empresa'
                    END AS [Segmento],
                    {FechaAltaExpression} AS [FechaAlta],
                    N'BBDD local read-only' AS [Resultado],
                    N'PII minimizada' AS [Datos],
                    N'Tabs relacionadas bloqueadas' AS [Relacionadas]
                FROM [dbo].[IdentidadCliente] AS [cliente]
                INNER JOIN [dbo].[Identidad] AS [identidad] ON [identidad].[Id] = [cliente].[ClienteId]
                {BuildWhereClause(request)}
            )
            SELECT
                [Id],
                [Referencia],
                [Alias],
                [Estado],
                [Segmento],
                [FechaAlta],
                [Resultado],
                [Datos],
                [Relacionadas]
            FROM [cliente_rows]
            ORDER BY {orderByColumn} {orderDirection}, [Id] ASC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
            """,
            parameters);
    }

    private static string BuildWhereClause(ClientesSearchRequest request)
    {
        var conditions = new List<string> { "[identidad].[BrokerIntegracionId] = @brokerId" };

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            conditions.Add(
                "([cliente].[ClienteId] = TRY_CONVERT(int, @textoExact) OR [identidad].[NombreCompleto] LIKE @texto OR [identidad].[RazonSocial] LIKE @texto)");
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            conditions.Add(BuildEstadoCondition(request.Estado));
        }

        if (!string.IsNullOrWhiteSpace(request.Segmento))
        {
            conditions.Add(BuildSegmentoCondition(request.Segmento));
        }

        if (request.FechaAltaDesde.HasValue)
        {
            conditions.Add($"{FechaAltaExpression} >= @fechaAltaDesde");
        }

        if (request.FechaAltaHasta.HasValue)
        {
            conditions.Add($"{FechaAltaExpression} <= @fechaAltaHasta");
        }

        return "WHERE " + string.Join("\n  AND ", conditions);
    }

    private static string BuildEstadoCondition(string estado) =>
        estado.Trim().Equals("Activo", StringComparison.OrdinalIgnoreCase) ? "1 = 1" : "1 = 0";

    private static string BuildSegmentoCondition(string segmento) =>
        segmento.Trim().ToLowerInvariant() switch
        {
            "particular" => "NULLIF(LTRIM(RTRIM(CAST([identidad].[RazonSocial] AS nvarchar(250)))), '') IS NULL",
            "empresa" => "NULLIF(LTRIM(RTRIM(CAST([identidad].[RazonSocial] AS nvarchar(250)))), '') IS NOT NULL",
            _ => "1 = 0"
        };

    private static List<PolizasSqlParameter> BuildParameters(ClientesSearchRequest request, int brokerId)
    {
        var parameters = new List<PolizasSqlParameter> { Int("@brokerId", brokerId) };

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            parameters.Add(Text("@texto", $"%{texto}%", 202));
            parameters.Add(Text("@textoExact", texto, 100));
        }

        if (request.FechaAltaDesde.HasValue)
        {
            parameters.Add(Date("@fechaAltaDesde", request.FechaAltaDesde.Value));
        }

        if (request.FechaAltaHasta.HasValue)
        {
            parameters.Add(Date("@fechaAltaHasta", request.FechaAltaHasta.Value));
        }

        return parameters;
    }

    private static PolizasSqlParameter Text(string name, string value, int size) =>
        new(name, value.Trim(), SqlDbType.NVarChar, size);

    private static PolizasSqlParameter Int(string name, int value) =>
        new(name, value, SqlDbType.Int);

    private static PolizasSqlParameter Date(string name, DateOnly value) =>
        new(name, value.ToDateTime(TimeOnly.MinValue), SqlDbType.Date);
}
