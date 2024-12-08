using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories();

        Task<CategoryDTO> GetCategoryById(int id);

        Task<ServiceResponse> AddCategory(CategoryDTO category);

        Task<ServiceResponse> UpdateCategory(CategoryDTO category);

        Task<ServiceResponse> DeleteCategory(int categoryId);
    }
}
