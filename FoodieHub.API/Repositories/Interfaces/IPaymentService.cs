using FoodieHub.API.Models.DTOs;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> Create(PaymentDTO payment);
    }
}
