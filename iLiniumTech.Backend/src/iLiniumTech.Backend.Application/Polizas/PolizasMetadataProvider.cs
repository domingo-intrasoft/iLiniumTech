using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public static class PolizasMetadataProvider
{
    public static PolizasComponentMetadata Create() =>
        new(
            Resource: "polizas",
            Version: 1,
            AppBuilder: new AppBuilderComponentReference(
                ApplicationId: AppBuilderPolizasMetadata.ApplicationId,
                ApplicationVersion: AppBuilderPolizasMetadata.ApplicationVersion,
                MenuId: AppBuilderPolizasMetadata.MenuId,
                RootComponentId: AppBuilderPolizasMetadata.RootComponentId,
                CrudComponentId: AppBuilderPolizasMetadata.CrudComponentId,
                ComponentDataSourceId: AppBuilderPolizasMetadata.ComponentDataSourceId,
                DataSourceId: AppBuilderPolizasMetadata.DataSourceId,
                DataSourceName: AppBuilderPolizasMetadata.DataSourceName,
                ModelObject: AppBuilderPolizasMetadata.ModelObject),
            Fields:
            [
                new("numero", "Poliza", "Poliza", "string", true, true, true, 3),
                new("aplicacion", "Aplicacion", "Aplicacion", "string", true, true, true, 4),
                new("tipoPoliza", "IdTipoPoliza", "Tipo poliza", "string", true, true, true, 5),
                new("documento", "NumDocumento", "Documento", "string", true, false, true, 6),
                new("ramo", "IdRamo", "Ramo", "string", true, true, true, 9),
                new("riesgo", "Riesgo", "Riesgo", "string", false, false, true, 10),
                new("fechaEfecto", "F_Efecto", "Fecha efecto", "date", false, true, true, 20),
                new("fechaVencimiento", "F_Vencimiento", "Fecha vencimiento", "date", true, true, true, 21),
                new("compania", "Cia", "Compania", "string", true, true, true, 1),
                new("primaAnual", "PAnualCartera", "Prima anual", "money", true, true, true, 39),
                new("cliente", "NombreCompleto", "Cliente", "string", true, true, true, 40),
                new("estado", "IdSituacion", "Situacion", "string", true, true, true, 8)
            ]);
}
