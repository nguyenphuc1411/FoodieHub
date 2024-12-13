using FoodieHub.MVC.Models.DTOs;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using System.Text.Json;

namespace FoodieHub.MVC.Service.Implementations
{
    public class ArticleCategoryService : IArticleCategoryService
    {
        private readonly HttpClient _httpClient;

        public ArticleCategoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<APIResponse> AddArticleCategory(ArticleCategoryDTO articleCategoryDTO)
        {
            var response = await _httpClient.PostAsJsonAsync($"ArticleCategories", articleCategoryDTO);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse>();
                return content;
            }

            return new APIResponse
            {
                Success = false,
                Message = "Failed to add article category"
            };
        }

        public async Task<IEnumerable<ArticleCategoryDTO>> GetAll()
        {
            var response = await _httpClient.GetAsync($"ArticleCategories");
            return await response.Content.ReadFromJsonAsync<IEnumerable<ArticleCategoryDTO>>() ?? new List<ArticleCategoryDTO>();
        }

        public async Task<APIResponse> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"ArticleCategories/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<ArticleCategoryDTO>>();

                return new APIResponse
                {
                    Success = true,
                    Message = "Category retrieved successfully.",
                    StatusCode = 200,
                    Data = content.Data
                };
            }
            else
            {
                return new APIResponse
                {
                    Success = false,
                    Message = "Failed to retrieve category.",
                    StatusCode = (int)response.StatusCode
                };
            }
        }


        public async Task<APIResponse> UpdateArticleCategory(ArticleCategoryDTO articleCategoryDTO)
        {

            // Serialize articleCategoryDTO thành JSON
            var jsonContent = JsonContent.Create(articleCategoryDTO);

            // Gửi yêu cầu PUT
            var response = await _httpClient.PutAsync("ArticleCategories", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse>();
                return content;
            }

            return new APIResponse
            {
                Success = false,
                Message = "Failed to update article category"
            };
        }


    }
}
