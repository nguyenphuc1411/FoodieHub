using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IRecipeCategoryService
    {
        Task<IEnumerable<RecipeCategory>> GetAllRecipeCategories();
        Task<ServiceResponse> AddRecipeCategory(RecipeCategoryDTO category);
        Task<ServiceResponse> UpdateRecipeCategoryWithImg(RecipeCategoryWithImgDTO category);

        Task<ServiceResponse> UpdateRecipeCategoryNoneImg(RecipeCategoryNoneImgDTO category);

        Task<ServiceResponse> UpdateStatusCategory(RecipeCategoryStatusDTO category);

        Task<ServiceResponse> DeleteRecipeCategory(int id);
    }
}
