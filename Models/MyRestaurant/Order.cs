using System;
using System.Collections.Generic;

namespace WebAppTest3.Models.MyRestaurant;

public partial class Order
{

    public int Id { get; set; }

    public string CustomerName { get; set; } = null!;

   

    public decimal TotalAmount { get; set; }

    public virtual ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
}
