using Blazor.Services.Orders.Models;

namespace Blazor.Services.Orders;

public interface IOrderService
{
    Task<IEnumerable<OrderModel>> GetAllAsync(CancellationToken ct = default);
    Task<OrderModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OrderModel?> CreateAsync(OrderRequestModel request, CancellationToken ct = default);
    Task<OrderModel?> UpdateAsync(Guid id, OrderRequestModel request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
