using System.Data;
using FluentAssertions;
using iLiniumTech.Backend.Application.Clientes;
using iLiniumTech.Backend.Domain.Clientes;
using iLiniumTech.Backend.Infrastructure.Clientes;

namespace iLiniumTech.Backend.Tests.Clientes;

public sealed class ClientesSqlQueryBuilderTests
{
    private readonly SqlClientesQueryBuilder _builder = new();

    [Fact]
    public void BuildSearchQuery_uses_real_cliente_sources_and_broker_filter()
    {
        var request = new ClientesSearchRequest(
            Page: 2,
            PageSize: 25,
            Sort: "alias:asc",
            Texto: "cliente",
            Estado: "Activo",
            Segmento: "Empresa",
            FechaAltaDesde: new DateOnly(2026, 1, 1),
            FechaAltaHasta: new DateOnly(2026, 12, 31));

        var query = _builder.BuildSearchQuery(request, new ClientesSort("alias", Descending: false), brokerId: 325);

        query.CommandText.Should().Contain("[dbo].[IdentidadCliente] AS [cliente]");
        query.CommandText.Should().Contain("[dbo].[Identidad] AS [identidad]");
        query.CommandText.Should().Contain("[identidad].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("ORDER BY [Alias] ASC");
        query.CommandText.Contains("SELECT *", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.Parameters.Should().Contain(parameter => parameter.Name == "@brokerId" && Equals(parameter.Value, 325));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@offset" && Equals(parameter.Value, 25));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@pageSize" && Equals(parameter.Value, 25));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@texto" && parameter.DbType == SqlDbType.NVarChar);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@fechaAltaDesde" && parameter.DbType == SqlDbType.Date);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@fechaAltaHasta" && parameter.DbType == SqlDbType.Date);
    }

    [Theory]
    [InlineData("NumDocumento")]
    [InlineData("Email")]
    [InlineData("Telefono")]
    [InlineData("Direccion")]
    [InlineData("IBAN")]
    [InlineData("Cuenta")]
    public void BuildSearchQuery_does_not_project_forbidden_pii_columns(string forbidden)
    {
        var request = new ClientesSearchRequest(
            Page: 1,
            PageSize: 10,
            Sort: null,
            Texto: null,
            Estado: null,
            Segmento: null,
            FechaAltaDesde: null,
            FechaAltaHasta: null);

        var query = _builder.BuildSearchQuery(request, new ClientesSort("fechaAlta", Descending: true), brokerId: 325);

        query.CommandText.Contains(forbidden, StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public void BuildSearchQuery_falls_back_to_safe_sort_for_unknown_field()
    {
        var request = new ClientesSearchRequest(
            Page: 1,
            PageSize: 10,
            Sort: null,
            Texto: null,
            Estado: null,
            Segmento: null,
            FechaAltaDesde: null,
            FechaAltaHasta: null);

        var query = _builder.BuildSearchQuery(
            request,
            new ClientesSort("referencia]; DROP TABLE Identidad;--", Descending: false),
            brokerId: 325);

        query.CommandText.Should().Contain("ORDER BY [Referencia] ASC");
        query.CommandText.Contains("DROP TABLE", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }
}
