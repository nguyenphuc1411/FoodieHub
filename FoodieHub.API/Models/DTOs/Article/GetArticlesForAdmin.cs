namespace FoodieHub.API.Models.DTOs.Article
{
    public class GetArticlesForAdmin
    {
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string MainImage { get; set; }
        public string Fullname { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int CategoryID {  get; set; }
        public string CategoryName { get; set; }
    }
}
