namespace Blazor.Services.Orders.Models;

public record OrderItemModel(
    Guid MenuItemId,
    string Name,
    decimal Price
);
