namespace FoodieHub.MVC.Models.Article
{
    public class ArticleViewModel
    {
        public GetArticle? FeatureArticle { get; set; }
        public List<GetArticle> TopArticlesByFavourite { get; set; } = new();
        public GetArticle? LatestArticle { get; set; }
        public List<GetArticle> LatestArticlesList { get; set; } = new();
    }
}
