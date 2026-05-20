using System.Data;
using iLiniumTech.Backend.Application.Suplementos;
using iLiniumTech.Backend.Domain.Suplementos;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Infrastructure.Suplementos;

public sealed class SqlSuplementosQueryBuilder
{
    private const string ReferenciaExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([sup].[Referencia] AS nvarchar(50)))), ''), NULLIF(LTRIM(RTRIM(CAST([sup].[ReferenciaCia] AS nvarchar(50)))), ''), CONCAT(N'SUP-', CONVERT(nvarchar(20), [sup].[Id])))";
    private const string FechaEfectoExpression =
        "CONVERT(date, COALESCE([sup].[F_Efecto], [sup].[FCR], SYSUTCDATETIME()))";
    private const string TipoExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([tipo].[Descripcion] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CONVERT(nvarchar(100), [sup].[IdTipo]))), ''), N'No informado')";
    private const string SituacionExpression =
        "COALESCE(NULLIF(LTRIM(RTRIM(CAST([situacion].[Descripcion] AS nvarchar(100)))), ''), NULLIF(LTRIM(RTRIM(CONVERT(nvarchar(100), [sup].[IdSituacion]))), ''), N'No informado')";

    private static readonly IReadOnlyDictionary<string, string> SortColumns =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["referencia"] = "[Referencia]",
            ["poliza"] = "[Poliza]",
            ["tipo"] = "[Tipo]",
            ["situacion"] = "[Situacion]",
            ["fechaEfecto"] = "[FechaEfecto]"
        };

    public PolizasSqlQuery BuildCountQuery(SuplementosSearchRequest request, int brokerId) =>
        new($"""
            SELECT COUNT_BIG(1)
            {BuildFromClause()}
            {BuildWhereClause(request)}
            """,
            BuildParameters(request, brokerId));

    public PolizasSqlQuery BuildSearchQuery(SuplementosSearchRequest request, SuplementosSort sort, int brokerId)
    {
        var orderByColumn = SortColumns.TryGetValue(sort.Field, out var column) ? column : "[FechaEfecto]";
        var orderDirection = sort.Descending ? "DESC" : "ASC";
        var offset = (request.Page - 1) * request.PageSize;
        var parameters = BuildParameters(request, brokerId);
        parameters.Add(Int("@offset", offset));
        parameters.Add(Int("@pageSize", request.PageSize));

        return new PolizasSqlQuery(
            $"""
            WITH [suplemento_rows] AS (
                SELECT
                    CONVERT(nvarchar(20), [sup].[Id]) AS [Id],
                    {ReferenciaExpression} AS [Referencia],
                    COALESCE(NULLIF(LTRIM(RTRIM(CAST([poliza].[Poliza] AS nvarchar(100)))), ''), N'No informado') AS [Poliza],
                    {TipoExpression} AS [Tipo],
                    {SituacionExpression} AS [Situacion],
                    {FechaEfectoExpression} AS [FechaEfecto],
                    N'No informado' AS [Concepto],
                    N'Listado read-only minimizado' AS [Resumen]
                {BuildFromClause()}
                {BuildWhereClause(request)}
            )
            SELECT
                [Id],
                [Referencia],
                [Poliza],
                [Tipo],
                [Situacion],
                [FechaEfecto],
                [Concepto],
                [Resumen]
            FROM [suplemento_rows]
            ORDER BY {orderByColumn} {orderDirection}, [Id] ASC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
            """,
            parameters);
    }

    private static string BuildFromClause() =>
        """
        FROM [dbo].[Suplemento] AS [sup]
        LEFT JOIN [dbo].[Poliza] AS [poliza]
            ON [poliza].[Id] = [sup].[PolizaId]
            AND [poliza].[BrokerIntegracionId] = @brokerId
        LEFT JOIN [dbo].[Catalogo] AS [tipo]
            ON [tipo].[Id] = [sup].[IdTipo]
        LEFT JOIN [dbo].[Catalogo] AS [situacion]
            ON [situacion].[Id] = [sup].[IdSituacion]
        """;

    private static string BuildWhereClause(SuplementosSearchRequest request)
    {
        var conditions = new List<string>
        {
            "[sup].[BrokerIntegracionId] = @brokerId"
        };

        if (!string.IsNullOrWhiteSpace(request.Referencia))
        {
            conditions.Add($"([sup].[Id] = TRY_CONVERT(int, @referenciaExact) OR {ReferenciaExpression} LIKE @referencia)");
        }

        if (!string.IsNullOrWhiteSpace(request.Poliza))
        {
            conditions.Add("[poliza].[Poliza] LIKE @poliza");
        }

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            conditions.Add($"({TipoExpression} = @tipo OR CONVERT(nvarchar(100), [sup].[IdTipo]) = @tipo)");
        }

        if (!string.IsNullOrWhiteSpace(request.Situacion))
        {
            conditions.Add($"({SituacionExpression} = @situacion OR CONVERT(nvarchar(100), [sup].[IdSituacion]) = @situacion)");
        }

        if (request.FechaEfectoDesde.HasValue)
        {
            conditions.Add($"{FechaEfectoExpression} >= @fechaEfectoDesde");
        }

        if (request.FechaEfectoHasta.HasValue)
        {
            conditions.Add($"{FechaEfectoExpression} <= @fechaEfectoHasta");
        }

        return "WHERE " + string.Join("\n  AND ", conditions);
    }

    private static List<PolizasSqlParameter> BuildParameters(SuplementosSearchRequest request, int brokerId)
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

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            parameters.Add(Text("@tipo", request.Tipo, 100));
        }

        if (!string.IsNullOrWhiteSpace(request.Situacion))
        {
            parameters.Add(Text("@situacion", request.Situacion, 100));
        }

        if (request.FechaEfectoDesde.HasValue)
        {
            parameters.Add(Date("@fechaEfectoDesde", request.FechaEfectoDesde.Value));
        }

        if (request.FechaEfectoHasta.HasValue)
        {
            parameters.Add(Date("@fechaEfectoHasta", request.FechaEfectoHasta.Value));
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
