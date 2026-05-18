using System.Text.RegularExpressions;
using FluentAssertions;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class PolizasSqlQueryBuilderTests
{
    private readonly PolizasSqlQueryBuilder _builder = new();

    [Fact]
    public void BuildSearchQuery_parameterizes_filter_values()
    {
        var request = new PolizasSearchRequest(
            Page: 2,
            PageSize: 25,
            Numero: "POL%' OR 1=1 --",
            Cliente: "Cliente;DROP TABLE Pantalla_Polizas",
            Estado: "Vigor",
            Compania: "Compania demo",
            Ramo: "Autos",
            FechaEfectoDesde: new DateOnly(2026, 1, 1),
            FechaEfectoHasta: new DateOnly(2026, 12, 31));

        var query = _builder.BuildSearchQuery(request, new PolizasSort("fechaEfecto", Descending: true));

        query.CommandText.Should().Contain("[p].[Poliza] LIKE @numero");
        query.CommandText.Should().Contain("[p].[Poliza] NOT LIKE 'ILMVP-DELETED-%'");
        query.CommandText.Should().Contain("[p].[RazonSocial]");
        query.CommandText.Should().Contain("[p].[NumDocumento]");
        query.CommandText.Should().Contain("LIKE @cliente");
        query.CommandText.Should().Contain("[p].[IdSituacion] = @estado");
        query.CommandText.Should().Contain("CONVERT(nvarchar(150), [p].[CiaRazonSocial]) = @compania");
        query.CommandText.Should().Contain("[p].[IdRamo] = @ramo");
        query.CommandText.Should().Contain("[p].[F_Efecto] >= @fechaEfectoDesde");
        query.CommandText.Should().Contain("[p].[F_Efecto] < @fechaEfectoHastaExclusiva");
        query.CommandText.Should().NotContain("OR 1=1");
        query.CommandText.Should().NotContain("DROP TABLE");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@numero" && (string)parameter.Value == "%POL%' OR 1=1 --%");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@cliente" && (string)parameter.Value == "%Cliente;DROP TABLE Pantalla_Polizas%");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@fechaEfectoDesde"
            && (DateTime)parameter.Value == new DateTime(2026, 1, 1));
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@fechaEfectoHastaExclusiva"
            && (DateTime)parameter.Value == new DateTime(2027, 1, 1));
    }

    [Fact]
    public void BuildSearchQuery_uses_whitelisted_sort_column()
    {
        var query = _builder.BuildSearchQuery(
            new PolizasSearchRequest(Page: 1, PageSize: 25),
            new PolizasSort("primaAnual", Descending: false));

        query.CommandText.Should().Contain("ORDER BY [p].[PolizaId] ASC, [p].[Poliza] ASC");
        query.CommandText.Should().Contain("WHERE [p].[Poliza] NOT LIKE 'ILMVP-DELETED-%'");
        query.CommandText.Should().NotContain("primaAnual ASC");
    }

    [Fact]
    public void BuildSearchQuery_does_not_duplicate_numero_tie_breaker()
    {
        var query = _builder.BuildSearchQuery(
            new PolizasSearchRequest(Page: 1, PageSize: 25),
            new PolizasSort("numero", Descending: false));

        query.CommandText.Should().Contain("ORDER BY [p].[Poliza] ASC");
        query.CommandText.Should().NotContain("ORDER BY [p].[Poliza] ASC, [p].[Poliza] ASC");
    }

    [Fact]
    public void BuildSearchQuery_projects_appbuilder_parity_fields_in_list_projection()
    {
        var query = _builder.BuildSearchQuery(
            new PolizasSearchRequest(Page: 1, PageSize: 25),
            new PolizasSort("numero", Descending: false));

        query.CommandText.Should().Contain("FROM [dbo].[vw_ClientePolizas] AS [p]");
        query.CommandText.Should().Contain("CAST([p].[PolizaId] AS nvarchar(100)) AS [Id]");
        query.CommandText.Should().Contain("CAST([p].[Poliza] AS nvarchar(100)) AS [Numero]");
        query.CommandText.Should().Contain("CAST([p].[ClienteId] AS nvarchar(100)) AS [ClienteId]");
        query.CommandText.Should().Contain("CAST([p].[RazonSocial] AS nvarchar(250))");
        query.CommandText.Should().Contain("AS [ClienteNombre]");
        query.CommandText.Should().Contain("CAST([p].[NumDocumento] AS nvarchar(100)) AS [Documento]");
        query.CommandText.Should().Contain("CAST([p].[CiaRazonSocial] AS nvarchar(150))");
        query.CommandText.Should().Contain("AS [Compania]");
        query.CommandText.Should().Contain("CAST([p].[Riesgo] AS nvarchar(250)) AS [Riesgo]");
        query.CommandText.Should().Contain("CAST([p].[Ramo] AS nvarchar(100))");
        query.CommandText.Should().Contain("AS [Ramo]");
        query.CommandText.Should().Contain("CAST(0 AS decimal(18,2)) AS [PrimaAnual]");
        ShouldNotProjectNumDocumentoAs(query.CommandText, "ClienteId");
    }

    [Theory]
    [InlineData(1, 25, 0)]
    [InlineData(2, 25, 25)]
    [InlineData(5, 10, 40)]
    public void BuildSearchQuery_applies_offset_fetch_pagination(int page, int pageSize, int expectedOffset)
    {
        var query = _builder.BuildSearchQuery(
            new PolizasSearchRequest(Page: page, PageSize: pageSize),
            new PolizasSort("numero", Descending: false));

        query.CommandText.Should().Contain("OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@offset" && (int)parameter.Value == expectedOffset);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@pageSize" && (int)parameter.Value == pageSize);
    }

    [Fact]
    public void BuildCountQuery_applies_same_date_effect_filters_without_pagination()
    {
        var request = new PolizasSearchRequest(
            Page: 3,
            PageSize: 50,
            FechaEfectoDesde: new DateOnly(2026, 5, 14),
            FechaEfectoHasta: new DateOnly(2026, 5, 31));

        var query = _builder.BuildCountQuery(request);

        query.CommandText.Should().Contain("[p].[F_Efecto] >= @fechaEfectoDesde");
        query.CommandText.Should().Contain("[p].[F_Efecto] < @fechaEfectoHastaExclusiva");
        query.CommandText.Should().NotContain("OFFSET @offset");
        query.CommandText.Should().NotContain("FETCH NEXT @pageSize");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@fechaEfectoDesde"
            && (DateTime)parameter.Value == new DateTime(2026, 5, 14));
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@fechaEfectoHastaExclusiva"
            && (DateTime)parameter.Value == new DateTime(2026, 6, 1));
        query.Parameters.Should().NotContain(parameter => parameter.Name == "@offset");
        query.Parameters.Should().NotContain(parameter => parameter.Name == "@pageSize");
    }

    [Fact]
    public void BuildDetailQuery_parameterizes_id()
    {
        var query = _builder.BuildDetailQuery("POL-1001'; SELECT 1 --");

        query.CommandText.Should().Contain("WHERE [p].[PolizaId] = TRY_CONVERT(int, @id) AND [p].[Poliza] NOT LIKE 'ILMVP-DELETED-%'");
        query.CommandText.Should().NotContain("SELECT 1 --");
        query.Parameters.Should().ContainSingle(parameter =>
            parameter.Name == "@id" && (string)parameter.Value == "POL-1001'; SELECT 1 --");
    }

    [Fact]
    public void BuildDetailQuery_can_scope_detail_by_ramo()
    {
        var query = _builder.BuildDetailQuery("1001", "Autos");

        query.CommandText.Should().Contain("WHERE [p].[PolizaId] = TRY_CONVERT(int, @id) AND [p].[Poliza] NOT LIKE 'ILMVP-DELETED-%' AND [p].[IdRamo] = @ramo");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@id" && (string)parameter.Value == "1001");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@ramo" && (string)parameter.Value == "Autos");
    }

    [Fact]
    public void BuildDetailQuery_projects_appbuilder_parity_fields_in_detail_projection()
    {
        var query = _builder.BuildDetailQuery("POL-1001");

        query.CommandText.Should().Contain("CAST([p].[PolizaId] AS nvarchar(100)) AS [Id]");
        query.CommandText.Should().Contain("CAST([p].[Poliza] AS nvarchar(100)) AS [Numero]");
        query.CommandText.Should().Contain("CAST([p].[ClienteId] AS nvarchar(100)) AS [ClienteId]");
        query.CommandText.Should().Contain("CAST([p].[RazonSocial] AS nvarchar(250))");
        query.CommandText.Should().Contain("AS [ClienteNombre]");
        query.CommandText.Should().Contain("CAST([p].[CiaRazonSocial] AS nvarchar(150))");
        query.CommandText.Should().Contain("AS [Compania]");
        query.CommandText.Should().Contain("CAST([p].[Riesgo] AS nvarchar(250)) AS [Riesgo]");
        query.CommandText.Should().Contain("CAST(0 AS decimal(18,2)) AS [PrimaAnual]");
        query.CommandText.Should().Contain("CAST([p].[NumDocumento] AS nvarchar(100)) AS [Documento]");
        query.CommandText.Should().Contain("CAST([p].[Ramo] AS nvarchar(100))");
        ShouldNotProjectNumDocumentoAs(query.CommandText, "ClienteId");
    }

    private static void ShouldNotProjectNumDocumentoAs(string commandText, string alias)
    {
        var projectsNumDocumentoAsAlias = Regex.IsMatch(
            commandText,
            $@"\[NumDocumento\][^\r\n,]*\s+AS\s+\[{Regex.Escape(alias)}\]",
            RegexOptions.IgnoreCase);

        projectsNumDocumentoAsAlias.Should().BeFalse(
            "SQL responses must not expose the complete legal document through {0}", alias);
    }
}
