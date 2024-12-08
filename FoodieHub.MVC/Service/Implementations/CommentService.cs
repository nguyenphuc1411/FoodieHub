using FoodieHub.API.Models.DTOs.Comment;
using FoodieHub.MVC.Models.Comment;
using FoodieHub.MVC.Service.Interfaces;

namespace FoodieHub.MVC.Service.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;

        public CommentService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentRecipe(int id)
        {
            var response = await _httpClient.GetAsync($"comments/recipes/{id}");

            if (response.IsSuccessStatusCode)
            {
                var comments = await response.Content.ReadFromJsonAsync<IEnumerable<CommentDTO>>();
                return comments ?? Enumerable.Empty<CommentDTO>();
            }
            else
            {
                throw new HttpRequestException($"Error fetching comments: {response.StatusCode}");
            };
        }
        public async Task<IEnumerable<CommentDTO>> GetCommentArticle(int id)
        {
            var response = await _httpClient.GetAsync($"comments/articles/{id}");

            if (response.IsSuccessStatusCode)
            {
                var comments = await response.Content.ReadFromJsonAsync<IEnumerable<CommentDTO>>();
                return comments ?? Enumerable.Empty<CommentDTO>();
            }
            else
            {
                throw new HttpRequestException($"Error fetching comments: {response.StatusCode}");
            };
        }
        public async Task<bool> Delete(int id)
        {          
            var response = await _httpClient.DeleteAsync($"comments/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> Create(CreateCommentDTO comment)
        {
            var response = await _httpClient.PostAsJsonAsync("comments", comment);
            return response.IsSuccessStatusCode;
        }


    }
}