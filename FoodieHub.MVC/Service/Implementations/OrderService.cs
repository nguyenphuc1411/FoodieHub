using FoodieHub.MVC.Models.Order;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Helpers;
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

        public async Task<APIResponse?> ChangeStatus(int orderID,string status,string cancellationReason)
        {
            var response = await _httpClient.PatchAsync($"Orders/ChangeStatusUser/{orderID}?status={status}&cancellationReason={cancellationReason}", null);
            return await response.Content.ReadFromJsonAsync<APIResponse>();
        }
        public async Task<APIResponse?> ChangeStatusForAdmin(int orderID,string status)
        {
            var response = await _httpClient.PatchAsync($"Orders/{orderID}?status={status}", null);
            return await response.Content.ReadFromJsonAsync<APIResponse>();
        }

        public async Task<PaginatedModel<GetOrder>?> Get(QueryOrderModel queryOrderModel)
        {
            var queryString = queryOrderModel.ToQueryString();
            var response = await _httpClient.GetAsync("orders"+queryString);
            return await response.Content.ReadFromJsonAsync<PaginatedModel<GetOrder>>();
        }

        public async Task<GetDetailOrder?> GetByID(int id)
        {
            var response = await _httpClient.GetAsync($"orders/{id}");
            return await response.Content.ReadFromJsonAsync<GetDetailOrder>();
        }
        public async Task<PaginatedModel<GetOrder>?> GetForAdmin(QueryOrderModel queryOrder)
        {
            var queryString = queryOrder.ToQueryString();
            var response = await _httpClient.GetAsync($"Orders{queryString}");

            return await response.Content.ReadFromJsonAsync<PaginatedModel<GetOrder>>();
        }
        public async Task<PaginatedModel<GetOrder>?> GetForUser(QueryOrderModel queryOrder)
        {
            var queryString = queryOrder.ToQueryString();
            var response = await _httpClient.GetAsync($"Orders/ordered{queryString}");

            return await response.Content.ReadFromJsonAsync<PaginatedModel<GetOrder>>();
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
                    };
                }
            }
            else
            {
                return new APIResponse<List<GetOrderDetailsByProductIdDTO>>
                {
                    Success = false,
                    Message = $"Error when calling the API: {response.StatusCode}"
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
                        Message = "The returned data is empty."
                    };
                }
            }
            else
            {
                return new APIResponse<List<GetOrderByUserIdDTO>>
                {
                    Success = false,
                    Message = $"Error when calling the API: {response.StatusCode}"
                };
            }
        }
       

    }
    

}
