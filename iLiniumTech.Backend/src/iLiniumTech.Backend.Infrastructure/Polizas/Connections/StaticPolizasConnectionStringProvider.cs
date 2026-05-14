namespace iLiniumTech.Backend.Infrastructure.Polizas.Connections;

public sealed class StaticPolizasConnectionStringProvider(string connectionString) : IPolizasConnectionStringProvider
{
    public Task<string> GetConnectionStringAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(connectionString) ||
            string.Equals(connectionString, "__SET_IN_ENVIRONMENT__", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Polizas SQL repository requires a configured connection string.");
        }

        return Task.FromResult(connectionString);
    }
}
