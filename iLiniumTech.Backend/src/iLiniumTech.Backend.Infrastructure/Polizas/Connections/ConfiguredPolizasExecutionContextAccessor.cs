using Microsoft.Extensions.Configuration;

namespace iLiniumTech.Backend.Infrastructure.Polizas.Connections;

public sealed class ConfiguredPolizasExecutionContextAccessor(IConfiguration configuration) : IPolizasExecutionContextAccessor
{
    public PolizasExecutionContext? Current
    {
        get
        {
            var brokerId = ReadInt("Polizas:BrokerId", "ILINIUMTECH:BROKER_ID");
            return brokerId is null or 0
                ? null
                : new PolizasExecutionContext(
                    BrokerId: brokerId.Value,
                    UserId: ReadInt("Polizas:UserId", "ILINIUMTECH:USER_ID"),
                    ProfileId: ReadInt("Polizas:ProfileId", "ILINIUMTECH:PROFILE_ID"),
                    ProfileTypeId: ReadString("Polizas:ProfileTypeId", "ILINIUMTECH:PROFILE_TYPE_ID"),
                    IsAdmin: ReadBool("Polizas:IsAdmin", "ILINIUMTECH:IS_ADMIN"));
        }
    }

    private int? ReadInt(string configurationKey, string environmentKey)
    {
        var value = ReadString(configurationKey, environmentKey);
        return int.TryParse(value, out var parsed) ? parsed : null;
    }

    private bool? ReadBool(string configurationKey, string environmentKey)
    {
        var value = ReadString(configurationKey, environmentKey);
        return bool.TryParse(value, out var parsed) ? parsed : null;
    }

    private string? ReadString(string configurationKey, string environmentKey) =>
        configuration[configurationKey] ?? configuration[environmentKey];
}
