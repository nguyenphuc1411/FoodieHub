using System.Net.Http.Json;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Service.Interfaces;

namespace FoodieHub.MVC.Service.Implementations
{
    public class ArticleService : IArticleService
    {
        private readonly HttpClient _httpClient;

        public ArticleService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<IEnumerable<GetArticle>> GetAll(string? search, int? pageSize, int? currentPage)
        {
            var response = await _httpClient.GetAsync("Articles");

            if (response.IsSuccessStatusCode)
            {
                // Sử dụng APIResponse<List<GetArticle>> để chuyển đổi phản hồi JSON
                var content = await response.Content.ReadFromJsonAsync<APIResponse<List<GetArticle>>>();

                if (content != null && content.Success)
                {
                    return content.Data.OrderByDescending(x=>x.CreatedAt).ToList() ?? new List<GetArticle>();
                }
            }

            return new List<GetArticle>();
        }

        public async Task<APIResponse<GetArticleDetail>> GetDetail(int id)
        {
            var response = await _httpClient.GetAsync($"Articles/{id}");

            if (response.IsSuccessStatusCode)
            {
                // Sử dụng APIResponse<GetArticleDetail> cho phản hồi chi tiết bài viết
                var content = await response.Content.ReadFromJsonAsync<APIResponse<GetArticleDetail>>();

                if (content != null)
                {
                    return new APIResponse<GetArticleDetail>
                    {
                        Success = content.Success,
                        Message = content.Message,
                        Data = content.Data,
                        StatusCode = (int)response.StatusCode
                    };
                }
            }

            return new APIResponse<GetArticleDetail>
            {
                Success = false,
                Message = "Failed to retrieve article by ID.",
                Data = null,
                StatusCode = (int)response.StatusCode
            };
        }

    }
}
