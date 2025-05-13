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
