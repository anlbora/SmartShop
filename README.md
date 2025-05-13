# 📦 SmartShop

SmartShop, ASP.NET Core MVC teknolojisiyle geliştirilmiş basit ama işlevsel bir ürün, sipariş ve müşteri yönetim sistemidir. Uygulama; CRUD işlemleri, filtreleme, raporlama ve PDF çıktısı gibi temel e-ticaret yönetimi ihtiyaçlarını karşılamaktadır.

## 🚀 Özellikler

- Kategori, ürün, müşteri ve sipariş CRUD işlemleri
- Gelişmiş dashboard görünümü ve özet bilgiler
- Dinamik ve filtrelenebilir raporlama sayfaları
- PDF çıktısı oluşturma desteği (`libwkhtmltox.dll`)
- Razor View ve Layout yapısı
- MVC mimarisine uygun modüler yapı

---

## 🧱 Proje Yapısı



```
SmartShop/
│
├── Controllers/
│ ├── CategoriesController.cs
│ ├── CustomersController.cs
│ ├── ProductsController.cs
│ ├── OrdersController.cs
│ ├── DashboardController.cs
│ ├── ReportsController.cs
│ ├── HomeController.cs
│ └── ControllerExtensions.cs
│
├── Models/
│ ├── Category.cs
│ ├── Customer.cs
│ ├── Product.cs
│ ├── Order.cs
│ ├── OrderItem.cs
│ ├── OrderStatus.cs
│ ├── CategoryProductCountViewModel.cs
│ ├── DashboardViewModel.cs
│ ├── ErrorViewModel.cs
│ └── PdfService.cs
│
├── Views/
│ ├── Categories/
│ │ ├── Create.cshtml
│ │ ├── Delete.cshtml
│ │ ├── Details.cshtml
│ │ ├── Edit.cshtml
│ │ └── Index.cshtml
│ │
│ ├── Customers/
│ │ └── (müşteri ile ilgili view dosyaları)
│ │
│ ├── Products/
│ │ ├── Create.cshtml
│ │ ├── Delete.cshtml
│ │ ├── Details.cshtml
│ │ ├── Edit.cshtml
│ │ └── Index.cshtml
│ │
│ ├── Orders/
│ │ ├── Create.cshtml
│ │ ├── Delete.cshtml
│ │ ├── Details.cshtml
│ │ ├── Edit.cshtml
│ │ └── Index.cshtml
│ │
│ ├── Dashboard/
│ │ ├── Index.cshtml
│ │ └── _OrderTablePartial.cshtml
│ │
│ ├── Reports/
│ │ ├── _CategoryReportPartial.cshtml
│ │ ├── _CustomersReportPartial.cshtml
│ │ ├── _OrdersReportPartial.cshtml
│ │ ├── _ProductsReportPartial.cshtml
│ │ └── _FullDashboardReportPartial.cshtml
│ │
│ └── Shared/
│ ├── _Layout.cshtml
│ ├── _ViewImports.cshtml
│ ├── _ViewStart.cshtml
│ ├── _ValidationScriptsPartial.cshtml
│ └── Error.cshtml
│
├── Data/
│ └── (DbContext ve Seed Data kodları buraya eklenebilir)
│
├── Migrations/
│ └── (EF Core migration dosyaları)
│
├── wwwroot/
│ └── (CSS, JS, görseller)
│
├── appsettings.json
├── Program.cs
├── libwkhtmltox.dll
└── SmartShop.csproj

```

---

## 🧩 Temel Modeller

### Category.cs
Kategori bilgilerini tutar. Her kategoriye birden fazla ürün bağlı olabilir.

```
public class Category
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    // Kategoriye ait ürünler
    public List<Product>? Products { get; set; }
}
```

### Customer.cs
Müşteri bilgilerini içerir. Zorunlu alanlar: `Name`, `Address`, `Phone`.

```
using System.ComponentModel.DataAnnotations;

namespace SmartShop.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required, StringLength(250)]
        public string Address { get; set; }

        public List<Order>? Orders { get; set; }
        [Required, StringLength(15)]
        [Phone]
        public string? Phone { get; set; }  // ← EKLENDİ
    }
}
```

### Product.cs
Ürün bilgileri, stok, fiyat ve kategori ilişkisi.

```
using System;
using System.ComponentModel.DataAnnotations;

namespace SmartShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        public string? ImagePath { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDiscounted { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountPrice { get; set; }
        public List<OrderItem>? OrderItems { get; set; }

        // Yeni: Kategori ilişkisi
        public int CategoryId { get; set; }
        public Category? Category { get; set; }


    }
}
```



### Order.cs / OrderItem.cs
Siparişler ve bu siparişlerdeki ürünlerin bilgilerini içerir.

```
using System.ComponentModel.DataAnnotations;
using SmartShop.Models;

namespace SmartShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public List<OrderItem> Items { get; set; } = new();

        // 🧾 Toplam Tutar Hesabı
        public decimal TotalAmount => Items?.Sum(i =>
            i.Quantity * (
                i.Product.IsDiscounted && i.Product.DiscountPrice.HasValue
                    ? i.Product.DiscountPrice.Value
                    : i.Product.Price)
            ) ?? 0;


        // 🚚 Sipariş Durumu
        public OrderStatus Status { get; set; } = OrderStatus.Received;

        // 👤 Müşteri bilgisi
        public int CustomerId { get; set; }  // Foreign Key
        [Required]
        public Customer Customer { get; set; }
    }

}
```
```
namespace SmartShop.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        // Sipariş ilişkisi
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // Ürün ilişkisi
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}

```


---

## 📊 Dashboard ve Raporlama

- **Dashboard**: Sipariş özeti, günlük siparişler ve grafiksel özetler
- **Raporlar**:
  - Kategori bazlı ürün dağılımı
  - Müşteri listesi
  - Sipariş durumu ve tarih bazlı siparişler
  - PDF çıktısı desteği (`PdfService.cs` ile)
    
---



