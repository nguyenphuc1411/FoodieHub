using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.API.Models.DTOs.Recipe;
using System.Net.Http.Headers;

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
        public async Task<IEnumerable<GetRecipeDTO>> GetRecipes(string? search, int? pageSize, int? currentPage)
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
                var content = await response.Content.ReadFromJsonAsync<APIResponse<List<GetRecipeDTO>>>();
                if (content != null && content.Success)
                {
                    return content.Data ?? new List<GetRecipeDTO>();
                }
            }

            return new List<GetRecipeDTO>();
        }


        public async Task<APIResponse<DetailRecipeDTO>> GetRecipeDetail(int recipeID)
        {
            var response = await _httpClient.GetAsync($"Recipes/{recipeID}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<DetailRecipeDTO>>();

                if (content != null && content.Success)
                {
                    return new APIResponse<DetailRecipeDTO>
                    {
                        Success = true,
                        Data = content.Data
                    };
                }
            }

            return new APIResponse<DetailRecipeDTO>
            {
                Success = false,
                Message = "Failed to retrieve recipe details."
            };
        }

        public async Task<bool> Rating(CreateRatingDTO ratingDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("recipes/ratings",ratingDTO);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Create(CreateRecipeDTO recipe)
        {
            using (var content = new MultipartFormDataContent())
            {
                // Thêm các thông tin khác của Recipe
                content.Add(new StringContent(recipe.Title), "Title");
                content.Add(new StringContent(recipe.CookTime.ToString()), "CookTime");
                content.Add(new StringContent(recipe.Serves.ToString()), "Serves");
                content.Add(new StringContent(recipe.CategoryID.ToString()), "CategoryID");
                content.Add(new StringContent(recipe.IsActive.ToString()), "IsActive");
                content.Add(new StringContent(recipe.Description ?? string.Empty), "Description");

                // Thêm file chính
                if (recipe.File != null)
                {
                    var fileContent = new StreamContent(recipe.File.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(recipe.File.ContentType);
                    content.Add(fileContent, "File", recipe.File.FileName);
                }

                // Sử lý các bước (RecipeSteps)
                for (int i = 0; i < recipe.RecipeSteps.Count; i++)
                {
                    content.Add(new StringContent(recipe.RecipeSteps[i].Step.ToString()), $"RecipeSteps[{i}].Step");
                    content.Add(new StringContent(recipe.RecipeSteps[i].Directions), $"RecipeSteps[{i}].Directions");

                    if (recipe.RecipeSteps[i].ImageStep != null)
                    {
                        var fileContentStep = new StreamContent(recipe.RecipeSteps[i].ImageStep.OpenReadStream());
                        fileContentStep.Headers.ContentType = new MediaTypeHeaderValue(recipe.RecipeSteps[i].ImageStep.ContentType);
                        content.Add(fileContentStep, $"RecipeSteps[{i}].ImageStep", recipe.RecipeSteps[i].ImageStep.FileName);
                    }
                }

                // Sử lý nguyên liệu (Ingredients)
                for (int i = 0; i < recipe.Ingredients.Count; i++)
                {
                    content.Add(new StringContent(recipe.Ingredients[i].Name), $"Ingredients[{i}].Name");
                    content.Add(new StringContent(recipe.Ingredients[i].Quantity.ToString()), $"Ingredients[{i}].Quantity");
                    content.Add(new StringContent(recipe.Ingredients[i].Unit), $"Ingredients[{i}].Unit");

                    if (recipe.Ingredients[i].ProductID.HasValue)
                    {
                        content.Add(new StringContent(recipe.Ingredients[i].ProductID.Value.ToString()), $"Ingredients[{i}].ProductID");
                    }
                }

                // Gửi yêu cầu HTTP POST
                var httpResponse = await _httpClient.PostAsync("recipes", content);
                return httpResponse.IsSuccessStatusCode;
            }
        }

        public async Task<IEnumerable<GetRecipeDTO>?> GetOfUser()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetRecipeDTO>>("recipes/user");
        }

        public async Task<IEnumerable<GetRecipeDTO>?> GetByUser(string userId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetRecipeDTO>>("recipes/user/"+userId);
        }
    }
}
