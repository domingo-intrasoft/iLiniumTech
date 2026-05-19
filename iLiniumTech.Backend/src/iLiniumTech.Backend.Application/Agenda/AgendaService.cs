using iLiniumTech.Backend.Domain.Agenda;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Agenda;

public sealed class AgendaService(IAgendaRepository repository, IAgendaWriteRepository writeRepository) : IAgendaService
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

    public Task<AgendaCreateResult> CreateAsync(AgendaCreateRequest request, CancellationToken cancellationToken)
    {
        AgendaWriteValidator.ValidateCreate(request);
        return writeRepository.CreateAsync(request, cancellationToken);
    }

    public Task<bool> UpdateAsync(string id, AgendaUpdateRequest request, CancellationToken cancellationToken)
    {
        _ = AgendaWriteValidator.ValidateAndParseId(id);
        AgendaWriteValidator.ValidateUpdate(request);
        return writeRepository.UpdateAsync(id, request, cancellationToken);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        _ = AgendaWriteValidator.ValidateAndParseId(id);
        return writeRepository.DeleteAsync(id, cancellationToken);
    }
}
