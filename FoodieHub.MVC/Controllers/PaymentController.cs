using FoodieHub.API.Models.DTOs.Order;
using FoodieHub.MVC.Libraries;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.VnPay;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace FoodieHub.MVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly HttpClient _httpClient;
		private readonly IVnPayService _vnPayService;
        public PaymentController(IHttpClientFactory httpClientFactory, PaypalClient paypalClient, IVnPayService vnPayService)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
            _paypalClient = paypalClient;
            _vnPayService = vnPayService;
        }
        [HttpPost("/Paypal/create-paypal-order")]
		public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
		{
            var data = HttpContext.Session.GetString("CurrentOrder");
            if (string.IsNullOrEmpty(data)) return BadRequest();
            var order = JsonSerializer.Deserialize<GetDetailOrder>(data);

            var totalAmount = order.TotalAmount - (order.Discount ?? 0) - (order.DiscountOfCoupon ?? 0);

            // Thông tin đơn hàng gửi qua Paypal
            var tongTien =totalAmount.ToString();
			var donViTienTe = "USD";
			var maDonHangThamChieu = order.OrderID.ToString();

			try
			{
				var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

				return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}

		[HttpPost("/Paypal/capture-paypal-order")]
		public async Task<IActionResult> CapturePaypalOrder(string orderId, CancellationToken cancellationToken)
		{
			try
			{
				// Xác nhận đơn hàng từ PayPal
				var response = await _paypalClient.CaptureOrder(orderId);

                var data = HttpContext.Session.GetString("CurrentOrder");
                if (string.IsNullOrEmpty(data)) return BadRequest();
                var order = JsonSerializer.Deserialize<GetDetailOrder>(data);

                var totalAmount = order.TotalAmount - (order.Discount ?? 0) - (order.DiscountOfCoupon ?? 0);
                var newPayment = new
                {
                    OrderID = order.OrderID,
                    PaymentMethod = "Paypal",
                    TotalAmount = totalAmount
                };

                // Lưu thông tin payment
                var responsePayment = await _httpClient.PostAsJsonAsync("payments", newPayment);
                if (responsePayment.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Payment successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment failed";
                }
                return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}

        public IActionResult PaypalSuccess()
        {
            var data = HttpContext.Session.GetString("CurrentOrder");
            if (string.IsNullOrEmpty(data)) return BadRequest();
            var order = JsonSerializer.Deserialize<GetDetailOrder>(data);
            var orderID = order.OrderID;
            HttpContext.Session.Remove("CurrentOrder");
            return Redirect("/Account/OrderDetail/" + orderID);
        }

		public async Task<IActionResult> Orders(string id)
		{
            ViewBag.PaypalClientdId = _paypalClient.ClientId;

			var response = await _httpClient.GetAsync("orders/" + id);
			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadFromJsonAsync<APIResponse<GetDetailOrder>>();
				if (data.Data.PaymentStatus)
				{
                    TempData["ErrorMessage"] = "The order has been paid.";
                    return RedirectToAction("Order", "Account");
                }
                var dataOrder = JsonSerializer.Serialize(data.Data);
				HttpContext.Session.SetString("CurrentOrder",dataOrder);
                return View(data.Data);
			}
			TempData["ErrorMessage"] = "Not found this order";
            return RedirectToAction("Order","Account");
        }
        public IActionResult VnPay()
		{
			var data = HttpContext.Session.GetString("CurrentOrder");
            if (string.IsNullOrEmpty(data)) return BadRequest(); 
            var order = JsonSerializer.Deserialize<GetDetailOrder>(data);

            var totalAmount = order.TotalAmount - (order.Discount ?? 0) - (order.DiscountOfCoupon ?? 0);
            var model = new PaymentInformationModel
            {
                OrderType = "Culinary Products",
                Amount = (double)totalAmount * 25300,
                OrderDescription = order.Note,
                Name = order.PhoneNumber
            };
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        public async Task<IActionResult> VnPayCallback()
        {
            var data = HttpContext.Session.GetString("CurrentOrder");
            if (string.IsNullOrEmpty(data)) return BadRequest();
            var order = JsonSerializer.Deserialize<GetDetailOrder>(data);
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode == "00")
            {       
                var totalAmount = order.TotalAmount - (order.Discount ?? 0) - (order.DiscountOfCoupon ?? 0);
                var newPayment = new
                {
                    OrderID = order.OrderID,
                    PaymentMethod = response.PaymentMethod,
                    TotalAmount = totalAmount
                };

                // Lưu thông tin payment
                var responsePayment = await _httpClient.PostAsJsonAsync("payments", newPayment);
                if (responsePayment.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Payment successfully";
                    HttpContext.Session.Remove("CurrentOrder");
                    return Redirect("/Account/OrderDetail/" + order.OrderID);
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment failed";
                }
                                    
            }
            TempData["ErrorMessage"] = "Failed to pay the order. Please try again later";
            return Redirect("/Payment/Orders/"+order.OrderID);
        }
    }
}
