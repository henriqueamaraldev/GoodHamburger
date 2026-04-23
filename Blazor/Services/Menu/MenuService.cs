using Blazor.Services.Base;
using Blazor.Services.Menu.Models;

namespace Blazor.Services.Menu;

public class MenuService : ApiService, IMenuService
{
    public MenuService(IHttpClientFactory factory) : base(factory) { }

    public async Task<IEnumerable<MenuItemModel>> GetMenuAsync(CancellationToken ct = default)
        => await GetAsync<IEnumerable<MenuItemModel>>("menu", ct) ?? [];
}
