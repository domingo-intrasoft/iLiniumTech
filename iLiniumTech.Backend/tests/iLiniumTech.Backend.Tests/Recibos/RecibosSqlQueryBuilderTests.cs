using System.Data;
using FluentAssertions;
using iLiniumTech.Backend.Application.Recibos;
using iLiniumTech.Backend.Domain.Recibos;
using iLiniumTech.Backend.Infrastructure.Recibos;

namespace iLiniumTech.Backend.Tests.Recibos;

public sealed class RecibosSqlQueryBuilderTests
{
    private readonly SqlRecibosQueryBuilder _builder = new();

    [Fact]
    public void BuildSearchQuery_uses_real_recibo_sources_and_broker_filter()
    {
        var request = new RecibosSearchRequest(
            Page: 2,
            PageSize: 25,
            Sort: "recibo:asc",
            Recibo: "REC-2026",
            Poliza: "POL",
            Situacion: "situacion-PE",
            Tipo: "tipo-PR",
            FechaVencimientoDesde: new DateOnly(2026, 1, 1),
            FechaVencimientoHasta: new DateOnly(2026, 12, 31));

        var query = _builder.BuildSearchQuery(request, new RecibosSort("recibo", Descending: false), brokerId: 325);

        query.CommandText.Should().Contain("[dbo].[Recibo] AS [rec]");
        query.CommandText.Should().Contain("[dbo].[Poliza] AS [poliza]");
        query.CommandText.Should().Contain("[rec].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("[poliza].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("ORDER BY [Recibo] ASC");
        query.CommandText.Contains("SELECT *", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.Parameters.Should().Contain(parameter => parameter.Name == "@brokerId" && Equals(parameter.Value, 325));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@offset" && Equals(parameter.Value, 25));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@pageSize" && Equals(parameter.Value, 25));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@recibo" && parameter.DbType == SqlDbType.NVarChar);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@poliza" && parameter.DbType == SqlDbType.NVarChar);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@fechaVencimientoDesde" && parameter.DbType == SqlDbType.Date);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@fechaVencimientoHasta" && parameter.DbType == SqlDbType.Date);
    }

    [Theory]
    [InlineData("vw_ClienteRecibos")]
    [InlineData("PBI_Recibos")]
    [InlineData("[rec].[PrimaNeta]")]
    [InlineData("[rec].[PrimaTotal]")]
    [InlineData("[rec].[Comision]")]
    [InlineData("[rec].[Comision2]")]
    [InlineData("[rec].[Consorcio]")]
    [InlineData("[rec].[Ips]")]
    [InlineData("[rec].[OtrosGastos]")]
    [InlineData("[rec].[OtrosImpuesto]")]
    [InlineData("[rec].[PctAplicado]")]
    [InlineData("[rec].[PctComision2Aplicado]")]
    [InlineData("CuentaBancaria")]
    [InlineData("IBAN")]
    [InlineData("NumDocumento")]
    [InlineData("Telefono")]
    [InlineData("Email")]
    [InlineData("Direccion")]
    [InlineData("Observaciones")]
    public void BuildSearchQuery_does_not_project_financial_bank_or_pii_columns(string forbidden)
    {
        var query = _builder.BuildSearchQuery(
            new RecibosSearchRequest(
                Page: 1,
                PageSize: 10,
                Sort: null,
                Recibo: null,
                Poliza: null,
                Situacion: null,
                Tipo: null,
                FechaVencimientoDesde: null,
                FechaVencimientoHasta: null),
            new RecibosSort("fechaVencimiento", Descending: true),
            brokerId: 325);

        query.CommandText.Contains(forbidden, StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public void BuildSearchQuery_falls_back_to_safe_sort_for_unknown_field()
    {
        var query = _builder.BuildSearchQuery(
            new RecibosSearchRequest(
                Page: 1,
                PageSize: 10,
                Sort: null,
                Recibo: null,
                Poliza: null,
                Situacion: null,
                Tipo: null,
                FechaVencimientoDesde: null,
                FechaVencimientoHasta: null),
            new RecibosSort("recibo]; DROP TABLE Recibo;--", Descending: false),
            brokerId: 325);

        query.CommandText.Should().Contain("ORDER BY [FechaVencimiento] ASC");
        query.CommandText.Contains("DROP TABLE", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }
}
