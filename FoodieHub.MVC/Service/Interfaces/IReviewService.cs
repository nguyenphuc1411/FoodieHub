using FoodieHub.MVC.Models.Product;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IReviewService
    {
        Task<APIResponse<List<GetOrderDetailsByProductIdDTO>>> GetListReview(int id);
        Task<APIResponse> AddNewReview(ReviewDTO review);
        Task<APIResponse> UpdateNewReview(UpdateReviewDTO review);
        Task<APIResponse> DeleteNewReview(int id);
    }
}
