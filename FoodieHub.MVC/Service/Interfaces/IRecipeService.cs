using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<GetRecipes>> GetRecipes(string? search, int? pageSize, int? currentPage);
        Task<APIResponse<GetRecipeDetail>> GetRecipeDetail(int recipeID);
        
    }
}
