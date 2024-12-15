using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Models.Response;
namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IRecipeService
    {
        Task<DetailRecipeDTO?> GetByID(int id);

        Task<bool> Rating(CreateRatingDTO ratingDTO);

        Task<bool> Create(CreateRecipeDTO recipeDTO);

        Task<bool> Update(UpdateRecipeDTO recipeDTO);

        Task<IEnumerable<GetRecipeDTO>?> GetOfUser();
        Task<IEnumerable<GetRecipeDTO>?> GetByUser(string userId);

        Task<bool> Delete(int id);

        Task<PaginatedModel<GetRecipeDTO>?> GetAll(QueryRecipeModel query);
    }
}
