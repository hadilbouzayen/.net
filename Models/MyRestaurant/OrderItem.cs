using System;
using System.Collections.Generic;

namespace WebAppTest3.Models.MyRestaurant;

public partial class OrderItem
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }

    public int OrderId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
    public bool IsSelected { get; set; }

    public virtual MenuItem? MenuItem { get; set; } = null!;

    public virtual Order? Order { get; set; } = null!;
}
