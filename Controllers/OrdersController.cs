using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppTest3.Models.MyRestaurant;

namespace WebAppTest3.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MydataContext _context;

        public OrdersController(MydataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        // GET: Orders
        public IActionResult Index()
        {
            //var ors = await _context.Orders.ToListAsync();
            //if (ors)
            //{

            //    return View(ors);
            //}
            return View(_context.Orders.ToList());
        }


        //****************
        public IActionResult SelectMenuItems()
        {
            // Retrieve all menu items from the database
            var menuItems = _context.MenuItems.ToList();

            // Create a list of MenuItemViewModel objects
            var menuItemsViewModel = menuItems.Select(m => new MenuItemViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Desc = m.Desc,
                Price = (decimal)m.Price,
                Image = m.Image,
                
            }).ToList();

            // Pass the list of MenuItemViewModel objects to the view
            return View(menuItemsViewModel);
        }


        //*********************  order total
        public decimal CalculateOrderTotal(Order order)
        {
            decimal total = 0;

            foreach (var item in order.OrderItems)
            {
                var a = (decimal)item.MenuItem.Price;
                total += item.Quantity * a;
            }

            return total;
        }

        //**************** add to order

        [HttpPost]
        public IActionResult AddToOrder(int menuItemId)
        {
            // Retrieve the selected menu item from the database
            var menuItem = _context.MenuItems.FirstOrDefault(m => m.Id == menuItemId);

            if (menuItem == null)
            {
                return NotFound();
            }

            // Retrieve the order from the session
            var orderId = HttpContext.Session.GetInt32("OrderId");

            if (!orderId.HasValue)
            {
                // If the order id doesn't exist, create a new order and set its id in the session
                var newOrder = new Order();
                newOrder.CustomerName = "hadil";
                newOrder.TotalAmount = 0;
                _context.Orders.Add(newOrder);
                _context.SaveChanges();
                orderId = newOrder.Id;
                HttpContext.Session.SetInt32("OrderId", orderId.Value);
            }

            // Retrieve the order from the database using the id
            var order = _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                                         .FirstOrDefault(o => o.Id == orderId.Value);

            if (order == null)
            {
                return NotFound();
            }
            var existingOrderItem = _context.OrderItems
                .Include(oi => oi.MenuItem)
                .FirstOrDefault(oi => oi.OrderId == orderId && oi.MenuItem.Id == menuItemId);

            if (existingOrderItem != null)
            {
                // If the menu item is already in the order, increment the quantity
                existingOrderItem.Quantity++;
                _context.SaveChanges();
            }
            else
            {
                // If the menu item is not already in the order, add it as a new order item
                var orderItem = new OrderItem
                {
                    MenuItem = menuItem,
                    Quantity = 1,
                    IsSelected = true,
                    Price = (decimal)menuItem.Price,
                    OrderId = orderId.Value,
                    
                };

                order.OrderItems.Add(orderItem);
                _context.SaveChanges();
            }

            // Save the updated order to the session
            HttpContext.Session.SetInt32("Order", order.Id);
            
            return RedirectToAction("SelectMenuItems");
        }
    


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerName,TotalAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,TotalAmount")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'MydataContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
