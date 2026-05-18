using iLiniumTech.Backend.Domain.Agenda;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Agenda;

public interface IAgendaRepository
{
    Task<PagedResult<AgendaEventListItem>> SearchAsync(
        AgendaSearchRequest request,
        AgendaSort sort,
        CancellationToken cancellationToken);

    Task<AgendaCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
