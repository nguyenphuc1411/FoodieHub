using FoodieHub.MVC.Models.Cart;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using FoodieHub.MVC.Service.Interfaces;

namespace FoodieHub.MVC.Views.Cart
{
    public class CartViewComponent: ViewComponent
    {
        private readonly IProductService _service;

        public CartViewComponent(IProductService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartItemsJson = HttpContext.Request.Cookies["cart"] ?? "[]";
            var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();
            var getCart = new List<GetCartDTO>();
            bool isExceedingStock = false;

            foreach (var item in cartItems)
            {
                var response = await _service.GetById(item.ProductID);
                if (response.Data!=null && response.Success)
                {
                    var product = response.Data;

                    if (item.Quantity > product.StockQuantity)
                    {
                        item.Quantity = product.StockQuantity;
                        isExceedingStock = true;
                    }

                    getCart.Add(new GetCartDTO
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        MainImage = product.MainImage,
                        Quantity = item.Quantity,
                        Discount = product.Discount,
                        StockQuantity = product.StockQuantity
                    });
                }
            }

            int distinctOrderCount = cartItems.Count;
            ViewBag.slOrder = distinctOrderCount.ToString();
            ViewBag.IsExceedingStock = isExceedingStock;
            return View(getCart);
        }
    }
}
