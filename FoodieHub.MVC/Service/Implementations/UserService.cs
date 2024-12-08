using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using static QRCoder.PayloadGenerator;

namespace FoodieHub.MVC.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<APIResponse> Create(CreateUserDTO user)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(user.Fullname), "Fullname");
                content.Add(new StringContent(user.Email), "Email");

                if (!string.IsNullOrEmpty(user.Bio))
                {
                    content.Add(new StringContent(user.Bio), "Bio");
                }

                content.Add(new StringContent(user.IsActive.ToString()), "IsActive");
                content.Add(new StringContent(user.Role), "Role");
                content.Add(new StringContent(user.Password), "Password");

                if (user.File != null && user.File.Length > 0)
                {
                    var fileContent = new StreamContent(user.File.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(user.File.ContentType);
                    content.Add(fileContent, "File", user.File.FileName);
                }

                // Send the request to the API
                var httpResponse = await _httpClient.PostAsync("users", content);

                return await httpResponse.Content.ReadFromJsonAsync<APIResponse>() ?? new APIResponse { Success = false, Message = "An error occured." };
            }
        }

        public async Task<bool> Disable(string id)
        {
            var response = await _httpClient.PatchAsync($"users/disable/{id}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<PaginatedModel<UserDTO>?> Get(QueryUserModel query)
        {
            var queryString = query.ToQueryString();
            var httpResponse = await _httpClient.GetAsync($"users{queryString}");
            return await httpResponse.Content.ReadFromJsonAsync<PaginatedModel<UserDTO>>();
        }

        public async Task<bool> Restore(string id)
        {
            var response = await _httpClient.PatchAsync($"users/restore/{id}", null);
            return response.IsSuccessStatusCode;
        }
    }
}
