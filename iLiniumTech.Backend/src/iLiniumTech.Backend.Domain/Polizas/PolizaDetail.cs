namespace iLiniumTech.Backend.Domain.Polizas;

public sealed record PolizaDetail(
    string Id,
    string Numero,
    string Aplicacion,
    string Estado,
    string Ramo,
    string Compania,
    PolizaCliente Cliente,
    PolizaProducto Producto,
    PolizaVigencia Vigencia,
    PolizaFinanciero Financiero,
    IReadOnlyList<PolizaRiesgo> Riesgos,
    IReadOnlyList<PolizaRecibo> Recibos);

public sealed record PolizaCliente(
    string Id,
    string Nombre,
    string? Documento);

public sealed record PolizaProducto(
    string Nombre,
    string Modalidad);

public sealed record PolizaVigencia(
    DateOnly FechaInicio,
    DateOnly FechaVencimiento,
    string Renovacion);

public sealed record PolizaFinanciero(
    decimal PrimaAnual,
    string Moneda);

public sealed record PolizaRiesgo(
    string Id,
    string Descripcion,
    string? TipoRiesgo = null,
    DateOnly? FechaAlta = null,
    DateOnly? FechaBaja = null);

public sealed record PolizaRecibo(
    string Id,
    string Numero,
    string Estado,
    string EstadoCia,
    string EstadoColab,
    string Tipo,
    string Gestor,
    decimal PrimaTotal,
    DateOnly FechaEfecto,
    DateOnly FechaVencimiento);
