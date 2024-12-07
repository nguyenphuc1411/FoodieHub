using FoodieHub.API.Models.DTOs;

namespace FoodieHub.API.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> Create(PaymentDTO payment);
    }
}
