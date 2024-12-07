namespace FoodieHub.API.Models.DTOs.Article
{
    public class GetArticles
    {
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string MainImage { get; set; }
        public string Fulllname { get; set; }
        public int? TotalComments { get; set; }
        public int? TotalFavorites { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string CategoryName { get; set; }
    }
}
