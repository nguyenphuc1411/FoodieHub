using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.MVC.Models.Product
{
    public class GetProductImages
    {
        public int ImageID { get; set; }

        public string ImageURL { get; set; }

        public int ProductID { get; set; }
    }
}
