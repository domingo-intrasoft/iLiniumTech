namespace iLiniumTech.Backend.Infrastructure.Polizas.Connections;

public interface IPolizasConnectionStringProvider
{
    Task<string> GetConnectionStringAsync(CancellationToken cancellationToken);
}
