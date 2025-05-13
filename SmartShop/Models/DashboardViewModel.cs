using SmartShop.Models;
using System.Collections.Generic;

namespace SmartShop.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public int LowStockProducts { get; set; }
        public int TotalCategories { get; set; }
        public List<CategoryProductCountViewModel> CategoryProductCounts { get; set; }

        public List<Order> RecentOrders { get; set; }
        public List<Product> TopSellingProducts { get; set; } // isteğe bağlı

        public List<string> ChartLabels { get; set; }
        public List<int> ChartData { get; set; }

        public List<string> StockChartLabels { get; set; } = new();
        public List<int> StockChartData { get; set; } = new();

        public List<string> OrderStatusLabels { get; set; }
        public List<int> OrderStatusData { get; set; }

        public List<string> AverageOrderLabels { get; set; } = new();
        public List<decimal> AverageOrderAmounts { get; set; } = new();
        public List<string> RevenueLabels { get; set; } = new();
        public List<decimal> TotalRevenueAmounts { get; set; } = new();

        public List<Product> Products { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();

        // 📦 Grafik Base64 verileri
        public string OrderChartBase64 { get; set; }
        public string StockChartBase64 { get; set; }
        public string StatusChartBase64 { get; set; }
        public string RevenueChartBase64 { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SelectedStatus { get; set; }
        public int? SelectedCustomerId { get; set; }

        public List<string> CategoryChartLabels { get; set; }
        public List<int> CategoryChartData { get; set; }
        public List<string> DailyRevenueLabels { get; internal set; }
        public List<decimal> DailyRevenueData { get; internal set; }
        public List<string> DailyAverageOrderLabels { get; internal set; }
        public List<decimal> DailyAverageOrderData { get; internal set; }
    }
}
