using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public sealed class PolizasService(IPolizasRepository repository) : IPolizasService
{
    public Task<PagedResult<PolizaListItem>> SearchAsync(PolizasSearchRequest request, CancellationToken cancellationToken)
    {
        PolizasSearchValidator.Validate(request);
        var sort = PolizasSearchValidator.ValidateAndParseSort(request.Sort);
        return repository.SearchAsync(request, sort, cancellationToken);
    }

    public Task<PolizaDetail?> GetByIdAsync(string id, CancellationToken cancellationToken, string? ramo = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new PolizasValidationException("Poliza id is required.");
        }

        return repository.GetByIdAsync(id.Trim(), cancellationToken, ramo?.Trim());
    }

    public Task<PolizasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        repository.GetCatalogsAsync(cancellationToken);
}
