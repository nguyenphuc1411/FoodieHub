using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Service.Interfaces;
using System.Net.Http.Headers;

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
                content.Add(new StringContent(user.ConfirmPassword), "ConfirmPassword");

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
            return await _httpClient.GetFromJsonAsync<PaginatedModel<UserDTO>>("users"+queryString);
        }

        public async Task<IEnumerable<UserDTO>> GetAdmin()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>("users/admins") ?? new List<UserDTO>();
        }

        public async Task<UserDTO?> GetByID(string id)
        {
            return await _httpClient.GetFromJsonAsync<UserDTO>("users/" + id);
        }

        public async Task<bool> Restore(string id)
        {
            var response = await _httpClient.PatchAsync($"users/restore/{id}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SetRole(SetRoleDTO roleDTO)
        {
            var response = await _httpClient.PutAsJsonAsync($"users", roleDTO);
            return response.IsSuccessStatusCode;
        }
    }
}
