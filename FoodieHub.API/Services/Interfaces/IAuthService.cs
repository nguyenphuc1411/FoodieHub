using FoodieHub.API.Data;
using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse> Login(LoginDTO login);

        Task<ServiceResponse> AdminLogin(LoginDTO login);

        Task<ServiceResponse> Register(RegisterDTO register);

        Task<string> ConfirmRegistion(ConfirmRegistion confirmRegistion);

        Task<ServiceResponse> UpdateUser(UpdateUser user);

        Task<ServiceResponse> RequestForgotPassword(ForgotPasswordDTO forgotPassword);

        Task<ServiceResponse> ResetPassword(ResetPassword resetPassword);

        Task<ServiceResponse> ChangePassword(ChangePassword changePassword);

        Task<ServiceResponse> GetProfile();

        Task<string> GoogleCallback();


        // Lấy thông tin User thông quan HttpContext đã xác thực token
        string GetUserID();
        Task<UserDTO> GetCurrentUserDTO();
        Task<ApplicationUser> GetCurrentUser();
    }
}
