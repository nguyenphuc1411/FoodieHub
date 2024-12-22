using FoodieHub.MVC.Models.Order;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.Cart;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Recipe;
namespace FoodieHub.MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IVnPayService _vnPayService;
        private readonly HttpClient _httpClient;
        public CartController(IVnPayService vnPayService, IProductService productService, IHttpClientFactory httpClientFactory)
        {
            _vnPayService = vnPayService;
            _productService = productService;
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Checkout()
        {
            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            var getCart = new List<GetCartDTO>();

            
            foreach (var item in cartItems)
            {
                var response = await _productService.GetById(item.ProductID);
                if (response.Data!=null && response.Success)
                {
                    var product = response.Data;
                    
                    getCart.Add(new GetCartDTO
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        Discount = product.Discount,
                        MainImage = product.MainImage,
                        Quantity = item.Quantity,
                        StockQuantity = product.StockQuantity
                    });
                }
            }

            // Lưu thông tin giỏ hàng vào ViewBag
            ViewBag.CartItems = getCart;

            return View();
        }
        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderDTO orderDto)
        {        
            // Get cart items from cookie
            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            // Combine address fields into one
            var province = Request.Form["Province"];
            var district = Request.Form["District"];
            var ward = Request.Form["Ward"];
            var address = Request.Form["ShippingAddress"];

            orderDto.ShippingAddress = $"{address}, {ward}, {district}, {province}";

            foreach (var item in cartItems)
            {            
                orderDto.OrderDetails.Add(new OrderDetailDtO
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity
                });
            }
                      
			var orderResponse = await _httpClient.PostAsJsonAsync("Orders", orderDto);
			var data = await orderResponse.Content.ReadFromJsonAsync<APIResponse<ReponseOrder>>();
			if (data != null && data.Success)
			{
				var orderID = data.Data.OrderID;

				// Clear the cart cookie
				Response.DeleteCookie("cart");
				TempData["SuccessMessage"] = data.Message;

                if (orderDto.PaymentMethod) // thanh toán thẻ
                {
                    return Redirect("/Payment/Orders/" + orderID);
                }


				return Redirect("/Account/OrderDetail/" + orderID);
			}
			else
			{
				if (data != null)
				{
					TempData["ErrorMessage"] = data.Message;
				}
				else
				{
					TempData["ErrorMessage"] = "There was an error placing your order.";
				}
				return RedirectToAction("Checkout");
			}
        }
        [ValidateTokenForUser]
        public IActionResult AddToCart(int productId, int quantity)
        {

            var cartItem = new CartItem
            {
                ProductID = productId,
                Quantity = quantity
            };

            var cartItemsJson = Request.GetCookie("cart") ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            var existingItem = cartItems.Find(item => item.ProductID == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cartItems.Add(cartItem);
            }

            var newCartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);
            Response.SetCookie("cart", newCartItemsJson);
            NotificationHelper.SetSuccessNotification(this, "The product has been added to the cart successfully!");
            var refererUrl = Request.Headers["Referer"].ToString();
            return Redirect(refererUrl ?? Url.Action("Index","Products"));
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> AddOrderItemsToCart(string id)
        {
            var response = await _httpClient.GetAsync($"Orders/{id}");
            var orderDetails = await response.Content.ReadFromJsonAsync<GetDetailOrder>();

            // Lấy giỏ hàng hiện tại từ cookie
            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            // Thêm các sản phẩm từ đơn hàng vào giỏ
            foreach (var detail in orderDetails.ProductForOrder)
            {
                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var existingItem = cartItems.FirstOrDefault(item => item.ProductID == detail.ProductID);
                if (existingItem != null)
                {
                    // Nếu đã có, tăng số lượng
                    existingItem.Quantity += detail.Quantity;
                }
                else
                {
                    // Nếu chưa có, thêm sản phẩm mới vào giỏ
                    cartItems.Add(new CartItem
                    {
                        ProductID = detail.ProductID,
                        Quantity = detail.Quantity
                    });
                }
            }

            // Lưu lại giỏ hàng vào cookie
            var newCartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);

            Response.Cookies.Append("cart", newCartItemsJson);
            NotificationHelper.SetSuccessNotification(this, "All products from the order have been added to the cart successfully!");

            return RedirectToAction("Checkout");
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> AddRecipeItemsToCart(string id)
        {
            var response = await _httpClient.GetAsync($"Recipes/{id}");
            var recipeDetails = await response.Content.ReadFromJsonAsync<DetailRecipeDTO>();

            // Lấy giỏ hàng hiện tại từ cookie
            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            var hasProduct = false;

            // Thêm các sản phẩm từ công thức vào giỏ
            foreach (var ingredient in recipeDetails.Ingredients)
            {
                if (ingredient.ProductID.HasValue)
                {
                    hasProduct = true;

                    // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                    var existingItem = cartItems.FirstOrDefault(item => item.ProductID == ingredient.ProductID);
                    if (existingItem != null)
                    {
                        // Nếu đã có, tăng số lượng
                        existingItem.Quantity += 1;
                    }
                    else
                    {
                        // Nếu chưa có, thêm sản phẩm mới vào giỏ
                        cartItems.Add(new CartItem
                        {
                            ProductID = (int)ingredient.ProductID,
                            Quantity = 1
                        });
                    }
                }
            }

            if (!hasProduct)
            {
                TempData["ErrorMessage"] = "No products were found in the ingredients!";
                return RedirectToAction("Detail", "Recipes", new { id = recipeDetails.RecipeID });
            }

            // Lưu lại giỏ hàng vào cookie
            var newCartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);

            Response.Cookies.Append("cart", newCartItemsJson);

            TempData["SuccessMessage"] = "All products from the recipe have been added to the cart successfully!";
            var refererUrl = Request.Headers["Referer"].ToString();
            return Redirect(refererUrl ?? Url.Action("Checkout", "Cart"));
        }

        [HttpPost]
        public IActionResult UpdateCartItem(int productId, int quantity)
        {
            // Retrieve the current cart from cookie
            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            // Find the item to update
            var existingItem = cartItems.Find(item => item.ProductID == productId);
            if (existingItem != null)
            {
                // Update the quantity, ensuring it's not less than 0
                existingItem.Quantity = quantity < 0 ? 0 : quantity;
            }

            // Save updated cart to cookie
            var newCartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);
            Response.SetCookie("cart", newCartItemsJson);

            return RedirectToAction("Checkout");
        }
        [ValidateTokenForUser]
        [HttpPost]
        public IActionResult UpdateCartItemLayout(int productId, int quantity)
        {
            // Retrieve the current cart from cookie
            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            // Find the item to update
            var existingItem = cartItems.Find(item => item.ProductID == productId);
            if (existingItem != null)
            {
                // Update the quantity, ensuring it's not less than 0
                existingItem.Quantity = quantity < 0 ? 0 : quantity;
            }

            // Save updated cart to cookie
            var newCartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);

            Response.SetCookie("cart", newCartItemsJson);

            // Redirect back to the referring page
            var refererUrl = Request.Headers["Referer"].ToString();
            return Redirect(refererUrl ?? Url.Action("Checkout"));
        }
        [ValidateTokenForUser]
        public IActionResult RemoveFromCart(int id)
        {
            // Lấy danh sách giỏ hàng hiện tại từ cookie
            var cartItemsJson = Request.GetCookie("cart") ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            // Tìm sản phẩm trong giỏ hàng
            var itemToRemove = cartItems.Find(item => item.ProductID == id);
            if (itemToRemove != null)
            {
                // Xóa sản phẩm khỏi giỏ hàng
                cartItems.Remove(itemToRemove);
            }

            // Lưu giỏ hàng đã cập nhật vào cookie
            var newCartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);

            Response.SetCookie("cart", newCartItemsJson);
            NotificationHelper.SetSuccessNotification(this, "The product has been removed from the cart successfully!");
            return RedirectToAction("Checkout"); // Hoặc chuyển hướng đến trang giỏ hàng
        }
        [ValidateTokenForUser]
        public IActionResult RemoveFromCartLayout(int id)
        {
            // Lấy danh sách giỏ hàng hiện tại từ cookie
            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            // Tìm sản phẩm trong giỏ hàng
            var itemToRemove = cartItems.Find(item => item.ProductID == id);
            if (itemToRemove != null)
            {
                // Xóa sản phẩm khỏi giỏ hàng
                cartItems.Remove(itemToRemove);
            }

            // Lưu giỏ hàng đã cập nhật vào cookie
            var newCartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);

            Response.SetCookie("cart", newCartItemsJson);
            NotificationHelper.SetSuccessNotification(this, "The product has been removed from the cart successfully!");
            var refererUrl = Request.Headers["Referer"].ToString();
            return Redirect(refererUrl ?? Url.Action("Index"));
        }
    }
}
