using iLiniumTech.Backend.Domain.Agenda;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Agenda;

public sealed class AgendaService(IAgendaRepository repository) : IAgendaService
{
    public Task<PagedResult<AgendaEventListItem>> SearchAsync(
        AgendaSearchRequest request,
        CancellationToken cancellationToken)
    {
        AgendaSearchValidator.Validate(request);
        var sort = AgendaSearchValidator.ValidateAndParseSort(request.Sort);
        return repository.SearchAsync(request, sort, cancellationToken);
    }

    public Task<AgendaCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        repository.GetCatalogsAsync(cancellationToken);
}
