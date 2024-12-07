using FoodieHub.API.Models.DTOs.Authentication;

namespace FoodieHub.API.Services.Interfaces
{
    public interface IMailService
    {
       Task<bool> SendEmailAsync(MailRequest mailRequest);
    }
}
