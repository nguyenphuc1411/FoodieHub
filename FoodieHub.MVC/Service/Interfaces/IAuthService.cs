using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.User;
using FoodieHub.MVC.Models.Response;
namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IAuthService
    {
        Task<UserDTO?> GetProfile();
        Task<APIResponse> Login(LoginDTO loginVM);  
        Task<APIResponse> Register(RegisterDTO registerVM);
        Task<APIResponse> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
        Task<bool> ResetPassword(ResetPasswordDTO resetPasswordDTO);

        Task<string?> ConfirmRegistion(ConfirmRegistion confirmRegistion);
    }
}
