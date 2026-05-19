using System.Data;
using iLiniumTech.Backend.Domain.Agenda;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Infrastructure.Agenda;

public sealed class SqlAgendaCommandBuilder
{
    public PolizasSqlQuery BuildCreateCommand(AgendaCreateRequest request, int brokerId, int? userId)
    {
        const string commandText = """
            INSERT INTO [dbo].[Agenda] (
                [IdOld],
                [Asunto],
                [Descripcion],
                [F_Inicio],
                [F_Fin],
                [Hora_Inicio],
                [Hora_Fin],
                [IdPrioridad],
                [IdObjeto],
                [BrokerIntegracionId],
                [FCR],
                [UCR]
            )
            VALUES (
                @referencia,
                @titulo,
                @descripcion,
                @fechaInicio,
                @fechaFin,
                @horaInicio,
                @horaFin,
                @prioridad,
                @objetoRelacionadoTipo,
                @brokerId,
                SYSUTCDATETIME(),
                @userId
            );
            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;

        return new PolizasSqlQuery(
            commandText,
            [
                Text("@referencia", request.Referencia!, 50),
                Text("@titulo", request.Titulo!, 100),
                Text("@descripcion", AgendaMvpWriteDefaults.Description, 2000),
                Date("@fechaInicio", DateOnly.FromDateTime(request.Inicio!.Value)),
                NullableDate("@fechaFin", request.Fin is null ? null : DateOnly.FromDateTime(request.Fin.Value)),
                Time("@horaInicio", TimeOnly.FromDateTime(request.Inicio!.Value)),
                NullableTime("@horaFin", request.Fin is null ? null : TimeOnly.FromDateTime(request.Fin.Value)),
                Text("@prioridad", request.Prioridad ?? "Media", 50),
                NullableText("@objetoRelacionadoTipo", request.ObjetoRelacionadoTipo, 50),
                Int("@brokerId", brokerId),
                NullableInt("@userId", userId)
            ]);
    }

    public PolizasSqlQuery BuildCreatedVisibilityQuery(int id, int brokerId) =>
        new(
            """
            SELECT COUNT_BIG(1)
            FROM [dbo].[Agenda]
            WHERE [Id] = @id
              AND [BrokerIntegracionId] = @brokerId
              AND [IdOld] LIKE @mvpReferenciaLike
              AND [IdOld] NOT LIKE @mvpDeletedReferenciaLike;
            """,
            [
                Int("@id", id),
                Int("@brokerId", brokerId),
                Text("@mvpReferenciaLike", $"{AgendaMvpWriteDefaults.ReferenciaPrefix}%", 50),
                Text("@mvpDeletedReferenciaLike", $"{AgendaMvpWriteDefaults.DeletedReferenciaPrefix}%", 50)
            ]);

    public PolizasSqlQuery BuildUpdateCommand(int id, AgendaUpdateRequest request, int brokerId, int? userId)
    {
        var assignments = new List<string> { "[FUM] = SYSUTCDATETIME()", "[UUM] = @userId" };
        var parameters = new List<PolizasSqlParameter>
        {
            Int("@id", id),
            Int("@brokerId", brokerId),
            NullableInt("@userId", userId),
            Text("@mvpReferenciaLike", $"{AgendaMvpWriteDefaults.ReferenciaPrefix}%", 50),
            Text("@mvpDeletedReferenciaLike", $"{AgendaMvpWriteDefaults.DeletedReferenciaPrefix}%", 50)
        };

        AddTextAssignment(assignments, parameters, "[Asunto]", "@titulo", request.Titulo, 100);
        AddDateTimeAssignments(assignments, parameters, request.Inicio, request.Fin);
        AddTextAssignment(assignments, parameters, "[IdPrioridad]", "@prioridad", request.Prioridad, 50);
        AddNullableTextAssignment(
            assignments,
            parameters,
            "[IdObjeto]",
            "@objetoRelacionadoTipo",
            request.ObjetoRelacionadoTipo,
            50);

        return new PolizasSqlQuery(
            $"""
            UPDATE [dbo].[Agenda]
            SET {string.Join(",\n                ", assignments)}
            WHERE [Id] = @id
              AND [BrokerIntegracionId] = @brokerId
              AND [IdOld] LIKE @mvpReferenciaLike
              AND [IdOld] NOT LIKE @mvpDeletedReferenciaLike;
            SELECT @@ROWCOUNT;
            """,
            parameters);
    }

    public PolizasSqlQuery BuildDeleteCommand(int id, int brokerId, int? userId) =>
        new(
            """
            UPDATE [dbo].[Agenda]
            SET [IdOld] = CONCAT(@mvpDeletedReferenciaPrefix, CONVERT(nvarchar(20), [Id])),
                [FUM] = SYSUTCDATETIME(),
                [UUM] = @userId
            WHERE [Id] = @id
              AND [BrokerIntegracionId] = @brokerId
              AND [IdOld] LIKE @mvpReferenciaLike
              AND [IdOld] NOT LIKE @mvpDeletedReferenciaLike;
            SELECT @@ROWCOUNT;
            """,
            [
                Int("@id", id),
                Int("@brokerId", brokerId),
                NullableInt("@userId", userId),
                Text("@mvpReferenciaLike", $"{AgendaMvpWriteDefaults.ReferenciaPrefix}%", 50),
                Text("@mvpDeletedReferenciaLike", $"{AgendaMvpWriteDefaults.DeletedReferenciaPrefix}%", 50),
                Text("@mvpDeletedReferenciaPrefix", AgendaMvpWriteDefaults.DeletedReferenciaPrefix, 50)
            ]);

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

    private static void AddNullableTextAssignment(
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
        parameters.Add(NullableText(parameterName, value, size));
    }

    private static void AddDateTimeAssignments(
        ICollection<string> assignments,
        ICollection<PolizasSqlParameter> parameters,
        DateTime? inicio,
        DateTime? fin)
    {
        if (inicio is not null)
        {
            assignments.Add("[F_Inicio] = @fechaInicio");
            assignments.Add("[Hora_Inicio] = @horaInicio");
            parameters.Add(Date("@fechaInicio", DateOnly.FromDateTime(inicio.Value)));
            parameters.Add(Time("@horaInicio", TimeOnly.FromDateTime(inicio.Value)));
        }

        if (fin is not null)
        {
            assignments.Add("[F_Fin] = @fechaFin");
            assignments.Add("[Hora_Fin] = @horaFin");
            parameters.Add(Date("@fechaFin", DateOnly.FromDateTime(fin.Value)));
            parameters.Add(Time("@horaFin", TimeOnly.FromDateTime(fin.Value)));
        }
    }

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

    private static PolizasSqlParameter Date(string name, DateOnly value) =>
        new(name, value.ToDateTime(TimeOnly.MinValue), SqlDbType.Date);

    private static PolizasSqlParameter NullableDate(string name, DateOnly? value) =>
        value is null ? new PolizasSqlParameter(name, DBNull.Value, SqlDbType.Date) : Date(name, value.Value);

    private static PolizasSqlParameter Time(string name, TimeOnly value) =>
        new(name, value.ToTimeSpan(), SqlDbType.Time);

    private static PolizasSqlParameter NullableTime(string name, TimeOnly? value) =>
        value is null ? new PolizasSqlParameter(name, DBNull.Value, SqlDbType.Time) : Time(name, value.Value);
}
