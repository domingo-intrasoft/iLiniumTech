using System.Data;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Infrastructure.Polizas.Sql;

public sealed class PolizasSqlCommandBuilder
{
    public PolizasSqlQuery BuildCreateCommand(PolizaCreateRequest request)
    {
        const string commandText = """
            INSERT INTO [dbo].[Poliza] (
                [Poliza],
                [Aplicacion],
                [CiaId],
                [ClienteId],
                [IdSituacion],
                [IdRamo],
                [IdTipoPoliza],
                [F_EfectoPrimero],
                [F_Efecto],
                [F_Vencimiento],
                [IdFraccionPago],
                [IdGestor],
                [IdSistemaOrigen],
                [FCR]
            )
            VALUES (
                @numero,
                @aplicacion,
                @ciaId,
                @clienteId,
                @estado,
                @ramo,
                @tipoPoliza,
                @fechaEfecto,
                @fechaEfecto,
                @fechaVencimiento,
                @fraccionPago,
                @gestor,
                @sistemaOrigen,
                SYSUTCDATETIME()
            );
            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;

        return new PolizasSqlQuery(
            commandText,
            [
                Text("@numero", request.Numero!, 50),
                Text("@aplicacion", request.Aplicacion!, 30),
                Int("@ciaId", request.CiaId!.Value),
                Int("@clienteId", request.ClienteId!.Value),
                Text("@estado", request.Estado!, 100),
                Text("@ramo", request.Ramo!, 100),
                Text("@tipoPoliza", request.TipoPoliza!, 100),
                Date("@fechaEfecto", request.FechaEfecto!.Value),
                NullableDate("@fechaVencimiento", request.FechaVencimiento),
                Text("@fraccionPago", PolizasMvpWriteDefaults.FraccionPago, 100),
                Text("@gestor", PolizasMvpWriteDefaults.Gestor, 100),
                Text("@sistemaOrigen", PolizasMvpWriteDefaults.SistemaOrigen, 100)
            ]);
    }

    public PolizasSqlQuery BuildCreatedVisibilityQuery(int id)
    {
        const string commandText = """
            SELECT COUNT_BIG(1)
            FROM [dbo].[Pantalla_Polizas]
            WHERE [Id] = @id
              AND [Poliza] LIKE @mvpNumeroLike
              AND [Poliza] NOT LIKE @mvpDeletedNumeroLike;
            """;

        return new PolizasSqlQuery(
            commandText,
            [
                Int("@id", id),
                Text("@mvpNumeroLike", $"{PolizasMvpWriteDefaults.NumeroPrefix}%", 50),
                Text("@mvpDeletedNumeroLike", $"{PolizasMvpWriteDefaults.DeletedNumeroPrefix}%", 50)
            ]);
    }

    public PolizasSqlQuery BuildUpdateCommand(int id, PolizaUpdateRequest request)
    {
        var assignments = new List<string>();
        var parameters = new List<PolizasSqlParameter>
        {
            Int("@id", id),
            Text("@mvpNumeroLike", $"{PolizasMvpWriteDefaults.NumeroPrefix}%", 50),
            Text("@mvpDeletedNumeroLike", $"{PolizasMvpWriteDefaults.DeletedNumeroPrefix}%", 50)
        };

        AddTextAssignment(assignments, parameters, "[Poliza]", "@numero", request.Numero, 50);
        AddTextAssignment(assignments, parameters, "[Aplicacion]", "@aplicacion", request.Aplicacion, 30);
        AddTextAssignment(assignments, parameters, "[IdSituacion]", "@estado", request.Estado, 100);
        AddTextAssignment(assignments, parameters, "[IdRamo]", "@ramo", request.Ramo, 100);
        AddTextAssignment(assignments, parameters, "[IdTipoPoliza]", "@tipoPoliza", request.TipoPoliza, 100);
        AddFechaEfectoAssignments(assignments, parameters, request.FechaEfecto);
        AddNullableDateAssignment(assignments, parameters, "[F_Vencimiento]", "@fechaVencimiento", request.FechaVencimiento);

        if (assignments.Count == 0)
        {
            throw new InvalidOperationException("At least one SQL assignment is required.");
        }

        var commandText = $"""
            UPDATE [dbo].[Poliza]
            SET {string.Join(",\n                ", assignments)}
            WHERE [Id] = @id
              AND [Poliza] LIKE @mvpNumeroLike
              AND [Poliza] NOT LIKE @mvpDeletedNumeroLike;
            SELECT @@ROWCOUNT;
            """;

        return new PolizasSqlQuery(commandText, parameters);
    }

    public PolizasSqlQuery BuildDeleteCommand(int id)
    {
        const string commandText = """
            UPDATE [dbo].[Poliza]
            SET [Poliza] = CONCAT(@mvpDeletedNumeroPrefix, CONVERT(nvarchar(20), [Id]))
            WHERE [Id] = @id
              AND [Poliza] LIKE @mvpNumeroLike
              AND [Poliza] NOT LIKE @mvpDeletedNumeroLike;
            SELECT @@ROWCOUNT;
            """;

        return new PolizasSqlQuery(
            commandText,
            [
                Int("@id", id),
                Text("@mvpNumeroLike", $"{PolizasMvpWriteDefaults.NumeroPrefix}%", 50),
                Text("@mvpDeletedNumeroLike", $"{PolizasMvpWriteDefaults.DeletedNumeroPrefix}%", 50),
                Text("@mvpDeletedNumeroPrefix", PolizasMvpWriteDefaults.DeletedNumeroPrefix, 50)
            ]);
    }

    private static void AddTextAssignment(
        ICollection<string> assignments,
        ICollection<PolizasSqlParameter> parameters,
        string column,
        string parameterName,
        string? value,
        int size)
    {
        if (value is null)
        {
            return;
        }

        assignments.Add($"{column} = {parameterName}");
        parameters.Add(Text(parameterName, value, size));
    }

    private static void AddFechaEfectoAssignments(
        ICollection<string> assignments,
        ICollection<PolizasSqlParameter> parameters,
        DateOnly? value)
    {
        if (value is null)
        {
            return;
        }

        assignments.Add("[F_Efecto] = @fechaEfecto");
        assignments.Add("[F_EfectoPrimero] = @fechaEfecto");
        parameters.Add(Date("@fechaEfecto", value.Value));
    }

    private static void AddNullableDateAssignment(
        ICollection<string> assignments,
        ICollection<PolizasSqlParameter> parameters,
        string column,
        string parameterName,
        DateOnly? value)
    {
        if (value is null)
        {
            return;
        }

        assignments.Add($"{column} = {parameterName}");
        parameters.Add(Date(parameterName, value.Value));
    }

    private static PolizasSqlParameter Text(string name, string value, int size) =>
        new(name, value.Trim(), SqlDbType.NVarChar, size);

    private static PolizasSqlParameter Int(string name, int value) =>
        new(name, value, SqlDbType.Int);

    private static PolizasSqlParameter Date(string name, DateOnly value) =>
        new(name, value.ToDateTime(TimeOnly.MinValue), SqlDbType.Date);

    private static PolizasSqlParameter NullableDate(string name, DateOnly? value) =>
        value is null ? new PolizasSqlParameter(name, DBNull.Value, SqlDbType.Date) : Date(name, value.Value);
}
