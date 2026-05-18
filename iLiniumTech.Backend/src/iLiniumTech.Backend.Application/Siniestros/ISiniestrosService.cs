using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Siniestros;

namespace iLiniumTech.Backend.Application.Siniestros;

public interface ISiniestrosService
{
    Task<PagedResult<SiniestroListItem>> SearchAsync(
        SiniestrosSearchRequest request,
        CancellationToken cancellationToken);

    Task<SiniestrosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
