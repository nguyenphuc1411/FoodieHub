using FoodieHub.MVC.Models.Cart;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;

namespace FoodieHub.MVC.Views.Cart
{
    public class CartViewComponent: ViewComponent
    {
        private readonly HttpClient _httpClient;

        public CartViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartItemsJson = HttpContext.Request.Cookies["cart"] ?? "[]";
            var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();
            var getCart = new List<GetCartDTO>();

            foreach (var item in cartItems)
            {
                var response = await _httpClient.GetAsync($"Products/getproductbyid/{item.ProductID}");
                if (response.IsSuccessStatusCode)
                {
                    var productResponse = await response.Content.ReadFromJsonAsync<APIResponse<GetProductDTO>>();
                    if (productResponse != null && productResponse.Success)
                    {
                        var product = productResponse.Data;
                        getCart.Add(new GetCartDTO
                        {
                            ProductID = product.ProductID,
                            ProductName = product.ProductName,
                            Price = product.Price,
                            MainImage = product.MainImage,
                            Quantity = item.Quantity,
                            Discount = product.Discount
                        });
                    }
                }
            }

            int distinctOrderCount = cartItems.Count;
            ViewBag.slOrder = distinctOrderCount.ToString();
            return View(getCart);
        }
    }
}
