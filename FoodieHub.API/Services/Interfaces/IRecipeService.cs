using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<bool> Create(CreateRecipeDTO recipeDTO);
        Task<ServiceResponse> Update(int recipeID,UpdateRecipe recipeDTO);
        Task<ServiceResponse> GetDetail(int recipeID);
        Task<ServiceResponse> Rating(RatingDTO ratingDTO);
        Task<ServiceResponse> Get(string? search, int? pageSize, int? currentPage);
        Task<ServiceResponse> GetForAdmin(string? search, int? categoryID, bool? isActive, bool? isDeleted, int pageSize, int currentPage);

        Task<ServiceResponse> GetDetailForAdmin(int receipeID);
        Task<ServiceResponse> GetByUser(string userID);
        Task<ServiceResponse> GetOfUser();

        Task<object> GetRecipeOfUserForAdmin(string status);
        Task<bool> DeleteOfUser(int id);
        Task<object> GetDetailForEdit(int id);
        Task<bool> DeleteRecipeOfUser(int id);

    }
}
