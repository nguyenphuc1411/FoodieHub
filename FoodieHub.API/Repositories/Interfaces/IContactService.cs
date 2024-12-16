using FoodieHub.API.Models.DTOs.Contact;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IContactService
    {
        Task<ServiceResponse> AddContact(ContactDTO contact);
        Task<ServiceResponse> Get();
        Task<ServiceResponse> ToggleIsRead(int Id);
    }
}
