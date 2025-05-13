# 📦 SmartShop

SmartShop, ASP.NET Core MVC teknolojisiyle geliştirilmiş basit ama işlevsel bir ürün, sipariş ve müşteri yönetim sistemidir. Uygulama; CRUD işlemleri, filtreleme, raporlama ve PDF çıktısı gibi temel e-ticaret yönetimi ihtiyaçlarını karşılamaktadır.

![Image](https://github.com/user-attachments/assets/0c4ea683-c56f-48a4-887c-54c86b8ba318)
![Image](https://github.com/user-attachments/assets/fa61e039-cb29-4830-9087-5889850f9c49)

## 🚀 Özellikler

- Kategori, ürün, müşteri ve sipariş CRUD işlemleri
- Gelişmiş dashboard görünümü ve özet bilgiler
- Dinamik ve filtrelenebilir raporlama sayfaları
- PDF çıktısı oluşturma desteği (`libwkhtmltox.dll`)
- Razor View ve Layout yapısı
- MVC mimarisine uygun modüler yapı

![Image](https://github.com/user-attachments/assets/3d35eb54-d719-4829-9853-43a69131c948)
![Image](https://github.com/user-attachments/assets/e95f9571-6da1-462d-8bb2-bfaaff6522bf)
![Image](https://github.com/user-attachments/assets/a526865c-34a9-4d13-b4d8-42b8f464d404)
![Image](https://github.com/user-attachments/assets/c51b97ee-dba8-4245-b6c8-b234ab912f0c)
![Image](https://github.com/user-attachments/assets/aa6a603c-b761-43f8-bcad-aaba9fdfb367)

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
![Image](https://github.com/user-attachments/assets/d6e2e1df-37c7-4184-be38-6394524ee0a2)
![Image](https://github.com/user-attachments/assets/f1acf4f2-3159-418e-b0bc-63492bd34efc)

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
![Image](https://github.com/user-attachments/assets/10bc75fb-cd72-4e27-adc5-0ef18a237343)
![Image](https://github.com/user-attachments/assets/2c8b78a1-789c-4ce0-b888-5edc3d273c2c)
![Image](https://github.com/user-attachments/assets/161eca38-0b53-4f8d-af30-d2f8e0bd25b0)
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
![Image](https://github.com/user-attachments/assets/9f180bbb-a1ab-42a7-a73e-6dccf3e6ce50)
![Image](https://github.com/user-attachments/assets/4ed54fd9-8033-4bb1-beb6-84ee649d9773)
![Image](https://github.com/user-attachments/assets/9e4ccea8-0af0-44bb-a2a0-12a857089ba7)
![Image](https://github.com/user-attachments/assets/0f338280-0b93-4ae8-92b9-b98b872a4f61)

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
![Image](https://github.com/user-attachments/assets/d6e2e1df-37c7-4184-be38-6394524ee0a2)
![Image](https://github.com/user-attachments/assets/f1acf4f2-3159-418e-b0bc-63492bd34efc)
![Image](https://github.com/user-attachments/assets/218f34a0-29a9-420d-86f2-a7b1905a0278)

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



