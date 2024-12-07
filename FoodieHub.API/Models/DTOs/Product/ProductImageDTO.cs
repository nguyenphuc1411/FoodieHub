namespace FoodieHub.API.Models.DTOs.Product
{
    public class ProductImageDTO
    {
        public IFormFile ImageURL { get; set; }

        // Foregin Key Property
        public int ProductID { get; set; }
    }
}
