using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data.Entities
{
    public class RecipeStep
    {
        public int Id { get; set; }

        public int RecipeID { get; set; }
        public Recipe Recipe { get; set; } = default!;

        public int Step { get; set; }

        [Column(TypeName ="varchar(255)")]
        public string? ImageURL { get; set; }

        public string Directions { get; set; } = default!;

    }
}
