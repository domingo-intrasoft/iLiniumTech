using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public interface IPolizasService
{
    Task<PagedResult<PolizaListItem>> SearchAsync(PolizasSearchRequest request, CancellationToken cancellationToken);

    Task<PolizaDetail?> GetByIdAsync(string id, CancellationToken cancellationToken, string? ramo = null);

    Task<PolizasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);

    Task<PolizaCreateResult> CreateAsync(PolizaCreateRequest request, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(string id, PolizaUpdateRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}
