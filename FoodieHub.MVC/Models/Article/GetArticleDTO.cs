namespace FoodieHub.MVC.Models.Article
{
    public class GetArticleDTO
    {
        public int ArticleID { get; set; }
        public string Title { get; set; } = default!;
        public string MainImage { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string UserID { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Avatar { get; set; }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = default!;

        public int TotalComments { get; set; } = 0;
        public int TotalFavorites { get; set; } = 0;
    }
}
