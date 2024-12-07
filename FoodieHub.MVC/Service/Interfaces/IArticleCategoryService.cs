using FoodieHub.MVC.Models.DTOs;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IArticleCategoryService
    {
        Task<IEnumerable<ArticleCategoryDTO>> GetAll();

        Task<APIResponse> GetById(int id);

        Task<APIResponse> AddArticleCategory(ArticleCategoryDTO articleCategoryDTO);
        Task<APIResponse> UpdateArticleCategory(ArticleCategoryDTO articleCategoryDTO);
    }
}
