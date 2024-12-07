using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.MVC.Models.Categories
{
    public class RecipeCategoryStatusDTO
    {
        public int CategoryID { get; set; }

        public bool IsActice { get; set; } = true;

        public bool IsDeleted { get; set; } = false;
    }
}
