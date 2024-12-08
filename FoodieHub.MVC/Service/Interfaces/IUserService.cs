using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IUserService
    {
        Task<bool> Restore(string id);
        Task<bool> Disable(string id);

        Task<PaginatedModel<UserDTO>?> Get(QueryUserModel query);

        Task<APIResponse> Create(CreateUserDTO createUserDTO);

        Task<UserDTO?> GetByID(string id);
    }
}
