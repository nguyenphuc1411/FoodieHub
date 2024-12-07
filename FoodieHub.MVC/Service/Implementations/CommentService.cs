using FoodieHub.MVC.Models.Comment;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodieHub.MVC.Service.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;

        public CommentService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        /*public async Task<APIResponse<List<GetComment>>> GetCommentsOfArticleAsync(int articleID, string order = "asc")
        {
            var response = await _httpClient.GetAsync($"comments/article/{articleID}?order={order}");
            return await response.Content.ReadFromJsonAsync<APIResponse<List<GetComment>>>();
        }*/
        public async Task<APIResponse<List<GetComment>>> GetCommentsOfArticleAsync(int articleID, string order = "asc")
        {
            var response = await _httpClient.GetAsync($"comments/article/{articleID}?order={order}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<APIResponse<List<GetComment>>>();
                // Đảm bảo UserID được map từ API response
                return result;
            }

            return new APIResponse<List<GetComment>>
            {
                Success = false,
                Message = "Failed to fetch comments",
                Data = new List<GetComment>()
            };
        }
        /*  public async Task<APIResponse> DeleteCommentAsync(int commentID, string type)
          {
              var response = await _httpClient.DeleteAsync($"comments/{commentID}?type={type}");
              return await response.Content.ReadFromJsonAsync<APIResponse>();
          }*/
        public async Task<APIResponse> DeleteCommentAsync(int commentID, string type)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"comments/{commentID}?type={type}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<APIResponse>();
                }

                return new APIResponse
                {
                    Success = false,
                    Message = "Failed to delete comment"
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
        public async Task<APIResponse> CreateArticleCommentAsync(CreateArticleComment comment)
        {
            var response = await _httpClient.PostAsJsonAsync("comments/article", comment);
            return await response.Content.ReadFromJsonAsync<APIResponse>();
        }
    }
}