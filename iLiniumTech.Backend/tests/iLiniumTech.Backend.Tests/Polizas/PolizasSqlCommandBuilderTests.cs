using FluentAssertions;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class PolizasSqlCommandBuilderTests
{
    private readonly PolizasSqlCommandBuilder _builder = new();

    [Fact]
    public void BuildCreateCommand_inserts_allowed_fields_and_mvp_origin()
    {
        var request = CreateValidRequest() with
        {
            Numero = "POL-MVP-0001'; DELETE FROM [dbo].[Poliza] --"
        };

        var query = _builder.BuildCreateCommand(request);

        query.CommandText.Should().Contain("INSERT INTO [dbo].[Poliza]");
        query.CommandText.Should().Contain("OUTPUT INSERTED.[Id]");
        query.CommandText.Should().Contain("[IdSistemaOrigen]");
        query.CommandText.Should().Contain("@idSistemaOrigen");
        query.CommandText.Should().NotContain("DELETE FROM [dbo].[Poliza] --");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@numero" &&
            (string)parameter.Value == "POL-MVP-0001'; DELETE FROM [dbo].[Poliza] --");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@idSistemaOrigen" &&
            (string)parameter.Value == PolizasSqlCommandBuilder.MvpOrigin);
    }

    [Fact]
    public void BuildCreateCommand_maps_fecha_efecto_to_first_and_current_effect_dates()
    {
        var query = _builder.BuildCreateCommand(CreateValidRequest());

        query.CommandText.Should().Contain("[F_EfectoPrimero]");
        query.CommandText.Should().Contain("[F_Efecto]");
        query.CommandText.Should().Contain("@fechaEfecto");
        query.Parameters.Should().ContainSingle(parameter => parameter.Name == "@fechaEfecto");
    }

    [Fact]
    public void BuildUpdateCommand_updates_only_allowed_supplied_fields_by_stable_id_and_mvp_origin()
    {
        var query = _builder.BuildUpdateCommand(
            1001,
            new PolizaUpdateRequest(
                Numero: "POL-MVP-EDIT'; DROP TABLE Poliza --",
                PrimaAnual: 99.95m));

        query.CommandText.Should().Contain("UPDATE [dbo].[Poliza]");
        query.CommandText.Should().Contain("SET [Poliza] = @numero");
        query.CommandText.Should().Contain("[PAnualCartera] = @primaAnual");
        query.CommandText.Should().Contain("WHERE [Id] = @id");
        query.CommandText.Should().Contain("AND [IdSistemaOrigen] = @idSistemaOrigen");
        query.CommandText.Should().NotContain("[ClienteId]");
        query.CommandText.Should().NotContain("[CiaId]");
        query.CommandText.Should().NotContain("DROP TABLE");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@numero" &&
            (string)parameter.Value == "POL-MVP-EDIT'; DROP TABLE Poliza --");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@id" &&
            (int)parameter.Value == 1001);
    }

    [Fact]
    public void BuildUpdateCommand_updates_fecha_efecto_consistently_when_supplied()
    {
        var query = _builder.BuildUpdateCommand(
            1001,
            new PolizaUpdateRequest(FechaEfecto: new DateOnly(2026, 5, 18)));

        query.CommandText.Should().Contain("[F_Efecto] = @fechaEfecto");
        query.CommandText.Should().Contain("[F_EfectoPrimero] = @fechaEfecto");
        query.Parameters.Should().ContainSingle(parameter => parameter.Name == "@fechaEfecto");
    }

    [Fact]
    public void BuildDeleteCommand_deletes_only_by_stable_id_and_mvp_origin()
    {
        var query = _builder.BuildDeleteCommand(1001);

        query.CommandText.Should().Contain("DELETE FROM [dbo].[Poliza]");
        query.CommandText.Should().Contain("WHERE [Id] = @id");
        query.CommandText.Should().Contain("AND [IdSistemaOrigen] = @idSistemaOrigen");
        query.CommandText.Should().Contain("SELECT @@ROWCOUNT");
        query.CommandText.Should().NotContain("1001");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@id" &&
            (int)parameter.Value == 1001);
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@idSistemaOrigen" &&
            (string)parameter.Value == PolizasSqlCommandBuilder.MvpOrigin);
    }

    private static PolizaCreateRequest CreateValidRequest() =>
        new(
            Numero: "POL-MVP-0001",
            Aplicacion: "MVP",
            CiaId: 1,
            ClienteId: 1,
            Estado: "Vigor",
            Ramo: "Autos",
            TipoPoliza: "Cartera",
            FechaEfecto: new DateOnly(2026, 1, 1),
            FechaVencimiento: new DateOnly(2026, 12, 31),
            PrimaAnual: 123.45m);
}
