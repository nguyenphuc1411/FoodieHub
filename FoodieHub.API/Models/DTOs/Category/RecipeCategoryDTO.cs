namespace FoodieHub.API.Models.DTOs.Category
{
    public class RecipeCategoryDTO
    {
        public int CategoryID { get; set; }

        public string? CategoryName { get; set; }


        public IFormFile? ImageURL { get; set; }
    }
}
