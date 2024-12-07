using FoodieHub.API.Models.DTOs.Category;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Product
{
    public class ProductDTO
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }


        public decimal Price { get; set; }


        public IFormFile MainImage { get; set; }


        public string Description { get; set; }

        public int Discount { get; set; }

        public int StockQuantity { get; set; }
        public int ShelfLife { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Foregin Key Property
        public int CategoryID { get; set; }

        


    }
}
