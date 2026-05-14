namespace iLiniumTech.Backend.Infrastructure.Polizas.Connections;

public sealed record PolizasExecutionContext(
    int BrokerId,
    int? UserId = null,
    int? ProfileId = null,
    string? ProfileTypeId = null,
    bool? IsAdmin = null)
{
    public int EntityMainId => BrokerId;
}
