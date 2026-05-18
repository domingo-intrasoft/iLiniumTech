using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Siniestros;

namespace iLiniumTech.Backend.Application.Siniestros;

public sealed class SiniestrosService(ISiniestrosRepository repository) : ISiniestrosService
{
    public Task<PagedResult<SiniestroListItem>> SearchAsync(
        SiniestrosSearchRequest request,
        CancellationToken cancellationToken)
    {
        SiniestrosSearchValidator.Validate(request);
        var sort = SiniestrosSearchValidator.ValidateAndParseSort(request.Sort);
        return repository.SearchAsync(request, sort, cancellationToken);
    }

    public Task<SiniestrosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        repository.GetCatalogsAsync(cancellationToken);
}
