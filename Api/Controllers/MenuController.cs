using Application.Services.Menu;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMenu(CancellationToken cancellationToken)
    {
        var result = await _menuService.GetMenuAsync(cancellationToken);
        return Ok(result);
    }
}
