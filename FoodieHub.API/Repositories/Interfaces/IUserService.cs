using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using System.Threading.Tasks;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedModel<UserDTO>> Get(QueryUserModel query);    
        Task<ServiceResponse> Create(CreateUserDTO createUser);
        Task<bool> Disable(string id);
        Task<bool> Restore(string id);
        Task<ServiceResponse> GetByID(string id);
    }
}
