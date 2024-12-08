using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IRecipeService
    {
        Task<bool> Create(CreateRecipeDTO recipeDTO);
        Task<bool> Update(UpdateRecipeDTO recipeDTO);
        Task<DetailRecipeDTO?> GetByID(int id);
        Task<bool> Rating(CreateRatingDTO ratingDTO);
        Task<PaginatedModel<GetRecipeDTO>> Get(QueryRecipeModel query);
        Task<IEnumerable<GetRecipeDTO>> GetByUser(string userID);
        Task<IEnumerable<GetRecipeDTO>> GetOfUser();
        Task<bool> Delete(int id);

    }
}
