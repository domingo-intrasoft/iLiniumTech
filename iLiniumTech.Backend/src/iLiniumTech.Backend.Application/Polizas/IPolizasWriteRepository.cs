using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public interface IPolizasWriteRepository
{
    Task<PolizaCreateResult> CreateAsync(PolizaCreateRequest request, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(int id, PolizaUpdateRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
