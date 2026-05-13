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

        query.CommandText.Should().Contain("[Poliza] LIKE @numero");
        query.CommandText.Should().Contain("[NombreCompleto] LIKE @cliente");
        query.CommandText.Should().Contain("[IdSituacion] = @estado");
        query.CommandText.Should().Contain("[Cia] = @compania");
        query.CommandText.Should().Contain("[IdRamo] = @ramo");
        query.CommandText.Should().NotContain("OR 1=1");
        query.CommandText.Should().NotContain("DROP TABLE");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@numero" && (string)parameter.Value == "%POL%' OR 1=1 --%");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@cliente" && (string)parameter.Value == "%Cliente;DROP TABLE Pantalla_Polizas%");
    }

    [Fact]
    public void BuildSearchQuery_uses_whitelisted_sort_column()
    {
        var query = _builder.BuildSearchQuery(
            new PolizasSearchRequest(Page: 1, PageSize: 25),
            new PolizasSort("primaAnual", Descending: false));

        query.CommandText.Should().Contain("ORDER BY [PAnualCartera] ASC");
        query.CommandText.Should().NotContain("primaAnual ASC");
    }

    [Fact]
    public void BuildDetailQuery_parameterizes_id()
    {
        var query = _builder.BuildDetailQuery("POL-1001'; SELECT 1 --");

        query.CommandText.Should().Contain("WHERE [Poliza] = @id");
        query.CommandText.Should().NotContain("SELECT 1 --");
        query.Parameters.Should().ContainSingle(parameter =>
            parameter.Name == "@id" && (string)parameter.Value == "POL-1001'; SELECT 1 --");
    }
}
