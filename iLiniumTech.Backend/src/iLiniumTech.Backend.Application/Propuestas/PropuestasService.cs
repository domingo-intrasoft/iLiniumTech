using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Propuestas;

namespace iLiniumTech.Backend.Application.Propuestas;

public sealed class PropuestasService(IPropuestasRepository repository) : IPropuestasService
{
    public Task<PagedResult<PropuestaListItem>> SearchAsync(
        PropuestasSearchRequest request,
        CancellationToken cancellationToken)
    {
        PropuestasSearchValidator.Validate(request);
        var sort = PropuestasSearchValidator.ValidateAndParseSort(request.Sort);
        return repository.SearchAsync(request, sort, cancellationToken);
    }

    public Task<PropuestasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        repository.GetCatalogsAsync(cancellationToken);
}
