using System.Data;
using FluentAssertions;
using iLiniumTech.Backend.Domain.Clientes;
using iLiniumTech.Backend.Infrastructure.Clientes;

namespace iLiniumTech.Backend.Tests.Clientes;

public sealed class ClientesSqlCommandBuilderTests
{
    private readonly SqlClientesCommandBuilder _builder = new();

    [Fact]
    public void BuildCreateCommand_writes_identidad_and_identidad_cliente_with_broker_filter_inputs()
    {
        var query = _builder.BuildCreateCommand(
            new ClienteCreateRequest("Cliente MVP local", "particular"),
            brokerId: 325,
            userId: 7);

        query.CommandText.Should().Contain("[dbo].[Identidad]");
        query.CommandText.Should().Contain("[dbo].[IdentidadCliente]");
        query.CommandText.Should().Contain("[BrokerIntegracionId]");
        query.CommandText.Should().Contain("[IdOld]");
        query.CommandText.Should().Contain("SCOPE_IDENTITY()");
        query.CommandText.Contains("NumDocumento", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.CommandText.Contains("Email", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.CommandText.Contains("Telefono", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.CommandText.Contains("Direccion", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.CommandText.Contains("IBAN", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@referencia" &&
            parameter.Value.ToString()!.StartsWith(ClientesMvpWriteDefaults.ReferenciaPrefix, StringComparison.Ordinal));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@brokerId" && Equals(parameter.Value, 325));
        query.Parameters.Should().Contain(parameter => parameter.Name == "@userId" && Equals(parameter.Value, 7));
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@nombreMostrable" && parameter.DbType == SqlDbType.NVarChar);
    }

    [Fact]
    public void BuildUpdateCommand_limits_changes_to_mvp_owned_rows()
    {
        var query = _builder.BuildUpdateCommand(
            id: 123,
            new ClienteUpdateRequest("Cliente actualizado", "empresa"),
            brokerId: 325,
            userId: 7);

        query.CommandText.Should().Contain("[identidad].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("[identidad].[IdOld] LIKE @mvpReferenciaLike");
        query.CommandText.Should().Contain("[identidad].[IdOld] NOT LIKE @mvpDeletedReferenciaLike");
        query.CommandText.Should().Contain("[cliente].[ClienteId] = @id");
        query.CommandText.Contains("DROP TABLE", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@mvpReferenciaLike" &&
            Equals(parameter.Value, $"{ClientesMvpWriteDefaults.ReferenciaPrefix}%"));
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@mvpDeletedReferenciaLike" &&
            Equals(parameter.Value, $"{ClientesMvpWriteDefaults.DeletedReferenciaPrefix}%"));
    }

    [Fact]
    public void BuildDeleteCommand_marks_mvp_row_as_deleted_without_physical_delete()
    {
        var query = _builder.BuildDeleteCommand(id: 123, brokerId: 325, userId: 7);

        query.CommandText.Should().Contain("UPDATE [identidad]");
        query.CommandText.Should().Contain("[IdOld] = CONCAT(@mvpDeletedReferenciaPrefix");
        query.CommandText.Should().Contain("[identidad].[BrokerIntegracionId] = @brokerId");
        query.CommandText.Should().Contain("[identidad].[IdOld] LIKE @mvpReferenciaLike");
        query.CommandText.Contains("DELETE FROM", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        query.CommandText.Contains("NumDocumento", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }
}
