using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data.Entities
{
    public class Favorite
    {
        public int Id { get; set; }
        public int? ArticleID { get; set; }
        public Article? Article { get; set; }

        public int? RecipeID { get; set; }
        public Recipe? Recipe { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
    }
}
