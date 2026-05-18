using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Suplementos;

namespace iLiniumTech.Backend.Application.Suplementos;

public sealed class SuplementosService(ISuplementosRepository repository) : ISuplementosService
{
    public Task<PagedResult<SuplementoListItem>> SearchAsync(
        SuplementosSearchRequest request,
        CancellationToken cancellationToken)
    {
        SuplementosSearchValidator.Validate(request);
        var sort = SuplementosSearchValidator.ValidateAndParseSort(request.Sort);
        return repository.SearchAsync(request, sort, cancellationToken);
    }

    public Task<SuplementosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        repository.GetCatalogsAsync(cancellationToken);
}
