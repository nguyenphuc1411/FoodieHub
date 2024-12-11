using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class Recipe
    {
        [Key]
        public int RecipeID { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string ImageURL { get; set; } = default!;
        public TimeOnly CookTime { get; set; }
        public int Serves { get; set; }
        public bool IsAdminUpload { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
        public int CategoryID { get; set; }
        public RecipeCategory RecipeCategory { get; set; } = default!;

        // Foreign Key Collections

        public ICollection<Favorite> Favorites { get; set; } = default!;
        public ICollection<Rating> Ratings { get; set; } = default!;
        public ICollection<Comment> Comments { get; set; } = default!;
        public ICollection<Ingredient> Ingredients { get; set; } = default!;
        public ICollection<RecipeStep> RecipeSteps { get; set; } = default!;
        public ICollection<RecipeProduct> RecipeProducts { get; set; } = default!;

    }
}
