using FoodieHub.API.Models.DTOs.Order;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IOrderService
    {
        Task<PaginatedModel<GetOrder>> Get(QueryOrderModel queryOrder);
        Task<List<RecentlyOrder>> GetRecently();
        Task<PaginatedModel<GetOrder>> GetByUser(QueryOrderModel queryOrder);
        Task<ServiceResponse> Create(OrderDTO order);
        Task<GetDetailOrder?> GetByID(int orderID);
        Task<ServiceResponse> ChangeStatus(int orderID,string status);

        Task<ServiceResponse> GetOrderWithUserId();

        Task<ServiceResponse> GetOrderDetailsWithProductId();
        Task<ServiceResponse> ChangeStatusUser(int orderID, string status, string? cancellationReason = null);
    }
}
