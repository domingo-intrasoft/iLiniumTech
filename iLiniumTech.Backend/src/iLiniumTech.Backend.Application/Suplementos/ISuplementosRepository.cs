using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Suplementos;

namespace iLiniumTech.Backend.Application.Suplementos;

public interface ISuplementosRepository
{
    Task<PagedResult<SuplementoListItem>> SearchAsync(
        SuplementosSearchRequest request,
        SuplementosSort sort,
        CancellationToken cancellationToken);

    Task<SuplementosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
