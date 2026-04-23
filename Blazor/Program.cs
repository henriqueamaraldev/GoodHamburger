using Blazor.Services.Menu;
using Blazor.Services.Orders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("Api", client =>
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!));

builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Blazor.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
