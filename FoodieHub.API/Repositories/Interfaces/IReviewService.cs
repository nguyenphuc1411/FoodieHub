using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Review;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IReviewService
    {
        public Task<ServiceResponse> GetListReview(int id);
        Task<ServiceResponse> AddNewReview(ReviewDTO review);
        Task<ServiceResponse> UpdateReview(ReviewDTO review);
        Task<ServiceResponse> DeleteNewReview(int id);
    }
}
