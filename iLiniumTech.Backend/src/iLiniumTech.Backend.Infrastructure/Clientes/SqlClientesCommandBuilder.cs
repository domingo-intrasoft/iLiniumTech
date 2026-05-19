using System.Data;
using iLiniumTech.Backend.Application.Clientes;
using iLiniumTech.Backend.Domain.Clientes;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Infrastructure.Clientes;

public sealed class SqlClientesCommandBuilder
{
    public PolizasSqlQuery BuildCreateCommand(ClienteCreateRequest request, int brokerId, int? userId)
    {
        const string commandText = """
            DECLARE @clienteId int;

            INSERT INTO [dbo].[Identidad] (
                [IdOld],
                [Nombre],
                [RazonSocial],
                [BrokerIntegracionId],
                [FCR],
                [UCR]
            )
            VALUES (
                @referencia,
                CASE WHEN @tipoCliente = N'empresa' THEN NULL ELSE @nombreMostrable END,
                CASE WHEN @tipoCliente = N'empresa' THEN @nombreMostrable ELSE NULL END,
                @brokerId,
                SYSUTCDATETIME(),
                @userId
            );

            SET @clienteId = CAST(SCOPE_IDENTITY() AS int);

            INSERT INTO [dbo].[IdentidadCliente] (
                [ClienteId],
                [FCR],
                [UCR]
            )
            VALUES (
                @clienteId,
                SYSUTCDATETIME(),
                @userId
            );

            SELECT @clienteId;
            """;

        return new PolizasSqlQuery(
            commandText,
            [
                Text("@referencia", CreateReference(), 50),
                Text("@nombreMostrable", request.NombreMostrable!, 120),
                Text("@tipoCliente", ClientesWriteValidator.NormalizeTipoCliente(request.TipoCliente), 20),
                Int("@brokerId", brokerId),
                NullableInt("@userId", userId)
            ]);
    }

    public PolizasSqlQuery BuildCreatedVisibilityQuery(int id, int brokerId) =>
        new(
            """
            SELECT COUNT_BIG(1)
            FROM [dbo].[IdentidadCliente] AS [cliente]
            INNER JOIN [dbo].[Identidad] AS [identidad] ON [identidad].[Id] = [cliente].[ClienteId]
            WHERE [cliente].[ClienteId] = @id
              AND [identidad].[BrokerIntegracionId] = @brokerId
              AND [identidad].[IdOld] LIKE @mvpReferenciaLike
              AND [identidad].[IdOld] NOT LIKE @mvpDeletedReferenciaLike;
            """,
            [
                Int("@id", id),
                Int("@brokerId", brokerId),
                Text("@mvpReferenciaLike", $"{ClientesMvpWriteDefaults.ReferenciaPrefix}%", 50),
                Text("@mvpDeletedReferenciaLike", $"{ClientesMvpWriteDefaults.DeletedReferenciaPrefix}%", 50)
            ]);

    public PolizasSqlQuery BuildUpdateCommand(int id, ClienteUpdateRequest request, int brokerId, int? userId) =>
        new(
            """
            UPDATE [identidad]
            SET [Nombre] = CASE WHEN [nextValues].[TipoCliente] = N'empresa' THEN NULL ELSE [nextValues].[Alias] END,
                [RazonSocial] = CASE WHEN [nextValues].[TipoCliente] = N'empresa' THEN [nextValues].[Alias] ELSE NULL END,
                [FUM] = SYSUTCDATETIME(),
                [UUM] = @userId
            FROM [dbo].[Identidad] AS [identidad]
            INNER JOIN [dbo].[IdentidadCliente] AS [cliente] ON [cliente].[ClienteId] = [identidad].[Id]
            CROSS APPLY (
                SELECT
                    COALESCE(
                        @nombreMostrable,
                        NULLIF(LTRIM(RTRIM(CAST([identidad].[RazonSocial] AS nvarchar(250)))), ''),
                        NULLIF(LTRIM(RTRIM(CAST([identidad].[Nombre] AS nvarchar(100)))), ''),
                        CONCAT(N'Cliente ', CONVERT(nvarchar(20), [identidad].[Id]))) AS [Alias],
                    COALESCE(
                        @tipoCliente,
                        CASE
                            WHEN NULLIF(LTRIM(RTRIM(CAST([identidad].[RazonSocial] AS nvarchar(250)))), '') IS NULL
                                THEN N'particular'
                            ELSE N'empresa'
                        END) AS [TipoCliente]
            ) AS [nextValues]
            WHERE [identidad].[Id] = @id
              AND [cliente].[ClienteId] = @id
              AND [identidad].[BrokerIntegracionId] = @brokerId
              AND [identidad].[IdOld] LIKE @mvpReferenciaLike
              AND [identidad].[IdOld] NOT LIKE @mvpDeletedReferenciaLike;
            SELECT @@ROWCOUNT;
            """,
            [
                Int("@id", id),
                Int("@brokerId", brokerId),
                NullableInt("@userId", userId),
                NullableText("@nombreMostrable", request.NombreMostrable, 120),
                NullableText("@tipoCliente", request.TipoCliente is null ? null : ClientesWriteValidator.NormalizeTipoCliente(request.TipoCliente), 20),
                Text("@mvpReferenciaLike", $"{ClientesMvpWriteDefaults.ReferenciaPrefix}%", 50),
                Text("@mvpDeletedReferenciaLike", $"{ClientesMvpWriteDefaults.DeletedReferenciaPrefix}%", 50)
            ]);

    public PolizasSqlQuery BuildDeleteCommand(int id, int brokerId, int? userId) =>
        new(
            """
            UPDATE [identidad]
            SET [IdOld] = CONCAT(@mvpDeletedReferenciaPrefix, CONVERT(nvarchar(20), [identidad].[Id])),
                [FUM] = SYSUTCDATETIME(),
                [UUM] = @userId
            FROM [dbo].[Identidad] AS [identidad]
            INNER JOIN [dbo].[IdentidadCliente] AS [cliente] ON [cliente].[ClienteId] = [identidad].[Id]
            WHERE [identidad].[Id] = @id
              AND [cliente].[ClienteId] = @id
              AND [identidad].[BrokerIntegracionId] = @brokerId
              AND [identidad].[IdOld] LIKE @mvpReferenciaLike
              AND [identidad].[IdOld] NOT LIKE @mvpDeletedReferenciaLike;
            SELECT @@ROWCOUNT;
            """,
            [
                Int("@id", id),
                Int("@brokerId", brokerId),
                NullableInt("@userId", userId),
                Text("@mvpReferenciaLike", $"{ClientesMvpWriteDefaults.ReferenciaPrefix}%", 50),
                Text("@mvpDeletedReferenciaLike", $"{ClientesMvpWriteDefaults.DeletedReferenciaPrefix}%", 50),
                Text("@mvpDeletedReferenciaPrefix", ClientesMvpWriteDefaults.DeletedReferenciaPrefix, 50)
            ]);

    private static string CreateReference() =>
        $"{ClientesMvpWriteDefaults.ReferenciaPrefix}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

    private static PolizasSqlParameter Text(string name, string value, int size) =>
        new(name, value.Trim(), SqlDbType.NVarChar, size);

    private static PolizasSqlParameter NullableText(string name, string? value, int size) =>
        string.IsNullOrWhiteSpace(value)
            ? new PolizasSqlParameter(name, DBNull.Value, SqlDbType.NVarChar, size)
            : Text(name, value, size);

    private static PolizasSqlParameter Int(string name, int value) =>
        new(name, value, SqlDbType.Int);

    private static PolizasSqlParameter NullableInt(string name, int? value) =>
        new(name, value ?? (object)DBNull.Value, SqlDbType.Int);
}
