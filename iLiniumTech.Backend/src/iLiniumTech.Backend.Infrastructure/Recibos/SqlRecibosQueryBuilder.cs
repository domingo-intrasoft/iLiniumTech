using System.Data;
using iLiniumTech.Backend.Application.Recibos;
using iLiniumTech.Backend.Domain.Recibos;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Infrastructure.Recibos;

public sealed class SqlRecibosQueryBuilder
{
    private const string ReciboExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([rec].[ReciboCia] AS nvarchar(50)))), ''), CONCAT(N'REC-', CONVERT(nvarchar(20), [rec].[Id])))";
    private const string FechaEfectoExpression =
        "CONVERT(date, COALESCE([rec].[F_Efecto], [rec].[FCR], SYSUTCDATETIME()))";
    private const string FechaVencimientoExpression =
        "CONVERT(date, COALESCE([rec].[F_Vencimiento], [rec].[F_Efecto], [rec].[FCR], SYSUTCDATETIME()))";
    private const string SituacionExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([situacion].[Descripcion] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CAST([rec].[IdSituacion] AS nvarchar(100)))), ''), N'No informado')";
    private const string TipoExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([tipoCia].[Descripcion] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CAST([tipo].[Descripcion] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CAST([rec].[IdTipoReciboCia] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CAST([rec].[IdTipoRecibo] AS nvarchar(100)))), ''), N'No informado')";
    private const string CanalExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([canal].[Descripcion] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CAST([rec].[IdCanalCobro] AS nvarchar(100)))), ''), N'No informado')";

    private static readonly IReadOnlyDictionary<string, string> SortColumns =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["recibo"] = "[Recibo]",
            ["poliza"] = "[Poliza]",
            ["situacion"] = "[Situacion]",
            ["tipo"] = "[Tipo]",
            ["compania"] = "[Compania]",
            ["fechaEfecto"] = "[FechaEfecto]",
            ["fechaVencimiento"] = "[FechaVencimiento]"
        };

    public PolizasSqlQuery BuildCountQuery(RecibosSearchRequest request, int brokerId) =>
        new($"""
            SELECT COUNT_BIG(1)
            {BuildFromClause()}
            {BuildWhereClause(request)}
            """,
            BuildParameters(request, brokerId));

    public PolizasSqlQuery BuildSearchQuery(RecibosSearchRequest request, RecibosSort sort, int brokerId)
    {
        var orderByColumn = SortColumns.TryGetValue(sort.Field, out var column) ? column : "[FechaVencimiento]";
        var orderDirection = sort.Descending ? "DESC" : "ASC";
        var offset = (request.Page - 1) * request.PageSize;
        var parameters = BuildParameters(request, brokerId);
        parameters.Add(Int("@offset", offset));
        parameters.Add(Int("@pageSize", request.PageSize));

        return new PolizasSqlQuery(
            $"""
            WITH [recibo_rows] AS (
                SELECT
                    CONVERT(nvarchar(20), [rec].[Id]) AS [Id],
                    {ReciboExpression} AS [Recibo],
                    COALESCE(NULLIF(LTRIM(RTRIM(CAST([poliza].[Poliza] AS nvarchar(100)))), ''), N'No informado') AS [Poliza],
                    CASE
                        WHEN [poliza].[ClienteId] IS NULL THEN N'No informado'
                        ELSE CONCAT(N'Cliente ', CONVERT(nvarchar(20), [poliza].[ClienteId]))
                    END AS [Cliente],
                    CASE
                        WHEN [poliza].[CiaId] IS NULL THEN N'No informado'
                        ELSE CONCAT(N'Cia ', CONVERT(nvarchar(20), [poliza].[CiaId]))
                    END AS [Compania],
                    {TipoExpression} AS [Tipo],
                    {SituacionExpression} AS [Situacion],
                    {FechaEfectoExpression} AS [FechaEfecto],
                    {FechaVencimientoExpression} AS [FechaVencimiento],
                    CASE
                        WHEN [rec].[F_Cobro] IS NOT NULL OR [rec].[F_CobroMediador] IS NOT NULL THEN N'Cobro registrado'
                        ELSE N'No operativo'
                    END AS [EstadoCobro],
                    {CanalExpression} AS [Canal]
                {BuildFromClause()}
                {BuildWhereClause(request)}
            )
            SELECT
                [Id],
                [Recibo],
                [Poliza],
                [Cliente],
                [Compania],
                [Tipo],
                [Situacion],
                [FechaEfecto],
                [FechaVencimiento],
                [EstadoCobro],
                [Canal]
            FROM [recibo_rows]
            ORDER BY {orderByColumn} {orderDirection}, [Id] ASC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
            """,
            parameters);
    }

    private static string BuildFromClause() =>
        """
        FROM [dbo].[Recibo] AS [rec]
        LEFT JOIN [dbo].[Poliza] AS [poliza]
            ON [poliza].[Id] = [rec].[PolizaId]
            AND [poliza].[BrokerIntegracionId] = @brokerId
        LEFT JOIN [dbo].[Catalogo] AS [situacion]
            ON [situacion].[Id] = [rec].[IdSituacion]
        LEFT JOIN [dbo].[Catalogo] AS [tipo]
            ON [tipo].[Id] = [rec].[IdTipoRecibo]
        LEFT JOIN [dbo].[Catalogo] AS [tipoCia]
            ON [tipoCia].[Id] = [rec].[IdTipoReciboCia]
        LEFT JOIN [dbo].[Catalogo] AS [canal]
            ON [canal].[Id] = [rec].[IdCanalCobro]
        """;

    private static string BuildWhereClause(RecibosSearchRequest request)
    {
        var conditions = new List<string>
        {
            "[rec].[BrokerIntegracionId] = @brokerId"
        };

        if (!string.IsNullOrWhiteSpace(request.Recibo))
        {
            conditions.Add($"([rec].[Id] = TRY_CONVERT(int, @reciboExact) OR {ReciboExpression} LIKE @recibo)");
        }

        if (!string.IsNullOrWhiteSpace(request.Poliza))
        {
            conditions.Add("[poliza].[Poliza] LIKE @poliza");
        }

        if (!string.IsNullOrWhiteSpace(request.Situacion))
        {
            conditions.Add($"({SituacionExpression} = @situacion OR CONVERT(nvarchar(100), [rec].[IdSituacion]) = @situacion)");
        }

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            conditions.Add($"({TipoExpression} = @tipo OR CONVERT(nvarchar(100), [rec].[IdTipoRecibo]) = @tipo OR CONVERT(nvarchar(100), [rec].[IdTipoReciboCia]) = @tipo)");
        }

        if (request.FechaVencimientoDesde.HasValue)
        {
            conditions.Add($"{FechaVencimientoExpression} >= @fechaVencimientoDesde");
        }

        if (request.FechaVencimientoHasta.HasValue)
        {
            conditions.Add($"{FechaVencimientoExpression} <= @fechaVencimientoHasta");
        }

        return "WHERE " + string.Join("\n  AND ", conditions);
    }

    private static List<PolizasSqlParameter> BuildParameters(RecibosSearchRequest request, int brokerId)
    {
        var parameters = new List<PolizasSqlParameter>
        {
            Int("@brokerId", brokerId)
        };

        if (!string.IsNullOrWhiteSpace(request.Recibo))
        {
            var recibo = request.Recibo.Trim();
            parameters.Add(Text("@recibo", $"%{recibo}%", 202));
            parameters.Add(Text("@reciboExact", recibo, 50));
        }

        if (!string.IsNullOrWhiteSpace(request.Poliza))
        {
            parameters.Add(Text("@poliza", $"%{request.Poliza.Trim()}%", 102));
        }

        if (!string.IsNullOrWhiteSpace(request.Situacion))
        {
            parameters.Add(Text("@situacion", request.Situacion, 100));
        }

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            parameters.Add(Text("@tipo", request.Tipo, 100));
        }

        if (request.FechaVencimientoDesde.HasValue)
        {
            parameters.Add(Date("@fechaVencimientoDesde", request.FechaVencimientoDesde.Value));
        }

        if (request.FechaVencimientoHasta.HasValue)
        {
            parameters.Add(Date("@fechaVencimientoHasta", request.FechaVencimientoHasta.Value));
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
