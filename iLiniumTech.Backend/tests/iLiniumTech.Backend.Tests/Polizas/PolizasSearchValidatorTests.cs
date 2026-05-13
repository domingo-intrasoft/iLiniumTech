using FluentAssertions;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class PolizasSearchValidatorTests
{
    [Fact]
    public void Validate_rejects_page_size_over_limit()
    {
        var request = new PolizasSearchRequest(Page: 1, PageSize: 101);

        var act = () => PolizasSearchValidator.Validate(request);

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("*PageSize*");
    }

    [Fact]
    public void ValidateAndParseSort_rejects_unknown_sort_fields()
    {
        var act = () => PolizasSearchValidator.ValidateAndParseSort("rawSql:desc");

        act.Should().Throw<PolizasValidationException>()
            .WithMessage("*not allowed*");
    }

    [Fact]
    public void ValidateAndParseSort_accepts_whitelisted_field()
    {
        var sort = PolizasSearchValidator.ValidateAndParseSort("fechaEfecto:asc");

        sort.Field.Should().Be("fechaEfecto");
        sort.Descending.Should().BeFalse();
    }
}
