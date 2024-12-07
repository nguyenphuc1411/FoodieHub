using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }


        [Column(TypeName = "nvarchar(255)")]
        public string ProductName { get; set; } = default!;

        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal Price { get; set; }


        [Column(TypeName = "varchar(255)")]
        public string MainImage { get; set; } = default!;


        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; } = default!;
        public int Discount { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;

        public int ShelfLife { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foregin Key Property
        public int CategoryID { get; set; }

        // Foreign Key Link
        public Category Category { get; set; } = default!;

        // Foreign Key Collections
        public ICollection<Review> Reviews { get; set; } = default!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = default!;
        public ICollection<ProductImage> ProductImages { get; set; } = default!;
        public ICollection<Contact> Contacts { get; set; } = default!;
        public ICollection<RecipeProduct> RecipeProducts { get; set; } = default!;

    }
}
