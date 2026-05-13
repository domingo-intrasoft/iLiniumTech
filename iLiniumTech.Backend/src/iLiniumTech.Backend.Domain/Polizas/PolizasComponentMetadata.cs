namespace iLiniumTech.Backend.Domain.Polizas;

public sealed record PolizasComponentMetadata(
    string Resource,
    int Version,
    AppBuilderComponentReference AppBuilder,
    IReadOnlyList<PolizaFieldMetadata> Fields);

public sealed record AppBuilderComponentReference(
    int ApplicationId,
    int ApplicationVersion,
    int MenuId,
    int RootComponentId,
    int CrudComponentId,
    int ComponentDataSourceId,
    int DataSourceId,
    string DataSourceName,
    string ModelObject);
