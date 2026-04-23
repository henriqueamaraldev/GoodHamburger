using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.MenuItems.AnyAsync())
            return;

        var menuItems = new List<MenuItem>
        {
            new() { Name = "X Burger",     Type = MenuItemType.Sandwich, SideType = null,           Price = 5.00m },
            new() { Name = "X Egg",        Type = MenuItemType.Sandwich, SideType = null,           Price = 4.50m },
            new() { Name = "X Bacon",      Type = MenuItemType.Sandwich, SideType = null,           Price = 7.00m },
            new() { Name = "Batata frita", Type = MenuItemType.Side,     SideType = SideType.Fries, Price = 2.00m },
            new() { Name = "Refrigerante", Type = MenuItemType.Side,     SideType = SideType.Soda,  Price = 2.50m },
        };

        await context.MenuItems.AddRangeAsync(menuItems);
        await context.SaveChangesAsync();
    }
}
