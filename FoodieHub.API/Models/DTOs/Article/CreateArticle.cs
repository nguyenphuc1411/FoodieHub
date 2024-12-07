using System.ComponentModel.DataAnnotations;
namespace FoodieHub.API.Models.DTOs.Article
{
    public class CreateArticle
    {
        [MaxLength(255)]
        public string Title { get; set; } = default!;
        public IFormFile File { get; set; } = default!;
        public string Description { get; set; } = default!;
        [Required(ErrorMessage ="Category is required")]
        public int CategoryID { get; set; }
    }
}
