using System.Data;
using iLiniumTech.Backend.Application.Agenda;
using iLiniumTech.Backend.Domain.Agenda;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Infrastructure.Agenda;

public sealed class SqlAgendaQueryBuilder
{
    private static readonly IReadOnlyDictionary<string, string> SortColumns =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["referencia"] = "[Referencia]",
            ["titulo"] = "[Titulo]",
            ["estado"] = "[Estado]",
            ["prioridad"] = "[Prioridad]",
            ["inicio"] = "[Inicio]",
            ["fin"] = "[Fin]",
            ["origen"] = "[Origen]"
        };

    public PolizasSqlQuery BuildCountQuery(AgendaSearchRequest request, int brokerId) =>
        new($"""
            SELECT COUNT_BIG(1)
            FROM [dbo].[Agenda] AS [agenda]
            {BuildWhereClause(request)}
            """,
            BuildParameters(request, brokerId));

    public PolizasSqlQuery BuildSearchQuery(AgendaSearchRequest request, AgendaSort sort, int brokerId)
    {
        var orderByColumn = SortColumns.TryGetValue(sort.Field, out var column) ? column : "[Inicio]";
        var orderDirection = sort.Descending ? "DESC" : "ASC";
        var offset = (request.Page - 1) * request.PageSize;
        var parameters = BuildParameters(request, brokerId);
        parameters.Add(Int("@offset", offset));
        parameters.Add(Int("@pageSize", request.PageSize));

        return new PolizasSqlQuery(
            $"""
            WITH [agenda_rows] AS (
                SELECT
                    CONVERT(nvarchar(20), [agenda].[Id]) AS [Id],
                    COALESCE(NULLIF([agenda].[IdOld], ''), CONCAT(N'AGE-', CONVERT(nvarchar(20), [agenda].[Id]))) AS [Referencia],
                    COALESCE(NULLIF([agenda].[Asunto], ''), N'No informado') AS [Titulo],
                    {BuildInicioExpression()} AS [Inicio],
                    {BuildFinExpression()} AS [Fin],
                    CASE
                        WHEN [agenda].[IdOld] LIKE @mvpDeletedReferenciaLike THEN N'Cerrado'
                        WHEN [agenda].[F_Fin] IS NOT NULL AND [agenda].[F_Fin] < CONVERT(date, SYSUTCDATETIME()) THEN N'Cerrado'
                        WHEN [agenda].[F_Inicio] > CONVERT(date, SYSUTCDATETIME()) THEN N'Programado'
                        ELSE N'Pendiente'
                    END AS [Estado],
                    COALESCE(NULLIF([agenda].[IdPrioridad], ''), N'Media') AS [Prioridad],
                    @origenLabel AS [Origen],
                    COALESCE(NULLIF([agenda].[IdObjeto], ''), N'Generico') AS [ObjetoRelacionadoTipo]
                FROM [dbo].[Agenda] AS [agenda]
                {BuildWhereClause(request)}
            )
            SELECT
                [Id],
                [Referencia],
                [Titulo],
                [Inicio],
                [Fin],
                [Estado],
                [Prioridad],
                [Origen],
                [ObjetoRelacionadoTipo]
            FROM [agenda_rows]
            ORDER BY {orderByColumn} {orderDirection}, [Id] ASC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
            """,
            parameters);
    }

    private static string BuildWhereClause(AgendaSearchRequest request)
    {
        var conditions = new List<string>
        {
            "[agenda].[BrokerIntegracionId] = @brokerId",
            "([agenda].[IdOld] IS NULL OR [agenda].[IdOld] NOT LIKE @mvpDeletedReferenciaLike)"
        };

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            conditions.Add("([agenda].[IdOld] LIKE @texto OR [agenda].[Asunto] LIKE @texto)");
        }

        if (!string.IsNullOrWhiteSpace(request.Prioridad))
        {
            conditions.Add("[agenda].[IdPrioridad] = @prioridad");
        }

        if (!string.IsNullOrWhiteSpace(request.Origen))
        {
            conditions.Add("@origen = @expectedOrigen");
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            conditions.Add(BuildEstadoCondition(request.Estado));
        }

        if (request.FechaDesde.HasValue)
        {
            conditions.Add("[agenda].[F_Inicio] >= @fechaDesde");
        }

        if (request.FechaHasta.HasValue)
        {
            conditions.Add("[agenda].[F_Inicio] <= @fechaHasta");
        }

        return "WHERE " + string.Join("\n  AND ", conditions);
    }

    private static string BuildEstadoCondition(string estado) =>
        estado.Trim().ToLowerInvariant() switch
        {
            "cerrado" => "([agenda].[F_Fin] IS NOT NULL AND [agenda].[F_Fin] < CONVERT(date, SYSUTCDATETIME()))",
            "programado" => "([agenda].[F_Inicio] > CONVERT(date, SYSUTCDATETIME()))",
            "pendiente" => "([agenda].[F_Inicio] <= CONVERT(date, SYSUTCDATETIME()) AND ([agenda].[F_Fin] IS NULL OR [agenda].[F_Fin] >= CONVERT(date, SYSUTCDATETIME())))",
            _ => "1 = 0"
        };

    private static List<PolizasSqlParameter> BuildParameters(AgendaSearchRequest request, int brokerId)
    {
        var parameters = new List<PolizasSqlParameter>
        {
            Int("@brokerId", brokerId),
            Text("@mvpDeletedReferenciaLike", $"{AgendaMvpWriteDefaults.DeletedReferenciaPrefix}%", 50),
            Text("@origenLabel", AgendaMvpWriteDefaults.Origen, 50)
        };

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            parameters.Add(Text("@texto", $"%{request.Texto.Trim()}%", 202));
        }

        if (!string.IsNullOrWhiteSpace(request.Prioridad))
        {
            parameters.Add(Text("@prioridad", request.Prioridad, 50));
        }

        if (!string.IsNullOrWhiteSpace(request.Origen))
        {
            parameters.Add(Text("@origen", request.Origen, 50));
            parameters.Add(Text("@expectedOrigen", AgendaMvpWriteDefaults.Origen, 50));
        }

        if (request.FechaDesde.HasValue)
        {
            parameters.Add(Date("@fechaDesde", request.FechaDesde.Value));
        }

        if (request.FechaHasta.HasValue)
        {
            parameters.Add(Date("@fechaHasta", request.FechaHasta.Value));
        }

        return parameters;
    }

    private static string BuildInicioExpression() =>
        """
        DATEADD(
            SECOND,
            DATEDIFF(SECOND, CAST('00:00:00' AS time), COALESCE([agenda].[Hora_Inicio], CAST('00:00:00' AS time))),
            CAST(COALESCE([agenda].[F_Inicio], CONVERT(date, [agenda].[FCR]), CONVERT(date, SYSUTCDATETIME())) AS datetime2))
        """;

    private static string BuildFinExpression() =>
        """
        CASE
            WHEN [agenda].[F_Fin] IS NULL THEN NULL
            ELSE DATEADD(
                SECOND,
                DATEDIFF(SECOND, CAST('00:00:00' AS time), COALESCE([agenda].[Hora_Fin], CAST('00:00:00' AS time))),
                CAST([agenda].[F_Fin] AS datetime2))
        END
        """;

    private static PolizasSqlParameter Text(string name, string value, int size) =>
        new(name, value.Trim(), SqlDbType.NVarChar, size);

    private static PolizasSqlParameter Int(string name, int value) =>
        new(name, value, SqlDbType.Int);

    private static PolizasSqlParameter Date(string name, DateOnly value) =>
        new(name, value.ToDateTime(TimeOnly.MinValue), SqlDbType.Date);
}
