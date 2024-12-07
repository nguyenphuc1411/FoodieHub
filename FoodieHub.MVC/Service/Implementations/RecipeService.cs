using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.MVC.Models.Recipe;
using System.Net.Http.Json;

namespace FoodieHub.MVC.Service.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly HttpClient _httpClient;

        public RecipeService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        //hien thi search
        public async Task<IEnumerable<GetRecipes>> GetRecipes(string? search, int? pageSize, int? currentPage)
        {
            // Xây dựng URL query
            var query = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(search)) query.Add("search", search);
            if (pageSize.HasValue) query.Add("pageSize", pageSize.Value.ToString());
            if (currentPage.HasValue) query.Add("currentPage", currentPage.Value.ToString());

            var queryString = string.Join("&", query.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var url = string.IsNullOrWhiteSpace(queryString) ? "Recipes" : $"Recipes?{queryString}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<List<GetRecipes>>>();
                if (content != null && content.Success)
                {
                    return content.Data ?? new List<GetRecipes>();
                }
            }

            return new List<GetRecipes>();
        }


        public async Task<APIResponse<GetRecipeDetail>> GetRecipeDetail(int recipeID)
        {
            var response = await _httpClient.GetAsync($"Recipes/{recipeID}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<GetRecipeDetail>>();

                if (content != null && content.Success)
                {
                    return new APIResponse<GetRecipeDetail>
                    {
                        Success = true,
                        Data = content.Data
                    };
                }
            }

            return new APIResponse<GetRecipeDetail>
            {
                Success = false,
                Message = "Failed to retrieve recipe details."
            };
        }

        



    }
}
