using Application.Exceptions;
using Application.Services.Orders;
using Application.Services.Orders.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database.Repositories;
using NSubstitute;
using System.Linq.Expressions;

namespace Tests.Application;

public class OrderServiceTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<MenuItem> _menuItemRepository;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _menuItemRepository = Substitute.For<IRepository<MenuItem>>();
        _sut = new OrderService(_orderRepository, _menuItemRepository);
    }

    private static MenuItem Sandwich(string name = "X Burger", decimal price = 5.00m) =>
        new() { Id = Guid.NewGuid(), Name = name, Type = MenuItemType.Sandwich, Price = price };

    private static MenuItem Fries() =>
        new() { Id = Guid.NewGuid(), Name = "Batata frita", Type = MenuItemType.Side, SideType = SideType.Fries, Price = 2.00m };

    private static MenuItem Soda() =>
        new() { Id = Guid.NewGuid(), Name = "Refrigerante", Type = MenuItemType.Side, SideType = SideType.Soda, Price = 2.50m };

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_EmptyItems_ThrowsBadRequest()
    {
        var ex = await Assert.ThrowsAsync<ServiceException>(() =>
            _sut.CreateAsync(new OrderRequestDto { MenuItemIds = new() }));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_DuplicateMenuItemId_ThrowsBadRequest()
    {
        var id = Guid.NewGuid();
        var ex = await Assert.ThrowsAsync<ServiceException>(() =>
            _sut.CreateAsync(new OrderRequestDto { MenuItemIds = new() { id, id } }));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Contains("duplicate", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_MenuItemNotFound_ThrowsBadRequest()
    {
        var id = Guid.NewGuid();
        _menuItemRepository.GetAsync(Arg.Any<Expression<Func<MenuItem, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(new List<MenuItem>());

        var ex = await Assert.ThrowsAsync<ServiceException>(() =>
            _sut.CreateAsync(new OrderRequestDto { MenuItemIds = new() { id } }));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_TwoSandwiches_ThrowsBadRequest()
    {
        var s1 = Sandwich("X Burger");
        var s2 = Sandwich("X Bacon", 7.00m);
        _menuItemRepository.GetAsync(Arg.Any<Expression<Func<MenuItem, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(new List<MenuItem> { s1, s2 });

        var ex = await Assert.ThrowsAsync<ServiceException>(() =>
            _sut.CreateAsync(new OrderRequestDto { MenuItemIds = new() { s1.Id, s2.Id } }));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Contains("sandwich", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_SandwichAndFries_ReturnsOrderWith10PercentDiscount()
    {
        var sandwich = Sandwich();
        var fries = Fries();
        _menuItemRepository.GetAsync(Arg.Any<Expression<Func<MenuItem, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(new List<MenuItem> { sandwich, fries });

        var result = await _sut.CreateAsync(new OrderRequestDto { MenuItemIds = new() { sandwich.Id, fries.Id } });

        Assert.Equal(7.00m, result.Subtotal);
        Assert.Equal(0.70m, result.Discount);
        Assert.Equal(6.30m, result.Total);
        Assert.Equal(OrderStatus.Pending, result.Status);
    }

    [Fact]
    public async Task CreateAsync_SandwichFriesSoda_ReturnsOrderWith20PercentDiscount()
    {
        var sandwich = Sandwich("X Bacon", 7.00m);
        var fries = Fries();
        var soda = Soda();
        _menuItemRepository.GetAsync(Arg.Any<Expression<Func<MenuItem, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(new List<MenuItem> { sandwich, fries, soda });

        var result = await _sut.CreateAsync(new OrderRequestDto { MenuItemIds = new() { sandwich.Id, fries.Id, soda.Id } });

        Assert.Equal(11.50m, result.Subtotal);
        Assert.Equal(2.30m, result.Discount);
        Assert.Equal(9.20m, result.Total);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_OrderNotFound_ThrowsNotFound()
    {
        _orderRepository.GetWithItemsAsync(Arg.Any<Guid>()).Returns((Order?)null);

        var ex = await Assert.ThrowsAsync<ServiceException>(() =>
            _sut.GetByIdAsync(Guid.NewGuid()));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_OrderNotFound_ThrowsNotFound()
    {
        _orderRepository.GetWithItemsAsync(Arg.Any<Guid>()).Returns((Order?)null);

        var ex = await Assert.ThrowsAsync<ServiceException>(() =>
            _sut.UpdateAsync(Guid.NewGuid(), new OrderRequestDto { MenuItemIds = new() { Guid.NewGuid() } }));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_OrderNotFound_ThrowsNotFound()
    {
        _orderRepository.FirstAsync(Arg.Any<Expression<Func<Order, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        var ex = await Assert.ThrowsAsync<ServiceException>(() =>
            _sut.DeleteAsync(Guid.NewGuid()));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
    }
}
