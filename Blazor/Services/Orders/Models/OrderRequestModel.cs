namespace Blazor.Services.Orders.Models;

public class OrderRequestModel
{
    public List<Guid> MenuItemIds { get; set; } = new();
}
