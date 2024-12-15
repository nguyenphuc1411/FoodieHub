using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.MVC.Models.Recipe;
using System.Net.Http.Headers;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Helpers;

namespace FoodieHub.MVC.Service.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly HttpClient _httpClient;

        public RecipeService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
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
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetRecipeDTO>>("recipes/users");
        }

        public async Task<IEnumerable<GetRecipeDTO>?> GetByUser(string userId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetRecipeDTO>>("recipes/users/"+userId);
        }

        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync("recipes/" + id);
            return response.IsSuccessStatusCode;
        }

        public async Task<DetailRecipeDTO?> GetByID(int id)
        {
            return await _httpClient.GetFromJsonAsync<DetailRecipeDTO>("recipes/" + id);
        }

        public async Task<PaginatedModel<GetRecipeDTO>?> GetAll(QueryRecipeModel query)
        {
            var queryString = query.ToQueryString();
            return await _httpClient.GetFromJsonAsync<PaginatedModel<GetRecipeDTO>>("recipes"+queryString);
        }

        public async Task<bool> Update(UpdateRecipeDTO recipeDTO)
        {
            // Create a multipart form data content
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(recipeDTO.RecipeID.ToString()), nameof(recipeDTO.RecipeID));
            content.Add(new StringContent(recipeDTO.Title), nameof(recipeDTO.Title));
            content.Add(new StringContent(recipeDTO.Description ?? ""), nameof(recipeDTO.Description));
            content.Add(new StringContent(recipeDTO.CookTime.ToString()), nameof(recipeDTO.CookTime));
            content.Add(new StringContent(recipeDTO.Serves.ToString()), nameof(recipeDTO.Serves));
            content.Add(new StringContent(recipeDTO.IsActive.ToString()), nameof(recipeDTO.IsActive));
            content.Add(new StringContent(recipeDTO.CategoryID.ToString()), nameof(recipeDTO.CategoryID));

            // Handle the optional file field (main recipe image)
            if (recipeDTO.File != null)
            {
                var fileContent = new StreamContent(recipeDTO.File.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(recipeDTO.File.ContentType);
                content.Add(fileContent, nameof(recipeDTO.File), recipeDTO.File.FileName);
            }

            // Handle steps and step images
            for (int i = 0; i < recipeDTO.RecipeSteps.Count; i++)
            {
                var step = recipeDTO.RecipeSteps[i];
                content.Add(new StringContent(step.Id.ToString()), $"RecipeSteps[{i}].Id");
                content.Add(new StringContent(step.Step.ToString()), $"RecipeSteps[{i}].Step");
                content.Add(new StringContent(step.Directions), $"RecipeSteps[{i}].Directions");

                if (step.FileStep != null)
                {
                    var stepFileContent = new StreamContent(step.FileStep.OpenReadStream());
                    stepFileContent.Headers.ContentType = new MediaTypeHeaderValue(step.FileStep.ContentType);
                    content.Add(stepFileContent, $"RecipeSteps[{i}].FileStep", step.FileStep.FileName);
                }
            }

            // Handle ingredients
            for (int i = 0; i < recipeDTO.Ingredients.Count; i++)
            {
                var ingredient = recipeDTO.Ingredients[i];
                content.Add(new StringContent(ingredient.Id.ToString()), $"Ingredients[{i}].Id");
                content.Add(new StringContent(ingredient.Name), $"Ingredients[{i}].Name");
                content.Add(new StringContent(ingredient.Quantity.ToString()), $"Ingredients[{i}].Quantity");
                content.Add(new StringContent(ingredient.Unit), $"Ingredients[{i}].Unit");
                if (ingredient.ProductID.HasValue)
                {
                    content.Add(new StringContent(ingredient.ProductID.Value.ToString()), $"Ingredients[{i}].ProductID");
                }
            }

            var response = await _httpClient.PutAsync("recipes/"+recipeDTO.RecipeID, content);

            // Check if the response indicates success
            return response.IsSuccessStatusCode;
        }
    }
}
