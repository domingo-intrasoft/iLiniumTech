using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Propuestas;

namespace iLiniumTech.Backend.Application.Propuestas;

public interface IPropuestasService
{
    Task<PagedResult<PropuestaListItem>> SearchAsync(
        PropuestasSearchRequest request,
        CancellationToken cancellationToken);

    Task<PropuestasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
