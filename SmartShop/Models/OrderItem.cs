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
