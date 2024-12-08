using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.User;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;

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
    }
}
