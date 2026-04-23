namespace Application.Services.Orders.Dtos;

public class OrderRequestDto
{
    public List<Guid> MenuItemIds { get; set; } = new();
}
