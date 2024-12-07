using FoodieHub.API.Models.DTOs.Order;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResponse> Get(int? pageSize, int? currentPage, DateOnly? orderDate, string? orderKey, bool? isDesc, string? status);
        Task<List<RecentlyOrder>> GetRecently();
        Task<ServiceResponse> GetByUser(int? pageSize, int? currentPage);
        Task<ServiceResponse> Create(OrderDTO order);
        Task<ServiceResponse> GetDetail(int orderID);
        Task<ServiceResponse> ChangeStatus(int orderID,string status);

        Task<ServiceResponse> GetOrderWithUserId();

        Task<ServiceResponse> GetOrderDetailsWithProductId();
        Task<ServiceResponse> ChangeStatusUser(int orderID, string status, string? cancellationReason = null);
    }
}
