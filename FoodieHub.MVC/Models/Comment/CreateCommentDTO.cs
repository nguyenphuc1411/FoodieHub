using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Comment
{
    public class CreateCommentDTO
    {
        [Required(ErrorMessage = "RecipeID or ArticleID is required.")]
        public int? RecipeID { get; set; }

        [Required(ErrorMessage = "RecipeID or ArticleID is required.")]
        public int? ArticleID { get; set; }

        [Required(ErrorMessage = "Comment content is required.")]
        [StringLength(255, ErrorMessage = "Comment content cannot be longer than 255 characters.")]
        public string CommentContent { get; set; } = default!;
    }
}
