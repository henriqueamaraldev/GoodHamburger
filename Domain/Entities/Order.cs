using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class Order : Entity
{
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
