using Application.Services.Menu.Dtos;
using Domain.Entities;
using Infrastructure.Database.Repositories;

namespace Application.Services.Menu;

public class MenuService : IMenuService
{
    private readonly IRepository<MenuItem> _menuItemRepository;

    public MenuService(IRepository<MenuItem> menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<IEnumerable<MenuItemDto>> GetMenuAsync(CancellationToken cancellationToken = default)
    {
        var items = await _menuItemRepository.GetAsync(m => true, cancellationToken: cancellationToken);
        return items.Select(m => new MenuItemDto
        {
            Id = m.Id,
            Name = m.Name,
            Type = m.Type,
            SideType = m.SideType,
            Price = m.Price
        });
    }
}
