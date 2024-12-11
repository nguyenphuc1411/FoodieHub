using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Article
{
    public class UpdateArticleDTO
    {
        public int ArticleID { get; set; }
        [MaxLength(255, ErrorMessage = "The title must not exceed 255 characters.")]
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = default!;

        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryID { get; set; }
        public bool IsActive { get; set; }
    }
}
