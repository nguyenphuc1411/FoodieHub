using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Comment
{
    public class CommentDTO
    {
        public int? RecipeID { get; set; }

        public int? ArticleID { get; set; }

        [Required(ErrorMessage = "Comment content is required.")]
        [StringLength(255, ErrorMessage = "Comment content cannot be longer than 500 characters.")]
        public string CommentContent { get; set; } = default!;
    }
}
