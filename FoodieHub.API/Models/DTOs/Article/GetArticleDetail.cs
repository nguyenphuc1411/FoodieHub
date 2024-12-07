namespace FoodieHub.API.Models.DTOs.Article
{
    public class GetArticleDetail
    {
        public int ArticleID { get; set; }
        public string Title { get; set; }

        public string MainImage { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string CategoryName { get; set; }
    }
}
