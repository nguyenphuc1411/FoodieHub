using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Service.Interfaces;

namespace FoodieHub.MVC.Service.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly HttpClient _httpClient;

        public FavoriteService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<bool> Create(FavoriteDTO favorite)
        {
            var response = await _httpClient.PostAsJsonAsync("favorites", favorite);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(FavoriteDTO favorite)
        {
            var response = await _httpClient.DeleteAsync($"favorites?recipeId={favorite.RecipeID}&articleId={favorite.ArticleID}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<GetArticleDTO>?> GetFA()
        {
            var response = await _httpClient.GetAsync("favorites/articles");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<GetArticleDTO>>();
            }
            return null;
        }

        public async Task<IEnumerable<GetRecipeDTO>?> GetFR()
        {
            var response = await _httpClient.GetAsync("favorites/recipes");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<GetRecipeDTO>>();
            }
            return null;
        }
    }
}
