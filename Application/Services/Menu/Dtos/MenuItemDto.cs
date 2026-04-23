using Domain.Enums;

namespace Application.Services.Menu.Dtos;

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public MenuItemType Type { get; set; }
    public SideType? SideType { get; set; }
    public decimal Price { get; set; }
}
