using FluentAssertions;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class PolizasSearchValidatorTests
{
    [Fact]
    public void Validate_rejects_page_lower_than_one()
    {
        var request = new PolizasSearchRequest(Page: 0);

        var act = () => PolizasSearchValidator.Validate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("*Page*greater than zero*");
    }

    [Fact]
    public void Validate_rejects_page_size_lower_than_one()
    {
        var request = new PolizasSearchRequest(Page: 1, PageSize: 0);

        var act = () => PolizasSearchValidator.Validate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("*PageSize*");
    }

    [Fact]
    public void Validate_rejects_page_size_over_limit()
    {
        var request = new PolizasSearchRequest(Page: 1, PageSize: 101);

        var act = () => PolizasSearchValidator.Validate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("*PageSize*");
    }

    [Fact]
    public void Validate_rejects_inverted_fecha_efecto_range()
    {
        var request = new PolizasSearchRequest(
            FechaEfectoDesde: new DateOnly(2026, 5, 1),
            FechaEfectoHasta: new DateOnly(2026, 1, 1));

        var act = () => PolizasSearchValidator.Validate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("*FechaEfectoDesde*");
    }

    [Fact]
    public void ValidateAndParseSort_rejects_unknown_sort_fields()
    {
        var act = () => PolizasSearchValidator.ValidateAndParseSort("rawSql:desc");

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("*not allowed*");
    }

    [Fact]
    public void ValidateAndParseSort_rejects_unknown_sort_direction()
    {
        var act = () => PolizasSearchValidator.ValidateAndParseSort("fechaEfecto:drop");

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("*direction*");
    }

    [Fact]
    public void ValidateAndParseSort_uses_safe_default_sort()
    {
        var sort = PolizasSearchValidator.ValidateAndParseSort(null);

        sort.Field.Should().Be("fechaEfecto");
        sort.Descending.Should().BeTrue();
    }

    [Fact]
    public void ValidateAndParseSort_accepts_whitelisted_field()
    {
        var sort = PolizasSearchValidator.ValidateAndParseSort("fechaEfecto:asc");

        sort.Field.Should().Be("fechaEfecto");
        sort.Descending.Should().BeFalse();
    }
}
