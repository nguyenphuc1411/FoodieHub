using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IArticleCategoryService
    {
        Task<IEnumerable<ArticleCategory>> GetAllArticleCategories();
        Task<ServiceResponse> AddArticleCategory(ArticleCategoryDTO category);
        Task<ServiceResponse> UpdateArticleCategory(ArticleCategoryDTO category);
        Task<ServiceResponse> DeleteArticleCategory(int id);
    }
}
