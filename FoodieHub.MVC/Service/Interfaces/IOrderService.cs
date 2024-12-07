using FoodieHub.MVC.Models.Order;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.User;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IOrderService
    {
        Task<APIResponse<List<GetOrderDetailsByProductIdDTO>>> GetOrderDetailsWithProductID();
        Task<APIResponse<List<GetOrderByUserIdDTO>>> GetOrderWithUserId();

        
    }
}
