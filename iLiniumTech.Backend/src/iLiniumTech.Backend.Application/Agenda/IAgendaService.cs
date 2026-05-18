using iLiniumTech.Backend.Domain.Agenda;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Agenda;

public interface IAgendaService
{
    Task<PagedResult<AgendaEventListItem>> SearchAsync(
        AgendaSearchRequest request,
        CancellationToken cancellationToken);

    Task<AgendaCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
