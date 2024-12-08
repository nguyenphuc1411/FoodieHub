using FoodieHub.MVC.Models.Article;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IArticleService
    {
      
        Task<IEnumerable<GetArticle>> GetAll(string? search, int? pageSize, int? currentPage);
    }
}
