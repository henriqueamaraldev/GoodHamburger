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

    public void Calculate()
    {
        var menuItems = Items.Select(i => i.MenuItem!).ToList();
        Subtotal = menuItems.Sum(i => i.Price);

        bool hasSandwich = menuItems.Any(i => i.Type == MenuItemType.Sandwich);
        bool hasFries = menuItems.Any(i => i.SideType == Enums.SideType.Fries);
        bool hasSoda = menuItems.Any(i => i.SideType == Enums.SideType.Soda);

        decimal discountRate = (hasSandwich, hasFries, hasSoda) switch
        {
            (true, true, true) => 0.20m,
            (true, false, true) => 0.15m,
            (true, true, false) => 0.10m,
            _ => 0m
        };

        Discount = Math.Round(Subtotal * discountRate, 2);
        Total = Subtotal - Discount;
    }
}
