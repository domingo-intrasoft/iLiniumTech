using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public sealed class PolizasService(IPolizasRepository repository) : IPolizasService
{
    public PolizasComponentMetadata GetMetadata() => PolizasMetadataProvider.Create();

    public Task<PagedResult<PolizaListItem>> SearchAsync(PolizasSearchRequest request, CancellationToken cancellationToken)
    {
        PolizasSearchValidator.Validate(request);
        var sort = PolizasSearchValidator.ValidateAndParseSort(request.Sort);
        return repository.SearchAsync(request, sort, cancellationToken);
    }

    public Task<PolizaDetail?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new PolizasValidationException("Poliza id is required.");
        }

        return repository.GetByIdAsync(id, cancellationToken);
    }
}
