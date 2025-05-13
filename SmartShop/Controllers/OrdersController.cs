using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartShop.Data;
using SmartShop.Models;

namespace SmartShop.Controllers
{

    public class ProductOrderInput
    {
        public int ProductId { get; set; }
        public bool IsSelected { get; set; }
        public int Quantity { get; set; }
    }

    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync();

            return View(orders);
        }



        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
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
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name");
            return View();
        }



        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, List<ProductOrderInput> ProductItems)
        {
            order.Items = new List<OrderItem>();

            foreach (var item in ProductItems)
            {
                if (item.IsSelected && item.Quantity > 0)
                {
                    order.Items.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }
            }

            if (order.Items.Count == 0)
            {
                ModelState.AddModelError("", "En az bir ürün seçmelisiniz.");
                ViewBag.Products = _context.Products.ToList();
                ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name");
                return View(order);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            ViewBag.Products = _context.Products.ToList();

            return View(order);
        }



        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order updatedOrder, List<int> ProductIds, List<int> Quantities)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Temel bilgiler
            order.CustomerId = updatedOrder.CustomerId;
            order.Status = updatedOrder.Status;

            // Mevcut ürünleri temizle
            order.Items.Clear();

            // Yeni ürünleri yeniden ekle
            for (int i = 0; i < ProductIds.Count; i++)
            {
                if (Quantities[i] > 0)
                {
                    order.Items.Add(new OrderItem
                    {
                        ProductId = ProductIds[i],
                        Quantity = Quantities[i]
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }


        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
