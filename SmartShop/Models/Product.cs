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
