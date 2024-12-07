
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models
{
    public class ProductImageDTO
    {
        public int ImageID { get; set; }
        public IFormFile ImageURL { get; set; }

        // Foregin Key Property
        public int ProductID { get; set; }

    }
}
