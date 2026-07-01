using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    Task DeleteAsync(Sale sale, CancellationToken cancellationToken = default);

    Task<(List<Sale> Items, int TotalCount)> GetPagedAsync(SaleFilter filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
