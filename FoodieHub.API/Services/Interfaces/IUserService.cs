using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.Response;
using System.Threading.Tasks;

namespace FoodieHub.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse> Get(string? role,string? email, int pageSize, int currentPage);    
        Task<ServiceResponse> Create(CreateUser createUser);
        Task<ServiceResponse> Disable(string id);
        Task<ServiceResponse> Restore(string id);
        Task<ServiceResponse> GetByID(string id);
    }
}
