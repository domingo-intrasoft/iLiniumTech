using Microsoft.Data.SqlClient;
using System.Data;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;

namespace iLiniumTech.Backend.Infrastructure.Polizas.Sql;

internal static class SqlServerSessionContext
{
    private const int SessionContextKeySize = 128;
    private const int ProfileTypeIdSize = 100;

    internal static PolizasSqlQuery BuildApplyQuery(PolizasExecutionContext? context)
    {
        var parameters = new List<PolizasSqlParameter>
        {
            Key("@brokerIdKey", "brokerId"),
            Value("@brokerId", context?.BrokerId, SqlDbType.Int),
            Key("@entityMainIdKey", "entityMainId"),
            Value("@entityMainId", context?.EntityMainId, SqlDbType.Int),
            Key("@userIdKey", "userId"),
            Value("@userId", context?.UserId, SqlDbType.Int),
            Key("@profileIdKey", "profileId"),
            Value("@profileId", context?.ProfileId, SqlDbType.Int),
            Key("@profileTypeIdKey", "profileTypeId"),
            Value("@profileTypeId", string.IsNullOrWhiteSpace(context?.ProfileTypeId) ? null : context.ProfileTypeId, SqlDbType.NVarChar, ProfileTypeIdSize),
            Key("@isAdminKey", "isAdmin"),
            Value("@isAdmin", context?.IsAdmin, SqlDbType.Bit)
        };

        return new PolizasSqlQuery(
            """
            EXEC sys.sp_set_session_context @key=@brokerIdKey, @value=@brokerId;
            EXEC sys.sp_set_session_context @key=@entityMainIdKey, @value=@entityMainId;
            EXEC sys.sp_set_session_context @key=@userIdKey, @value=@userId;
            EXEC sys.sp_set_session_context @key=@profileIdKey, @value=@profileId;
            EXEC sys.sp_set_session_context @key=@profileTypeIdKey, @value=@profileTypeId;
            EXEC sys.sp_set_session_context @key=@isAdminKey, @value=@isAdmin;
            """,
            parameters);
    }

    public static async Task ApplyAsync(
        SqlConnection connection,
        PolizasExecutionContext? context,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        var query = BuildApplyQuery(context);
        command.CommandText = query.CommandText;
        command.CommandType = CommandType.Text;

        foreach (var parameter in query.Parameters)
        {
            var sqlParameter = parameter.Size.HasValue && parameter.DbType.HasValue
                ? command.Parameters.Add(parameter.Name, parameter.DbType.Value, parameter.Size.Value)
                : parameter.DbType.HasValue
                    ? command.Parameters.Add(parameter.Name, parameter.DbType.Value)
                    : command.Parameters.AddWithValue(parameter.Name, parameter.Value);
            sqlParameter.Value = parameter.Value;
        }

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static PolizasSqlParameter Key(string name, string value) =>
        new(name, value, SqlDbType.NVarChar, SessionContextKeySize);

    private static PolizasSqlParameter Value(string name, object? value, SqlDbType dbType, int? size = null) =>
        new(name, value ?? DBNull.Value, dbType, size);
}
