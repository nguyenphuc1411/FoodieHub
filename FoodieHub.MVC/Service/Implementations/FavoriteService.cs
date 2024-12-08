using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Favorite;
using FoodieHub.API.Models.DTOs.Recipe;
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

        public async Task<bool> Create(CreateFavoriteDTO favorite)
        {
            var response = await _httpClient.PostAsJsonAsync("favorites", favorite);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"favorites/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<GetArticleDTO>?> GetFA()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetArticleDTO>>("favorites/articles");
        }

        public async Task<IEnumerable<GetRecipeDTO>?> GetFR()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetRecipeDTO>>("favorites/recipes");
        }
    }
}
