using FoodieHub.MVC.Models.Order;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Service.Interfaces;
using System.Net.Http;

namespace FoodieHub.MVC.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }
        public async Task<APIResponse<List<GetOrderDetailsByProductIdDTO>>> GetOrderDetailsWithProductID()
        {
            var response = await _httpClient.GetAsync("Orders/OrderDetailsByProductId");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<List<GetOrderDetailsByProductIdDTO>>>();

                if (content?.Data != null)
                {
                    return new APIResponse<List<GetOrderDetailsByProductIdDTO>>
                    {
                        Success = true,
                        Message = "Successfully retrieved the order list.",
                        Data = content.Data
                    };
                }
                else
                {
                    return new APIResponse<List<GetOrderDetailsByProductIdDTO>>
                    {
                        Success = false,
                        Message = "The returned data is empty.",
                        Data = null
                    };
                }
            }
            else
            {
                return new APIResponse<List<GetOrderDetailsByProductIdDTO>>
                {
                    Success = false,
                    Message = $"Error when calling the API: {response.StatusCode}",
                    Data = null
                };
            }
        }


        public async Task<APIResponse<List<GetOrderByUserIdDTO>>> GetOrderWithUserId()
        {
            var response = await _httpClient.GetAsync("Orders/OrderByUserId");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<List<GetOrderByUserIdDTO>>>();

                if (content?.Data != null)
                {
                    return new APIResponse<List<GetOrderByUserIdDTO>>
                    {
                        Success = true,
                        Message = "Successfully retrieved the order list.",
                        Data = content.Data
                    };
                }
                else
                {
                    return new APIResponse<List<GetOrderByUserIdDTO>>
                    {
                        Success = false,
                        Message = "The returned data is empty.",
                        Data = null
                    };
                }
            }
            else
            {
                return new APIResponse<List<GetOrderByUserIdDTO>>
                {
                    Success = false,
                    Message = $"Error when calling the API: {response.StatusCode}",
                    Data = null
                };
            }
        }
       

    }
    

}
