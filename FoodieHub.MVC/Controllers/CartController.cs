using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Cart;
using FoodieHub.MVC.Models.Order;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace FoodieHub.MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IVnPayService _vnPayService;
        public CartController(IHttpClientFactory httpClientFactory, IVnPayService vnPayService)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
            _vnPayService = vnPayService;
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Checkout()
        {
            // Lấy danh sách sản phẩm trong giỏ hàng từ cookie
            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

            var getCart = new List<GetCartDTO>();

            // Duyệt qua từng item trong giỏ hàng để lấy thông tin chi tiết từ API
            foreach (var item in cartItems)
            {
                // Gọi API để lấy thông tin sản phẩm
                var response = await _httpClient.GetAsync($"Products/getproductbyid/{item.ProductID}");
                if (response.IsSuccessStatusCode)
                {
                    var productResponse = await response.Content.ReadFromJsonAsync<APIResponse<GetProductDTO>>();
                    if (productResponse != null && productResponse.Success)
                    {
                        var product = productResponse.Data;

                        // Thêm thông tin sản phẩm vào danh sách cartDetails
                        getCart.Add(new GetCartDTO
                        {
                            ProductID = product.ProductID,
                            ProductName = product.ProductName,
                            Price = product.Price,
                            Discount = product.Discount,
                            MainImage = product.MainImage,
                            Quantity = item.Quantity // Lưu số lượng từ giỏ hàng
                        });
                    }
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
                // Get product details from API
                var response = await _httpClient.GetAsync($"Products/getproductbyid/{item.ProductID}");
                if (response.IsSuccessStatusCode)
                {
                    var productResponse = await response.Content.ReadFromJsonAsync<APIResponse<GetProductDTO>>();
                    if (productResponse != null && productResponse.Success)
                    {
                        orderDto.OrderDetails.Add(new Models.Order.OrderDetailDto
                        {
                            ProductID = productResponse.Data.ProductID,
                            Quantity = item.Quantity// Ensure to retrieve the price of the product
                        });
                    }
                }
            }
                      
			var orderResponse = await _httpClient.PostAsJsonAsync("Orders", orderDto);
			var data = await orderResponse.Content.ReadFromJsonAsync<APIResponse<ReponseOrder>>();
			if (data != null && data.Success)
			{
				var orderID = data.Data.OrderID;

				// Clear the cart cookie
				Response.Cookies.Delete("cart");
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

            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
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
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(30)
            };

            Response.Cookies.Append("cart", newCartItemsJson, cookieOptions);

            TempData["SuccessMessage"] = "The product has been added to the cart successfully!";

            var refererUrl = Request.Headers["Referer"].ToString();
            return Redirect(refererUrl ?? Url.Action("Index","Products"));
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> AddOrderItemsToCart(string id)
        {
            var response = await _httpClient.GetAsync($"Orders/{id}");
            var content = await response.Content.ReadFromJsonAsync<APIResponse<GetDetailOrder>>();
            var orderDetails = content?.Data;

            // Lấy giỏ hàng hiện tại từ cookie
            var cartItemsJson = Request.Cookies["cart"] ?? "[]";
            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartItemsJson) ?? new List<CartItem>();

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
                    // Nếu chưa có, thêm sản phẩm mới
                    cartItems.Add(new CartItem
                    {
                        ProductID = detail.ProductID,
                        Quantity = detail.Quantity
                    });
                }
            }

            // Lưu lại giỏ hàng vào cookie
            var newCartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(30)
            };

            Response.Cookies.Append("cart", newCartItemsJson, cookieOptions);

            TempData["SuccessMessage"] = "All products from the order have been added to the cart successfully!";

            return RedirectToAction("Checkout");
        }
        [ValidateTokenForUser]
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
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Adjust based on your environment
                Expires = DateTime.UtcNow.AddDays(30)
            };

            Response.Cookies.Append("cart", newCartItemsJson, cookieOptions);

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
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Adjust based on your environment
                Expires = DateTime.UtcNow.AddDays(30)
            };

            Response.Cookies.Append("cart", newCartItemsJson, cookieOptions);

            // Redirect back to the referring page
            var refererUrl = Request.Headers["Referer"].ToString();
            return Redirect(refererUrl ?? Url.Action("Checkout"));
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> RemoveFromCart(int id)
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
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Điều chỉnh tùy thuộc vào môi trường của bạn
                Expires = DateTime.UtcNow.AddDays(30) // hoặc không set Expires để là session cookie
            };

            Response.Cookies.Append("cart", newCartItemsJson, cookieOptions);

            TempData["SuccessMessage"] = "The product has been removed from the cart successfully!";

            return RedirectToAction("Checkout"); // Hoặc chuyển hướng đến trang giỏ hàng
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> RemoveFromCartLayout(int id)
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
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Điều chỉnh tùy thuộc vào môi trường của bạn
                Expires = DateTime.UtcNow.AddDays(30) // hoặc không set Expires để là session cookie
            };

            Response.Cookies.Append("cart", newCartItemsJson, cookieOptions);

            TempData["SuccessMessage"] = "The product has been removed from the cart successfully!";

            var refererUrl = Request.Headers["Referer"].ToString();
            return Redirect(refererUrl ?? Url.Action("Index"));
        }
    }
}
