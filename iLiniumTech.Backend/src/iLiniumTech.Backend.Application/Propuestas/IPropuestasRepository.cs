using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Propuestas;

namespace iLiniumTech.Backend.Application.Propuestas;

public interface IPropuestasRepository
{
    Task<PagedResult<PropuestaListItem>> SearchAsync(
        PropuestasSearchRequest request,
        PropuestasSort sort,
        CancellationToken cancellationToken);

    Task<PropuestasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
