using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Models.QueryModel;
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

        Task<IEnumerable<UserDTO>> GetAdmin();
        Task<bool> SetRole(SetRoleDTO roleDTO);
    }
}
