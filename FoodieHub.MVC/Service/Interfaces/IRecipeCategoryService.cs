using FoodieHub.MVC.Models.DTOs;
using FoodieHub.MVC.Models.Categories;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IRecipeCategoryService
    {
        Task<IEnumerable<GetRecipeCategoryDTO>> GetAll();

        Task<APIResponse> AddRecipeCategory(RecipeCategoryDTO recipeCategoryDTO);

        Task<APIResponse> UpdateRecipeCategoryWithImg(RecipeCategoryWithImgDTO recipeCategoryDTO);

        Task<APIResponse> UpdateRecipeCategoryNoneImg(RecipeCategoryNoneImgDTO recipeCategoryDTO);

        Task<APIResponse> UpdateRecipeStatus(RecipeCategoryStatusDTO recipeCategoryStatusDTO);

        Task<APIResponse> DeleteRecipeCategory(RecipeCategoryDTO recipeCategoryDTO);
    }
}
