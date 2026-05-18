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
            Numero = "ILMVP-0001'; DELETE FROM [dbo].[Poliza] --"
        };

        var query = _builder.BuildCreateCommand(request);

        query.CommandText.Should().Contain("INSERT INTO [dbo].[Poliza]");
        query.CommandText.Should().Contain("SELECT CAST(SCOPE_IDENTITY() AS int)");
        query.CommandText.Should().NotContain("OUTPUT INSERTED");
        query.CommandText.Should().Contain("[IdSistemaOrigen]");
        query.CommandText.Should().Contain("[IdFraccionPago]");
        query.CommandText.Should().Contain("[IdGestor]");
        query.CommandText.Should().Contain("[FCR]");
        query.CommandText.Should().Contain("SYSUTCDATETIME()");
        query.CommandText.Should().NotContain("[PAnualCartera]");
        query.CommandText.Should().Contain("@fraccionPago");
        query.CommandText.Should().Contain("@gestor");
        query.CommandText.Should().Contain("@sistemaOrigen");
        query.CommandText.Should().NotContain("@fcr");
        query.CommandText.Should().NotContain("@primaAnual");
        query.CommandText.Should().NotContain("DELETE FROM [dbo].[Poliza] --");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@numero" &&
            (string)parameter.Value == "ILMVP-0001'; DELETE FROM [dbo].[Poliza] --");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@sistemaOrigen" &&
            (string)parameter.Value == PolizasMvpWriteDefaults.SistemaOrigen);
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@fraccionPago" &&
            (string)parameter.Value == PolizasMvpWriteDefaults.FraccionPago);
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@gestor" &&
            (string)parameter.Value == PolizasMvpWriteDefaults.Gestor);
    }

    [Fact]
    public void BuildCreatedVisibilityQuery_checks_created_mvp_row_through_read_view()
    {
        var query = _builder.BuildCreatedVisibilityQuery(1001);

        query.CommandText.Should().Contain("FROM [dbo].[Pantalla_Polizas]");
        query.CommandText.Should().Contain("WHERE [Id] = @id");
        query.CommandText.Should().Contain("AND [Poliza] LIKE @mvpNumeroLike");
        query.CommandText.Should().Contain("AND [Poliza] NOT LIKE @mvpDeletedNumeroLike");
        query.CommandText.Should().NotContain("1001");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@id" &&
            (int)parameter.Value == 1001);
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@mvpNumeroLike" &&
            (string)parameter.Value == $"{PolizasMvpWriteDefaults.NumeroPrefix}%");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@mvpDeletedNumeroLike" &&
            (string)parameter.Value == $"{PolizasMvpWriteDefaults.DeletedNumeroPrefix}%");
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
    public void BuildUpdateCommand_updates_only_allowed_supplied_fields_by_stable_id_and_mvp_prefix()
    {
        var query = _builder.BuildUpdateCommand(
            1001,
            new PolizaUpdateRequest(
                Numero: "ILMVP-EDIT'; DROP TABLE Poliza --"));

        query.CommandText.Should().Contain("UPDATE [dbo].[Poliza]");
        query.CommandText.Should().Contain("SET [Poliza] = @numero");
        query.CommandText.Should().NotContain("[PAnualCartera]");
        query.CommandText.Should().NotContain("@primaAnual");
        query.CommandText.Should().Contain("WHERE [Id] = @id");
        query.CommandText.Should().Contain("AND [Poliza] LIKE @mvpNumeroLike");
        query.CommandText.Should().Contain("AND [Poliza] NOT LIKE @mvpDeletedNumeroLike");
        query.CommandText.Should().NotContain("[ClienteId]");
        query.CommandText.Should().NotContain("[CiaId]");
        query.CommandText.Should().NotContain("DROP TABLE");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@numero" &&
            (string)parameter.Value == "ILMVP-EDIT'; DROP TABLE Poliza --");
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
    public void BuildDeleteCommand_soft_deletes_only_by_stable_id_and_mvp_prefix()
    {
        var query = _builder.BuildDeleteCommand(1001);

        query.CommandText.Should().Contain("UPDATE [dbo].[Poliza]");
        query.CommandText.Should().Contain("SET [Poliza] = CONCAT(@mvpDeletedNumeroPrefix");
        query.CommandText.Should().Contain("WHERE [Id] = @id");
        query.CommandText.Should().Contain("AND [Poliza] LIKE @mvpNumeroLike");
        query.CommandText.Should().Contain("AND [Poliza] NOT LIKE @mvpDeletedNumeroLike");
        query.CommandText.Should().Contain("SELECT @@ROWCOUNT");
        query.CommandText.Should().NotContain("1001");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@id" &&
            (int)parameter.Value == 1001);
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@mvpNumeroLike" &&
            (string)parameter.Value == $"{PolizasMvpWriteDefaults.NumeroPrefix}%");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@mvpDeletedNumeroLike" &&
            (string)parameter.Value == $"{PolizasMvpWriteDefaults.DeletedNumeroPrefix}%");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@mvpDeletedNumeroPrefix" &&
            (string)parameter.Value == PolizasMvpWriteDefaults.DeletedNumeroPrefix);
    }

    private static PolizaCreateRequest CreateValidRequest() =>
        new(
            Numero: "ILMVP-0001",
            Aplicacion: "MVP",
            CiaId: 1,
            ClienteId: 1,
            Estado: "Vigor",
            Ramo: "Autos",
            TipoPoliza: "Cartera",
            FechaEfecto: new DateOnly(2026, 1, 1),
            FechaVencimiento: new DateOnly(2026, 12, 31),
            PrimaAnual: null);
}
