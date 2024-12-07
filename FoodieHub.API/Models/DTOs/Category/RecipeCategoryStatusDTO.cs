namespace FoodieHub.API.Models.DTOs.Category
{
    public class RecipeCategoryStatusDTO
    {
        public int CategoryID { get; set; }

        public bool IsActice { get; set; } = true;

        public bool IsDeleted { get; set; } = false;
    }
}
