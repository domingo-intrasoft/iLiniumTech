using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Suplementos;

namespace iLiniumTech.Backend.Application.Suplementos;

public interface ISuplementosService
{
    Task<PagedResult<SuplementoListItem>> SearchAsync(
        SuplementosSearchRequest request,
        CancellationToken cancellationToken);

    Task<SuplementosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
