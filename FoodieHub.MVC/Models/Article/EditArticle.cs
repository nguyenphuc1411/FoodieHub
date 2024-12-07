using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Article
{
    public class EditArticle
    {
        public int ArticleID { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        public IFormFile? File { get; set; }
        [Required]
        public string Description { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }
    }
}
