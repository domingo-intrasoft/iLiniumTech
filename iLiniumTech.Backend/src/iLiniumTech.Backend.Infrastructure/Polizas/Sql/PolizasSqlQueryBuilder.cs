using System.Text;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Infrastructure.Polizas.Sql;

public sealed class PolizasSqlQueryBuilder
{
    private const string SourceObject = "[dbo].[Pantalla_Polizas]";

    private static readonly IReadOnlyDictionary<string, string> SortColumns =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["numero"] = "[Poliza]",
            ["aplicacion"] = "[Aplicacion]",
            ["estado"] = "[IdSituacion]",
            ["ramo"] = "[IdRamo]",
            ["compania"] = "[Cia]",
            ["cliente"] = "[NombreCompleto]",
            ["fechaEfecto"] = "[F_Efecto]",
            ["fechaVencimiento"] = "[F_Vencimiento]",
            ["primaAnual"] = "[PAnualCartera]"
        };

    private static readonly string ListSelect = """
        SELECT
            CAST([Poliza] AS nvarchar(100)) AS [Id],
            CAST([Poliza] AS nvarchar(100)) AS [Numero],
            CAST([Aplicacion] AS nvarchar(100)) AS [Aplicacion],
            CAST([IdSituacion] AS nvarchar(100)) AS [Estado],
            CAST([IdRamo] AS nvarchar(100)) AS [Ramo],
            CAST('' AS nvarchar(100)) AS [ClienteId],
            CAST([NombreCompleto] AS nvarchar(250)) AS [ClienteNombre],
            CAST([Cia] AS nvarchar(150)) AS [Compania],
            [F_Efecto] AS [FechaEfecto],
            [F_Vencimiento] AS [FechaVencimiento],
            [PAnualCartera] AS [PrimaAnual],
            CAST('EUR' AS nvarchar(3)) AS [Moneda]
        """;

    private static readonly string DetailSelect = """
        SELECT TOP (1)
            CAST([Poliza] AS nvarchar(100)) AS [Id],
            CAST([Poliza] AS nvarchar(100)) AS [Numero],
            CAST([Poliza] AS nvarchar(100)) AS [Certificado],
            CAST([IdTipoPoliza] AS nvarchar(100)) AS [TipoPoliza],
            CAST([Aplicacion] AS nvarchar(100)) AS [Aplicacion],
            CAST([IdSituacion] AS nvarchar(100)) AS [Estado],
            CAST([IdRamo] AS nvarchar(100)) AS [Ramo],
            CAST('' AS nvarchar(100)) AS [ClienteId],
            CAST([NombreCompleto] AS nvarchar(250)) AS [ClienteNombre],
            CAST([Cia] AS nvarchar(150)) AS [Compania],
            CAST([Riesgo] AS nvarchar(250)) AS [Riesgo],
            [F_Efecto] AS [FechaEfecto],
            [F_Vencimiento] AS [FechaVencimiento],
            [PAnualCartera] AS [PrimaAnual],
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
            CAST('' AS nvarchar(100)) AS [Documento],
            CAST('' AS nvarchar(100)) AS [Apellido1],
            CAST('' AS nvarchar(100)) AS [Apellido2],
            CAST([NombreCompleto] AS nvarchar(250)) AS [Nombre],
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
        var commandText = $"""
            {ListSelect}
            FROM {SourceObject}
            {where.Clause}
            ORDER BY {orderColumn} {direction}
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
            """;

        return new PolizasSqlQuery(commandText, parameters);
    }

    public PolizasSqlQuery BuildCountQuery(PolizasSearchRequest request)
    {
        var where = BuildWhereClause(request);
        var commandText = $"""
            SELECT COUNT_BIG(1)
            FROM {SourceObject}
            {where.Clause};
            """;

        return new PolizasSqlQuery(commandText, where.Parameters);
    }

    public PolizasSqlQuery BuildDetailQuery(string id)
    {
        var commandText = $"""
            {DetailSelect}
            FROM {SourceObject}
            WHERE [Poliza] = @id;
            """;

        return new PolizasSqlQuery(commandText, [new PolizasSqlParameter("@id", id)]);
    }

    private static PolizasWhereClause BuildWhereClause(PolizasSearchRequest request)
    {
        var clauses = new List<string>();
        var parameters = new List<PolizasSqlParameter>();

        AddLike(clauses, parameters, "[Poliza]", "@numero", request.Numero);
        AddLike(clauses, parameters, "[NombreCompleto]", "@cliente", request.Cliente);
        AddEquals(clauses, parameters, "[IdSituacion]", "@estado", request.Estado);
        AddEquals(clauses, parameters, "[Cia]", "@compania", request.Compania);
        AddEquals(clauses, parameters, "[IdRamo]", "@ramo", request.Ramo);

        if (request.FechaEfectoDesde.HasValue)
        {
            clauses.Add("[F_Efecto] >= @fechaEfectoDesde");
            parameters.Add(new PolizasSqlParameter(
                "@fechaEfectoDesde",
                request.FechaEfectoDesde.Value.ToDateTime(TimeOnly.MinValue)));
        }

        if (request.FechaEfectoHasta.HasValue)
        {
            clauses.Add("[F_Efecto] < @fechaEfectoHastaExclusiva");
            parameters.Add(new PolizasSqlParameter(
                "@fechaEfectoHastaExclusiva",
                request.FechaEfectoHasta.Value.AddDays(1).ToDateTime(TimeOnly.MinValue)));
        }

        if (clauses.Count == 0)
        {
            return new PolizasWhereClause(string.Empty, parameters);
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
