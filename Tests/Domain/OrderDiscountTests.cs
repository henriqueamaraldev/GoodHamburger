using Domain.Entities;
using Domain.Enums;

namespace Tests.Domain;

public class OrderDiscountTests
{
    private static OrderItem Item(MenuItemType type, SideType? sideType, decimal price) =>
        new() { MenuItem = new MenuItem { Type = type, SideType = sideType, Price = price } };

    [Fact]
    public void Calculate_SandwichOnly_NoDiscount()
    {
        var order = new Order();
        order.Items.Add(Item(MenuItemType.Sandwich, null, 5.00m));

        order.Calculate();

        Assert.Equal(5.00m, order.Subtotal);
        Assert.Equal(0m, order.Discount);
        Assert.Equal(5.00m, order.Total);
    }

    [Fact]
    public void Calculate_SandwichAndFries_Applies10PercentDiscount()
    {
        var order = new Order();
        order.Items.Add(Item(MenuItemType.Sandwich, null, 5.00m));
        order.Items.Add(Item(MenuItemType.Side, SideType.Fries, 2.00m));

        order.Calculate();

        Assert.Equal(7.00m, order.Subtotal);
        Assert.Equal(0.70m, order.Discount);
        Assert.Equal(6.30m, order.Total);
    }

    [Fact]
    public void Calculate_SandwichAndSoda_Applies15PercentDiscount()
    {
        var order = new Order();
        order.Items.Add(Item(MenuItemType.Sandwich, null, 4.50m));
        order.Items.Add(Item(MenuItemType.Side, SideType.Soda, 2.50m));

        order.Calculate();

        Assert.Equal(7.00m, order.Subtotal);
        Assert.Equal(1.05m, order.Discount);
        Assert.Equal(5.95m, order.Total);
    }

    [Fact]
    public void Calculate_SandwichFriesSoda_Applies20PercentDiscount()
    {
        var order = new Order();
        order.Items.Add(Item(MenuItemType.Sandwich, null, 7.00m));
        order.Items.Add(Item(MenuItemType.Side, SideType.Fries, 2.00m));
        order.Items.Add(Item(MenuItemType.Side, SideType.Soda, 2.50m));

        order.Calculate();

        Assert.Equal(11.50m, order.Subtotal);
        Assert.Equal(2.30m, order.Discount);
        Assert.Equal(9.20m, order.Total);
    }
}
