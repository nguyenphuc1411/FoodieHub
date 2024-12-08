using FoodieHub.MVC.Models.DTOs;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using System.Net.Http;
using System.Text.Json;

namespace FoodieHub.MVC.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }


        public async Task<IEnumerable<CategoryDTO>> GetAll()
        {

            var response = await _httpClient.GetAsync("Categories");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<IEnumerable<CategoryDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return categories;
            }
            else
            {
                throw new Exception("Failed to retrieve categories from API");
            }
        }

        public async Task<APIResponse> AddNewProductCategory(CategoryDTO categoryDTO)
        {


            var httpResponse = await _httpClient.PostAsJsonAsync("Categories", categoryDTO);

            if (httpResponse.IsSuccessStatusCode)
            {
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                return apiResponse;
            }
            else
            {
                return new APIResponse
                {
                    Success = false,
                    Message = "Failed to add new product category.",
                    StatusCode = (int)httpResponse.StatusCode
                };
            }
        }

        public async Task<APIResponse> DeleteProductCategory(int id)
        {


            var httpResponse = await _httpClient.DeleteAsync($"Categories/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                return apiResponse;
            }
            else
            {
                return new APIResponse
                {
                    Success = false,
                    Message = $"Failed to delete product category with ID {id}.",
                    StatusCode = (int)httpResponse.StatusCode
                };
            }
        }

        public async Task<APIResponse> UpdateProductCategory(CategoryDTO categoryDTO)
        {


            var httpResponse = await _httpClient.PutAsJsonAsync("Categories", categoryDTO);

            if (httpResponse.IsSuccessStatusCode)
            {
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                return apiResponse;
            }
            else
            {
                return new APIResponse
                {
                    Success = false,
                    Message = $"Failed to update product category with ID {categoryDTO.CategoryID}.",
                    StatusCode = (int)httpResponse.StatusCode
                };
            }
        }

        public async Task<CategoryDTO> GetProductCategoryById(int id)
        {

            var httpResponse = await _httpClient.GetAsync($"Categories/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var category = await httpResponse.Content.ReadFromJsonAsync<CategoryDTO>();
                return category;  
            }
            else
            {
                throw new Exception("Failed to retrieve product category.");
            }
        }


    }
}
