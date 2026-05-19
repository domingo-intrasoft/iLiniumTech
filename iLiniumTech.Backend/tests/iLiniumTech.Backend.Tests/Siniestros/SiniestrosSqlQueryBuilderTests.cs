using System.Data;
using FluentAssertions;
using iLiniumTech.Backend.Application.Siniestros;
using iLiniumTech.Backend.Domain.Siniestros;
using iLiniumTech.Backend.Infrastructure.Siniestros;

namespace iLiniumTech.Backend.Tests.Siniestros;

public sealed class SiniestrosSqlQueryBuilderTests
{
    private readonly SqlSiniestrosQueryBuilder _builder = new();

    [Fact]
    public void BuildSearchQuery_uses_real_siniestro_sources_and_broker_filter()
    {
        var request = new SiniestrosSearchRequest(
            Page: 2,
            PageSize: 25,
            Sort: "referencia:asc",
            Referencia: "SIN-2026",
            Poliza: "POL",
            Estado: "situacion-AL",
            Prioridad: "Alta",
            FechaSiniestroDesde: new DateOnly(2026, 1, 1),
            FechaSiniestroHasta: new DateOnly(2026, 12, 31));

        var query = _builder.BuildSearchQuery(request, new SiniestrosSort("referencia", Descending: false), brokerId: 325);

        query.CommandText.Should().Contain("[dbo].[Siniestro] AS [sin]");
        query.CommandText.Should().Contain("[dbo].[RiesgoPoliza] AS [riesgoPoliza]");
        query.CommandText.Should().Contain("[dbo].[Poliza] AS [poliza]");
        query.CommandText.Should().Contain("[sin].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("[riesgoPoliza].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("[poliza].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("ORDER BY [Referencia] ASC");
        query.CommandText.Contains("SELECT *", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.Parameters.Should().Contain(parameter => parameter.Name == "@brokerId" && Equals(parameter.Value, 325));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@offset" && Equals(parameter.Value, 25));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@pageSize" && Equals(parameter.Value, 25));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@referencia" && parameter.DbType == SqlDbType.NVarChar);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@poliza" && parameter.DbType == SqlDbType.NVarChar);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@fechaSiniestroDesde" && parameter.DbType == SqlDbType.Date);
        query.Parameters.Should().Contain(parameter => parameter.Name == "@fechaSiniestroHasta" && parameter.DbType == SqlDbType.Date);
    }

    [Theory]
    [InlineData("[sin].[Descripcion]")]
    [InlineData("[sin].[Danos]")]
    [InlineData("[sin].[DanosCoche]")]
    [InlineData("[sin].[Garantias]")]
    [InlineData("[sin].[Franquicia]")]
    [InlineData("Observaciones")]
    [InlineData("Telefono")]
    [InlineData("Email")]
    [InlineData("Matricula")]
    [InlineData("[sin].[Reserva]")]
    [InlineData("[sin].[Indemnizacion]")]
    [InlineData("SiniestroInterviniente")]
    public void BuildSearchQuery_does_not_project_forbidden_sensitive_columns(string forbidden)
    {
        var query = _builder.BuildSearchQuery(
            new SiniestrosSearchRequest(
                Page: 1,
                PageSize: 10,
                Sort: null,
                Referencia: null,
                Poliza: null,
                Estado: null,
                Prioridad: null,
                FechaSiniestroDesde: null,
                FechaSiniestroHasta: null),
            new SiniestrosSort("fechaSiniestro", Descending: true),
            brokerId: 325);

        query.CommandText.Contains(forbidden, StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public void BuildSearchQuery_falls_back_to_safe_sort_for_unknown_field()
    {
        var query = _builder.BuildSearchQuery(
            new SiniestrosSearchRequest(
                Page: 1,
                PageSize: 10,
                Sort: null,
                Referencia: null,
                Poliza: null,
                Estado: null,
                Prioridad: null,
                FechaSiniestroDesde: null,
                FechaSiniestroHasta: null),
            new SiniestrosSort("referencia]; DROP TABLE Siniestro;--", Descending: false),
            brokerId: 325);

        query.CommandText.Should().Contain("ORDER BY [FechaSiniestro] ASC");
        query.CommandText.Contains("DROP TABLE", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }
}
