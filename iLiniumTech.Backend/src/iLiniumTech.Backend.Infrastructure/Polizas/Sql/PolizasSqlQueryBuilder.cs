using System.Text;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Infrastructure.Polizas.Sql;

public sealed class PolizasSqlQueryBuilder
{
    private const string SourceObject = "[dbo].[vw_ClientePolizas]";
    private const string SourceAlias = "[p]";
    private const string ExcludeDeletedMvpPolizasClause = "[p].[Poliza] NOT LIKE 'ILMVP-DELETED-%'";
    private const string RamoDescriptionExpression = "COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[Ramo] AS nvarchar(100)))), ''), CAST([p].[IdRamo] AS nvarchar(100)))";
    private const string ClienteNombreExpression = "COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[RazonSocial] AS nvarchar(250)))), ''), CAST([p].[ClienteId] AS nvarchar(250)))";
    private const string ClienteSearchExpression = "CONCAT(COALESCE(CAST([p].[RazonSocial] AS nvarchar(250)), ''), ' ', COALESCE(CAST([p].[NumDocumento] AS nvarchar(100)), ''), ' ', COALESCE(CAST([p].[ClienteId] AS nvarchar(50)), ''))";

    private static readonly IReadOnlyDictionary<string, string> SortColumns =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["numero"] = "[p].[Poliza]",
            ["aplicacion"] = "[p].[Aplicacion]",
            ["estado"] = "[p].[IdSituacion]",
            ["ramo"] = RamoDescriptionExpression,
            ["compania"] = "[p].[CiaRazonSocial]",
            ["cliente"] = ClienteNombreExpression,
            ["fechaEfecto"] = "[p].[F_Efecto]",
            ["fechaVencimiento"] = "[p].[F_Vencimiento]",
            ["primaAnual"] = "[p].[PolizaId]"
        };

    private static readonly string FromSource = $"FROM {SourceObject} AS {SourceAlias}";

    private static readonly string ListSelect = """
        SELECT
            CAST([p].[PolizaId] AS nvarchar(100)) AS [Id],
            CAST([p].[Poliza] AS nvarchar(100)) AS [Numero],
            CAST('' AS nvarchar(100)) AS [Aplicacion],
            COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[Situacion] AS nvarchar(100)))), ''), CAST([p].[IdSituacion] AS nvarchar(100))) AS [Estado],
            COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[Ramo] AS nvarchar(100)))), ''), CAST([p].[IdRamo] AS nvarchar(100))) AS [Ramo],
            CAST([p].[ClienteId] AS nvarchar(100)) AS [ClienteId],
            COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[RazonSocial] AS nvarchar(250)))), ''), CAST([p].[ClienteId] AS nvarchar(250))) AS [ClienteNombre],
            CAST([p].[NumDocumento] AS nvarchar(100)) AS [Documento],
            COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[CiaRazonSocial] AS nvarchar(150)))), ''), CAST([p].[CiaDgs] AS nvarchar(150))) AS [Compania],
            CAST([p].[Riesgo] AS nvarchar(250)) AS [Riesgo],
            [p].[F_Efecto] AS [FechaEfecto],
            [p].[F_Vencimiento] AS [FechaVencimiento],
            CAST(0 AS decimal(18,2)) AS [PrimaAnual],
            CAST('EUR' AS nvarchar(3)) AS [Moneda]
        """;

    private static readonly string DetailSelect = """
        SELECT TOP (1)
            CAST([p].[PolizaId] AS nvarchar(100)) AS [Id],
            CAST([p].[Poliza] AS nvarchar(100)) AS [Numero],
            CAST([p].[Poliza] AS nvarchar(100)) AS [Certificado],
            CAST('' AS nvarchar(100)) AS [TipoPoliza],
            CAST('' AS nvarchar(100)) AS [Aplicacion],
            COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[Situacion] AS nvarchar(100)))), ''), CAST([p].[IdSituacion] AS nvarchar(100))) AS [Estado],
            COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[Ramo] AS nvarchar(100)))), ''), CAST([p].[IdRamo] AS nvarchar(100))) AS [Ramo],
            CAST([p].[ClienteId] AS nvarchar(100)) AS [ClienteId],
            COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[RazonSocial] AS nvarchar(250)))), ''), CAST([p].[ClienteId] AS nvarchar(250))) AS [ClienteNombre],
            COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[CiaRazonSocial] AS nvarchar(150)))), ''), CAST([p].[CiaDgs] AS nvarchar(150))) AS [Compania],
            CAST([p].[Riesgo] AS nvarchar(250)) AS [Riesgo],
            [p].[F_Efecto] AS [FechaEfecto],
            [p].[F_Vencimiento] AS [FechaVencimiento],
            CAST(0 AS decimal(18,2)) AS [PrimaAnual],
            CAST('EUR' AS nvarchar(3)) AS [Moneda],
            CAST('' AS nvarchar(100)) AS [Oficina],
            CAST('' AS nvarchar(100)) AS [Division],
            CAST('' AS nvarchar(100)) AS [Colaborador1],
            CAST('' AS nvarchar(100)) AS [Administrativo],
            CAST('' AS nvarchar(100)) AS [Comercial],
            CAST('' AS nvarchar(100)) AS [Siniestros],
            CAST('' AS nvarchar(100)) AS [Gestor],
            CAST('' AS nvarchar(100)) AS [CanalCobro],
            CAST('' AS nvarchar(100)) AS [FraccionPago],
            CAST('' AS nvarchar(100)) AS [Ccaa],
            CAST([p].[NumDocumento] AS nvarchar(100)) AS [Documento],
            CAST('' AS nvarchar(100)) AS [Apellido1],
            CAST('' AS nvarchar(100)) AS [Apellido2],
            COALESCE(NULLIF(LTRIM(RTRIM(CAST([p].[RazonSocial] AS nvarchar(250)))), ''), CAST([p].[ClienteId] AS nvarchar(250))) AS [Nombre],
            CAST('' AS nvarchar(20)) AS [Sexo],
            CAST('19000101' AS date) AS [FechaNacimiento],
            CAST(0 AS int) AS [Edad],
            CAST('' AS nvarchar(100)) AS [EstadoCivil],
            CAST(0 AS int) AS [Hijos],
            CAST('' AS nvarchar(100)) AS [RegimenLaboral],
            CAST('' AS nvarchar(100)) AS [Profesion],
            CAST('' AS nvarchar(250)) AS [Email],
            CAST('' AS nvarchar(50)) AS [Telefono]
        """;

    public PolizasSqlQuery BuildSearchQuery(PolizasSearchRequest request, PolizasSort sort)
    {
        var where = BuildWhereClause(request);
        var parameters = where.Parameters.ToList();
        parameters.Add(new PolizasSqlParameter("@offset", (request.Page - 1) * request.PageSize));
        parameters.Add(new PolizasSqlParameter("@pageSize", request.PageSize));

        var orderColumn = SortColumns[sort.Field];
        var direction = sort.Descending ? "DESC" : "ASC";
        var tieBreaker = sort.Field.Equals("numero", StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : ", [p].[Poliza] ASC";
        var commandText = $"""
            {ListSelect}
            {FromSource}
            {where.Clause}
            ORDER BY {orderColumn} {direction}{tieBreaker}
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
            """;

        return new PolizasSqlQuery(commandText, parameters);
    }

    public PolizasSqlQuery BuildCountQuery(PolizasSearchRequest request)
    {
        var where = BuildWhereClause(request);
        var commandText = $"""
            SELECT COUNT_BIG(1)
            {FromSource}
            {where.Clause};
            """;

        return new PolizasSqlQuery(commandText, where.Parameters);
    }

    public PolizasSqlQuery BuildDetailQuery(string id, string? ramo = null)
    {
        var clauses = new List<string> { "[p].[PolizaId] = TRY_CONVERT(int, @id)", ExcludeDeletedMvpPolizasClause };
        var parameters = new List<PolizasSqlParameter> { new("@id", id) };

        AddEquals(clauses, parameters, "[p].[IdRamo]", "@ramo", ramo);

        var commandText = $"""
            {DetailSelect}
            {FromSource}
            WHERE {string.Join(" AND ", clauses)};
            """;

        return new PolizasSqlQuery(commandText, parameters);
    }

    private static PolizasWhereClause BuildWhereClause(PolizasSearchRequest request)
    {
        var clauses = new List<string>();
        var parameters = new List<PolizasSqlParameter>();

        clauses.Add(ExcludeDeletedMvpPolizasClause);
        AddLike(clauses, parameters, "[p].[Poliza]", "@numero", request.Numero);
        AddLike(clauses, parameters, ClienteSearchExpression, "@cliente", request.Cliente);
        AddEquals(clauses, parameters, "[p].[IdSituacion]", "@estado", request.Estado);
        AddEquals(clauses, parameters, "CONVERT(nvarchar(150), [p].[CiaRazonSocial])", "@compania", request.Compania);
        AddEquals(clauses, parameters, "[p].[IdRamo]", "@ramo", request.Ramo);

        if (request.FechaEfectoDesde.HasValue)
        {
            clauses.Add("[p].[F_Efecto] >= @fechaEfectoDesde");
            parameters.Add(new PolizasSqlParameter(
                "@fechaEfectoDesde",
                request.FechaEfectoDesde.Value.ToDateTime(TimeOnly.MinValue)));
        }

        if (request.FechaEfectoHasta.HasValue)
        {
            clauses.Add("[p].[F_Efecto] < @fechaEfectoHastaExclusiva");
            parameters.Add(new PolizasSqlParameter(
                "@fechaEfectoHastaExclusiva",
                request.FechaEfectoHasta.Value.AddDays(1).ToDateTime(TimeOnly.MinValue)));
        }

        var builder = new StringBuilder("WHERE ");
        builder.AppendJoin(" AND ", clauses);
        return new PolizasWhereClause(builder.ToString(), parameters);
    }

    private static void AddLike(
        ICollection<string> clauses,
        ICollection<PolizasSqlParameter> parameters,
        string column,
        string parameterName,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        clauses.Add($"{column} LIKE {parameterName}");
        parameters.Add(new PolizasSqlParameter(parameterName, $"%{value}%"));
    }

    private static void AddEquals(
        ICollection<string> clauses,
        ICollection<PolizasSqlParameter> parameters,
        string column,
        string parameterName,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        clauses.Add($"{column} = {parameterName}");
        parameters.Add(new PolizasSqlParameter(parameterName, value));
    }

    private sealed record PolizasWhereClause(
        string Clause,
        IReadOnlyList<PolizasSqlParameter> Parameters);
}
