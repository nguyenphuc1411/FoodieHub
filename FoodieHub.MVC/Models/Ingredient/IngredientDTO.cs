using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Ingredient
{
    public class IngredientDTO
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = default!;
        [MaxLength(50)]
        public string Unit { get; set; } = default!;
    }
}
