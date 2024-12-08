using FoodieHub.API.Models.DTOs.Authentication;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IMailService
    {
       Task<bool> SendEmailAsync(MailRequest mailRequest);
    }
}
