using FluentAssertions;
using iLiniumTech.Backend.Application.Clientes;
using iLiniumTech.Backend.Domain.Clientes;

namespace iLiniumTech.Backend.Tests.Clientes;

public sealed class ClientesWriteValidatorTests
{
    [Fact]
    public void ValidateCreate_rejects_blank_name()
    {
        var act = () => ClientesWriteValidator.ValidateCreate(new ClienteCreateRequest(" ", "particular"));

        act.Should().Throw<ClientesValidationException>()
            .WithMessage("*NombreMostrable*");
    }

    [Fact]
    public void ValidateCreate_rejects_invalid_tipo_cliente()
    {
        var act = () => ClientesWriteValidator.ValidateCreate(new ClienteCreateRequest("Cliente MVP", "vip"));

        act.Should().Throw<ClientesValidationException>()
            .WithMessage("*TipoCliente*");
    }

    [Fact]
    public void ValidateUpdate_requires_at_least_one_field()
    {
        var act = () => ClientesWriteValidator.ValidateUpdate(new ClienteUpdateRequest(null, null));

        act.Should().Throw<ClientesValidationException>()
            .WithMessage("*At least one editable field*");
    }
}
