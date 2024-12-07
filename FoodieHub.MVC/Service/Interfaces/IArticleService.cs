using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IArticleService
    {
      
        Task<IEnumerable<GetArticle>> GetAll(string? search, int? pageSize, int? currentPage);
        Task<APIResponse<GetArticleDetail>> GetDetail(int id);
    }
}
