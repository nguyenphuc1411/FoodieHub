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

        public async Task<IEnumerable<GetCommentDTO>> GetCommentRecipe(int id)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetCommentDTO>>($"comments/recipes/{id}") ?? new List<GetCommentDTO>();        
        }
        public async Task<IEnumerable<GetCommentDTO>> GetCommentArticle(int id)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetCommentDTO>>($"comments/articles/{id}") ?? new List<GetCommentDTO>();
        }
        public async Task<bool> Delete(int id)
        {          
            var response = await _httpClient.DeleteAsync($"comments/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> Create(CommentDTO comment)
        {
            var response = await _httpClient.PostAsJsonAsync("comments", comment);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Edit(int id, CommentDTO comment)
        {
            var response = await _httpClient.PutAsJsonAsync($"comments/{id}", comment);
            return response.IsSuccessStatusCode;
        }
    }
}