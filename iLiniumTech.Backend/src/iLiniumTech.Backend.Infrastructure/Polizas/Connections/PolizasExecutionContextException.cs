namespace iLiniumTech.Backend.Infrastructure.Polizas.Connections;

public sealed class PolizasExecutionContextException(string message) : InvalidOperationException(message);
