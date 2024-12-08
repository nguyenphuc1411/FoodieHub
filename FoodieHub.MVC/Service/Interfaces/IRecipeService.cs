using FoodieHub.API.Models.DTOs.Recipe;
namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IRecipeService
    {
/*        Task<IEnumerable<GetRecipes>> GetRecipes(string? search, int? pageSize, int? currentPage);
        Task<APIResponse<GetRecipeDetail>> GetRecipeDetail(int recipeID);*/

        Task<bool> Rating(CreateRatingDTO ratingDTO);

        Task<bool> Create(CreateRecipeDTO recipeDTO);
    }
}
