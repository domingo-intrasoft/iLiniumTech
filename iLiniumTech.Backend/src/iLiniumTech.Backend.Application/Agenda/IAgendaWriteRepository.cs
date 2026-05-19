using iLiniumTech.Backend.Domain.Agenda;

namespace iLiniumTech.Backend.Application.Agenda;

public interface IAgendaWriteRepository
{
    Task<AgendaCreateResult> CreateAsync(AgendaCreateRequest request, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(string id, AgendaUpdateRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}
