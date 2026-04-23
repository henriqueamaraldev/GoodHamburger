using Domain.Entities;

namespace Infrastructure.Database.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
}
