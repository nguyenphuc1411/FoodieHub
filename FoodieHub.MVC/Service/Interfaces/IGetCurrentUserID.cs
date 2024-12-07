using FoodieHub.MVC.Models.User;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IGetCurrentUserID
    {
        Task<UserDTO> GetCurrentUserId();
    }
}
