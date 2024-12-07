using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Article
{
    public class UpdateArticle
    {
        [MaxLength(255)]
        public string Title { get; set; }
        public IFormFile? File { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }
    }
}
