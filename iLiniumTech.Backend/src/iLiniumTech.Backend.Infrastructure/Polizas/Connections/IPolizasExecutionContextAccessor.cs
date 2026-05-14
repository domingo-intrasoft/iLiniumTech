namespace iLiniumTech.Backend.Infrastructure.Polizas.Connections;

public interface IPolizasExecutionContextAccessor
{
    PolizasExecutionContext? Current { get; }
}
