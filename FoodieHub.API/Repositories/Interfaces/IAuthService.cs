using FoodieHub.API.Data;
using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse> Login(LoginDTO login);

        Task<ServiceResponse> AdminLogin(LoginDTO login);

        Task<ServiceResponse> Register(RegisterDTO register);

        Task<string> ConfirmRegistion(ConfirmRegistion confirmRegistion);

        Task<ServiceResponse> UpdateUser(UpdateProfileDTO user);

        Task<ServiceResponse> RequestForgotPassword(ForgotPasswordDTO forgotPassword);

        Task<bool> ResetPassword(ResetPasswordDTO resetPassword);

        Task<ServiceResponse> ChangePassword(ChangePassword changePassword);
        Task<string> GoogleCallback();


        // Lấy thông tin User thông quan HttpContext đã xác thực token
        string GetUserID();
        Task<UserDTO?> GetCurrentUserDTO();
        Task<ApplicationUser?> GetCurrentUser();

        Task<bool> IsAdmin(string userID);
    }
}
