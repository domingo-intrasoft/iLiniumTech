using FluentAssertions;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class PolizasWriteValidatorTests
{
    [Fact]
    public void ValidateCreate_accepts_minimum_safe_payload()
    {
        var request = CreateValidRequest();

        var act = () => PolizasWriteValidator.ValidateCreate(request);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateCreate_rejects_missing_numero(string? numero)
    {
        var request = CreateValidRequest() with
        {
            Numero = numero
        };

        var act = () => PolizasWriteValidator.ValidateCreate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("Numero is required.");
    }

    [Fact]
    public void ValidateCreate_rejects_zero_relational_ids()
    {
        var request = CreateValidRequest() with
        {
            CiaId = 0
        };

        var act = () => PolizasWriteValidator.ValidateCreate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("CiaId must be a non-zero integer.");
    }

    [Fact]
    public void ValidateCreate_accepts_negative_appbuilder_relational_ids()
    {
        var request = CreateValidRequest() with
        {
            CiaId = -2639
        };

        var act = () => PolizasWriteValidator.ValidateCreate(request);

        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateCreate_rejects_reserved_deleted_mvp_prefix()
    {
        var request = CreateValidRequest() with
        {
            Numero = $"{PolizasMvpWriteDefaults.DeletedNumeroPrefix}1001"
        };

        var act = () => PolizasWriteValidator.ValidateCreate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("Numero cannot use the reserved MVP deleted prefix.");
    }

    [Fact]
    public void ValidateCreate_rejects_vencimiento_before_efecto()
    {
        var request = CreateValidRequest() with
        {
            FechaEfecto = new DateOnly(2026, 2, 1),
            FechaVencimiento = new DateOnly(2026, 1, 31)
        };

        var act = () => PolizasWriteValidator.ValidateCreate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("FechaVencimiento must be greater than or equal to FechaEfecto.");
    }

    [Fact]
    public void ValidateCreate_rejects_prima_anual_until_writable_column_is_confirmed()
    {
        var request = CreateValidRequest() with
        {
            PrimaAnual = 1
        };

        var act = () => PolizasWriteValidator.ValidateCreate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("PrimaAnual is read-only until DBA/UAT confirms a writable column.");
    }

    [Fact]
    public void ValidateUpdate_rejects_prima_anual_until_writable_column_is_confirmed()
    {
        var act = () => PolizasWriteValidator.ValidateUpdate(new PolizaUpdateRequest(
            PrimaAnual: 1));

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("PrimaAnual is read-only until DBA/UAT confirms a writable column.");
    }

    [Fact]
    public void ValidateCreate_rejects_long_numero()
    {
        var request = CreateValidRequest() with
        {
            Numero = new string('P', 51)
        };

        var act = () => PolizasWriteValidator.ValidateCreate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("Numero length must be lower than or equal to 50.");
    }

    [Fact]
    public void ValidateUpdate_requires_at_least_one_field()
    {
        var act = () => PolizasWriteValidator.ValidateUpdate(new PolizaUpdateRequest());

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("At least one editable field is required.");
    }

    [Fact]
    public void ValidateUpdate_rejects_empty_text_changes()
    {
        var act = () => PolizasWriteValidator.ValidateUpdate(new PolizaUpdateRequest(
            Numero: "   "));

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("Numero cannot be empty.");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1001")]
    public void ValidateAndParseId_accepts_positive_integer_ids(string id)
    {
        var parsed = PolizasWriteValidator.ValidateAndParseId(id);

        parsed.Should().Be(int.Parse(id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("POL-1001")]
    public void ValidateAndParseId_rejects_non_numeric_or_non_positive_ids(string? id)
    {
        var act = () => PolizasWriteValidator.ValidateAndParseId(id);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("Poliza id must be a positive integer.");
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
