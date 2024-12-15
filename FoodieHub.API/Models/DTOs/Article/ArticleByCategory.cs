namespace FoodieHub.API.Models.DTOs.Article
{
    public class ArticleByCategory
    {
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }

        public GetArticleDTO FeatureArticle { get; set; } = default!;
        public IEnumerable<GetArticleDTO> LastedArticles { get; set; } = new List<GetArticleDTO>(); 
    }
}
