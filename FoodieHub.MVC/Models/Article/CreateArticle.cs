using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Article
{
    public class CreateArticle
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int CategoryID { get; set; }
    }
}
