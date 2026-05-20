using System.Data;
using FluentAssertions;
using iLiniumTech.Backend.Application.Suplementos;
using iLiniumTech.Backend.Domain.Suplementos;
using iLiniumTech.Backend.Infrastructure.Suplementos;

namespace iLiniumTech.Backend.Tests.Suplementos;

public sealed class SuplementosSqlQueryBuilderTests
{
    private readonly SqlSuplementosQueryBuilder _builder = new();

    [Fact]
    public void BuildSearchQuery_uses_real_suplemento_sources_and_broker_filter()
    {
        var request = new SuplementosSearchRequest(
            Page: 2,
            PageSize: 25,
            Sort: "referencia:asc",
            Referencia: "SUP-2026",
            Poliza: "POL",
            Tipo: "tipo-AL",
            Situacion: "situacion-PE",
            FechaEfectoDesde: new DateOnly(2026, 1, 1),
            FechaEfectoHasta: new DateOnly(2026, 12, 31));

        var query = _builder.BuildSearchQuery(request, new SuplementosSort("referencia", Descending: false), brokerId: 325);

        query.CommandText.Should().Contain("[dbo].[Suplemento] AS [sup]");
        query.CommandText.Should().Contain("[dbo].[Poliza] AS [poliza]");
        query.CommandText.Should().Contain("[sup].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("[poliza].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("ORDER BY [Referencia] ASC");
        query.CommandText.Contains("SELECT *", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.Parameters.Should().Contain(parameter => parameter.Name == "@brokerId" && Equals(parameter.Value, 325));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@offset" && Equals(parameter.Value, 25));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@pageSize" && Equals(parameter.Value, 25));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@referencia" && parameter.DbType == SqlDbType.NVarChar);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@poliza" && parameter.DbType == SqlDbType.NVarChar);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@fechaEfectoDesde" && parameter.DbType == SqlDbType.Date);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@fechaEfectoHasta" && parameter.DbType == SqlDbType.Date);
    }

    [Theory]
    [InlineData("SuplementoDatosTomador")]
    [InlineData("SuplementoBeneficiario")]
    [InlineData("SuplementoDomiciliacionBancaria")]
    [InlineData("SuplementoRecibo")]
    [InlineData("SuplementoDeclaracion")]
    [InlineData("RegularizacionDeclaracion")]
    [InlineData("EIACSuplemento")]
    [InlineData("VwClienteSuplemento")]
    [InlineData("[sup].[Concepto]")]
    [InlineData("[sup].[Valor]")]
    [InlineData("[sup].[ValorAnterior]")]
    [InlineData("[sup].[EmailComunicacion]")]
    [InlineData("Prima")]
    [InlineData("Importe")]
    [InlineData("Comision")]
    [InlineData("Cuenta")]
    [InlineData("IBAN")]
    [InlineData("Documento")]
    [InlineData("Telefono")]
    [InlineData("Email")]
    [InlineData("Direccion")]
    [InlineData("Observaciones")]
    [InlineData("Workflow")]
    [InlineData("Adjunto")]
    public void BuildSearchQuery_does_not_project_pii_financial_bank_workflow_or_detail_sources(string forbidden)
    {
        var query = _builder.BuildSearchQuery(
            new SuplementosSearchRequest(
                Page: 1,
                PageSize: 10,
                Sort: null,
                Referencia: null,
                Poliza: null,
                Tipo: null,
                Situacion: null,
                FechaEfectoDesde: null,
                FechaEfectoHasta: null),
            new SuplementosSort("fechaEfecto", Descending: true),
            brokerId: 325);

        query.CommandText.Contains(forbidden, StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public void BuildSearchQuery_falls_back_to_safe_sort_for_unknown_field()
    {
        var query = _builder.BuildSearchQuery(
            new SuplementosSearchRequest(
                Page: 1,
                PageSize: 10,
                Sort: null,
                Referencia: null,
                Poliza: null,
                Tipo: null,
                Situacion: null,
                FechaEfectoDesde: null,
                FechaEfectoHasta: null),
            new SuplementosSort("referencia]; DROP TABLE Suplemento;--", Descending: false),
            brokerId: 325);

        query.CommandText.Should().Contain("ORDER BY [FechaEfecto] ASC");
        query.CommandText.Contains("DROP TABLE", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }
}
