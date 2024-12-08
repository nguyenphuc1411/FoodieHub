using FoodieHub.API.Models.DTOs.Order;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.MVC.Models.Order;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IOrderService
    {
        Task<APIResponse<List<GetOrderDetailsByProductIdDTO>>> GetOrderDetailsWithProductID();
        Task<APIResponse<List<GetOrderByUserIdDTO>>> GetOrderWithUserId();
        Task<PaginatedModel<GetOrder>?> GetForAdmin(QueryOrderModel queryOrder);
        Task<PaginatedModel<GetOrder>?> GetForUser(QueryOrderModel queryOrder);

        Task<GetDetailOrder?> GetByID(int id);
        Task<APIResponse?> ChangeStatus(int orderID,string status,string cancellationReason);
    }
}
