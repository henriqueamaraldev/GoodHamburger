using Domain.Entities.Base;

namespace Domain.Entities;

public class OrderItem : Entity
{
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }
}
