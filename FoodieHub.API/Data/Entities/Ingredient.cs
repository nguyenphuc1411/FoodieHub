using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Column(TypeName ="nvarchar(255)")]
        public string Name { get; set; } = default!;

        public float Quantity { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Unit { get; set; } = default!;

        public int RecipeID { get; set; }
        public Recipe Recipe { get; set; } = default!;

        public int? ProductID { get; set; }
        public Product? Product { get; set; }
    }
}
