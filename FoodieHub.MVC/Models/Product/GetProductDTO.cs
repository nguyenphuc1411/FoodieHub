using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.MVC.Models
{
    public class GetProductDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }


        public decimal Price { get; set; }

        //public string MainImage { get; set; }

        public string MainImage { get; set; }

        [NotMapped]
        public IFormFile ImgFileUpdate { get; set; }

        public string Description { get; set; }

        public int Discount { get; set; }

        public int StockQuantity { get; set; }

        public int ShelfLife { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryID { get; set; }
    }
}
