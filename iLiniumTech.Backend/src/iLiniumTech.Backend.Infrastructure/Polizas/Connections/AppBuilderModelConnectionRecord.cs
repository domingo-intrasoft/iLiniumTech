namespace iLiniumTech.Backend.Infrastructure.Polizas.Connections;

public sealed record AppBuilderModelConnectionRecord(
    string Server,
    string Database,
    string DatabaseUser,
    string DatabasePassword);
