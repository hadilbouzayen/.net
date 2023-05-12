using System;
using System.Collections.Generic;

namespace WebAppTest3.Models.MyRestaurant;

public partial class MenuItem
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public string? Desc { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
