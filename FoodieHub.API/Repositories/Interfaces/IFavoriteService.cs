using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Favorite;
using FoodieHub.API.Models.DTOs.Recipe;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IFavoriteService
    {
        Task<Favorite?> Create(FavoriteDTO favorite);

        Task<bool> Delete(FavoriteDTO favorite);
        Task<IEnumerable<GetArticleDTO>> GetFavoriteArticle();
        Task<IEnumerable<GetRecipeDTO>> GetFavoriteRecipe();

    }
}
