using FoodieHub.API.Models.DTOs.Recipe;
namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IRecipeService
    {
        Task<DetailRecipeDTO?> GetByID(int id);

        Task<bool> Rating(CreateRatingDTO ratingDTO);

        Task<bool> Create(CreateRecipeDTO recipeDTO);

        Task<IEnumerable<GetRecipeDTO>?> GetOfUser();
        Task<IEnumerable<GetRecipeDTO>?> GetByUser(string userId);

        Task<bool> Delete(int id);
    }
}
