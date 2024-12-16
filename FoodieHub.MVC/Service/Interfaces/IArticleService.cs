using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IArticleService
    {
        Task<PaginatedModel<GetArticleDTO>> Get(QueryArticleModel query);
        Task<bool> Create(CreateArticleDTO article);
        Task<GetArticleDTO?> GetByID(int id);
        Task<bool> Delete(int id);
        Task<bool> Update(int id,UpdateArticleDTO article);
        Task<IEnumerable<GetArticleDTO>> GetOfUser(string userID);

        Task<IEnumerable<ArticleByCategory>> GetByCategory();
    }
}
