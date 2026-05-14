namespace iLiniumTech.Backend.Infrastructure.Polizas.Sql;

public sealed record PolizasSqlQuery(
    string CommandText,
    IReadOnlyList<PolizasSqlParameter> Parameters);
