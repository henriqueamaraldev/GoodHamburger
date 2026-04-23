using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class MenuItem : Entity
{
    public string Name { get; set; } = string.Empty;
    public MenuItemType Type { get; set; }
    public SideType? SideType { get; set; }
    public decimal Price { get; set; }
}
