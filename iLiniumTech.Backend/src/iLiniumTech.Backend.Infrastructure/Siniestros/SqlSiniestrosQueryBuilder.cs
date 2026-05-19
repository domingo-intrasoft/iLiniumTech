using System.Data;
using iLiniumTech.Backend.Application.Siniestros;
using iLiniumTech.Backend.Domain.Siniestros;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Infrastructure.Siniestros;

public sealed class SqlSiniestrosQueryBuilder
{
    private const string FechaSiniestroExpression =
        "CONVERT(date, COALESCE([sin].[F_Siniestro], [sin].[F_Parte], [sin].[FCR], SYSUTCDATETIME()))";
    private const string FechaParteExpression =
        "CONVERT(date, COALESCE([sin].[F_Parte], [sin].[F_Siniestro], [sin].[FCR], SYSUTCDATETIME()))";
    private const string ReferenciaExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([sin].[ReferenciaMediador] AS nvarchar(50)))), ''), NULLIF(LTRIM(RTRIM(CAST([sin].[ReferenciaCia] AS nvarchar(50)))), ''), NULLIF(LTRIM(RTRIM(CAST([sin].[ReferenciaCliente] AS nvarchar(50)))), ''), CONCAT(N'SIN-', CONVERT(nvarchar(20), [sin].[Id])))";
    private const string EstadoExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([estado].[Descripcion] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CAST([sin].[IdEstado] AS nvarchar(100)))), ''), N'No informado')";
    private const string SituacionExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([situacion].[Descripcion] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CAST([sin].[IdSituacion] AS nvarchar(100)))), ''), N'No informado')";
    private const string PrioridadExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([prioridad].[Descripcion] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CAST([sin].[IdPrioridad] AS nvarchar(100)))), ''), N'Media')";

    private static readonly IReadOnlyDictionary<string, string> SortColumns =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["referencia"] = "[Referencia]",
            ["poliza"] = "[Poliza]",
            ["estado"] = "[Estado]",
            ["prioridad"] = "[Prioridad]",
            ["compania"] = "[Compania]",
            ["fechaSiniestro"] = "[FechaSiniestro]",
            ["fechaParte"] = "[FechaParte]"
        };

    public PolizasSqlQuery BuildCountQuery(SiniestrosSearchRequest request, int brokerId) =>
        new($"""
            SELECT COUNT_BIG(1)
            {BuildFromClause()}
            {BuildWhereClause(request)}
            """,
            BuildParameters(request, brokerId));

    public PolizasSqlQuery BuildSearchQuery(SiniestrosSearchRequest request, SiniestrosSort sort, int brokerId)
    {
        var orderByColumn = SortColumns.TryGetValue(sort.Field, out var column) ? column : "[FechaSiniestro]";
        var orderDirection = sort.Descending ? "DESC" : "ASC";
        var offset = (request.Page - 1) * request.PageSize;
        var parameters = BuildParameters(request, brokerId);
        parameters.Add(Int("@offset", offset));
        parameters.Add(Int("@pageSize", request.PageSize));

        return new PolizasSqlQuery(
            $"""
            WITH [siniestro_rows] AS (
                SELECT
                    CONVERT(nvarchar(20), [sin].[Id]) AS [Id],
                    {ReferenciaExpression} AS [Referencia],
                    COALESCE(NULLIF(LTRIM(RTRIM(CAST([poliza].[Poliza] AS nvarchar(100)))), ''), N'No informado') AS [Poliza],
                    CASE
                        WHEN [poliza].[ClienteId] IS NULL THEN N'No informado'
                        ELSE CONCAT(N'Cliente ', CONVERT(nvarchar(20), [poliza].[ClienteId]))
                    END AS [Cliente],
                    CASE
                        WHEN [poliza].[CiaId] IS NULL THEN N'No informado'
                        ELSE CONCAT(N'Cia ', CONVERT(nvarchar(20), [poliza].[CiaId]))
                    END AS [Compania],
                    {SituacionExpression} AS [Situacion],
                    {EstadoExpression} AS [Estado],
                    {PrioridadExpression} AS [Prioridad],
                    {FechaSiniestroExpression} AS [FechaSiniestro],
                    {FechaParteExpression} AS [FechaParte],
                    CASE
                        WHEN [sin].[TramitadorId] IS NULL THEN N'No asignado'
                        ELSE CONCAT(N'Tramitador ', CONVERT(nvarchar(20), [sin].[TramitadorId]))
                    END AS [Tramitador]
                {BuildFromClause()}
                {BuildWhereClause(request)}
            )
            SELECT
                [Id],
                [Referencia],
                [Poliza],
                [Cliente],
                [Compania],
                [Situacion],
                [Estado],
                [Prioridad],
                [FechaSiniestro],
                [FechaParte],
                [Tramitador]
            FROM [siniestro_rows]
            ORDER BY {orderByColumn} {orderDirection}, [Id] ASC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
            """,
            parameters);
    }

    private static string BuildFromClause() =>
        """
        FROM [dbo].[Siniestro] AS [sin]
        LEFT JOIN [dbo].[RiesgoPoliza] AS [riesgoPoliza]
            ON [riesgoPoliza].[Id] = [sin].[RiesgoPolizaId]
            AND [riesgoPoliza].[BrokerIntegracionId] = @brokerId
        LEFT JOIN [dbo].[Poliza] AS [poliza]
            ON [poliza].[Id] = [riesgoPoliza].[PolizaId]
            AND [poliza].[BrokerIntegracionId] = @brokerId
        LEFT JOIN [dbo].[Catalogo] AS [estado]
            ON [estado].[Id] = [sin].[IdEstado]
        LEFT JOIN [dbo].[Catalogo] AS [situacion]
            ON [situacion].[Id] = [sin].[IdSituacion]
        LEFT JOIN [dbo].[Catalogo] AS [prioridad]
            ON [prioridad].[Id] = [sin].[IdPrioridad]
        """;

    private static string BuildWhereClause(SiniestrosSearchRequest request)
    {
        var conditions = new List<string>
        {
            "[sin].[BrokerIntegracionId] = @brokerId"
        };

        if (!string.IsNullOrWhiteSpace(request.Referencia))
        {
            conditions.Add(
                $"([sin].[Id] = TRY_CONVERT(int, @referenciaExact) OR {ReferenciaExpression} LIKE @referencia)");
        }

        if (!string.IsNullOrWhiteSpace(request.Poliza))
        {
            conditions.Add("[poliza].[Poliza] LIKE @poliza");
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            conditions.Add($"({EstadoExpression} = @estado OR [sin].[IdEstado] = @estado)");
        }

        if (!string.IsNullOrWhiteSpace(request.Prioridad))
        {
            conditions.Add($"({PrioridadExpression} = @prioridad OR [sin].[IdPrioridad] = @prioridad)");
        }

        if (request.FechaSiniestroDesde.HasValue)
        {
            conditions.Add($"{FechaSiniestroExpression} >= @fechaSiniestroDesde");
        }

        if (request.FechaSiniestroHasta.HasValue)
        {
            conditions.Add($"{FechaSiniestroExpression} <= @fechaSiniestroHasta");
        }

        return "WHERE " + string.Join("\n  AND ", conditions);
    }

    private static List<PolizasSqlParameter> BuildParameters(SiniestrosSearchRequest request, int brokerId)
    {
        var parameters = new List<PolizasSqlParameter>
        {
            Int("@brokerId", brokerId)
        };

        if (!string.IsNullOrWhiteSpace(request.Referencia))
        {
            var referencia = request.Referencia.Trim();
            parameters.Add(Text("@referencia", $"%{referencia}%", 202));
            parameters.Add(Text("@referenciaExact", referencia, 50));
        }

        if (!string.IsNullOrWhiteSpace(request.Poliza))
        {
            parameters.Add(Text("@poliza", $"%{request.Poliza.Trim()}%", 102));
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            parameters.Add(Text("@estado", request.Estado, 100));
        }

        if (!string.IsNullOrWhiteSpace(request.Prioridad))
        {
            parameters.Add(Text("@prioridad", request.Prioridad, 100));
        }

        if (request.FechaSiniestroDesde.HasValue)
        {
            parameters.Add(Date("@fechaSiniestroDesde", request.FechaSiniestroDesde.Value));
        }

        if (request.FechaSiniestroHasta.HasValue)
        {
            parameters.Add(Date("@fechaSiniestroHasta", request.FechaSiniestroHasta.Value));
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
