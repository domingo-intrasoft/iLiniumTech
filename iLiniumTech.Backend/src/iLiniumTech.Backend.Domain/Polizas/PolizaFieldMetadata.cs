namespace iLiniumTech.Backend.Domain.Polizas;

public sealed record PolizaFieldMetadata(
    string Name,
    string SourceField,
    string Label,
    string Type,
    bool Filterable,
    bool Sortable,
    bool Visible,
    int Order);
