using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public sealed class PolizasService(IPolizasRepository repository, IPolizasWriteRepository writeRepository) : IPolizasService
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

    public Task<PolizaCreateResult> CreateAsync(PolizaCreateRequest request, CancellationToken cancellationToken)
    {
        PolizasWriteValidator.ValidateCreate(request);
        return writeRepository.CreateAsync(request, cancellationToken);
    }

    public Task<bool> UpdateAsync(string id, PolizaUpdateRequest request, CancellationToken cancellationToken)
    {
        var parsedId = PolizasWriteValidator.ValidateAndParseId(id);
        PolizasWriteValidator.ValidateUpdate(request);
        return writeRepository.UpdateAsync(parsedId, request, cancellationToken);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var parsedId = PolizasWriteValidator.ValidateAndParseId(id);
        return writeRepository.DeleteAsync(parsedId, cancellationToken);
    }
}
