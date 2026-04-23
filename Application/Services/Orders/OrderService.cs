using Application.Exceptions;
using Application.Services.Orders.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Services.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<MenuItem> _menuItemRepository;

    public OrderService(IOrderRepository orderRepository, IRepository<MenuItem> menuItemRepository)
    {
        _orderRepository = orderRepository;
        _menuItemRepository = menuItemRepository;
    }

    public async Task<OrderDto> CreateAsync(OrderRequestDto dto, CancellationToken cancellationToken = default)
    {
        var menuItems = await ValidateAndResolveItemsAsync(dto.MenuItemIds, cancellationToken);

        var order = new Order { Status = OrderStatus.Pending };
        foreach (var menuItem in menuItems)
            order.Items.Add(new OrderItem { MenuItemId = menuItem.Id, MenuItem = menuItem });

        order.Calculate();

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.CommitAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAsync(
            o => true,
            q => q.Include(o => o.Items).ThenInclude(i => i.MenuItem),
            cancellationToken);

        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithItemsAsync(id, cancellationToken);
        if (order == null)
            throw new ServiceException("Order not found.", HttpStatusCode.NotFound);

        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateAsync(Guid id, OrderRequestDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithItemsAsync(id, cancellationToken);
        if (order == null)
            throw new ServiceException("Order not found.", HttpStatusCode.NotFound);

        var menuItems = await ValidateAndResolveItemsAsync(dto.MenuItemIds, cancellationToken);

        order.Items.Clear();
        foreach (var menuItem in menuItems)
            order.Items.Add(new OrderItem { MenuItemId = menuItem.Id, MenuItem = menuItem });

        order.Calculate();
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _orderRepository.CommitAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.FirstAsync(o => o.Id == id, cancellationToken: cancellationToken);
        if (order == null)
            throw new ServiceException("Order not found.", HttpStatusCode.NotFound);

        _orderRepository.Delete(order);
        await _orderRepository.CommitAsync(cancellationToken);
    }

    private async Task<List<MenuItem>> ValidateAndResolveItemsAsync(List<Guid> menuItemIds, CancellationToken cancellationToken)
    {
        if (menuItemIds.Count == 0)
            throw new ServiceException("Order must contain at least one item.", HttpStatusCode.BadRequest);

        if (menuItemIds.Count != menuItemIds.Distinct().Count())
            throw new ServiceException("Order contains duplicate items.", HttpStatusCode.BadRequest);

        var menuItems = (await _menuItemRepository.GetAsync(m => menuItemIds.Contains(m.Id), cancellationToken: cancellationToken)).ToList();

        if (menuItems.Count != menuItemIds.Count)
            throw new ServiceException("One or more menu items not found.", HttpStatusCode.BadRequest);

        if (menuItems.Count(m => m.Type == MenuItemType.Sandwich) > 1)
            throw new ServiceException("An order can contain only one sandwich.", HttpStatusCode.BadRequest);

        if (menuItems.Count(m => m.SideType == SideType.Fries) > 1)
            throw new ServiceException("An order can contain only one serving of fries.", HttpStatusCode.BadRequest);

        if (menuItems.Count(m => m.SideType == SideType.Soda) > 1)
            throw new ServiceException("An order can contain only one soda.", HttpStatusCode.BadRequest);

        return menuItems;
    }

    private static OrderDto MapToDto(Order order) => new()
    {
        Id = order.Id,
        Status = order.Status,
        Subtotal = order.Subtotal,
        Discount = order.Discount,
        Total = order.Total,
        Items = order.Items.Select(i => new OrderItemDto
        {
            MenuItemId = i.MenuItemId,
            Name = i.MenuItem?.Name ?? string.Empty,
            Price = i.MenuItem?.Price ?? 0
        }).ToList()
    };
}
