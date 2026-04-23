namespace Blazor.Services.Orders.Models;

public record OrderModel(
    Guid Id,
    string Status,
    decimal Subtotal,
    decimal Discount,
    decimal Total,
    List<OrderItemModel> Items
);
