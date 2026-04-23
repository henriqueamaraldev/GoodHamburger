using Domain.Enums;

namespace Application.Services.Orders.Dtos;

public class OrderDto
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}
