using Application.Services.Menu.Dtos;

namespace Application.Services.Menu;

public interface IMenuService
{
    Task<IEnumerable<MenuItemDto>> GetMenuAsync(CancellationToken cancellationToken = default);
}
