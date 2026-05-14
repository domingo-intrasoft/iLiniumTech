using System.Data;
using Microsoft.Data.SqlClient;

namespace iLiniumTech.Backend.Infrastructure.Polizas.Connections;

public sealed class AppBuilderMasterPolizasConnectionStringProvider(
    string masterConnectionString,
    IPolizasExecutionContextAccessor executionContextAccessor,
    AppBuilderConnectionValueProtector? valueProtector = null,
    string modelDatabaseTypeId = AppBuilderMasterPolizasConnectionStringProvider.DefaultModelDatabaseTypeId)
    : IPolizasConnectionStringProvider
{
    public const string DefaultModelDatabaseTypeId = "tipobd-MO";

    private readonly AppBuilderConnectionValueProtector _valueProtector = valueProtector ?? new(null);

    public async Task<string> GetConnectionStringAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(masterConnectionString) ||
            string.Equals(masterConnectionString, "__SET_IN_ENVIRONMENT__", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("AppBuilder master connection is required to resolve model connections.");
        }

        var executionContext = executionContextAccessor.Current
            ?? throw new InvalidOperationException("AppBuilderMaster resolver requires a broker execution context.");

        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT TOP (1)
                [Server],
                [DataBase],
                [DatabaseUser],
                [DatabasePassword]
            FROM [dbo].[IAPM_Connection]
            WHERE [IdentityId] = @brokerId
              AND [DatabaseTypeId] = @databaseTypeId
              AND ([Disabled] IS NULL OR [Disabled] = 0)
            ORDER BY [Id];
            """;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@brokerId", executionContext.BrokerId);
        command.Parameters.AddWithValue("@databaseTypeId", modelDatabaseTypeId);

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException("No AppBuilder model connection was found for the configured broker.");
        }

        return BuildModelConnectionString(ReadConnectionRecord(reader), _valueProtector);
    }

    public static string BuildModelConnectionString(
        AppBuilderModelConnectionRecord record,
        AppBuilderConnectionValueProtector valueProtector)
    {
        var server = valueProtector.DecryptData(record.Server);
        var database = valueProtector.DecryptData(record.Database);
        var user = valueProtector.DecryptData(record.DatabaseUser);
        var password = valueProtector.DecryptData(record.DatabasePassword);

        if (string.IsNullOrWhiteSpace(server) ||
            string.IsNullOrWhiteSpace(database) ||
            string.IsNullOrWhiteSpace(user) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("The resolved AppBuilder model connection is incomplete.");
        }

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            UserID = user,
            Password = password,
            TrustServerCertificate = true,
            IntegratedSecurity = false,
            MultipleActiveResultSets = true,
            ConnectRetryCount = 3,
            ConnectRetryInterval = 10,
            ConnectTimeout = 30,
            ApplicationName = "iLiniumTech"
        };

        return builder.ConnectionString;
    }

    private static AppBuilderModelConnectionRecord ReadConnectionRecord(IDataRecord reader) =>
        new(
            Server: ReadString(reader, "Server"),
            Database: ReadString(reader, "DataBase"),
            DatabaseUser: ReadString(reader, "DatabaseUser"),
            DatabasePassword: ReadString(reader, "DatabasePassword"));

    private static string ReadString(IDataRecord record, string name)
    {
        var value = record[name];
        return value is DBNull ? string.Empty : Convert.ToString(value, null) ?? string.Empty;
    }
}
