using Blazor.Services.Base;
using Blazor.Services.Orders.Models;

namespace Blazor.Services.Orders;

public class OrderService : ApiService, IOrderService
{
    public OrderService(IHttpClientFactory factory) : base(factory) { }

    public async Task<IEnumerable<OrderModel>> GetAllAsync(CancellationToken ct = default)
        => await GetAsync<IEnumerable<OrderModel>>("orders", ct) ?? [];

    public Task<OrderModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetAsync<OrderModel>($"orders/{id}", ct);

    public Task<OrderModel?> CreateAsync(OrderRequestModel request, CancellationToken ct = default)
        => PostAsync<OrderModel>("orders", request, ct);

    public Task<OrderModel?> UpdateAsync(Guid id, OrderRequestModel request, CancellationToken ct = default)
        => PutAsync<OrderModel>($"orders/{id}", request, ct);

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => DeleteAsync($"orders/{id}", ct);
}
