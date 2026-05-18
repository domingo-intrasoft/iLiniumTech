using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Recibos;

namespace iLiniumTech.Backend.Application.Recibos;

public interface IRecibosService
{
    Task<PagedResult<ReciboListItem>> SearchAsync(
        RecibosSearchRequest request,
        CancellationToken cancellationToken);

    Task<RecibosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
