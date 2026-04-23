namespace Blazor.Services.Menu.Models;

public record MenuItemModel(
    Guid Id,
    string Name,
    string Type,
    string? SideType,
    decimal Price
)
{
    public bool IsSandwich => Type == "Sandwich";
    public bool IsSide => Type == "Side";
    public bool IsFries => SideType == "Fries";
    public bool IsSoda  => SideType == "Soda";
}
