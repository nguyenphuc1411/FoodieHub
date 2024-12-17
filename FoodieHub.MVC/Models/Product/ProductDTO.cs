using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models
{
    public class ProductDTO
    {
        [Key]
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        //public string MainImage { get; set; }
        public IFormFile MainImage { get; set; }

        public string Description { get; set; }

        public int Discount { get; set; }


        public int StockQuantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ShelfLife { get; set; }

        [NotMapped]
        public List<IFormFile>? Images { get; set; } 

        public bool IsActive { get; set; } = true;

        // Foreign Key Property
        public int CategoryID { get; set; }



    }
}
