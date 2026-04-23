using Application.Services.Orders.Dtos;

namespace Application.Services.Orders;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(OrderRequestDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateAsync(Guid id, OrderRequestDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
