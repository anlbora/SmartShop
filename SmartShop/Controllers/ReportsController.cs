using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShop.Data;
using SmartShop.Models;
using SmartShop.Services;
using SmartShop.ViewModels;

namespace SmartShop.Controllers
{
    [Route("[controller]/[action]")]
    public class ReportsController : Controller
    {
        private readonly PdfService _pdfService;
        private readonly AppDbContext _context;

        public ReportsController(PdfService pdfService, AppDbContext context)
        {
            _pdfService = pdfService;
            _context = context;
        }

        public async Task<IActionResult> OrdersReport()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();


            var html = await this.RenderViewAsync("_OrdersReportPartial", orders, true);
            var pdf = _pdfService.GeneratePdfFromHtml(html);

            return File(pdf, "application/pdf", "orders-report.pdf");
        }

        public async Task<IActionResult> CustomerReport()
        {
            var customers = await _context.Customers.ToListAsync();

            var html = await this.RenderViewAsync("_CustomersReportPartial", customers, true);
            var pdf = _pdfService.GeneratePdfFromHtml(html);

            return File(pdf, "application/pdf", "customers-report.pdf");
        }

        public async Task<IActionResult> ProductReport()
        {
            var products = await _context.Products.ToListAsync();
            var html = await this.RenderViewAsync("_ProductsReportPartial", products, true);
            var pdf = _pdfService.GeneratePdfFromHtml(html);
            return File(pdf, "application/pdf", "product-report.pdf");
        }

        public async Task<IActionResult> FullDashboardReport()
        {
            var model = new DashboardViewModel
            {
                Products = await _context.Products.ToListAsync(),
                Customers = await _context.Customers.ToListAsync(),
                RecentOrders = await _context.Orders
                                    .Include(o => o.Customer)
                                    .OrderByDescending(o => o.OrderDate)
                                    .Take(20)
                                    .ToListAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TodayOrders = await _context.Orders.CountAsync(o => o.OrderDate.Date == DateTime.Today),
                LowStockProducts = await _context.Products.CountAsync(p => p.Stock < 5)
            };

            var html = await this.RenderViewAsync("_FullDashboardReportPartial", model, true);
            var pdf = _pdfService.GeneratePdfFromHtml(html);

            return File(pdf, "application/pdf", "dashboard-report.pdf");
        }

        public async Task<IActionResult> CategoryReport()
        {
            var data = await _context.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryProductCountViewModel
                {
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();

            var html = await this.RenderViewAsync("_CategoriesReportPartial", data, true);
            var pdf = _pdfService.GeneratePdfFromHtml(html);

            return File(pdf, "application/pdf", "categories-report.pdf");
        }


    }
}
