namespace iLiniumTech.Backend.Domain.Polizas;

public sealed record PolizasCatalogs(
    IReadOnlyList<PolizaCatalogOption> TipoPoliza,
    IReadOnlyList<PolizaCatalogOption> Compania,
    IReadOnlyList<PolizaCatalogOption> Ramo,
    IReadOnlyList<PolizaCatalogOption> Oficina,
    IReadOnlyList<PolizaCatalogOption> Division,
    IReadOnlyList<PolizaCatalogOption> Colaborador1,
    IReadOnlyList<PolizaCatalogOption> Administrativo,
    IReadOnlyList<PolizaCatalogOption> Comercial,
    IReadOnlyList<PolizaCatalogOption> Siniestros,
    IReadOnlyList<PolizaCatalogOption> Gestor,
    IReadOnlyList<PolizaCatalogOption> CanalCobro,
    IReadOnlyList<PolizaCatalogOption> FraccionPago,
    IReadOnlyList<PolizaCatalogOption> Ccaa,
    IReadOnlyList<PolizaCatalogOption> Sexo,
    IReadOnlyList<PolizaCatalogOption> EstadoCivil,
    IReadOnlyList<PolizaCatalogOption> RegimenLaboral,
    IReadOnlyList<PolizaCatalogOption> Profesion);

public sealed record PolizaCatalogOption(
    string Value,
    string Label);
