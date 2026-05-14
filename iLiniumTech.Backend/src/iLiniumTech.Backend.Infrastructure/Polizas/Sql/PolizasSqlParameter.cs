using System.Data;

namespace iLiniumTech.Backend.Infrastructure.Polizas.Sql;

public sealed record PolizasSqlParameter(
    string Name,
    object Value,
    SqlDbType? DbType = null,
    int? Size = null);
