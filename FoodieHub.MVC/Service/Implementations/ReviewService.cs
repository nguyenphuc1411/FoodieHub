using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Product;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;

namespace FoodieHub.MVC.Service.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly HttpClient _httpClient;

        public ReviewService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }
        public async Task<APIResponse<List<GetOrderDetailsByProductIdDTO>>> GetListReview(int id)
        {
            var response = await _httpClient.GetAsync($"Review/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<List<GetOrderDetailsByProductIdDTO>>>();

                return new APIResponse<List<GetOrderDetailsByProductIdDTO>>
                {
                    Success = true,
                    Message = "Lấy danh sách review thành công.",
                    Data = content?.Data // Lấy Data từ phản hồi API
                };
            }

            return new APIResponse<List<GetOrderDetailsByProductIdDTO>>
            {
                Success = false,
                Message = $"Lỗi: {response.StatusCode}",
                Data = null // Không có dữ liệu khi lỗi
            };
        }


        public async Task<APIResponse> AddNewReview(ReviewDTO review)
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("Review", review);

            if (httpResponse.IsSuccessStatusCode)
            {
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                return apiResponse;
            }
            else
            {
                return new APIResponse
                {
                    Success = false,
                    Message = "Failed to add new product category.",
                    StatusCode = (int)httpResponse.StatusCode
                };
            }
        }

        public async Task<APIResponse> DeleteNewReview(int id)
        {
            var httpResponse = await _httpClient.DeleteAsync($"Review/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                return apiResponse;
            }
            else
            {
                return new APIResponse
                {
                    Success = false,
                    Message = $"Failed to delete product category with ID {id}.",
                    StatusCode = (int)httpResponse.StatusCode
                };
            }
        }

       

        public async Task<APIResponse> UpdateNewReview(UpdateReviewDTO review)
        {
            var httpResponse = await _httpClient.PutAsJsonAsync("Review", review);

            if (httpResponse.IsSuccessStatusCode)
            {
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                return apiResponse;
            }
            else
            {
                return new APIResponse
                {
                    Success = false,
                    Message = $"Failed to update product category with ID {review.ReviewID}.",
                    StatusCode = (int)httpResponse.StatusCode
                };
            }
        }
    }
}
