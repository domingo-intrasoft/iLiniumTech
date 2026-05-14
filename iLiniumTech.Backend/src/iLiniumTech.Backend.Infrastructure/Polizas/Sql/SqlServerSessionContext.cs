using Microsoft.Data.SqlClient;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;

namespace iLiniumTech.Backend.Infrastructure.Polizas.Sql;

internal static class SqlServerSessionContext
{
    public static async Task ApplyAsync(
        SqlConnection connection,
        PolizasExecutionContext? context,
        CancellationToken cancellationToken)
    {
        if (context is null)
        {
            return;
        }

        await using var command = connection.CreateCommand();
        command.CommandText = """
            EXEC sys.sp_set_session_context @key=N'brokerId', @value=@brokerId;
            EXEC sys.sp_set_session_context @key=N'entityMainId', @value=@entityMainId;
            EXEC sys.sp_set_session_context @key=N'userId', @value=@userId;
            EXEC sys.sp_set_session_context @key=N'profileId', @value=@profileId;
            EXEC sys.sp_set_session_context @key=N'profileTypeId', @value=@profileTypeId;
            EXEC sys.sp_set_session_context @key=N'isAdmin', @value=@isAdmin;
            """;
        command.Parameters.AddWithValue("@brokerId", context.BrokerId);
        command.Parameters.AddWithValue("@entityMainId", context.EntityMainId);
        command.Parameters.AddWithValue("@userId", context.UserId is null ? DBNull.Value : context.UserId.Value);
        command.Parameters.AddWithValue("@profileId", context.ProfileId is null ? DBNull.Value : context.ProfileId.Value);
        command.Parameters.AddWithValue("@profileTypeId", string.IsNullOrWhiteSpace(context.ProfileTypeId) ? DBNull.Value : context.ProfileTypeId);
        command.Parameters.AddWithValue("@isAdmin", context.IsAdmin is null ? DBNull.Value : context.IsAdmin.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
