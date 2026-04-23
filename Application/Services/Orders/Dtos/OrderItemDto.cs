namespace Application.Services.Orders.Dtos;

public class OrderItemDto
{
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
