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
