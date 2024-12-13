using FoodieHub.MVC.Models.Authentication;
using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http;

namespace FoodieHub.MVC.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient httpClient;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<UserDTO?> GetProfile()
        {
            return await httpClient.GetFromJsonAsync<UserDTO>("auth/profile");
        }

        public async Task<APIResponse> Login(LoginDTO loginVM)
        {
            var httpResponse = await httpClient.PostAsJsonAsync("auth/login", loginVM);
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
            return data ?? new APIResponse { Success = false,Message = "Server error. Please try again later." };
        }

        public async Task<APIResponse> Register(RegisterDTO registerVM)
        {
            var httpResponse = await httpClient.PostAsJsonAsync("auth/register", registerVM);

            return await httpResponse.Content.ReadFromJsonAsync<APIResponse>() ?? new APIResponse { Success = false, Message = "Server error. Please try again later." };
        }

        public async Task<APIResponse> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            var httpResponse = await httpClient.PostAsJsonAsync("auth/request-forgot-password", forgotPasswordDTO);

            return await httpResponse.Content.ReadFromJsonAsync<APIResponse>() ?? new APIResponse { Success = false, Message = "Server error. Please try again later." };
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var httpResponse = await httpClient.PostAsJsonAsync("auth/reset-password", resetPasswordDTO);

            return httpResponse.IsSuccessStatusCode;
        }

        public async Task<string?> ConfirmRegistion(ConfirmRegistion confirmRegistion)
        {
            var response = await httpClient.PostAsJsonAsync("auth/confirm-registion", confirmRegistion);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

        public async Task<APIResponse?> UpdateProfile(UpdateProfileDTO user)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(user.Fullname), "Fullname"); 
                content.Add(new StringContent(user.Email), "Email");

                if (!string.IsNullOrEmpty(user.Bio))
                {
                    content.Add(new StringContent(user.Bio), "Bio");
                }
                if (!string.IsNullOrEmpty(user.OldPassword) && !string.IsNullOrEmpty(user.NewPassword))
                {
                    content.Add(new StringContent(user.OldPassword), "OldPassword");
                    content.Add(new StringContent(user.NewPassword), "NewPassword");
                }
                if (user.File != null && user.File.Length > 0)
                {
                    var fileContent = new StreamContent(user.File.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(user.File.ContentType);
                    content.Add(fileContent, "File", user.File.FileName);
                }
                var httpResponse = await httpClient.PutAsync("auth", content);
                return await httpResponse.Content.ReadFromJsonAsync<APIResponse>();               
            }
        }

        public async Task<APIResponse> AdminLogin(LoginDTO login)
        {
            var httpResponse = await httpClient.PostAsJsonAsync("auth/admin/login", login);
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
            return data ?? new APIResponse { Success = false, Message = "Server error. Please try again later." };
        }    
    }
}
