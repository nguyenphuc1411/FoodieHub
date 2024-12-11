using System.ComponentModel.DataAnnotations;
namespace FoodieHub.MVC.Models.Article
{
    public class CreateArticleDTO
    {
        [MaxLength(255, ErrorMessage = "The title must not exceed 255 characters.")]
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = default!;

        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } = default!;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryID { get; set; }
        public bool IsActive { get; set; }
    }

}
