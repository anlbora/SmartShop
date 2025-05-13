using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShop.Data;
using SmartShop.Models;
using SmartShop.Services;
using SmartShop.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartShop.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PdfService _pdfService;

        public DashboardController(AppDbContext context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<IActionResult> FilterOrders(DateTime? startDate, DateTime? endDate, string status, int? customerId)
        {
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .AsQueryable();

            if (startDate.HasValue)
                orders = orders.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                orders = orders.Where(o => o.OrderDate <= endDate.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var parsedStatus))
                orders = orders.Where(o => o.Status == parsedStatus);

            if (customerId.HasValue)
                orders = orders.Where(o => o.CustomerId == customerId);

            var filtered = await orders.OrderByDescending(o => o.OrderDate).ToListAsync(); // tümünü alıyoruz

            return PartialView("_OrderTablePartial", filtered);
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string status, int? customerId)
        {
            var today = DateTime.Today;

            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var todayOrders = await _context.Orders.CountAsync(o => o.OrderDate.Date == today);
            var lowStockProducts = await _context.Products.CountAsync(p => p.Stock <= 5);

            // Siparişleri filtreleme
            var recentOrdersQuery = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items) // bu var
                .ThenInclude(i => i.Product) // bu da olmalı
                .AsQueryable();


            if (startDate.HasValue)
                recentOrdersQuery = recentOrdersQuery.Where(o => o.OrderDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                recentOrdersQuery = recentOrdersQuery.Where(o => o.OrderDate.Date <= endDate.Value.Date);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var parsedStatus))
                recentOrdersQuery = recentOrdersQuery.Where(o => o.Status == parsedStatus);

            if (customerId.HasValue)
                recentOrdersQuery = recentOrdersQuery.Where(o => o.CustomerId == customerId);

            var recentOrders = await recentOrdersQuery
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(); // 🔁 tümünü alıyoruz

            // Aylık sipariş sayısı
            var ordersByMonth = await _context.Orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var chartLabels = ordersByMonth.Select(x => $"{x.Month}/{x.Year}").ToList();
            var chartData = ordersByMonth.Select(x => x.Count).ToList();

            var products = await _context.Products.ToListAsync();
            var stockLabels = products.Select(p => p.Name).ToList();
            var stockData = products.Select(p => p.Stock).ToList();

            var statusGroups = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            var monthlyAverage = await _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .ToListAsync();

            // Günlük ortalama sipariş tutarı
            var dailyAverages = monthlyAverage
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    AvgAmount = g.Average(x => x.Items.Sum(i => i.Quantity *
                        (i.Product.IsDiscounted && i.Product.DiscountPrice.HasValue
                            ? i.Product.DiscountPrice.Value
                            : i.Product.Price)))
                })
                .OrderBy(g => g.Date)
                .ToList();

            var dailyRevenues = monthlyAverage
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(x => x.Items.Sum(i => i.Quantity *
                        (i.Product.IsDiscounted && i.Product.DiscountPrice.HasValue
                            ? i.Product.DiscountPrice.Value
                            : i.Product.Price)))
                })
                .OrderBy(g => g.Date)
                .ToList();

            var averageGroups = monthlyAverage
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    AvgAmount = g.Average(x => x.Items.Sum(i => i.Quantity *
                        (i.Product.IsDiscounted && i.Product.DiscountPrice.HasValue
                            ? i.Product.DiscountPrice.Value
                            : i.Product.Price)))
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            var revenueGroups = monthlyAverage
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalRevenue = g.Sum(x => x.Items.Sum(i => i.Quantity *
                        (i.Product.IsDiscounted && i.Product.DiscountPrice.HasValue
                            ? i.Product.DiscountPrice.Value
                            : i.Product.Price)))
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            var allProducts = await _context.Products.ToListAsync();
            var allCustomers = await _context.Customers.ToListAsync();

            var categoryCounts = await _context.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryProductCountViewModel
                {
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();

            var totalCategories = await _context.Categories.CountAsync();

            var categoryCountsChart = await _context.Categories
                .Select(c => new {
                    Name = c.Name,
                    Count = c.Products.Count
                }).ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TodayOrders = todayOrders,
                LowStockProducts = lowStockProducts,
                RecentOrders = recentOrders, // ✅ Tüm siparişler artık buraya geliyor
                ChartLabels = chartLabels,
                ChartData = chartData,
                StockChartLabels = stockLabels,
                StockChartData = stockData,
                OrderStatusLabels = statusGroups.Select(s => s.Status).ToList(),
                OrderStatusData = statusGroups.Select(s => s.Count).ToList(),
                AverageOrderAmounts = averageGroups.Select(x => x.AvgAmount).ToList(),
                AverageOrderLabels = averageGroups.Select(x => $"{x.Month}/{x.Year}").ToList(),
                TotalRevenueAmounts = revenueGroups.Select(x => x.TotalRevenue).ToList(),
                RevenueLabels = revenueGroups.Select(x => $"{x.Month}/{x.Year}").ToList(),
                Products = allProducts,
                Customers = allCustomers,
                OrderChartBase64 = _pdfService.GenerateChartAsBase64("Sipariş Grafiği"),
                StockChartBase64 = _pdfService.GenerateChartAsBase64("Stok Dağılımı"),
                StatusChartBase64 = _pdfService.GenerateChartAsBase64("Sipariş Durumları"),
                RevenueChartBase64 = _pdfService.GenerateChartAsBase64("Aylık Ciro"),
                StartDate = startDate,
                EndDate = endDate,
                SelectedStatus = status,
                SelectedCustomerId = customerId,
                CategoryProductCounts = categoryCounts,
                TotalCategories = totalCategories,
                CategoryChartLabels = categoryCountsChart.Select(c => c.Name).ToList(),
                CategoryChartData = categoryCountsChart.Select(c => c.Count).ToList(),
                DailyRevenueLabels = dailyRevenues.Select(x => x.Date.ToString("dd.MM.yyyy")).ToList(),
                DailyRevenueData = dailyRevenues.Select(x => x.Total).ToList(),
                DailyAverageOrderLabels = dailyAverages.Select(x => x.Date.ToString("dd.MM.yyyy")).ToList(),
                DailyAverageOrderData = dailyAverages.Select(x => x.AvgAmount).ToList()

            };

            return View(viewModel);
        }
    }
}
