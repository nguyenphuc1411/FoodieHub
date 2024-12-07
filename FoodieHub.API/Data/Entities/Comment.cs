using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        public int? RecipeID { get; set; }
        public Recipe? Recipe { get; set; }

        public int? ArticleID { get; set; }
        public Article? Article { get; set; }


        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; }= default!;
        public ApplicationUser User { get; set; } = default!;


        [Column(TypeName = "nvarchar(255)")]
        public string CommentContent { get; set; } = default!;

        public DateTime CommentedAt { get; set; } = DateTime.Now;
  
    }
}
