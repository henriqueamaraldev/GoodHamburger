using Blazor.Services.Menu.Models;

namespace Blazor.Services.Menu;

public interface IMenuService
{
    Task<IEnumerable<MenuItemModel>> GetMenuAsync(CancellationToken ct = default);
}
