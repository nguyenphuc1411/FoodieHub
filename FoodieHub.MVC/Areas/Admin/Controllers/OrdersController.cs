using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Configurations;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Models.DTOs.Order;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class OrdersController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        public OrdersController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        // GET: Orders
        [HttpGet]
        public async Task<IActionResult> Index(int? pageSize, int? currentPage, DateOnly? orderDate, string? orderKey, bool? isDesc, string? orderStatus)
        {
            pageSize ??= 10;
            currentPage ??= 1;

            var client = _httpClientFactory.CreateClient("MyAPI");

            var url = $"Orders?pageSize={pageSize}&currentPage={currentPage}" +
                      $"{(orderDate.HasValue ? $"&orderDate={orderDate.Value:yyyy-MM-dd}" : "")}" +
                      $"{(!string.IsNullOrEmpty(orderKey) ? $"&orderKey={orderKey}" : "")}" +
                      $"{(isDesc.HasValue ? $"&isDesc={isDesc.Value.ToString().ToLower()}" : "")}" +
                      $"{(!string.IsNullOrEmpty(orderStatus) ? $"&status={orderStatus}" : "")}";

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<PaginatedModel<GetOrder>>>();
                var orderList = content?.Data.Items;

                int totalOrders = content?.Data.TotalItems ?? 0;
                int totalPages = content?.Data.TotalPages ?? 0;

                ViewData["CurrentPage"] = currentPage;
                ViewData["TotalPages"] = totalPages;
                ViewData["PageSize"] = pageSize;
                ViewData["OrderKey"] = orderKey;
                ViewData["OrderDate"] = orderDate?.ToString("yyyy-MM-dd");
                ViewData["SelectedStatus"] = orderStatus;

                ViewData["IsOrderedDateDescending"] = isDesc.HasValue && orderKey == "ORDEREDDATE" && isDesc.Value;
                ViewData["IsTotalAmountDescending"] = isDesc.HasValue && orderKey == "PRICE" && isDesc.Value;

                if (orderList == null)
                {
                    TempData["ErrorMessage"] = "No orders found.";
                    return View(new List<GetOrder>());
                }

                return View(orderList);
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to load order list from API.";
                return View(new List<GetOrder>());
            }
        }


        public async Task<IActionResult> Details(string id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync($"Orders/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<GetDetailOrder>>();
                var orderDetails = content?.Data;

                if (orderDetails == null)
                {
                    TempData["ErrorMessage"] = content.Message;
                    return RedirectToAction("Index");
                }

                // Assign possible statuses

                return View(orderDetails); // Return view with order details
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to load order details from API.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string orderId, string status)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(status))
            {
                TempData["ErrorMessage"] = "Invalid order ID or status.";
                return RedirectToAction("Details", new { id = orderId });
            }

            var client = _httpClientFactory.CreateClient("MyAPI");

            // Send PATCH request with status as a query parameter
            var response = await client.PatchAsync($"Orders/{orderId}?status={status}", null);
            var data = await response.Content.ReadFromJsonAsync<APIResponse>();
            if (data.Success)
            {
                TempData["SuccessMessage"] = data?.Message ?? "Status updated successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Unable to update status. Error details: {errorDetails}";
            }

            return RedirectToAction("Details", new { id = orderId });
        }
    }
}
