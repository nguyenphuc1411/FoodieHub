namespace FoodieHub.MVC.Models.Article
{
    public class ArticleViewModel
    {
        public IEnumerable<GetArticleDTO> TopArticles { get; set; } = new List<GetArticleDTO>();
        public IEnumerable<GetArticleDTO> LatestArticlesList { get; set; }= new List<GetArticleDTO>();
    }
}
