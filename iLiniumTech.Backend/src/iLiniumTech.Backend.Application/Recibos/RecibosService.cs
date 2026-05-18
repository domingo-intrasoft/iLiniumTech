using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Recibos;

namespace iLiniumTech.Backend.Application.Recibos;

public sealed class RecibosService(IRecibosRepository repository) : IRecibosService
{
    public Task<PagedResult<ReciboListItem>> SearchAsync(
        RecibosSearchRequest request,
        CancellationToken cancellationToken)
    {
        RecibosSearchValidator.Validate(request);
        var sort = RecibosSearchValidator.ValidateAndParseSort(request.Sort);
        return repository.SearchAsync(request, sort, cancellationToken);
    }

    public Task<RecibosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        repository.GetCatalogsAsync(cancellationToken);
}
