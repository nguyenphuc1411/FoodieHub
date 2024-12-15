using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Helpers;
using System.Net.Http.Headers;

namespace FoodieHub.MVC.Service.Implementations
{
    public class ArticleService : IArticleService
    {
        private readonly HttpClient _httpClient;

        public ArticleService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<bool> Create(CreateArticleDTO article)
        {
            using (var content = new MultipartFormDataContent())
            {
                // Thêm các thông tin khác của Article
                content.Add(new StringContent(article.Title), "Title");
                content.Add(new StringContent(article.Description), "Description");
                content.Add(new StringContent(article.CategoryID.ToString()), "CategoryID");
                content.Add(new StringContent(article.IsActive.ToString()), "IsActive");

                var fileContent = new StreamContent(article.File.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(article.File.ContentType);
                content.Add(fileContent, "File", article.File.FileName);

                var httpResponse = await _httpClient.PostAsync("articles", content);

                return httpResponse.IsSuccessStatusCode;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync("articles/" + id);
            return response.IsSuccessStatusCode;
        }

        public async Task<PaginatedModel<GetArticleDTO>> Get(QueryArticleModel query)
        {
            var queryString = query.ToQueryString();
            var response = await _httpClient.GetAsync("Articles"+queryString);
            return await response.Content.ReadFromJsonAsync<PaginatedModel<GetArticleDTO>>() ?? new PaginatedModel<GetArticleDTO>();
        }

        public async Task<IEnumerable<ArticleByCategory>> GetByCategory()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ArticleByCategory>>("articles/categories") ?? new List<ArticleByCategory>();
        }

        public async Task<GetArticleDTO?> GetByID(int id)
        {
            return await _httpClient.GetFromJsonAsync<GetArticleDTO>("articles/" + id);
        }

        public async Task<IEnumerable<GetArticleDTO>> GetOfUser(string userID)
        {
            var response = await _httpClient.GetAsync("articles/users/"+userID);
            return await response.Content.ReadFromJsonAsync<IEnumerable<GetArticleDTO>>() ?? new List<GetArticleDTO>();
        }

        public async Task<bool> Update(int id, UpdateArticleDTO article)
        {
            using (var content = new MultipartFormDataContent())
            {
                // Thêm các thông tin khác của Article
                content.Add(new StringContent(article.ArticleID.ToString()), "ArticleID");
                content.Add(new StringContent(article.Title), "Title");
                content.Add(new StringContent(article.Description), "Description");
                content.Add(new StringContent(article.CategoryID.ToString()), "CategoryID");
                content.Add(new StringContent(article.IsActive.ToString()), "IsActive");
                if (article.File != null)
                {
                    var fileContent = new StreamContent(article.File.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(article.File.ContentType);
                    content.Add(fileContent, "File", article.File.FileName);
                }
                var httpResponse = await _httpClient.PutAsync($"articles/{id}", content);

                return httpResponse.IsSuccessStatusCode;
            }
        }
    }
}
