using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Recibos;

namespace iLiniumTech.Backend.Application.Recibos;

public interface IRecibosRepository
{
    Task<PagedResult<ReciboListItem>> SearchAsync(
        RecibosSearchRequest request,
        RecibosSort sort,
        CancellationToken cancellationToken);

    Task<RecibosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
